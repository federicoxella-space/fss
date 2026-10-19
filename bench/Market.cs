using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sim.Bench;

// Point 4: clearing at market places with the villages routed to them, and a class cleared once with K variants.
// The stand-in is not the simulator; sizes, the routing and the split are measurement inputs, no P-number (R-050).

// The world definition shared by Mkt and MktRef: which places exist, what they hold, who clears where.
// No update code lives here.
sealed class Layout
{
    // Types 0..4 = village, borgo, cittadina, city, metropolis (SQ-024), as (goods, rows); ids are spread evenly over 300 goods and 41 rows.
    public static readonly (int goods, int rows)[] Size = { (15, 6), (30, 10), (60, 18), (150, 30), (300, 41) };
    public const int ClassGood = 20;   // a good id every type holds (a multiple of 20); the class that carries the K variants
    public int N, Period, K = 1;
    public bool Ample, PerVillage;     // Ample: supply exceeds any demand; PerVillage: a routed village enters the ranking as one entry
    public bool Dense;                 // point 5: every place holds the catalogue's 41 rows x 300 goods, those it does not have masked off (the rest of the update code is the same)
    public int[] Type, Bucket, ClearsAt;

    public static int[] GoodIds(int type) { var a = new int[Size[type].goods]; for (int k = 0; k < a.Length; k++) a[k] = k * 300 / a.Length; return a; }
    public static int[] RowIds(int type) { var a = new int[Size[type].rows]; for (int k = 0; k < a.Length; k++) a[k] = k * 41 / a.Length; return a; }
    public long ClassSupply;           // when > 0, the class good's supply, so it can be short (the K groups)
    public long Supply(int gid) => Ample ? 1_000_000 : gid == ClassGood && ClassSupply > 0 ? ClassSupply : StandIn.Supply(gid);

    // P-12 as SQ-024 answered it: 2 metropolises, 8 cities, 30 cittadine, 60 borghi, 1,900 villages; a market place every 20th id.
    // atMarkets: each village routes to the market place of the village's ordinal modulo 100 (19 villages each); otherwise every place clears itself.
    public static Layout P12(bool atMarkets, bool perVillage = false, bool ample = false, bool dense = false)
    {
        var l = new Layout { N = 2000, Period = 7, PerVillage = perVillage, Ample = ample, Dense = dense };
        l.Type = new int[2000]; l.Bucket = new int[2000]; l.ClearsAt = new int[2000];
        var mt = new int[100]; Array.Fill(mt, -1);
        foreach (var (t, c) in new[] { (4, 2), (3, 8), (2, 30) })
            for (int k = 0; k < c; k++) { int j = (k * 100 + 50) / c % 100; while (mt[j] >= 0) j = (j + 1) % 100; mt[j] = t; }
        for (int j = 0; j < 100; j++) { if (mt[j] < 0) mt[j] = 1; l.Type[j * 20] = mt[j]; }
        int ord = 0;
        for (int p = 0; p < 2000; p++)
        {
            l.Bucket[p] = p % 7;
            l.ClearsAt[p] = p % 20 == 0 || !atMarkets ? p : ord++ % 100 * 20;
        }
        return l;
    }

    // One borgo market and 19 villages routed to it, all updated together; K variants of the class good.
    // split: villages update on even days and the market on odd ones, so the two can be timed apart. mtype: the market's type.
    public static Layout Group(int k, long classSupply = 100, int mtype = 1, bool split = false, bool dense = false, bool perVillage = false)
    {
        var l = new Layout { N = 20, Period = split ? 2 : 1, K = k, ClassSupply = classSupply, Dense = dense, PerVillage = perVillage };
        l.Type = new int[20]; l.Bucket = new int[20]; l.ClearsAt = new int[20];
        l.Type[0] = mtype;
        if (split) l.Bucket[0] = 1;
        return l;
    }

    // One place of a type that clears itself: the cell of a settlement type with clearing everywhere.
    public static Layout One(int type, bool dense)
    {
        var l = new Layout { N = 1, Period = 1, Dense = dense };
        l.Type = new[] { type }; l.Bucket = new int[1]; l.ClearsAt = new int[1];
        return l;
    }

    // Bytes of the village-to-market assignment if the layout holds it: one int (the market's id) per routed place.
    public long AssignmentBytes => 4L * Enumerable.Range(0, N).Count(p => ClearsAt[p] != p);
}

enum Fault { None, RoutedUnranked, IgnoreRouted, DropVariant, DropLast }   // DropLast: clearing skips the last good the place holds (point 3's off-by-one)

