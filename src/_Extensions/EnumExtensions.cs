using System;
using System.Text.RegularExpressions;

namespace Nekres.RotationTrainer {
    internal static class EnumExtensions {
        public static string ToFriendlyString(this Enum e) {
            return Regex.Replace(e.ToString(), "([A-Z]|[1-9])", " $1").Trim();
        }
    }
}
