using System;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

namespace Sim.Bench;

// Stand-in for the settlement update: five passes over dense per-good arrays, integer
// arithmetic only. It is not the simulator; its numbers mean what it resembles (SQ-004).
// The sizes are measurement inputs, not simulation constants.
sealed class StandIn
{
    const int Goods = 7;
    readonly int rows;
    readonly long[] stock, prod, need, cash, price, carry, spoilNum, paid;
    readonly int[] order;
    readonly long[] key, residual;
    public long Checksum;

    public StandIn(int rows)
    {
        this.rows = rows;
        int n = rows * Goods;
        stock = new long[n]; prod = new long[n]; need = new long[n]; carry = new long[n];
        spoilNum = new long[n]; paid = new long[n];
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
        // 3. market clearing: per good, rank rows by ability to pay (selection sort, quadratic in rows)
        for (int g = 0; g < Goods; g++)
        {
            for (int r = 0; r < rows; r++) { order[r] = r; key[r] = cash[r] + stock[r * Goods + g]; }
            // selection sort by descending key: n(n-1)/2 comparisons whatever the data
            for (int a = 0; a < rows - 1; a++)
            {
                int m = a;
                for (int b = a + 1; b < rows; b++) if (key[b] > key[m]) m = b;
                int t = order[a]; order[a] = order[m]; order[m] = t;
                long u = key[a]; key[a] = key[m]; key[m] = u;
            }
            long supply = 1000 + g * 37, p = price[g];
            for (int k = 0; k < rows && supply > 0; k++)
            {
                int r = order[k];
                long want = cash[r] / (p * 40);
                long take = want < supply ? want : supply;
                long cost = take * p;
                cash[r] -= cost; paid[r * Goods + g] = cost; supply -= take;
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
        // refill cash so the state stays in range across 10^4+ updates
        for (int r = 0; r < rows; r++) cash[r] += 600;
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

    static int Main()
    {
        try
        {
            var p = Process.GetCurrentProcess();
            p.ProcessorAffinity = (IntPtr)(1L << (Environment.ProcessorCount > 2 ? 2 : 0));
            p.PriorityClass = ProcessPriorityClass.High;
        }
        catch (Exception e) { Console.WriteLine("pinning failed: " + e.Message); return 2; }

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
}
