using Newtonsoft.Json;

namespace Nekres.RotationTrainer {
    internal static class TaskUtil {
        public static bool TryParseJson<T>(string json, out T result) {
            bool success = true;
            var settings = new JsonSerializerSettings {
                Error                 = (_, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(json, settings);
            return success;
        }
    }
}
