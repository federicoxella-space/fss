using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sim.Bench;

// Point 5 (R-104): the table of the cost measurement plan. Four layouts (dense or sparse, clearing everywhere or at market
// places), the market-places ones once per reading of routed demand, every cell measured in this run on Mkt's one update code.
// The sizes, the routing and the readings are measurement inputs, no P-number (R-050).
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
[System.Runtime.Versioning.SupportedOSPlatform("linux")]
static class PointFive
{
    static readonly int[] Count = { 1900, 60, 30, 8, 2 };   // P-12 as SQ-024 answered it, by type: village .. metropolis
    static readonly string[] TypeName = { "village", "borgo", "cittadina", "city", "metropolis" };
    static readonly (string name, bool dense, bool markets, bool perVillage)[] Spec =
    {
        ("sparse, clearing everywhere", false, false, false),
        ("dense, clearing everywhere", true, false, false),
        ("sparse, market places, every routed row ranked", false, true, false),
        ("sparse, market places, one entry per village", false, true, true),
        ("dense, market places, every routed row ranked", true, true, false),
        ("dense, market places, one entry per village", true, true, true),
    };

    sealed class Cfg
    {
        public string Name = ""; public bool Dense, AtMarkets, PerVillage;
        public long[] Ns = new long[5];      // p99 per update in ns, by type; at market places [0] is a village that does no clearing, [1..4] a market with its 19 villages
        public double Direct = -1;           // p99 of a day timed directly, ms; -1 = not timed
        public long Bytes, Expected;         // the world's bytes counted from the arrays, and recomputed from the sizes alone
        public long TotalNs { get { long s = 0; for (int t = 0; t < 5; t++) s += Count[t] * Ns[t]; return s; } }
    }

    static long ToNs(double us) => (long)Math.Round(us * 1000);
    static string Us(long ns) => $"{ns / 1000}.{ns % 1000:D3}";
    // the criterion's formula in integers: sum over types of count x cost per update, divided by 7, in ms to the nanosecond,
    // so that a cell edited by one microsecond moves the day
    static string DayMs(long totalNs) { long ns = (totalNs + 3) / 7; return $"{ns / 1_000_000}.{ns % 1_000_000:D6}"; }
    static string Word(decimal v, decimal budget) => v <= budget ? "within" : "over";
    static decimal Dec(string s) => decimal.Parse(s, CultureInfo.InvariantCulture);
    static string[] Cells(string line) => line.Trim().Trim('|').Split('|').Select(x => x.Trim()).ToArray();

    static List<string> T1(Cfg[] cs)
    {
        var l = new List<string> { "| layout | village (us) | borgo (us) | cittadina (us) | city (us) | metropolis (us) | day, recomputed (ms) |", "|---|---|---|---|---|---|---|" };
        foreach (var c in cs) l.Add($"| {c.Name} | {string.Join(" | ", c.Ns.Select(Us))} | {DayMs(c.TotalNs)} |");
        return l;
    }

    static List<string> T2(Cfg[] cs)
    {
        var l = new List<string> { "| layout | day, recomputed (ms) | day, timed directly, p99 (ms) | day read on (ms) | vs P-01 10 ms | vs 5 ms | world state (MB) | vs NFR-07 50 MB |", "|---|---|---|---|---|---|---|---|" };
        foreach (var c in cs)
        {
            decimal rec = Dec(DayMs(c.TotalNs)), direct = c.Direct < 0 ? 0 : Math.Round((decimal)c.Direct, 2, MidpointRounding.AwayFromZero);
            decimal on = c.Direct < 0 ? rec : Math.Max(rec, direct);
            decimal mb = c.Bytes / 1_000_000m;
            l.Add($"| {c.Name} | {DayMs(c.TotalNs)} | {(c.Direct < 0 ? "not timed" : direct.ToString("F2", CultureInfo.InvariantCulture))} | {Math.Round(on, 2, MidpointRounding.AwayFromZero).ToString("F2", CultureInfo.InvariantCulture)} | {Word(on, 10)} | {Word(on, 5)} | {mb.ToString("F3", CultureInfo.InvariantCulture)} | {Word(mb, 50)} |");
        }
        return l;
    }