// The stand-in's market update: places in ascending id; a place in the day's bucket runs local passes, then, if it
// is a market place, clears its own rows and the rows routed to it, then the closing local passes.
// Routed demand is served at the market on the market's tick and what it bought is in the village at once (SQ-024 item 3 is open).
sealed class Mkt
{
    sealed class Pl
    {
        public int Rows, Goods, Cols, LastLive;
        public bool Dense;
        public bool[] RowOn, GoodOn;                   // a dense place holds 41 x 300 and masks off the rows and goods its type does not have
        public int[] RowId, GoodId, ColStart, Routed, Loc;   // Loc: for a routed village, the local good index of each good of its market, or -1
        public long[] Stock, Prod, Need, Carry, Spoil, Cash, Price, Resid, Sup;
        public int[] Cur;                              // per row and good, where the next unit of an even split starts (a remainder accumulator)
        public long Checksum;
    }

    readonly Layout L;
    readonly Fault fault;
    readonly Pl[] pl;
    readonly int[] ord, ePl, eRow, eLg;
    readonly long[] eKey, eWant;
    int day;
    public long Denied, DeniedClass, VillageWanted, VillageServed;
    int nLive;                                         // live entries of the good being cleared

    public Mkt(Layout l, Fault f = Fault.None)
    {
        L = l; fault = f; pl = new Pl[l.N];
        for (int p = 0; p < l.N; p++) pl[p] = Build(p);
        int maxE = 0;
        for (int p = 0; p < l.N; p++)
        {
            if (l.ClearsAt[p] != p) continue;
            var rt = new List<int>();
            for (int q = 0; q < l.N; q++) if (q != p && l.ClearsAt[q] == p) rt.Add(q);
            pl[p].Routed = rt.ToArray();
            int e = pl[p].Rows;
            foreach (int q in rt)
            {
                e += l.PerVillage ? 1 : pl[q].Rows;
                pl[q].Loc = pl[p].GoodId.Select(g => Array.IndexOf(pl[q].GoodId, g)).ToArray();
            }
            maxE = Math.Max(maxE, e);
        }
        if (maxE > 1023) throw new InvalidOperationException("the tie-break holds 1023 entries");
        ord = new int[maxE]; ePl = new int[maxE]; eRow = new int[maxE]; eLg = new int[maxE]; eKey = new long[maxE]; eWant = new long[maxE];
    }

    Pl Build(int p)
    {
        var q = new Pl();
        int t = L.Type[p]; var rid = Layout.RowIds(t); var gid = Layout.GoodIds(t);
        q.Dense = L.Dense;
        q.Rows = L.Dense ? 41 : rid.Length; q.Goods = L.Dense ? 300 : gid.Length;
        q.RowId = new int[q.Rows]; q.GoodId = new int[q.Goods]; q.RowOn = new bool[q.Rows]; q.GoodOn = new bool[q.Goods];
        for (int r = 0; r < q.Rows; r++) { q.RowId[r] = L.Dense ? r : rid[r]; q.RowOn[r] = !L.Dense; }
        for (int g = 0; g < q.Goods; g++) { q.GoodId[g] = L.Dense ? g : gid[g]; q.GoodOn[g] = !L.Dense; }
        if (L.Dense) { foreach (int r in rid) q.RowOn[r] = true; foreach (int g in gid) q.GoodOn[g] = true; }
        for (int g = 0; g < q.Goods; g++) if (q.GoodOn[g]) q.LastLive = g;
        q.ColStart = new int[q.Goods + 1];
        for (int g = 0; g < q.Goods; g++) q.ColStart[g + 1] = q.ColStart[g] + (q.GoodId[g] == Layout.ClassGood ? L.K : 1);
        q.Cols = q.ColStart[q.Goods];
        int n = q.Rows * q.Cols;
        q.Stock = new long[n]; q.Prod = new long[n]; q.Need = new long[n]; q.Carry = new long[n]; q.Spoil = new long[n];
        q.Cash = new long[q.Rows]; q.Price = new long[q.Goods]; q.Resid = new long[q.Goods]; q.Sup = new long[q.Goods];
        q.Cur = new int[q.Rows * q.Goods];
        for (int r = 0; r < q.Rows; r++)
        {
            if (!q.RowOn[r]) continue;
            int rowKey = p * 41 + q.RowId[r];
            q.Cash[r] = World.Cash(rowKey);
            for (int g = 0; g < q.Goods; g++)
            {
                if (!q.GoodOn[g]) continue;
                for (int c = q.ColStart[g], v = 0; c < q.ColStart[g + 1]; c++, v++)
                    (q.Prod[r * q.Cols + c], q.Need[r * q.Cols + c], q.Stock[r * q.Cols + c], q.Spoil[r * q.Cols + c]) = World.Cell(rowKey, q.GoodId[g] + 300 * v);
            }
        }
        for (int g = 0; g < q.Goods; g++) { q.Price[g] = World.Price0(q.GoodId[g]); q.Sup[g] = q.GoodOn[g] ? L.Supply(q.GoodId[g]) : 0; }
        return q;
    }

