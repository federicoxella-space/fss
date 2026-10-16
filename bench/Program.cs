using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sim.Bench;

// Stand-in for the settlement update: five passes over dense per-good arrays, integer
// arithmetic only. It is not the simulator; its numbers mean what it resembles (SQ-004).
// The sizes are measurement inputs, not simulation constants.
sealed class StandIn
{
    readonly int rows, Goods;
    readonly long[] stock, prod, need, cash, price, carry, spoilNum;
    readonly int[] order;
    readonly long[] key, residual;
    public long Checksum;

    // Every row wants 1..4 of every good, so each good, the last included, moves stock and cash in every cell.
    // Supply per good: every third good is scarce, so rationing binds (R-101, item 2); the rest are plentiful.
    // Measurement input, no P-number (R-050).
    public static long Supply(int g) => g % 3 == 0 ? 4 : 1000 + g * 37;

    // Bytes of SIM-STATE settlement state at its widths (R-102): goods 4, goodsCarry 8, currency 8, price 8.
    // stock stands for goods, carry for goodsCarry, cash for currency, price for the per-good price.
    // prod, need, spoilNum (content-derived rates) and order, key, residual (scratch) are left out (R-102).
    public long Bytes => stock.Length * 4L + carry.Length * 8L + cash.Length * 8L + price.Length * 8L;

    public StandIn(int rows, int goods = 7)
    {
        this.rows = rows; Goods = goods;
        int n = rows * Goods;
        stock = new long[n]; prod = new long[n]; need = new long[n]; carry = new long[n];
        spoilNum = new long[n];
        cash = new long[rows]; price = new long[Goods]; order = new int[rows]; key = new long[rows]; residual = new long[Goods];
        uint s = 12345;
        for (int i = 0; i < n; i++)
        {
            s = s * 1664525u + 1013904223u;
            prod[i] = 5 + (s >> 24) % 20;
            need[i] = 4 + (s >> 16) % 12;
            stock[i] = 100 + (s >> 8) % 200;
            spoilNum[i] = 1 + (s >> 4) % 7;
        }
        for (int r = 0; r < rows; r++) { s = s * 1664525u + 1013904223u; cash[r] = 1000 + (s >> 12) % 5000; }
        for (int g = 0; g < Goods; g++) price[g] = 100 + g * 10;
    }

    public void Update()
    {
        int n = rows * Goods;
        // 1. production
        for (int i = 0; i < n; i++) stock[i] += prod[i];
        // 2. own consumption
        for (int i = 0; i < n; i++)
        {
            long c = need[i] < stock[i] ? need[i] : stock[i];
            stock[i] -= c;
        }
        // 3. market clearing: per good, rank rows by ability to pay, ties to the lower row id (selection sort, quadratic in rows)
        for (int g = 0; g < Goods; g++)
        {
            for (int r = 0; r < rows; r++) { order[r] = r; key[r] = (cash[r] + stock[r * Goods + g]) * 1024 + (1023 - r); }
            // selection sort by descending key: n(n-1)/2 comparisons whatever the data
            for (int a = 0; a < rows - 1; a++)
            {
                int m = a;
                for (int b = a + 1; b < rows; b++) if (key[b] > key[m]) m = b;
                int t = order[a]; order[a] = order[m]; order[m] = t;
                long u = key[a]; key[a] = key[m]; key[m] = u;
            }
            long supply = Supply(g), p = price[g];
            for (int k = 0; k < rows && supply > 0; k++)
            {
                int r = order[k];
                long want = 1 + ((cash[r] >> 6) & 3);
                long take = want < supply ? want : supply;
                long cost = take * p;
                cash[r] -= cost; supply -= take;
                stock[r * Goods + g] += take;
            }
            residual[g] = supply;
        }
        // 4. prices: read the residual supply clearing left per good
        for (int g = 0; g < Goods; g++)
        {
            long q = price[g] + (residual[g] > 300 ? -1 : 1) + (g & 1);
            price[g] = q < 50 ? 50 : q;
        }
        // after the market pass, refill cash into 1000..5095
        for (int r = 0; r < rows; r++) cash[r] = 1000 + (cash[r] & 4095);
        // 5. spoilage with a remainder carry per row and good
        for (int i = 0; i < n; i++)
        {
            long t = stock[i] * spoilNum[i] + carry[i];
            long lost = t / 1000;
            carry[i] = t - lost * 1000;
            stock[i] -= lost;
        }
        // consume outputs so the JIT cannot drop the work
        long h = Checksum;
        for (int i = 0; i < n; i++) h = h * 31 + stock[i];
        for (int g = 0; g < Goods; g++) h = h * 31 + price[g];
        Checksum = h;
    }
}

