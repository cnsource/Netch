using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using Netch.Models;

namespace Netch.Utils
{
    public class ServerConverterWithTypeDiscriminator : JsonConverter<Server>
    {
#nullable enable
        public override Server Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var jsonDocument = JsonSerializer.Deserialize<JsonDocument>(ref reader);

            if (!jsonDocument!.RootElement.TryGetProperty("Type", out var type))
                throw new JsonException();

            var t = ServerHelper.GetTypeByTypeName(type.GetString());
            var m = typeof(JsonExtensions).GetMethod("ToObject")!;
            var gm = m.MakeGenericMethod(t);
            var result = gm.Invoke(null, new object[] {jsonDocument.RootElement, null!});

            return (Server) result;
        }

        public override void Write(Utf8JsonWriter writer, Server value, JsonSerializerOptions options)
        {
            JsonSerializer.Serialize(writer, value, options);
        }
    }

    internal static class JsonExtensions
    {
        public static T ToObject<T>(this JsonElement element, JsonSerializerOptions? options = null)
        {
            return JsonSerializer.Deserialize<T>(element.GetRawText(), options) ?? throw new JsonException();
        }
    }
}