    // Bytes of place p's settlement state by R-102's rule: goods 4 (Stock), goodsCarry 8 (Carry), currency 8 (Cash), price 8,
    // and, in a sparse place, the int id of each row and good held. Prod, Need, Spoil, Resid, Sup, Cur, the masks and the
    // routing arrays are left out, as R-102 leaves out what SIM-STATE's list does not name. The assignment is Layout.AssignmentBytes.
    public long Bytes(int p)
    {
        var q = pl[p];
        return q.Stock.Length * 4L + q.Carry.Length * 8L + q.Cash.Length * 8L + q.Price.Length * 8L + (q.Dense ? 0 : (q.RowId.Length + q.GoodId.Length) * 4L);
    }

    public int NextBucket() => day % L.Period;

    // One tick: the places of the day's bucket, ascending by id.
    public void Day()
    {
        int b = day++ % L.Period;
        for (int p = 0; p < L.N; p++)
        {
            if (L.Bucket[p] != b) continue;
            Pl q = pl[p];
            int n = q.Rows * q.Cols;
            for (int i = 0; i < n; i++) q.Stock[i] += q.Prod[i];
            for (int i = 0; i < n; i++) { long c = q.Need[i] < q.Stock[i] ? q.Need[i] : q.Stock[i]; q.Stock[i] -= c; }
            bool clears = L.ClearsAt[p] == p;
            if (clears) Clear(p, q);
            for (int r = 0; r < q.Rows; r++) q.Cash[r] = 1000 + (q.Cash[r] & 4095);
            for (int i = 0; i < n; i++)
            {
                long t = q.Stock[i] * q.Spoil[i] + q.Carry[i];
                long lost = t / 1000;
                q.Carry[i] = t - lost * 1000;
                q.Stock[i] -= lost;
            }
            // the checksum covers the cells present, rows then goods in ascending id, whichever the layout
            long h = q.Checksum;
            for (int r = 0; r < q.Rows; r++)
            {
                if (!q.RowOn[r]) continue;
                for (int g = 0; g < q.Goods; g++)
                {
                    if (!q.GoodOn[g]) continue;
                    for (int c = q.ColStart[g]; c < q.ColStart[g + 1]; c++) h = h * 31 + q.Stock[r * q.Cols + c];
                }
            }
            if (clears) for (int g = 0; g < q.Goods; g++) if (q.GoodOn[g]) h = h * 31 + q.Price[g];
            q.Checksum = h;
        }
    }

    // Clearing and prices of market place p, per clearing (good): own rows, then the routed villages in ascending id,
    // each routed row (or each village, under PerVillage) one entry; ranked by ability to pay, ties to the earlier entry.
    void Clear(int p, Pl m)
    {
        for (int gi = 0; gi < m.Goods; gi++)
        {
            if (fault == Fault.DropLast && gi == m.LastLive) continue;
            int ne = 0; nLive = 0;
            for (int r = 0; r < m.Rows; r++) Add(p, m, r, gi, ne++, false);
            int nOwn = ne;
            if (fault != Fault.IgnoreRouted)
                foreach (int q in m.Routed)
                {
                    Pl v = pl[q]; int lj = v.Loc[gi];
                    if (lj < 0) continue;
                    if (L.PerVillage) AddVillage(q, v, lj, ne++);
                    else for (int r = 0; r < v.Rows; r++) Add(q, v, r, lj, ne++, true);
                }
            for (int i = 0; i < ne; i++) ord[i] = i;
            int sortN = fault == Fault.RoutedUnranked ? nOwn : ne;
            for (int a = 0; a < sortN - 1; a++)
            {
                int mx = a;
                for (int b = a + 1; b < sortN; b++) if (eKey[b] > eKey[mx]) mx = b;
                int t = ord[a]; ord[a] = ord[mx]; ord[mx] = t;
                long u = eKey[a]; eKey[a] = eKey[mx]; eKey[mx] = u;
            }
            long supply = m.Sup[gi], price = m.Price[gi]; bool rationed = false; int k = 0, done = 0;
            for (; k < ne && supply > 0; k++)
            {
                int e = ord[k];
                long want = eWant[e];
                if (want == 0) continue;                      // an entry the layout masks off
                long take = want < supply ? want : supply;
                if (take < want) rationed = true;
                done++; supply -= take;
                Pl q = pl[ePl[e]];
                if (eRow[e] >= 0) Give(q, eRow[e], eLg[e], take, price);
                else
                {
                    long rem = take;
                    for (int r = 0; r < q.Rows; r++)
                    {
                        if (!q.RowOn[r]) continue;
                        long w = 1 + ((q.Cash[r] >> 6) & 3), t = w < rem ? w : rem;
                        Give(q, r, eLg[e], t, price); rem -= t;
                    }
                }
                if (ePl[e] != p) VillageServed += take;
            }
            if (done < nLive) rationed = true;
            if (rationed) { Denied++; if (m.GoodId[gi] == Layout.ClassGood) DeniedClass++; }
            m.Resid[gi] = supply;
        }
        for (int g = 0; g < m.Goods; g++)
        {
            long q = m.Price[g] + (m.Resid[g] > 300 ? -1 : 1) + (m.GoodId[g] & 1);
            m.Price[g] = q < 50 ? 50 : q;
        }
    }