    // The criterion's check: from the cells as printed, in decimal arithmetic apart from the integer one that printed them,
    // each day is the sum over types of count x cost per update / 7; and the second table says what the first says.
    static bool Verify(List<string> t1, List<string> t2, long[] expectedBytes)
    {
        if (t1.Count != t2.Count || t1.Count != 2 + expectedBytes.Length) return false;
        bool ok = true;
        for (int i = 2; i < t1.Count; i++)
        {
            string[] a = Cells(t1[i]), b = Cells(t2[i]);
            decimal sum = 0; for (int t = 0; t < 5; t++) sum += Count[t] * Dec(a[1 + t]);
            decimal rec = Math.Round(sum / 7m / 1000m, 6, MidpointRounding.AwayFromZero);
            string day = rec.ToString("F6", CultureInfo.InvariantCulture);
            decimal on = b[2] == "not timed" ? rec : Math.Max(rec, Dec(b[2]));
            decimal mb = expectedBytes[i - 2] / 1_000_000m;
            ok &= a[6] == day && b[1] == day && a[0] == b[0]
               && b[3] == Math.Round(on, 2, MidpointRounding.AwayFromZero).ToString("F2", CultureInfo.InvariantCulture)
               && b[4] == Word(on, 10) && b[5] == Word(on, 5)
               && b[6] == mb.ToString("F3", CultureInfo.InvariantCulture) && b[7] == Word(mb, 50);
        }
        return ok;
    }

    // Edits one printed cell of a copy of the first table, the way a hand edit would: the metropolis cost by one us, or the day by one ns.
    static List<string> Edited(List<string> t1, bool editDay)
    {
        var c = new List<string>(t1);
        var cells = Cells(c[2]);
        if (editDay) cells[6] = (Dec(cells[6]) + 0.000001m).ToString("F6", CultureInfo.InvariantCulture);
        else cells[5] = Us((long)(Dec(cells[5]) * 1000) + 1000);
        c[2] = "| " + string.Join(" | ", cells) + " |";
        return c;
    }

    public static int Run(bool smoke)
    {
        int updates = smoke ? 100 : 10000, chunk = smoke ? 20 : 500, warm = smoke ? 300 : 3000;
        var clock = Stopwatch.StartNew();
        Console.WriteLine(smoke ? "SMOKE RUN: small counts to exercise the pipeline, NOT the plan's method; it never exits 0" : "point 5, the plan's method on every cell");
        Console.WriteLine($"machine: {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} logical cores, pinned to one");
        Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"clock: Stopwatch {Stopwatch.Frequency} Hz, resolution {1e9 / Stopwatch.Frequency:F0} ns");
        Console.WriteLine($"method: Release, one thread pinned; {warm} ms warm-up of the cells timed together, interleaved in chunks of {chunk}; {updates} timed updates per run (a day = the places of one bucket, id % 7; a market-places cell = {updates} updates of the market, and {updates} updates of its 19 villages together, p99 of that / 19); p99 per run, median of 3 runs");
        bool ok = true;
        void Phase(string s) => Console.WriteLine($"[{clock.Elapsed:hh\\:mm\\:ss}] {s}");

        // ---- the world: P-12 as SQ-024 answered it
        var rowL = Layout.P12(true);
        var counts = new int[5]; foreach (int t in rowL.Type) counts[t]++;
        int markets = Enumerable.Range(0, 2000).Count(p => rowL.ClearsAt[p] == p), perMarket = Enumerable.Range(0, 2000).Count(p => rowL.ClearsAt[p] == 0 && p != 0);
        bool mixOk = counts.SequenceEqual(Count) && markets == 100 && perMarket == 19;
        ok &= mixOk;
        Console.WriteLine($"mix: {counts[4]} metropolises, {counts[3]} cities, {counts[2]} cittadine, {counts[1]} borghi, {counts[0]} villages; {markets} market places; {perMarket} villages routed to market place 0 {(mixOk ? "ok" : "MISMATCH")}");

