// Copyright (c) Peter Vrenken. All rights reserved. 

namespace EtAlii.Unicon
{
    using System.IO;
    using Serilog.Events;
    using Serilog.Parsing;

    // ReSharper disable once InconsistentNaming
    public static partial class LogEntrySerialization
    {
        private static void SerializeMessageTemplate(MessageTemplate messageTemplate, BinaryWriter writer)
        {
            writer.Write(messageTemplate.Text);
            writer.Write(((MessageTemplateToken[])messageTemplate.Tokens).Length);
            foreach (var token in messageTemplate.Tokens)
            {
                SerializeMessageTemplateToken(token, writer);
            }
        }

        private static MessageTemplate DeserializeMessageTemplate(BinaryReader reader)
        {
            var text = reader.ReadString();
            var tokenCount = reader.ReadInt32();
            var tokens = new MessageTemplateToken[tokenCount];
            for (var i = 0; i < tokenCount; i++)
            {
                tokens[i] = DeserializeMessageTemplateToken(reader);
            }
            return new MessageTemplate(text, tokens);
        }

        private static void SerializeMessageTemplateToken(MessageTemplateToken token, BinaryWriter writer)
        {
            var isProperty = token is PropertyToken;
            writer.Write(isProperty);
            if (isProperty)
            {
                var propertyToken = (PropertyToken)token;
                writer.Write(propertyToken.PropertyName);
                writer.Write(propertyToken.ToString());
                var hasFormat = propertyToken.Format != null;
                if (hasFormat)
                {
                    writer.Write(true);
                    writer.Write(propertyToken.Format);
                }
                else
                {
                    writer.Write(false);
                }

                if (propertyToken.Alignment.HasValue)
                {
                    writer.Write(true);
                    writer.Write((byte)propertyToken.Alignment.Value.Direction);
                    writer.Write(propertyToken.Alignment.Value.Width);
                }
                else
                {
                    writer.Write(false);
                }
                writer.Write((byte)propertyToken.Destructuring);
            }
            else
            {
                var textToken = (TextToken)token;
                writer.Write(textToken.Text);
            }
        }

        private static MessageTemplateToken DeserializeMessageTemplateToken(BinaryReader reader)
        {
            var isProperty = reader.ReadBoolean();
            if (isProperty)
            {
                var propertyName = reader.ReadString();
                var rawText = reader.ReadString();
                var hasFormat = reader.ReadBoolean();
                var format = hasFormat ? reader.ReadString() : null;
                Alignment? alignment = null;
                var hasAlignment = reader.ReadBoolean();
                if (hasAlignment)
                {
                    var direction = (AlignmentDirection)reader.ReadByte();
                    var width = reader.ReadInt32();
                    alignment = new Alignment(direction, width);
                }
                var destructuring = (Destructuring)reader.ReadByte();
                return new PropertyToken(propertyName, rawText, format, alignment, destructuring);
            }

            var text = reader.ReadString();
            return new TextToken(text);
        }
    }
}