    void Add(int place, Pl q, int row, int lg, int e, bool routed)
    {
        long s = 0; int b = row * q.Cols;
        for (int c = q.ColStart[lg]; c < q.ColStart[lg + 1]; c++) s += q.Stock[b + c];
        eKey[e] = (q.Cash[row] + s) * 1024 + (1023 - e);
        eWant[e] = q.RowOn[row] && q.GoodOn[lg] ? 1 + ((q.Cash[row] >> 6) & 3) : 0;   // 0 marks an entry the layout masks off
        ePl[e] = place; eRow[e] = row; eLg[e] = lg;
        if (eWant[e] > 0) nLive++;
        if (routed) VillageWanted += eWant[e];
    }

    void AddVillage(int place, Pl v, int lg, int e)
    {
        long s = 0, w = 0; bool held = v.GoodOn[lg];
        for (int r = 0; r < v.Rows; r++)
        {
            if (!v.RowOn[r]) continue;
            s += v.Cash[r]; if (held) w += 1 + ((v.Cash[r] >> 6) & 3);
            for (int c = v.ColStart[lg]; c < v.ColStart[lg + 1]; c++) s += v.Stock[r * v.Cols + c];
        }
        eKey[e] = s * 1024 + (1023 - e); eWant[e] = w;
        ePl[e] = place; eRow[e] = -1; eLg[e] = lg;
        if (w > 0) nLive++;
        VillageWanted += w;
    }

    // Pay for take units and deal them among the K variants of the good, one at a time from the row's cursor,
    // so a split of fewer units than variants still reaches every variant over time.
    void Give(Pl q, int row, int lg, long take, long price)
    {
        q.Cash[row] -= take * price;
        int c0 = q.ColStart[lg], kk = q.ColStart[lg + 1] - c0;
        int cur = q.Cur[row * q.Goods + lg];
        long each = take / kk, rem = take % kk;
        int kv = fault == Fault.DropVariant && kk > 1 ? kk - 1 : kk;
        for (int v = 0; v < kv; v++) q.Stock[row * q.Cols + c0 + v] += each + ((v - cur + kk) % kk < rem ? 1 : 0);
        q.Cur[row * q.Goods + lg] = (int)((cur + take) % kk);
    }

    // The running per-place checksums and the state at this moment, in ascending id.
    public long Checksum()
    {
        long h = 0;
        for (int p = 0; p < L.N; p++)
        {
            Pl q = pl[p];
            h = h * 31 + q.Checksum;
            for (int r = 0; r < q.Rows; r++) if (q.RowOn[r]) h = h * 31 + q.Cash[r];
            for (int r = 0; r < q.Rows; r++)
                if (q.RowOn[r])
                    for (int g = 0; g < q.Goods; g++)
                        if (q.GoodOn[g]) for (int c = q.ColStart[g]; c < q.ColStart[g + 1]; c++) h = h * 31 + q.Stock[r * q.Cols + c];
            if (L.ClearsAt[p] == p) for (int g = 0; g < q.Goods; g++) if (q.GoodOn[g]) h = h * 31 + q.Price[g];
            for (int r = 0; r < q.Rows; r++) if (q.RowOn[r]) for (int g = 0; g < q.Goods; g++) if (q.GoodOn[g]) h = h * 31 + q.Cur[r * q.Goods + g];
        }
        return h;
    }
}

// Plain reference of the same market update: 2-D arrays, objects for entries, an insertion sort, every entry
// served with no early exit; shares only Layout and World with Mkt.
static class MktRef
{
    sealed class P
    {
        public int Rows, Goods, Cols; public int[] Gid, Col0, Kv;
        public long[,] Stock, Prod, Need, Spoil, Carry; public long[] Cash, Price, Resid; public int[,] Cur; public long Hash;
    }
    sealed class E { public P Pl; public int Place, Row, Lg; public long Ability, Want; public bool Routed; }

    public static long RationedClass;   // set by Run: clearings of the class good short of supply with routed rows asking

