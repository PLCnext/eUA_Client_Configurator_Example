// Copyright PHOENIX CONTACT Electronics GmbH

using Opc.Ua;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Arp.OpcUA.ClientConfiguration
{
    internal class ExpandedNodeIdJsonConverter : JsonConverter<ExpandedNodeId>
    {
        public override ExpandedNodeId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            return ExpandedNodeId.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, ExpandedNodeId value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value.Format());
        }
    }
}