        // ---- R-104 item 2: every configuration of the table against MktRef, rationing binding, the faults red
        Phase("reference checks (engine checksum vs independent reference; 'rationed' = clearings where demand exceeded supply; each fault must differ; DropLast = clearing skips the last good the place holds)");
        var rr = new[] { Fault.RoutedUnranked, Fault.IgnoreRouted, Fault.DropLast };
        var drop = new[] { Fault.DropLast };
        foreach (bool dense in new[] { false, true })
        {
            string d = dense ? "dense" : "sparse";
            ok &= PointFour.Check($"{d} everywhere (P-12 mix)", Layout.P12(false, false, false, dense), 140, drop, false);
            for (int t = 0; t < 5; t++)
                ok &= PointFour.Check($"{d} one {TypeName[t]} clearing itself", Layout.One(t, dense), 140, drop, false);
            if (!dense)   // the sparse P-12 worlds at market places: point 4's checks, run again here
            {
                ok &= PointFour.Check("sparse market places, every routed row ranked (P-12 mix)", Layout.P12(true), 140, rr, true);
                ok &= PointFour.Check("sparse market places, one entry per village (P-12 mix)", Layout.P12(true, true), 140, rr, true);
            }
            foreach (bool pv in new[] { false, true })
                for (int t = 1; t < 5; t++)
                    ok &= PointFour.Check($"{d} market places ({(pv ? "village" : "rows")}), {TypeName[t]} market, 19 villages", Layout.Group(1, 0, t, true, dense, pv), dense ? 40 : 400, rr, true);
        }
        Phase("reference checks done");

        // ---- the cells
        var cfgs = Spec.Select(s => new Cfg { Name = s.name, Dense = s.dense, AtMarkets = s.markets, PerVillage = s.perVillage }).ToArray();
        // ---- R-104 item 6: the world's bytes, counted from the arrays and recomputed from the sizes alone
        long assignment = Layout.P12(true).AssignmentBytes;
        foreach (var c in cfgs)
        {
            for (int t = 0; t < 5; t++)
            {
                c.Bytes += Count[t] * new Mkt(Layout.One(t, c.Dense)).Bytes(0);
                var (g, r) = Layout.Size[t];
                c.Expected += Count[t] * (c.Dense ? Program.ExpectedBytes(41, 300, false) : Program.ExpectedBytes(r, g, true));
            }
            if (c.AtMarkets) { c.Bytes += assignment; c.Expected += 4L * Count[0]; }
            bool same = c.Bytes == c.Expected;
            ok &= same;
            Console.WriteLine($"  bytes, {c.Name}: counted {c.Bytes}, recomputed {c.Expected} {(same ? "ok" : "MISMATCH")}");
        }

        // the table's recomputation and its controls on a synthetic table, so that a fault in the rendering shows before any timing
        {
            var fake = Spec.Select((x, i) => new Cfg { Name = x.name, Bytes = cfgs[i].Bytes, Expected = cfgs[i].Expected, Direct = i % 2 == 0 ? 4.5 : -1 }).ToArray();
            foreach (var c in fake) for (int t = 0; t < 5; t++) c.Ns[t] = 1234567L * (t + 1) + 7 * t;
            var f1 = T1(fake); var f2 = T2(fake); var fe = fake.Select(c => c.Expected).ToArray();
            bool fOk = Verify(f1, f2, fe), fCell = !Verify(Edited(f1, false), f2, fe), fDay = !Verify(Edited(f1, true), f2, fe);
            ok &= fOk && fCell && fDay;
            Console.WriteLine($"table recomputation on a synthetic table: {(fOk ? "equal" : "DIFFERENT")}; cost edited {(fCell ? "caught" : "NOT CAUGHT")}; day edited {(fDay ? "caught" : "NOT CAUGHT")}");
        }
        if (!ok) { Phase("FAIL before any timing"); Console.WriteLine("FAIL"); return 1; }
        for (int i = 0; i < 2; i++)   // clearing everywhere: one place of each type clearing itself
        {
            var c = cfgs[i];
            Phase($"cells: {c.Name}, one place of each type");
            var w = Enumerable.Range(0, 5).Select(t => new Mkt(Layout.One(t, c.Dense))).ToArray();
            var r = PointFour.Time(w, updates, TypeName.Select(n => $"{(c.Dense ? "dense" : "sparse")} {n}, clearing itself").ToArray(), chunk, warm);
            for (int t = 0; t < 5; t++) c.Ns[t] = ToNs(r[t].p99);
        }
        foreach (int i in new[] { 3, 2, 5, 4 })   // market places: a market with its 19 villages, per market type; the cheap ones first
        {
            var c = cfgs[i];
            Phase($"cells: {c.Name}, a market of each type with its 19 villages routed");
            var w = new[] { 1, 2, 3, 4 }.Select(t => new Mkt(Layout.Group(1, 0, t, true, c.Dense, c.PerVillage))).ToArray();
            var tt = PointFour.TimeSplit(w, updates, chunk, warm);
            c.Ns[0] = ToNs(tt[0].village / 19);
            for (int t = 1; t < 5; t++) c.Ns[t] = ToNs(tt[t - 1].market);
            for (int k = 0; k < 4; k++)
                Console.WriteLine($"  {TypeName[k + 1],-10} market with 19 villages routed: {tt[k].market:F2} us; village without clearing: {tt[k].village / 19:F2} us (p99 of 19 village updates {tt[k].village:F2} / 19; the cell takes the borgo group's)");
        }

