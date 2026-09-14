using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using NUnit.Framework;

namespace Sim.Tests
{
    /// <summary>
    /// AC-58 and NFR-02. The banned-symbol analyser catches a reference to
    /// <c>System.Double</c>, but not the keyword: <c>static double X()</c> compiles
    /// clean, and so does integer-looking code that goes through the float unit on the
    /// stack. Only the compiled assembly knows, so that is what this reads.
    /// </summary>
    /// <remarks>
    /// Reflection is banned inside the core because it breaks ahead-of-time compilation.
    /// The test assembly is not the core and is free to use it.
    /// </remarks>
    [TestFixture]
    public sealed class NoFloatingPointTest
    {
        private const BindingFlags AllDeclared =
            BindingFlags.Public | BindingFlags.NonPublic |
            BindingFlags.Instance | BindingFlags.Static |
            BindingFlags.DeclaredOnly;

        [Test]
        public void AC58_NoFloatingPointInTheCore()
        {
            Assembly core = typeof(EntityId).Assembly;
            var offenders = new List<string>();

            foreach (Type type in core.GetTypes())
            {
                foreach (FieldInfo field in type.GetFields(AllDeclared))
                {
                    Check(offenders, field.FieldType, type, "field " + field.Name);
                }

                foreach (PropertyInfo property in type.GetProperties(AllDeclared))
                {
                    Check(offenders, property.PropertyType, type, "property " + property.Name);
                }

                foreach (ConstructorInfo constructor in type.GetConstructors(AllDeclared))
                {
                    CheckMethod(offenders, constructor, type, null);
                }

                foreach (MethodInfo method in type.GetMethods(AllDeclared))
                {
                    CheckMethod(offenders, method, type, method.ReturnType);
                }
            }

            Assert.That(
                offenders,
                Is.Empty,
                "floating point in the core (NFR-02, DEC-001):" + Environment.NewLine +
                string.Join(Environment.NewLine, offenders));
        }

        private static void CheckMethod(List<string> offenders, MethodBase method, Type type, Type returnType)
        {
            string where = method.Name;

            if (returnType != null)
            {
                Check(offenders, returnType, type, where + " return type");
            }

            foreach (ParameterInfo parameter in method.GetParameters())
            {
                Check(offenders, parameter.ParameterType, type, where + " parameter " + parameter.Name);
            }

            MethodBody body = method.GetMethodBody();
            if (body == null)
            {
                return;
            }

            foreach (LocalVariableInfo local in body.LocalVariables)
            {
                Check(offenders, local.LocalType, type, where + " local slot " + local.LocalIndex.ToString());
            }

            // A local slot is not the whole story. `(int)(a * 15) / 10` is honest integer
            // work, `(int)(a * 1.5)` is a double multiply the optimiser keeps entirely on
            // the stack: no signature and no local mentions it, and the result is not
            // reproducible across builds. The instructions are the only witness.
            foreach (string instruction in FloatingPointInstructions(body))
            {
                offenders.Add(type.FullName + "." + where + ": " + instruction);
            }
        }

        private static void Check(List<string> offenders, Type candidate, Type type, string where)
        {
            if (IsFloatingPoint(candidate))
            {
                offenders.Add(type.FullName + "." + where + ": " + candidate.Name);
            }
        }

        private static bool IsFloatingPoint(Type type)
        {
            if (type == null)
            {
                return false;
            }

            if (type.IsByRef || type.IsPointer || type.IsArray)
            {
                return IsFloatingPoint(type.GetElementType());
            }

            if (type == typeof(float) || type == typeof(double) || type == typeof(decimal))
            {
                return true;
            }

            if (type.IsGenericType)
            {
                foreach (Type argument in type.GetGenericArguments())
                {
                    if (IsFloatingPoint(argument))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        /// Names of the floating-point instructions in a method body. Every one of them
        /// ends in <c>.r4</c>, <c>.r8</c> or <c>.r.un</c> — ldc, conv, ldind, stind,
        /// ldelem, stelem — so the suffix is the rule and there is no list to keep
        /// current.
        /// </summary>
        private static IEnumerable<string> FloatingPointInstructions(MethodBody body)
        {
            byte[] il = body.GetILAsByteArray();
            if (il == null)
            {
                yield break;
            }

            int position = 0;
            while (position < il.Length)
            {
                short value = il[position];
                position++;
                if (value == 0xFE && position < il.Length)
                {
                    value = unchecked((short)(0xFE00 | il[position]));
                    position++;
                }

                if (!OpCodesByValue.TryGetValue(value, out OpCode opCode))
                {
                    // An opcode this walker does not know means the walk is no longer
                    // aligned, and a silent stop would turn into silent coverage.
                    throw new InvalidOperationException(
                        "unknown IL opcode 0x" + value.ToString("X4") + "; the instruction walker needs updating");
                }

                string name = opCode.Name;
                if (name.EndsWith(".r4", StringComparison.Ordinal) ||
                    name.EndsWith(".r8", StringComparison.Ordinal) ||
                    name.EndsWith(".r.un", StringComparison.Ordinal))
                {
                    yield return name;
                }

                position += OperandSize(opCode, il, position);
            }
        }

        private static int OperandSize(OpCode opCode, byte[] il, int position)
        {
            switch (opCode.OperandType)
            {
                case OperandType.InlineNone:
                    return 0;
                case OperandType.ShortInlineBrTarget:
                case OperandType.ShortInlineI:
                case OperandType.ShortInlineVar:
                    return 1;
                case OperandType.InlineVar:
                    return 2;
                case OperandType.InlineI8:
                case OperandType.InlineR:
                    return 8;
                case OperandType.InlineSwitch:
                    // A four-byte count followed by that many four-byte targets.
                    return 4 + 4 * BitConverter.ToInt32(il, position);
                default:
                    return 4;
            }
        }

        private static readonly Dictionary<short, OpCode> OpCodesByValue = BuildOpCodeTable();

        private static Dictionary<short, OpCode> BuildOpCodeTable()
        {
            var table = new Dictionary<short, OpCode>();
            foreach (FieldInfo field in typeof(OpCodes).GetFields(BindingFlags.Public | BindingFlags.Static))
            {
                if (field.FieldType == typeof(OpCode))
                {
                    var opCode = (OpCode)field.GetValue(null);
                    table[opCode.Value] = opCode;
                }
            }

            return table;
        }
    }
}