    public static long Run(Layout l, int days, out long rationedRouted, out long wanted, out long served)
    {
        var ps = new P[l.N];
        for (int p = 0; p < l.N; p++)
        {
            int t = l.Type[p]; var rid = Layout.RowIds(t); var q = ps[p] = new P();
            q.Gid = Layout.GoodIds(t); q.Rows = rid.Length; q.Goods = q.Gid.Length;
            q.Kv = q.Gid.Select(g => g == Layout.ClassGood ? l.K : 1).ToArray();
            q.Col0 = new int[q.Goods]; q.Cols = 0;
            for (int g = 0; g < q.Goods; g++) { q.Col0[g] = q.Cols; q.Cols += q.Kv[g]; }
            q.Stock = new long[q.Rows, q.Cols]; q.Prod = new long[q.Rows, q.Cols]; q.Need = new long[q.Rows, q.Cols];
            q.Spoil = new long[q.Rows, q.Cols]; q.Carry = new long[q.Rows, q.Cols];
            q.Cash = new long[q.Rows]; q.Price = new long[q.Goods]; q.Resid = new long[q.Goods]; q.Cur = new int[q.Rows, q.Goods];
            for (int r = 0; r < q.Rows; r++)
            {
                q.Cash[r] = World.Cash(p * 41 + rid[r]);
                for (int g = 0; g < q.Goods; g++)
                    for (int v = 0; v < q.Kv[g]; v++)
                        (q.Prod[r, q.Col0[g] + v], q.Need[r, q.Col0[g] + v], q.Stock[r, q.Col0[g] + v], q.Spoil[r, q.Col0[g] + v]) = World.Cell(p * 41 + rid[r], q.Gid[g] + 300 * v);
            }
            for (int g = 0; g < q.Goods; g++) q.Price[g] = World.Price0(q.Gid[g]);
        }
        rationedRouted = 0; wanted = 0; served = 0; RationedClass = 0;
        for (int d = 0; d < days; d++)
            for (int p = 0; p < l.N; p++)
            {
                if (l.Bucket[p] != d % l.Period) continue;
                var q = ps[p];
                for (int r = 0; r < q.Rows; r++)
                    for (int c = 0; c < q.Cols; c++) { q.Stock[r, c] += q.Prod[r, c]; q.Stock[r, c] -= Math.Min(q.Need[r, c], q.Stock[r, c]); }
                bool clears = l.ClearsAt[p] == p;
                if (clears)
                {
                    var routed = Enumerable.Range(0, l.N).Where(x => x != p && l.ClearsAt[x] == p).ToList();
                    for (int g = 0; g < q.Goods; g++) ClearGood(l, ps, p, routed, g, ref rationedRouted, ref wanted, ref served);
                    for (int g = 0; g < q.Goods; g++) q.Price[g] = Math.Max(50, q.Price[g] + (q.Resid[g] > 300 ? -1 : 1) + (q.Gid[g] & 1));
                }
                for (int r = 0; r < q.Rows; r++) q.Cash[r] = 1000 + (q.Cash[r] & 4095);
                for (int r = 0; r < q.Rows; r++)
                    for (int c = 0; c < q.Cols; c++)
                    {
                        long t = q.Stock[r, c] * q.Spoil[r, c] + q.Carry[r, c];
                        q.Carry[r, c] = t % 1000; q.Stock[r, c] -= t / 1000;
                    }
                for (int r = 0; r < q.Rows; r++) for (int c = 0; c < q.Cols; c++) q.Hash = q.Hash * 31 + q.Stock[r, c];
                if (clears) for (int g = 0; g < q.Goods; g++) q.Hash = q.Hash * 31 + q.Price[g];
            }
        long h = 0;
        for (int p = 0; p < l.N; p++)
        {
            var q = ps[p];
            h = h * 31 + q.Hash;
            for (int r = 0; r < q.Rows; r++) h = h * 31 + q.Cash[r];
            for (int r = 0; r < q.Rows; r++) for (int c = 0; c < q.Cols; c++) h = h * 31 + q.Stock[r, c];
            if (l.ClearsAt[p] == p) for (int g = 0; g < q.Goods; g++) h = h * 31 + q.Price[g];
            for (int r = 0; r < q.Rows; r++) for (int g = 0; g < q.Goods; g++) h = h * 31 + q.Cur[r, g];
        }
        return h;
    }

    static E Entry(P q, int place, int row, int lg, bool routed)
    {
        long stock = 0;
        for (int v = 0; v < q.Kv[lg]; v++) stock += q.Stock[row, q.Col0[lg] + v];
        return new E { Pl = q, Place = place, Row = row, Lg = lg, Ability = q.Cash[row] + stock, Want = 1 + ((q.Cash[row] >> 6) & 3), Routed = routed };
    }

