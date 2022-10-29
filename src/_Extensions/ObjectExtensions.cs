namespace Nekres.RotationTrainer {
    internal static class ObjectExtensions {
        public static bool TryCast<T>(this object obj, out T result) {
            if (obj is T obj1) {
                result = obj1;
                return true;
            }

            result = default(T);
            return false;
        }
    }
}