// Plain reference of the five passes: 2-D arrays, every good and every row, no early exit, a
// comparison insertion sort; shares no code with StandIn. Same devices (R-101).
static class Reference
{
    // Returns the checksum after n updates; denied counts (update, good) pairs where demand exceeded supply.
    public static long Run(int rows, int goods, int n, bool rank, out long denied)
    {
        var stock = new long[rows, goods]; var prod = new long[rows, goods]; var need = new long[rows, goods];
        var spoil = new long[rows, goods]; var carry = new long[rows, goods];
        var cash = new long[rows]; var price = new long[goods];
        uint s = 12345;
        for (int r = 0; r < rows; r++)
            for (int g = 0; g < goods; g++)
            {
                s = s * 1664525u + 1013904223u;
                prod[r, g] = 5 + (s >> 24) % 20; need[r, g] = 4 + (s >> 16) % 12;
                stock[r, g] = 100 + (s >> 8) % 200; spoil[r, g] = 1 + (s >> 4) % 7;
            }
        for (int r = 0; r < rows; r++) { s = s * 1664525u + 1013904223u; cash[r] = 1000 + (s >> 12) % 5000; }
        for (int g = 0; g < goods; g++) price[g] = 100 + g * 10;
        long h = 0; denied = 0;
        for (int u = 0; u < n; u++)
        {
            for (int r = 0; r < rows; r++)
                for (int g = 0; g < goods; g++)
                {
                    stock[r, g] += prod[r, g];
                    stock[r, g] -= Math.Min(need[r, g], stock[r, g]);
                }
            var residual = new long[goods];
            for (int g = 0; g < goods; g++)
            {
                var ord = new List<int>();
                foreach (int r in Enumerable.Range(0, rows))
                {
                    int j = ord.Count;
                    ord.Add(r);
                    // higher cash + stock first, ties to the lower row id; without ranking, row id alone
                    while (j > 0 && rank && cash[ord[j - 1]] + stock[ord[j - 1], g] < cash[r] + stock[r, g]) { ord[j] = ord[j - 1]; j--; }
                    ord[j] = r;
                }
                long supply = QSupply(g), p = price[g]; bool rationed = false;
                foreach (int r in ord)
                {
                    long want = 1 + ((cash[r] >> 6) & 3), take = Math.Min(want, supply);
                    if (take < want) rationed = true;
                    cash[r] -= take * p; supply -= take; stock[r, g] += take;
                }
                if (rationed) denied++;
                residual[g] = supply;
            }
            for (int g = 0; g < goods; g++) price[g] = Math.Max(50, price[g] + (residual[g] > 300 ? -1 : 1) + (g & 1));
            for (int r = 0; r < rows; r++) cash[r] = 1000 + (cash[r] & 4095);
            for (int r = 0; r < rows; r++)
                for (int g = 0; g < goods; g++)
                {
                    long t = stock[r, g] * spoil[r, g] + carry[r, g];
                    carry[r, g] = t % 1000; stock[r, g] -= t / 1000;
                }
            for (int r = 0; r < rows; r++) for (int g = 0; g < goods; g++) h = h * 31 + stock[r, g];
            for (int g = 0; g < goods; g++) h = h * 31 + price[g];
        }
        return h;
    }

    static long QSupply(int g) => g % 3 == 0 ? 4 : 1000 + g * 37;
}

// The platforms where Process.ProcessorAffinity is supported; declaring them keeps CA1416 quiet without a suppression.
[System.Runtime.Versioning.SupportedOSPlatform("windows")]
[System.Runtime.Versioning.SupportedOSPlatform("linux")]
static class Program
{
    const int Updates = 100000, Runs = 3, Chunk = 500, WarmMs = 3000;

