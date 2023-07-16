#nullable enable
namespace EtAlii.UniCon.Editor
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.CompilerServices;
    using Serilog.Events;
    using Serilog.Formatting.Json;
    using Serilog.Parsing;

    internal static class MarkerMessageTemplateRenderer
    {
        private const string MarkerTag = "<mark=#11ff1122>";
        private static readonly JsonValueFormatter JsonValueFormatter = new("$type");

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static string Render(
            MessageTemplate messageTemplate,
            IReadOnlyDictionary<string, LogEventPropertyValue> properties)
        {
            using var writer = ReusableStringWriter.GetOrCreate();
            Render(messageTemplate, properties, writer);
            return writer.ToString();
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void Render(
            MessageTemplate messageTemplate,
            IReadOnlyDictionary<string, LogEventPropertyValue> properties, 
            TextWriter output, 
            string? format = null,
            IFormatProvider? formatProvider = null)
        {
            bool isLiteral = false, isJson = false;

            if (format != null)
            {
                for (var i = 0; i < format.Length; ++i)
                {
                    if (format[i] == 'l')
                        isLiteral = true;
                    else if (format[i] == 'j')
                        isJson = true;
                }
            }

            foreach (var token in messageTemplate.Tokens)
            {
                if (token is TextToken tt)
                {
                    RenderTextToken(tt, output);
                }
                else
                {
                    var pt = (PropertyToken)token;
                    RenderPropertyToken(pt, properties, output, formatProvider, isLiteral, isJson);
                }
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RenderTextToken(TextToken tt, TextWriter output)
        {
            output.Write(tt.Text);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static void RenderPropertyToken(PropertyToken pt,
            IReadOnlyDictionary<string, LogEventPropertyValue> properties, TextWriter output,
            IFormatProvider? formatProvider, bool isLiteral, bool isJson)
        {
            if (!properties.TryGetValue(pt.PropertyName, out var propertyValue))
            {
                output.Write(pt.ToString());
                return;
            }

            if (!pt.Alignment.HasValue)
            {
                RenderValue(propertyValue, isLiteral, isJson, output, pt.Format, formatProvider);
                return;
            }

            using var valueOutput = ReusableStringWriter.GetOrCreate();
            RenderValue(propertyValue, isLiteral, isJson, valueOutput, pt.Format, formatProvider);
            var sb = valueOutput.GetStringBuilder();

            if (sb.Length >= pt.Alignment.Value.Width)
            {
#if FEATURE_WRITE_STRINGBUILDER
                output.Write(sb);
#else
                output.Write(sb.ToString());
#endif
                return;
            }

#if FEATURE_WRITE_STRINGBUILDER
            Padding.Apply(output, sb, pt.Alignment.Value);
#else
            Padding.Apply(output, sb.ToString(), pt.Alignment.Value);
#endif
        }

        static void RenderValue(
            LogEventPropertyValue propertyValue, 
            bool literal, 
            bool json, 
            TextWriter output,
            string? format, IFormatProvider? formatProvider)
        {
            output.Write(MarkerTag);
            if (literal && propertyValue is ScalarValue { Value: string str })
            {
                output.Write(str);
            }
            else if (json && format == null)
            {
                JsonValueFormatter.Format(propertyValue, output);
            }
            else
            {
                propertyValue.Render(output, format, formatProvider);
            }
            output.Write("</mark>");
        }
    }
}