        // ---- R-104 item 4: a day timed directly, for every sparse layout and any dense layout whose recomputed day is under 20 ms
        Phase("a day timed directly, the sparse layouts together");
        var sp = cfgs.Where(c => !c.Dense).ToArray();
        var rs = PointFour.Time(sp.Select(c => new Mkt(Layout.P12(c.AtMarkets, c.PerVillage))).ToArray(), updates, sp.Select(c => c.Name).ToArray(), chunk, warm);
        for (int i = 0; i < sp.Length; i++) sp[i].Direct = rs[i].p99 / 1000;
        foreach (var c in cfgs.Where(c => c.Dense))
        {
            if (c.TotalNs >= 20_000_000L * 7) { Console.WriteLine($"  {c.Name}: recomputed day {DayMs(c.TotalNs)} ms is not under 20 ms, not timed directly"); continue; }
            Phase($"a day timed directly: {c.Name}");
            var r = PointFour.Time(new[] { new Mkt(Layout.P12(c.AtMarkets, c.PerVillage, false, true)) }, updates, new[] { c.Name }, chunk, warm);
            c.Direct = r[0].p99 / 1000;
        }

        // ---- the table, and the criterion's recomputation of it, with its fault controls
        var t1 = T1(cfgs); var t2 = T2(cfgs); var exp = cfgs.Select(c => c.Expected).ToArray();
        Phase("the table (Markdown, for SQ-023)");
        Console.WriteLine();
        Console.WriteLine("Per type, p99 us per update (median of 3 runs). At market places, borgo..metropolis is a market place of that type with its 19 villages routed, village is one village that does no clearing. Day = sum over types of count x cost / 7, count = 1,900 / 60 / 30 / 8 / 2.");
        Console.WriteLine();
        t1.ForEach(Console.WriteLine);
        Console.WriteLine();
        Console.WriteLine("Day against the budgets: 'day read on' is the higher of the recomputed day and the day timed directly in the same run (p99 of days); 5 ms is P-01 with P-17's allowance for the phases the stand-in does not run. World state = P-12 mix, bytes counted by R-102's rule (1 MB = 10^6 bytes); the market-places rows count 4 bytes of assignment per routed village.");
        Console.WriteLine();
        t2.ForEach(Console.WriteLine);
        Console.WriteLine();
        Console.WriteLine("Measurement inputs the table rests on (R-104 item 7):");
        Console.WriteLine("- sizes per type (goods, rows): village 15, 6; borgo 30, 10; cittadina 60, 18; city 150, 30; metropolis 300, 41; dense = the catalogue's 300 goods and 41 rows, absent ones masked");
        Console.WriteLine("- 19 villages per market place by a fixed assignment (the village's ordinal modulo 100), the same for every market type; 100 market places of 2,000");
        Console.WriteLine("- both readings of routed demand, SQ-024 item 4 being open: every routed row one entry of the ranking, or one entry per routed village");
        Console.WriteLine("- goods variants: one class good (id 20) can carry K variants; every cell of this table is K = 1, K = 4 and 16 are point 4's and not in the table");
        Console.WriteLine("- the stand-in is five passes, calibrated to P-63 (5 of 12 phases); it is not the simulator (SQ-004)");

        bool recomputed = Verify(t1, t2, exp);
        bool catchesCell = !Verify(Edited(t1, false), t2, exp), catchesDay = !Verify(Edited(t1, true), t2, exp);
        ok &= recomputed && catchesCell && catchesDay;
        Console.WriteLine();
        Console.WriteLine($"table recomputed from its printed cells (decimal, count x cost / 7): {(recomputed ? "equal" : "DIFFERENT")}; a cost edited by one us {(catchesCell ? "caught" : "NOT CAUGHT")}; a day edited by one ns {(catchesDay ? "caught" : "NOT CAUGHT")}");
        Phase(ok ? "PASS" : "FAIL");
        Console.WriteLine(ok ? "PASS" : "FAIL");
        return ok ? (smoke ? 3 : 0) : 1;
    }
}