    // One run: both cells sampled in alternating chunks of Chunk updates, so any drift in clock
    // or noise lands on both sides of the ratio. Each cell keeps its own p99.
    static void Run(StandIn s9, StandIn s27, long[] t9, long[] t27, out double p9, out double p27)
    {
        for (int i = 0; i < Updates; i += Chunk)
        {
            Sample(s9, t9, i);
            Sample(s27, t27, i);
        }
        p9 = P99(t9); p27 = P99(t27);
    }

    static void Sample(StandIn s, long[] t, int from)
    {
        for (int i = from; i < from + Chunk; i++)
        {
            long a = Stopwatch.GetTimestamp();
            s.Update();
            t[i] = Stopwatch.GetTimestamp() - a;
        }
    }

    static double P99(long[] t)
    {
        Array.Sort(t);
        return t[(int)(Updates * 0.99)] * 1e6 / Stopwatch.Frequency;
    }

    static double Median(double[] r)
    {
        var c = (double[])r.Clone();
        Array.Sort(c);
        return c[c.Length / 2];
    }

    static int Main(string[] args)
    {
        try
        {
            var p = Process.GetCurrentProcess();
            p.ProcessorAffinity = (IntPtr)(1L << (Environment.ProcessorCount > 2 ? 2 : 0));
            p.PriorityClass = ProcessPriorityClass.High;
        }
        catch (Exception e) { Console.WriteLine("pinning failed: " + e.Message); return 2; }
        return args.Length > 0 && args[0] == "point1" ? PointOne() : args.Length > 0 && args[0] == "point3" ? PointThree() : args.Length > 0 && args[0] == "point4" ? PointFour.Run() : PointTwo();
    }

    // Point 1's check (9 and 27 rows, 7 goods, its bounds): `dotnet run -c Release --project bench -- point1`.
    static int PointOne()
    {
        Console.WriteLine($"machine: {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} logical cores, pinned to one");
        Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"clock: Stopwatch {Stopwatch.Frequency} Hz, resolution {1e9 / Stopwatch.Frequency:F0} ns");
        Console.WriteLine($"method: {WarmMs} ms warm-up of both cells, {Updates} timed updates per run, p99 per run, cells interleaved in chunks of {Chunk}, median of {Runs} runs, 7 goods");