    static void ClearGood(Layout l, P[] ps, int p, List<int> routed, int g, ref long rationedRouted, ref long wanted, ref long served)
    {
        var m = ps[p];
        var entries = new List<E>();
        for (int r = 0; r < m.Rows; r++) entries.Add(Entry(m, p, r, g, false));
        foreach (int x in routed)
        {
            var v = ps[x]; int lg = Array.IndexOf(v.Gid, m.Gid[g]);
            if (lg < 0) continue;
            if (l.PerVillage)
            {
                // one entry for the village: the sums over its rows, Row = -1
                var rows = Enumerable.Range(0, v.Rows).Select(r => Entry(v, x, r, lg, true)).ToList();
                entries.Add(new E { Pl = v, Place = x, Row = -1, Lg = lg, Ability = rows.Sum(e => e.Ability), Want = rows.Sum(e => e.Want), Routed = true });
            }
            else for (int r = 0; r < v.Rows; r++) entries.Add(Entry(v, x, r, lg, true));
        }
        bool anyRouted = entries.Any(e => e.Routed);
        // insertion sort, higher ability first; equal ability keeps the earlier entry first
        var ord = new List<E>();
        foreach (var e in entries)
        {
            int j = ord.Count; ord.Add(e);
            while (j > 0 && ord[j - 1].Ability < e.Ability) { ord[j] = ord[j - 1]; j--; }
            ord[j] = e;
        }
        long full = l.Supply(m.Gid[g]), supply = full, price = m.Price[g], demand = entries.Sum(e => e.Want);
        foreach (var e in ord)
        {
            long take = Math.Min(e.Want, supply); supply -= take;
            if (e.Routed) { wanted += e.Want; served += take; }
            var q = e.Pl;
            if (e.Row >= 0) Give(q, e.Row, e.Lg, take, price);
            else
            {
                long rem = take;
                for (int r = 0; r < q.Rows; r++) { long w = 1 + ((q.Cash[r] >> 6) & 3), t = Math.Min(w, rem); Give(q, r, e.Lg, t, price); rem -= t; }
            }
        }
        if (demand > full && anyRouted) { rationedRouted++; if (m.Gid[g] == Layout.ClassGood) RationedClass++; }
        m.Resid[g] = supply;
    }

    // Deal take units one by one, each to the variant the row's cursor points at, and advance the cursor.
    static void Give(P q, int row, int lg, long take, long price)
    {
        q.Cash[row] -= take * price;
        for (long u = 0; u < take; u++) { q.Stock[row, q.Col0[lg] + q.Cur[row, lg]]++; q.Cur[row, lg] = (q.Cur[row, lg] + 1) % q.Kv[lg]; }
    }
}

static class PointFour
{
    const int CheckDays = 140, CheckGroup = 2000, DayUpdates = 10000, GroupUpdates = 20000, Runs = 3, Chunk = 500, WarmMs = 3000;

    static double P99(long[] t, int n) { Array.Sort(t, 0, n); return t[(int)(n * 0.99)] * 1e6 / Stopwatch.Frequency; }
    static double Median(double[] r) { var c = (double[])r.Clone(); Array.Sort(c); return c[c.Length / 2]; }

