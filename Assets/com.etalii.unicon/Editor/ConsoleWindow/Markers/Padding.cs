namespace EtAlii.UniCon.Editor
{
    using System.IO;
    using System.Linq;
    using Serilog.Parsing;

    static class Padding
    {
        static readonly char[] PaddingChars = Enumerable.Repeat(' ', 80).ToArray();

        /// <summary>
        /// Writes the provided value to the output, applying direction-based padding when <paramref name="alignment"/> is provided.
        /// </summary>
        public static void Apply(TextWriter output, string value, in Alignment? alignment)
        {
            if (alignment == null || value.Length >= alignment.Value.Width)
            {
                output.Write(value);
                return;
            }

            var pad = alignment.Value.Width - value.Length;

            if (alignment.Value.Direction == AlignmentDirection.Left)
                output.Write(value);

            if (pad <= PaddingChars.Length)
            {
                output.Write(PaddingChars, 0, pad);
            }
            else
            {
                output.Write(new string(' ', pad));
            }

            if (alignment.Value.Direction == AlignmentDirection.Right)
                output.Write(value);
        }

#if FEATURE_WRITE_STRINGBUILDER
    /// <summary>
    /// Writes the provided value to the output, applying direction-based padding when <paramref name="alignment"/> is provided.
    /// This is a full copy of the method above that allows to write <see cref="StringBuilder"/> directly into provided
    /// <paramref name="output"/> without <see cref="StringBuilder.ToString()"/> call on the caller side.
    /// </summary>
    public static void Apply(TextWriter output, StringBuilder value, in Alignment? alignment)
    {
        if (alignment == null || value.Length >= alignment.Value.Width)
        {
            output.Write(value);
            return;
        }

        var pad = alignment.Value.Width - value.Length;

        if (alignment.Value.Direction == AlignmentDirection.Left)
            output.Write(value);

        if (pad <= PaddingChars.Length)
        {
            output.Write(PaddingChars, 0, pad);
        }
        else
        {
            output.Write(new string(' ', pad));
        }

        if (alignment.Value.Direction == AlignmentDirection.Right)
            output.Write(value);
    }
#endif
    }
}

