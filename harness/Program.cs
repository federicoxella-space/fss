using System;
using System.Globalization;
using System.IO;
using Sim;

namespace Sim.Harness
{
    /// <summary>
    /// <c>sim --ticks N --seed S [--hashes]</c>: runs N ticks headless (NFR-12, DEC-035).
    /// With <c>--hashes</c> it prints the state hash after every tick, one per line, so
    /// line n is tick n; without it, the final hash only. Output is 16 lowercase hex
    /// digits and <c>\n</c>, the same bytes on every machine, so CI compares runs by
    /// redirecting stdout (AC-02, AC-19).
    /// </summary>
    internal static class Program
    {
        /// <summary>P-02, the generator default. Phase 3 has no generator; the count only sizes the buckets.</summary>
        private const int SettlementCount = 2000;

        private static int Main(string[] args)
        {
            long ticks = -1;
            ulong? seed = null;
            var hashes = false;

            for (var i = 0; i < args.Length; i++)
            {
                switch (args[i])
                {
                    case "--ticks" when i + 1 < args.Length
                        && long.TryParse(args[i + 1], NumberStyles.None, CultureInfo.InvariantCulture, out ticks):
                        i++;
                        break;
                    case "--seed" when i + 1 < args.Length
                        && ulong.TryParse(args[i + 1], NumberStyles.None, CultureInfo.InvariantCulture, out var s):
                        seed = s;
                        i++;
                        break;
                    case "--hashes":
                        hashes = true;
                        break;
                    default:
                        return Usage("unrecognised or malformed argument: " + args[i]);
                }
            }

            if (ticks < 0 || seed == null)
            {
                return Usage("--ticks and --seed are required");
            }

            var sim = new Simulation(seed.Value, new GenerationParams(SettlementCount));
            using var output = new StreamWriter(Console.OpenStandardOutput()) { NewLine = "\n" };

            if (hashes)
            {
                for (long t = 0; t < ticks; t++)
                {
                    sim.Advance(1);
                    output.WriteLine(sim.StateHash.ToString("x16", CultureInfo.InvariantCulture));
                }
            }
            else
            {
                sim.Advance(ticks);
                output.WriteLine(sim.StateHash.ToString("x16", CultureInfo.InvariantCulture));
            }

            return 0;
        }

        private static int Usage(string error)
        {
            Console.Error.WriteLine("sim: " + error);
            Console.Error.WriteLine("usage: sim --ticks N --seed S [--hashes]");
            return 2;
        }
    }
}
