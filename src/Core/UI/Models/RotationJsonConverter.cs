using Nekres.RotationTrainer.Player.Models;
using Newtonsoft.Json;
using System;

namespace Nekres.RotationTrainer.Core.UI.Models {
    internal class RotationJsonConverter : JsonConverter {
        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer) {
            if (value == null) {
                return;
            }
            writer.WriteValue(value.ToString());
        }

        public override object? ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer) {
            if (reader.Value != null && reader.Value.GetType() != typeof(string)) {
                return null;
            }
            return Rotation.TryParse((string)reader.Value, out var value) ? value : throw new JsonReaderException("Unable to deserialize rotation.");
        }

        public override bool CanConvert(Type objectType) {
            return objectType == typeof(string);
        }
    }
}