    // Times the worlds interleaved in chunks; per world, per run, the p99 and the mean of one update, in microseconds.
    internal static (double p99, double mean)[] Time(Mkt[] w, int updates, string[] names, int chunk = Chunk, int warmMs = WarmMs)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < warmMs) foreach (var s in w) s.Day();
        var t = w.Select(_ => new long[updates]).ToArray();
        var p99 = w.Select(_ => new double[Runs]).ToArray(); var mean = w.Select(_ => new double[Runs]).ToArray();
        for (int k = 0; k < Runs; k++)
        {
            for (int i = 0; i < updates; i += chunk)
                for (int c = 0; c < w.Length; c++)
                    for (int j = i; j < i + chunk; j++) { long a = Stopwatch.GetTimestamp(); w[c].Day(); t[c][j] = Stopwatch.GetTimestamp() - a; }
            for (int c = 0; c < w.Length; c++)
            {
                mean[c][k] = t[c].Sum() * 1e6 / Stopwatch.Frequency / updates;
                p99[c][k] = P99(t[c], updates);
            }
        }
        var res = new (double, double)[w.Length];
        for (int c = 0; c < w.Length; c++)
        {
            Console.WriteLine($"  {names[c],-34} p99 per run: {string.Join(" / ", p99[c].Select(x => x.ToString("F1")))} us; mean per run: {string.Join(" / ", mean[c].Select(x => x.ToString("F1")))} us");
            res[c] = (Median(p99[c]), Median(mean[c]));
        }
        return res;
    }

    internal static long Fresh(Layout l, int days, Fault f, out Mkt m) { m = new Mkt(l, f); for (int i = 0; i < days; i++) m.Day(); return m.Checksum(); }

    public static int Run()
    {
        Console.WriteLine($"machine: {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} logical cores, pinned to one");
        Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"clock: Stopwatch {Stopwatch.Frequency} Hz, resolution {1e9 / Stopwatch.Frequency:F0} ns");
        Console.WriteLine($"method: {WarmMs} ms warm-up of the cells timed together, interleaved in chunks of {Chunk}; {DayUpdates} timed days (a day = the places of one bucket, id % 7) or {GroupUpdates} group updates per run; p99 and mean per run, median of {Runs} runs; checksums at {CheckDays} days / {CheckGroup} group updates on fresh state against an independent reference");
        bool ok = true;

        // ---- the world
        var every = Layout.P12(false); var rowL = Layout.P12(true); var vilL = Layout.P12(true, true);
        var counts = new int[5]; foreach (int t in rowL.Type) counts[t]++;
        int markets = Enumerable.Range(0, 2000).Count(p => rowL.ClearsAt[p] == p), perMarket = Enumerable.Range(0, 2000).Count(p => rowL.ClearsAt[p] == 0 && p != 0);
        bool mixOk = counts[0] == 1900 && counts[1] == 60 && counts[2] == 30 && counts[3] == 8 && counts[4] == 2 && markets == 100 && perMarket == 19;
        ok &= mixOk;
        Console.WriteLine($"mix: {counts[4]} metropolises, {counts[3]} cities, {counts[2]} cittadine, {counts[1]} borghi, {counts[0]} villages; {markets} market places; {perMarket} villages routed to market place 0 {(mixOk ? "ok" : "MISMATCH")}");

        // ---- R-103 item 1: each configuration against the reference, and the two faults (item 2) and the Fails-when fault
        Console.WriteLine("reference checks (R-103, R-005 case 1): engine checksum vs independent reference; 'rationed' = clearings where demand exceeded supply with routed rows asking; faults must differ");
        ok &= Check("everywhere", every, CheckDays, new Fault[0], false);
        ok &= Check("market places, every routed row ranked", rowL, CheckDays, new[] { Fault.RoutedUnranked, Fault.IgnoreRouted }, true);
        ok &= Check("market places, one entry per village", vilL, CheckDays, new[] { Fault.RoutedUnranked, Fault.IgnoreRouted }, true);
        foreach (int k in new[] { 1, 4, 16 })
            ok &= Check($"borgo group, class with K={k} variants", Layout.Group(k), CheckGroup, k > 1 ? new[] { Fault.DropVariant, Fault.RoutedUnranked } : new[] { Fault.RoutedUnranked }, true, true);

        // the per-type cells timed below, the same layouts, split bucket (200 market updates each)
        foreach (var (t, nm) in new[] { (1, "borgo"), (2, "cittadina"), (3, "city"), (4, "metropolis") })
            ok &= Check($"timed cell: {nm} market, 19 villages", Layout.Group(1, 0, t, true), 400, new[] { Fault.RoutedUnranked }, true);

        // ---- closed by (c): supply ample, every unit a village demands is served at its market
        foreach (var (name, l) in new[] { ("every routed row ranked", Layout.P12(true, false, true)), ("one entry per village", Layout.P12(true, true, true)) })
        {
            Fresh(l, CheckDays, Fault.None, out var m); Fresh(l, CheckDays, Fault.IgnoreRouted, out var bad);
            MktRef.Run(l, CheckDays, out _, out long rw, out long rs);
            bool served = m.VillageWanted > 0 && m.VillageServed == m.VillageWanted && rw == m.VillageWanted && rs == m.VillageServed && m.Denied == 0;
            bool catches = bad.VillageServed < rw;
            ok &= served && catches;
            Console.WriteLine($"ample supply, {name}: village demand {m.VillageWanted} units, served {m.VillageServed} (reference {rw} / {rs}), clearings rationed {m.Denied} {(served ? "ok" : "NOT ALL SERVED")}; ignoring routed rows serves {bad.VillageServed} of the reference's {rw} {(catches ? "(caught)" : "(NOT CAUGHT)")}");
        }

        // ---- closed by (a): a day on the P-12 mix
        Console.WriteLine("cost of one simulated day on the P-12 mix, sparse layout (a day = one bucket of 1/7 of the places):");
        var wd = new[] { new Mkt(every), new Mkt(rowL), new Mkt(vilL) };
        var rd = Time(wd, DayUpdates, new[] { "clearing everywhere", "market places, every row ranked", "market places, entry per village" });
        Console.WriteLine($"  p99 ms/day: everywhere {rd[0].p99 / 1000:F2}; at market places {rd[1].p99 / 1000:F2} (every routed row ranked), {rd[2].p99 / 1000:F2} (one entry per village); P-01 is 10 ms");
        Console.WriteLine($"  mean ms/day: everywhere {rd[0].mean / 1000:F2}; at market places {rd[1].mean / 1000:F2}, {rd[2].mean / 1000:F2}; p99 market/everywhere {rd[1].p99 / rd[0].p99:F2} and {rd[2].p99 / rd[0].p99:F2}");

        // ---- closed by (b): cost per variant count
        Console.WriteLine("cost of one update of a borgo market place with its 19 villages, the class good with K variants:");
        var wg = new[] { new Mkt(Layout.Group(1)), new Mkt(Layout.Group(4)), new Mkt(Layout.Group(16)) };
        var rg = Time(wg, GroupUpdates, new[] { "K = 1", "K = 4", "K = 16" });
        Console.WriteLine($"  p99 us/update: K=1 {rg[0].p99:F2}, K=4 {rg[1].p99:F2}, K=16 {rg[2].p99:F2}; K=16/K=1 {rg[2].p99 / rg[0].p99:F2}");

        // ---- per-type cells of the market-places configuration, for point 5's recomputation of a day
        Console.WriteLine("p99 us per update by type, market-places configuration (a market with its 19 routed villages, every row ranked; villages do no clearing):");
        var wt = new[] { 1, 2, 3, 4 }.Select(t => new Mkt(Layout.Group(1, 0, t, true))).ToArray();
        var tt = TimeSplit(wt);
        for (int i = 0; i < 4; i++) Console.WriteLine($"  {new[] { "borgo", "cittadina", "city", "metropolis" }[i],-10} market with 19 villages routed: {tt[i].market:F2} us; village without clearing: {tt[i].village / 19:F2} us (p99 of 19 village updates {tt[i].village:F2} / 19)");

        Console.WriteLine(ok ? "PASS" : "FAIL");
        return ok ? 0 : 1;
    }

    // Worlds with Period 2: even days update the villages, odd days the market; p99 of each, interleaved, median of runs.
    internal static (double village, double market)[] TimeSplit(Mkt[] w, int half = GroupUpdates / 2, int chunk = Chunk, int warmMs = WarmMs)
    {
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < warmMs) foreach (var s in w) s.Day();
        var tv = w.Select(_ => new long[half]).ToArray(); var tm = w.Select(_ => new long[half]).ToArray();
        var pv = w.Select(_ => new double[Runs]).ToArray(); var pm = w.Select(_ => new double[Runs]).ToArray();
        foreach (var s in w) if (s.NextBucket() != 0) s.Day();
        for (int k = 0; k < Runs; k++)
        {
            for (int i = 0; i < half; i += chunk / 2)
                for (int c = 0; c < w.Length; c++)
                    for (int j = i; j < i + chunk / 2; j++)
                    {
                        long a = Stopwatch.GetTimestamp(); w[c].Day(); long b = Stopwatch.GetTimestamp(); w[c].Day(); long d = Stopwatch.GetTimestamp();
                        tv[c][j] = b - a; tm[c][j] = d - b;
                    }
            for (int c = 0; c < w.Length; c++) { pv[c][k] = P99(tv[c], half); pm[c][k] = P99(tm[c], half); }
        }
        return Enumerable.Range(0, w.Length).Select(c => (Median(pv[c]), Median(pm[c]))).ToArray();
    }

    internal static bool Check(string name, Layout l, int days, Fault[] faults, bool routed, bool cls = false)
    {
        long sum = Fresh(l, days, Fault.None, out var m);
        long refSum = MktRef.Run(l, days, out long rr, out _, out _);
        bool match = sum == refSum, binds = !routed || rr > 0 && m.Denied > 0, anyDenied = m.Denied > 0;
        long rc = MktRef.RationedClass; bool clsBinds = !cls || rc > 0 && m.DeniedClass > 0;
        var sb = new List<string>(); bool caught = true;
        foreach (var f in faults)
        {
            bool red = Fresh(l, days, f, out _) != refSum;
            caught &= red; sb.Add($"{f} {(red ? "red" : "NOT RED")}");
        }
        Console.WriteLine($"  {name,-42} {sum,20} {(match ? "=" : "<>")} {refSum,20}; {(routed ? $"rationed with routed rows {rr}, engine rationed {m.Denied} {(binds && anyDenied ? "binds" : "NEVER RATIONS")}" : $"rationing on own rows only (no routed rows): {m.Denied} {(anyDenied ? "binds" : "NEVER RATIONS")}")}{(cls ? $"; class good short with routed rows asking: reference {rc}, engine {m.DeniedClass} {(clsBinds ? "binds" : "NEVER SHORT")}" : "")}; {(faults.Length == 0 ? "" : string.Join(", ", sb))}{(cls && faults.All(f => f != Fault.DropVariant) ? ", DropVariant n/a (no variant to drop)" : "")}");
        return match && binds && anyDenied && caught && clsBinds;
    }
}
