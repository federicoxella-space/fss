using System;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// NFR-01, AC-02, and SIM-STATE's serialisation note: "the state hash covers every
    /// field above. A field excluded from the hash is a determinism hole."
    /// </summary>
    /// <remarks>
    /// <c>StateHash</c> cannot enumerate its own fields — reflection is banned in the
    /// core by NFR-08 and §20 — so its list of folds is written out by hand and this
    /// fixture is what keeps the hand honest. The test assembly is not the core and
    /// reflects freely, the arrangement <c>WorldStateTests</c> and
    /// <c>NoFloatingPointTest</c> already run under.
    /// </remarks>
    [TestFixture]
    public sealed class StateHashTests
    {
        private const BindingFlags DeclaredInstance =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.DeclaredOnly;

        /// <summary>
        /// Every field, perturbed one at a time, moves the digest. A field appended to
        /// <see cref="WorldState"/> in a later phase and left out of the fold fails
        /// here without anyone remembering this test exists — which is the only reason
        /// it is written by enumeration rather than as five assertions.
        /// </summary>
        [Test]
        public void NFR01_EveryStateFieldEntersTheHash()
        {
            var unreached = new List<string>();

            foreach (Path path in Paths(typeof(WorldState), "WorldState"))
            {
                WorldState state = Reference();
                ulong before = StateHash.Of(state);
                path.Perturb(state);

                if (StateHash.Of(state) == before)
                {
                    unreached.Add(path.Name);
                }
            }

            Assert.That(
                unreached,
                Is.Empty,
                "fields outside the state hash, each one a determinism hole " +
                "(NFR-01, AC-02, SIM-STATE §Serialisation notes):" + Environment.NewLine +
                string.Join(Environment.NewLine, unreached));
        }

        /// <summary>
        /// The enumeration reached something. An empty walk makes the test above pass
        /// against a hash of nothing at all, silently and for the rest of the project's
        /// life; the leaves are spelt out so that a field removed from
        /// <see cref="WorldState"/> is noticed here too.
        /// </summary>
        [Test]
        public void NFR01_TheWalkReachesEveryLeafOfTheWorldRow()
        {
            var names = new List<string>();
            foreach (Path path in Paths(typeof(WorldState), "WorldState"))
            {
                names.Add(path.Name);
            }

            names.Sort(StringComparer.Ordinal);

            Assert.That(
                names,
                Is.EqualTo(new[]
                {
                    "WorldState.CurrencyTotal",
                    "WorldState.GenerationParams.SettlementCount",
                    "WorldState.RuleVersion",
                    "WorldState.Tick",
                    "WorldState.WorldSeed",
                }));
        }

        /// <summary>
        /// NFR-01 asks for an identical hash from identical inputs, which is the half
        /// the perturbation test cannot see: a fold returning a fresh value on every
        /// call would move for every field and pass it.
        /// </summary>
        [Test]
        public void NFR01_EqualStatesHashEqual()
        {
            Assert.That(StateHash.Of(Reference()), Is.EqualTo(StateHash.Of(Reference())));
            Assert.That(
                StateHash.Of(new WorldState(0, default, 0)),
                Is.EqualTo(StateHash.Of(new WorldState(0, default, 0))));
        }

        /// <summary>
        /// The digest of <see cref="Reference"/>, written out. AC-02 compares hash
        /// sequences <em>across builds and machines</em>, and nothing else in this
        /// fixture would notice a change to the fold: reorder two folds, widen a cast,
        /// change the seed, and the three tests above stay green while every world ever
        /// saved hashes differently. This is the same guard, and the same reason, as the
        /// pinned mixing vectors in <c>PrimitivesTests</c>.
        /// </summary>
        /// <remarks>
        /// A change here is not a test to update. It is a save format change, and it
        /// belongs to the migration path NFR-08 requires from the first write.
        /// </remarks>
        [Test]
        public void NFR01_TheDigestIsPinned()
        {
            Assert.That(StateHash.Of(Reference()), Is.EqualTo(0xCE31A6F9C074C14DUL));
        }

        /// <summary>
        /// The tripwire rejects what it is there to reject. <see cref="Different"/>
        /// failing on a type it cannot perturb is what makes an unhandled field loud
        /// instead of skipped; a permissive fallback put there later to quiet a phase-4
        /// array would pass every other test in this file.
        /// </summary>
        [Test]
        public void NFR01_ThePerturbationRejectsWhatItCannotChange()
        {
            Assert.Throws<AssertionException>(() => Different(typeof(long[]), null), "an array is not a leaf this fixture can change");
            Assert.Throws<AssertionException>(() => Different(typeof(string), null));
            Assert.Throws<AssertionException>(() => Different(typeof(double), 0.0), "and NFR-02 keeps this one out of state anyway");

            Assert.That(Different(typeof(long), 1L), Is.Not.EqualTo(1L));
            Assert.That(Different(typeof(HashChannel), HashChannel.Market), Is.Not.EqualTo((int)HashChannel.Market));
        }

        /// <summary>
        /// A state whose fields are all distinct and none of them zero, so that a fold
        /// omitting one is not covered up by the omitted field happening to be the
        /// default.
        /// </summary>
        private static WorldState Reference() =>
            new WorldState(0x0123456789ABCDEFUL, new GenerationParams(2000), 7)
            {
                Tick = 41,
                CurrencyTotal = 1000003,
            };

        /// <summary>
        /// One leaf of the state, and how to change it. Composites are walked through
        /// rather than perturbed whole: perturbing <c>GenerationParams</c> as a unit
        /// would prove the struct reaches the hash, not that each of its members does,
        /// and the two coincide only while it has one member (D-067).
        /// </summary>
        private readonly struct Path
        {
            public readonly string Name;
            private readonly FieldInfo[] _chain;

            public Path(string name, FieldInfo[] chain)
            {
                Name = name;
                _chain = chain;
            }

            /// <summary>
            /// Writes a different value into the leaf. Structs on the way down are
            /// boxed, written, and assigned back into their parent, because a value
            /// type read out of a <see cref="FieldInfo"/> is a copy and writing to it
            /// otherwise changes nothing.
            /// </summary>
            public void Perturb(WorldState state)
            {
                object[] boxes = new object[_chain.Length];
                object owner = state;

                for (int i = 0; i < _chain.Length - 1; i++)
                {
                    boxes[i] = _chain[i].GetValue(owner);
                    owner = boxes[i];
                }

                FieldInfo leaf = _chain[_chain.Length - 1];
                leaf.SetValue(owner, Different(leaf.FieldType, leaf.GetValue(owner)));

                for (int i = _chain.Length - 2; i >= 0; i--)
                {
                    object parent = i == 0 ? (object)state : boxes[i - 1];
                    _chain[i].SetValue(parent, boxes[i]);
                }
            }
        }

        /// <summary>
        /// Every leaf of <paramref name="type"/>, depth first, in declaration order.
        /// </summary>
        /// <remarks>
        /// A leaf is a field this fixture knows how to change. A field of any other
        /// shape — an array, a nested reference — fails in <see cref="Different"/>
        /// rather than being skipped: a type this walk cannot perturb is a field it
        /// cannot vouch for, and the phase that introduces one has to say how it enters
        /// the hash. That is the same tripwire as the missing fold, one level up.
        /// </remarks>
        private static List<Path> Paths(Type type, string where)
        {
            var paths = new List<Path>();
            Walk(paths, type, where, new List<FieldInfo>());
            return paths;
        }

        private static void Walk(List<Path> paths, Type type, string where, List<FieldInfo> chain)
        {
            foreach (FieldInfo field in type.GetFields(DeclaredInstance))
            {
                chain.Add(field);
                string name = where + "." + field.Name;

                if (IsComposite(field.FieldType))
                {
                    Walk(paths, field.FieldType, name, chain);
                }
                else
                {
                    paths.Add(new Path(name, chain.ToArray()));
                }

                chain.RemoveAt(chain.Count - 1);
            }
        }

        /// <summary>
        /// A value type carrying fields of its own, which is what
        /// <c>GenerationParams</c> is. Enums are their underlying integer and
        /// primitives are leaves; a reference type is neither, and NFR-10 forbids one
        /// here already, so it falls to <see cref="Different"/> to reject.
        /// </summary>
        private static bool IsComposite(Type type) =>
            type.IsValueType && !type.IsPrimitive && !type.IsEnum;

        /// <summary>
        /// A value of <paramref name="type"/> that is not <paramref name="current"/>.
        /// </summary>
        private static object Different(Type type, object current)
        {
            if (type.IsEnum)
            {
                current = Convert.ChangeType(current, Enum.GetUnderlyingType(type));
                type = Enum.GetUnderlyingType(type);
            }

            if (type == typeof(long)) { return unchecked((long)current + 1); }
            if (type == typeof(ulong)) { return unchecked((ulong)current + 1); }
            if (type == typeof(int)) { return unchecked((int)current + 1); }
            if (type == typeof(uint)) { return unchecked((uint)current + 1); }
            if (type == typeof(short)) { return unchecked((short)((short)current + 1)); }
            if (type == typeof(ushort)) { return unchecked((ushort)((ushort)current + 1)); }
            if (type == typeof(byte)) { return unchecked((byte)((byte)current + 1)); }
            if (type == typeof(sbyte)) { return unchecked((sbyte)((sbyte)current + 1)); }
            if (type == typeof(bool)) { return !(bool)current; }

            Assert.Fail(
                "this fixture cannot perturb " + type.FullName +
                ", so it cannot say whether that field reaches the state hash. " +
                "Teach it the type, or the field goes unverified (NFR-01).");
            return null;
        }
    }
}