        // Both cells are warmed together for WarmMs of wall time (steady clock), then measured
        // in alternating chunks of Chunk updates so drift lands on both sides of the ratio.
        var s9 = new StandIn(9); var s27 = new StandIn(27);
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedMilliseconds < WarmMs) for (int i = 0; i < 1000; i++) { s9.Update(); s27.Update(); }
        var t9 = new long[Updates]; var t27 = new long[Updates];
        var r9 = new double[Runs]; var r27 = new double[Runs];
        for (int i = 0; i < Runs; i++) Run(s9, s27, t9, t27, out r9[i], out r27[i]);
        long sum = s9.Checksum ^ s27.Checksum;
        double c9 = Median(r9), c27 = Median(r27), ratio = c27 / c9;
        Console.WriteLine($"p99 per run, 9 rows: {string.Join(", ", r9.Select(x => x.ToString("F2")))}; 27 rows: {string.Join(", ", r27.Select(x => x.ToString("F2")))}");
        Console.WriteLine($"cost at 9 rows:  {c9:F2} us (bounds 1.6 .. 6.6)");
        Console.WriteLine($"cost at 27 rows: {c27:F2} us");
        Console.WriteLine($"ratio 27/9:      {ratio:F2} (bounds 3.8 .. 5.8)");
        Console.WriteLine($"checksum: {sum}");

        bool ok = c9 >= 1.6 && c9 <= 6.6 && ratio >= 3.8 && ratio <= 5.8;
        Console.WriteLine(ok ? "PASS" : "FAIL");
        return ok ? 0 : 1;
    }
    // Point 2's check: the seven cells, each checksum at CheckUpdates against the plain reference, and each
    // state rationing within them (R-101).
    const int CheckUpdates = 2000, CellUpdates = 20000, CellWarmMs = 1000;
    static readonly (int rows, int goods)[] Cells = { (12, 8), (12, 30), (12, 100), (12, 300), (12, 100), (24, 100), (41, 100) };

    static int PointTwo()
    {
        Console.WriteLine($"machine: {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} logical cores, pinned to one");
        Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"clock: Stopwatch {Stopwatch.Frequency} Hz, resolution {1e9 / Stopwatch.Frequency:F0} ns");
        Console.WriteLine($"method: {CellWarmMs} ms warm-up per cell, {CellUpdates} timed updates per run, p99 per run, median of {Runs} runs; checksum at {CheckUpdates} updates on a fresh state, against a plain reference");
        Console.WriteLine("rows goods   bytes  p99 us (3 runs)             median   checksum             reference            denied  ranking");
        bool ok = true;
        foreach (var (rows, goods) in Cells)
        {
            var chk = new StandIn(rows, goods);
            for (int i = 0; i < CheckUpdates; i++) chk.Update();
            long refSum = Reference.Run(rows, goods, CheckUpdates, true, out long denied);
            long flat = Reference.Run(rows, goods, CheckUpdates, false, out _);
            bool match = chk.Checksum == refSum, binds = denied > 0, reaches = flat != refSum;
            bool bytesOk = new StandIn(rows, goods).Bytes == ExpectedBytes(rows, goods, false);
            ok &= match && binds && reaches && bytesOk;

            var s = new StandIn(rows, goods);
            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < CellWarmMs) for (int i = 0; i < 100; i++) s.Update();
            var t = new long[CellUpdates]; var runs = new double[Runs];
            for (int k = 0; k < Runs; k++)
            {
                for (int i = 0; i < CellUpdates; i++) { long a = Stopwatch.GetTimestamp(); s.Update(); t[i] = Stopwatch.GetTimestamp() - a; }
                Array.Sort(t);
                runs[k] = t[(int)(CellUpdates * 0.99)] * 1e6 / Stopwatch.Frequency;
            }
            Console.WriteLine($"{rows,4} {goods,5} {s.Bytes,7}{(bytesOk ? " " : "!")} {string.Join(" / ", runs.Select(x => x.ToString("F2").PadLeft(8)))}  {Median(runs),8:F2}   {chk.Checksum,20} {(match ? "=" : "<>")} {refSum,20}  {denied,6}  {(binds ? "binds" : "NEVER RATIONS")}, {(reaches ? "reaches checksum" : "NOT IN CHECKSUM")}");
        }
        Console.WriteLine(ok ? "PASS" : "FAIL");
        return ok ? 0 : 1;
    }

    // R-102 item 2: the state bytes recomputed from the sizes alone, sharing nothing with the layouts' array lengths.
    // Per cell goods 4 + carry 8; per row currency 8; per good price 8; a sparse layout adds one int id per row and per good.
    static long ExpectedBytes(long rows, long goods, bool sparse) => rows * goods * 12 + rows * 8 + goods * 8 + (sparse ? (rows + goods) * 4 : 0);

    // Point 3's check: `dotnet run -c Release --project bench -- point3`.
    // The five SQ-024 types as (name, goods, rows); the dense layout is always 300 goods and 41 rows.
    static readonly (string name, int goods, int rows)[] Types = { ("village", 15, 6), ("borgo", 30, 10), ("cittadina", 60, 18), ("city", 150, 30), ("metropolis", 300, 41) };

    static int PointThree()
    {
        Console.WriteLine($"machine: {RuntimeInformation.OSDescription}, {RuntimeInformation.ProcessArchitecture}, {Environment.ProcessorCount} logical cores, pinned to one");
        Console.WriteLine($"runtime: {RuntimeInformation.FrameworkDescription}");
        Console.WriteLine($"clock: Stopwatch {Stopwatch.Frequency} Hz, resolution {1e9 / Stopwatch.Frequency:F0} ns");
        Console.WriteLine($"method: {CellWarmMs} ms warm-up of both layouts, {CellUpdates} timed updates per run, p99 per run, layouts interleaved in chunks of {Chunk}, median of {Runs} runs; checksum at {CheckUpdates} updates on a fresh state");
        bool ok = true;
        foreach (var (name, ng, nr) in Types)
        {
            var gIds = new int[ng]; var rIds = new int[nr];
            for (int k = 0; k < ng; k++) gIds[k] = k * 300 / ng;
            for (int k = 0; k < nr; k++) rIds[k] = k * 41 / nr;
            var cs = new Settlement(rIds, gIds, false); var cd = new Settlement(rIds, gIds, true); var cf = new Settlement(rIds, gIds, false, false);
            for (int i = 0; i < CheckUpdates; i++) { cs.Update(); cd.Update(); cf.Update(); }
            long refSum = RefWorld.Run(rIds, gIds, CheckUpdates);
            bool same = cs.Checksum == refSum && cd.Checksum == refSum, binds = cs.Denied > 0, reaches = cf.Checksum != cs.Checksum;
            long eS = ExpectedBytes(nr, ng, true), eD = ExpectedBytes(41, 300, false);
            bool bytesOk = cs.Bytes == eS && cd.Bytes == eD;
            ok &= same && binds && reaches && bytesOk;
            var ss = new Settlement(rIds, gIds, false); var sd = new Settlement(rIds, gIds, true);
            // the timed pair is built apart from the checked ones: each must be the layout it is labelled, at its expected bytes
            bool timedOk = ss.Sparse && !sd.Sparse && ss.Bytes == eS && sd.Bytes == eD;
            ok &= timedOk;

            var sw = Stopwatch.StartNew();
            while (sw.ElapsedMilliseconds < CellWarmMs) for (int i = 0; i < 100; i++) { ss.Update(); sd.Update(); }
            var ts = new long[CellUpdates]; var td = new long[CellUpdates]; var rs = new double[Runs]; var rd = new double[Runs];
            for (int k = 0; k < Runs; k++)
            {
                for (int i = 0; i < CellUpdates; i += Chunk) { Sample(ss, ts, i); Sample(sd, td, i); }
                Array.Sort(ts); Array.Sort(td);
                rs[k] = ts[(int)(CellUpdates * 0.99)] * 1e6 / Stopwatch.Frequency;
                rd[k] = td[(int)(CellUpdates * 0.99)] * 1e6 / Stopwatch.Frequency;
            }
            double ms = Median(rs), md = Median(rd);
            Console.WriteLine($"{name,-10} {ng,3} goods {nr,2} rows | sparse p99 {ms,8:F2} us ({string.Join(" / ", rs.Select(x => x.ToString("F2")))}) | dense 300x41 p99 {md,8:F2} us ({string.Join(" / ", rd.Select(x => x.ToString("F2")))}) | dense/sparse {md / ms:F2}");
            Console.WriteLine($"{"",-10} checksum sparse {cs.Checksum} dense {cd.Checksum} reference {refSum} {(same ? "=" : "<>")}; bytes sparse {cs.Bytes} (expected {eS}), dense {cd.Bytes} (expected {eD}) {(bytesOk ? "ok" : "MISMATCH")}; timed pair {(timedOk ? "ok" : "WRONG LAYOUT")}; denied {cs.Denied} {(binds ? "binds" : "NEVER RATIONS")}, {(reaches ? "ranking reaches checksum" : "NOT IN CHECKSUM")}");
        }
        Console.WriteLine(ok ? "PASS" : "FAIL");
        return ok ? 0 : 1;
    }

    static void Sample(Settlement s, long[] t, int from)
    {
        for (int i = from; i < from + Chunk; i++)
        {
            long a = Stopwatch.GetTimestamp();
            s.Update();
            t[i] = Stopwatch.GetTimestamp() - a;
        }
    }
}

