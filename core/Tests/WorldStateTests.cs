using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// NFR-10 and DEC-003. Serialised state holds no object references, so that it
    /// serialises, copies, hashes and compares in bulk — which is what AC-02 and AC-03
    /// rest on. The compiler has no opinion about this; the assembly does.
    /// </summary>
    /// <remarks>
    /// Reflection is banned inside the core because it breaks ahead-of-time
    /// compilation. The test assembly is not the core and is free to use it, the same
    /// arrangement <c>NoFloatingPointTest</c> already runs under.
    /// </remarks>
    [TestFixture]
    public sealed class WorldStateTests
    {
        private const BindingFlags DeclaredInstance =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly;

        [Test]
        public void NFR10_StateHoldsNoObjectReferences()
        {
            var offenders = new List<string>();

            foreach (FieldInfo field in typeof(WorldState).GetFields(DeclaredInstance))
            {
                Check(offenders, field.FieldType, "WorldState." + field.Name);
            }

            Assert.That(
                offenders,
                Is.Empty,
                "object references inside serialised state (NFR-10, DEC-003):" + Environment.NewLine +
                string.Join(Environment.NewLine, offenders));
        }

        /// <summary>
        /// The five fields of SIM-STATE's World row, no more and no fewer (FR-W-01).
        /// A missing one is state the save cannot carry; a sixth is not a violation but
        /// a widening, and the list is here so that the widening is deliberate. Points
        /// 5 and 6 of this plan do it twice — the command queue and the chronicle both
        /// influence a future tick and both belong in state — and phase 4 does it for
        /// every parallel array it adds.
        /// </summary>
        [Test]
        public void FRW01_WorldStateHoldsTheWorldRowAndNothingElse()
        {
            var names = new List<string>();
            foreach (FieldInfo field in typeof(WorldState).GetFields(DeclaredInstance))
            {
                names.Add(field.Name);
            }

            names.Sort(StringComparer.Ordinal);

            Assert.That(
                names,
                Is.EqualTo(new[] { "CurrencyTotal", "GenerationParams", "RuleVersion", "Tick", "WorldSeed" }));
        }

        /// <summary>
        /// A state that is not a determinism hole is one whose fields survive a save.
        /// The types SIM-STATE gives are the ones the serialiser of point 7 will write,
        /// and a widened or narrowed field is a save format changed by accident.
        /// </summary>
        [Test]
        public void FRW01_TheWorldRowCarriesTheDeclaredTypes()
        {
            var state = new WorldState(1, new GenerationParams(2000), 3);

            Assert.That(typeof(WorldState).GetField("Tick").FieldType, Is.EqualTo(typeof(long)));
            Assert.That(typeof(WorldState).GetField("WorldSeed").FieldType, Is.EqualTo(typeof(ulong)));
            Assert.That(typeof(WorldState).GetField("RuleVersion").FieldType, Is.EqualTo(typeof(int)));
            Assert.That(typeof(WorldState).GetField("CurrencyTotal").FieldType, Is.EqualTo(typeof(long)));

            Assert.That(state.Tick, Is.Zero, "a new world starts at tick 0");
            Assert.That(state.CurrencyTotal, Is.Zero, "an empty world has no currency (A-02, A-03)");
            Assert.That(state.WorldSeed, Is.EqualTo(1UL));
            Assert.That(state.GenerationParams.SettlementCount, Is.EqualTo(2000));
            Assert.That(state.RuleVersion, Is.EqualTo(3));
        }

        /// <summary>
        /// Guard on the walker itself. A checker that passes everything passes
        /// <see cref="WorldState"/> too, and would do it silently for the rest of the
        /// project's life.
        /// </summary>
        [Test]
        public void NFR10_TheWalkerRejectsWhatItIsThereToReject()
        {
            Assert.That(Offenders(typeof(string)), Is.Not.Empty, "string is a reference type");
            Assert.That(Offenders(typeof(int[][])), Is.Not.Empty, "a jagged array is an array of references");
            Assert.That(Offenders(typeof(List<int>)), Is.Not.Empty);
            Assert.That(Offenders(typeof(WorldState)), Is.Not.Empty, "the container is not allowed inside itself");
            Assert.That(Offenders(typeof(HoldsAReference)), Is.Not.Empty, "a struct hiding a reference one level down");

            Assert.That(Offenders(typeof(long)), Is.Empty);
            Assert.That(Offenders(typeof(HashChannel)), Is.Empty, "an enum is its underlying integer");
            Assert.That(Offenders(typeof(EntityId)), Is.Empty, "the handle NFR-10 names as the replacement");
            Assert.That(Offenders(typeof(EntityId[])), Is.Empty, "struct-of-arrays needs arrays of value types");
        }

        private struct HoldsAReference
        {
#pragma warning disable CS0649 // never assigned: the walker reads the type, not the value
            public EntityId Handle;
            public object Escape;
#pragma warning restore CS0649
        }

        private static List<string> Offenders(Type type)
        {
            var offenders = new List<string>();
            Check(offenders, type, type.Name);
            return offenders;
        }

        /// <summary>
        /// A type is state-safe when it is a value type all the way down, or a flat
        /// array of one. The array itself is a reference and is allowed: struct-of-arrays
        /// is made of them, and NFR-10 bans references <em>inside</em> the state — a row
        /// naming another row, which is what <see cref="EntityId"/> exists to do instead.
        /// One level only: a jagged array is an array holding references, which is the
        /// object graph the rule is against. Rectangular arrays fall through the same
        /// branch, so <c>int[,]</c> passes and <c>int[][]</c> does not.
        ///
        /// <para>
        /// The walk cannot cycle, and not because a struct cannot contain itself — it
        /// can, through an array field, which the CLR permits. It cannot cycle because
        /// the only recursion that descends passes <c>arrayAllowed: false</c>, and an
        /// array met that way is reported without being entered. A later phase that
        /// relaxes that flag reopens the question and needs a visited set.
        /// </para>
        /// </summary>
        private static void Check(List<string> offenders, Type type, string where, bool arrayAllowed = true)
        {
            if (type.IsArray)
            {
                if (!arrayAllowed)
                {
                    offenders.Add(where + ": " + type.FullName);
                    return;
                }

                Check(offenders, type.GetElementType(), where + "[]", false);
                return;
            }

            if (!type.IsValueType)
            {
                offenders.Add(where + ": " + type.FullName);
                return;
            }

            if (type.IsPrimitive || type.IsEnum)
            {
                return;
            }

            foreach (FieldInfo field in type.GetFields(DeclaredInstance))
            {
                Check(offenders, field.FieldType, where + "." + field.Name, false);
            }
        }
    }
}
