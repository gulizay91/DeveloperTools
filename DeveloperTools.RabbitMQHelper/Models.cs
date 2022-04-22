// ref: https://app.quicktype.io/

namespace DeveloperTools.RabbitMQHelper
{
    using System;

    using System.Globalization;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;

    public partial class Nodes
    {
        [JsonProperty("rabbit_version")]
        public string RabbitVersion { get; set; }

        [JsonProperty("parameters")]
        public object[] Parameters { get; set; }

        [JsonProperty("policies")]
        public object[] Policies { get; set; }

        [JsonProperty("queues")]
        public Queue[] Queues { get; set; }

        [JsonProperty("exchanges")]
        public Exchange[] Exchanges { get; set; }

        [JsonProperty("bindings")]
        public Binding[] Bindings { get; set; }
    }

    public partial class Binding
    {
        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("destination")]
        public string Destination { get; set; }

        [JsonProperty("destination_type")]
        public DestinationType DestinationType { get; set; }

        [JsonProperty("routing_key")]
        public string RoutingKey { get; set; }

        [JsonProperty("arguments")]
        public Arguments Arguments { get; set; }
    }

    public partial class Arguments
    {
    }

    public partial class Exchange
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("type")]
        public TypeEnum Type { get; set; }

        [JsonProperty("durable")]
        public bool Durable { get; set; }

        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; }

        [JsonProperty("internal")]
        public bool Internal { get; set; }

        [JsonProperty("arguments")]
        public Arguments Arguments { get; set; }
    }

    public partial class Queue
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("durable")]
        public bool Durable { get; set; }

        [JsonProperty("auto_delete")]
        public bool AutoDelete { get; set; }

        [JsonProperty("arguments")]
        public Arguments Arguments { get; set; }
    }

    public enum DestinationType { Exchange, Queue };

    public enum TypeEnum { Direct, Fanout, Topic, Header };

    internal static class Converter
    {
        public static readonly JsonSerializerSettings Settings = new JsonSerializerSettings
        {
            MetadataPropertyHandling = MetadataPropertyHandling.Ignore,
            DateParseHandling = DateParseHandling.None,
            Converters =
            {
                DestinationTypeConverter.Singleton,
                TypeEnumConverter.Singleton,
                new IsoDateTimeConverter { DateTimeStyles = DateTimeStyles.AssumeUniversal }
            },
        };
    }

    internal class DestinationTypeConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(DestinationType) || t == typeof(DestinationType?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            switch (value)
            {
                case "exchange":
                    return DestinationType.Exchange;
                case "queue":
                    return DestinationType.Queue;
            }
            throw new Exception("Cannot unmarshal type DestinationType");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (DestinationType)untypedValue;
            switch (value)
            {
                case DestinationType.Exchange:
                    serializer.Serialize(writer, "exchange");
                    return;
                case DestinationType.Queue:
                    serializer.Serialize(writer, "queue");
                    return;
            }
            throw new Exception("Cannot marshal type DestinationType");
        }

        public static readonly DestinationTypeConverter Singleton = new DestinationTypeConverter();
    }

    internal class TypeEnumConverter : JsonConverter
    {
        public override bool CanConvert(Type t) => t == typeof(TypeEnum) || t == typeof(TypeEnum?);

        public override object ReadJson(JsonReader reader, Type t, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;
            var value = serializer.Deserialize<string>(reader);
            if (value == "fanout")
            {
                return TypeEnum.Fanout;
            }
            throw new Exception("Cannot unmarshal type TypeEnum");
        }

        public override void WriteJson(JsonWriter writer, object untypedValue, JsonSerializer serializer)
        {
            if (untypedValue == null)
            {
                serializer.Serialize(writer, null);
                return;
            }
            var value = (TypeEnum)untypedValue;
            if (value == TypeEnum.Fanout)
            {
                serializer.Serialize(writer, "fanout");
                return;
            }
            throw new Exception("Cannot marshal type TypeEnum");
        }

        public static readonly TypeEnumConverter Singleton = new TypeEnumConverter();
    }
}