// The id-based definition of point 3's world, shared by Settlement and RefWorld: a row's cash, a cell's
// content and a good's opening price depend on the ids only. No update code lives here.
static class World
{
    public static long Cash(int rid) { uint c = (uint)(rid + 7) * 2654435761u; c = c * 1664525u + 1013904223u; return 1000 + (c >> 12) % 5000; }
    public static (long prod, long need, long stock, long spoil) Cell(int rid, int gid)
    {
        uint s = (uint)(rid * 300 + gid + 1) * 2654435761u; s = s * 1664525u + 1013904223u;
        return (5 + (s >> 24) % 20, 4 + (s >> 16) % 12, 100 + (s >> 8) % 200, 1 + (s >> 4) % 7);
    }
    public static long Price0(int gid) => 100 + gid * 10;
}

// Plain reference of the five passes over the rows and goods present, in ascending id order: 2-D arrays,
// every good and every row, no masks, no early exit, an insertion sort; shares no update code with Settlement.
static class RefWorld
{
    public static long Run(int[] rIds, int[] gIds, int n)
    {
        int rows = rIds.Length, goods = gIds.Length;
        var stock = new long[rows, goods]; var prod = new long[rows, goods]; var need = new long[rows, goods];
        var spoil = new long[rows, goods]; var carry = new long[rows, goods];
        var cash = new long[rows]; var price = new long[goods];
        for (int r = 0; r < rows; r++)
        {
            cash[r] = World.Cash(rIds[r]);
            for (int g = 0; g < goods; g++) (prod[r, g], need[r, g], stock[r, g], spoil[r, g]) = World.Cell(rIds[r], gIds[g]);
        }
        for (int g = 0; g < goods; g++) price[g] = World.Price0(gIds[g]);
        long h = 0;
        for (int u = 0; u < n; u++)
        {
            for (int r = 0; r < rows; r++)
                for (int g = 0; g < goods; g++)
                {
                    stock[r, g] += prod[r, g];
                    stock[r, g] -= Math.Min(need[r, g], stock[r, g]);
                }
            var residual = new long[goods];
            for (int g = 0; g < goods; g++)
            {
                var ord = new List<int>();
                for (int r = 0; r < rows; r++)
                {
                    int j = ord.Count;
                    ord.Add(r);
                    while (j > 0 && cash[ord[j - 1]] + stock[ord[j - 1], g] < cash[r] + stock[r, g]) { ord[j] = ord[j - 1]; j--; }
                    ord[j] = r;
                }
                long supply = StandIn.Supply(gIds[g]), p = price[g];
                foreach (int r in ord)
                {
                    long take = Math.Min(1 + ((cash[r] >> 6) & 3), supply);
                    cash[r] -= take * p; supply -= take; stock[r, g] += take;
                }
                residual[g] = supply;
            }
            for (int g = 0; g < goods; g++) price[g] = Math.Max(50, price[g] + (residual[g] > 300 ? -1 : 1) + (gIds[g] & 1));
            for (int r = 0; r < rows; r++) cash[r] = 1000 + (cash[r] & 4095);
            for (int r = 0; r < rows; r++)
                for (int g = 0; g < goods; g++)
                {
                    long t = stock[r, g] * spoil[r, g] + carry[r, g];
                    carry[r, g] = t % 1000; stock[r, g] -= t / 1000;
                }
            for (int r = 0; r < rows; r++) for (int g = 0; g < goods; g++) h = h * 31 + stock[r, g];
            for (int g = 0; g < goods; g++) h = h * 31 + price[g];
        }
        return h;
    }
}

// Point 3's two layouts of one settlement, one update code for both; only the data differ.
// Sparse: arrays of rows x goods present, plus the ids of the rows and goods present (the index, counted as state).
// Dense: arrays of the catalogue's 41 x 300, ids are positions; absent rows and goods are masked (world definition, not state).
// A cell's content, a row's cash and a good's supply depend on the ids only, so both layouts hold the same world.
// Measurement input, no P-number (R-050).
sealed class Settlement
{
    readonly int rows, Goods;
    readonly bool sparse, rank;
    readonly long[] stock, prod, need, carry, spoilNum, cash, price, key, residual, sup;
    readonly int[] order, rowId, goodId;
    readonly bool[] rowOn, goodOn;
    public long Checksum, Denied;
    public bool Sparse => sparse;

    // R-102: stock stands for goods (4), carry for goodsCarry (8), cash for currency (8), price for the per-good price (8),
    // rowId and goodId for the sparse index (4 each). prod, need, spoilNum, order, key, residual, sup and the masks are left out.
    public long Bytes => stock.Length * 4L + carry.Length * 8L + cash.Length * 8L + price.Length * 8L
        + (sparse ? rowId.Length * 4L + goodId.Length * 4L : 0);

    public Settlement(int[] rIds, int[] gIds, bool dense, bool rank = true)
    {
        sparse = !dense; this.rank = rank;
        rows = dense ? 41 : rIds.Length; Goods = dense ? 300 : gIds.Length;
        rowId = new int[rows]; goodId = new int[Goods]; rowOn = new bool[rows]; goodOn = new bool[Goods];
        for (int r = 0; r < rows; r++) { rowId[r] = dense ? r : rIds[r]; rowOn[r] = !dense; }
        for (int g = 0; g < Goods; g++) { goodId[g] = dense ? g : gIds[g]; goodOn[g] = !dense; }
        if (dense) { foreach (int r in rIds) rowOn[r] = true; foreach (int g in gIds) goodOn[g] = true; }
        int n = rows * Goods;
        stock = new long[n]; prod = new long[n]; need = new long[n]; carry = new long[n]; spoilNum = new long[n];
        cash = new long[rows]; price = new long[Goods]; sup = new long[Goods];
        order = new int[rows]; key = new long[rows]; residual = new long[Goods];
        for (int r = 0; r < rows; r++)
        {
            if (!rowOn[r]) continue;
            cash[r] = World.Cash(rowId[r]);
            for (int g = 0; g < Goods; g++)
            {
                if (!goodOn[g]) continue;
                int i = r * Goods + g;
                (prod[i], need[i], stock[i], spoilNum[i]) = World.Cell(rowId[r], goodId[g]);
            }
        }
        for (int g = 0; g < Goods; g++) { price[g] = World.Price0(goodId[g]); sup[g] = goodOn[g] ? StandIn.Supply(goodId[g]) : 0; }
    }

    public void Update()
    {
        int n = rows * Goods;
        for (int i = 0; i < n; i++) stock[i] += prod[i];
        for (int i = 0; i < n; i++) { long c = need[i] < stock[i] ? need[i] : stock[i]; stock[i] -= c; }
        for (int g = 0; g < Goods; g++)
        {
            for (int r = 0; r < rows; r++) { order[r] = r; key[r] = (cash[r] + stock[r * Goods + g]) * 1024 + (1023 - rowId[r]); }
            if (rank)
                for (int a = 0; a < rows - 1; a++)
                {
                    int m = a;
                    for (int b = a + 1; b < rows; b++) if (key[b] > key[m]) m = b;
                    int t = order[a]; order[a] = order[m]; order[m] = t;
                    long u = key[a]; key[a] = key[m]; key[m] = u;
                }
            long supply = sup[g], p = price[g];
            for (int k = 0; k < rows && supply > 0; k++)
            {
                int r = order[k];
                if (!rowOn[r]) continue;
                long want = 1 + ((cash[r] >> 6) & 3);
                long take = want < supply ? want : supply;
                if (take < want) Denied++;
                cash[r] -= take * p; supply -= take;
                stock[r * Goods + g] += take;
            }
            residual[g] = supply;
        }
        for (int g = 0; g < Goods; g++)
        {
            long q = price[g] + (residual[g] > 300 ? -1 : 1) + (goodId[g] & 1);
            price[g] = q < 50 ? 50 : q;
        }
        for (int r = 0; r < rows; r++) cash[r] = 1000 + (cash[r] & 4095);
        for (int i = 0; i < n; i++)
        {
            long t = stock[i] * spoilNum[i] + carry[i];
            long lost = t / 1000;
            carry[i] = t - lost * 1000;
            stock[i] -= lost;
        }
        // checksum over the cells present, in row id then good id order (positions are ascending in id in both layouts)
        long h = Checksum;
        for (int r = 0; r < rows; r++) if (rowOn[r]) for (int g = 0; g < Goods; g++) if (goodOn[g]) h = h * 31 + stock[r * Goods + g];
        for (int g = 0; g < Goods; g++) if (goodOn[g]) h = h * 31 + price[g];
        Checksum = h;
    }
}
