using System;
using System.Globalization;
using Nekres.RotationTrainer.Core.UI.Controls;

namespace Nekres.RotationTrainer.Core.Controls {
    internal sealed class NumericInputPrompt : InputPrompt<int, NumericInputPrompt>
    {
        public NumericInputPrompt(Action<bool, int> callback, string text, string defaultValue, string confirmButtonText, string cancelButtonText) : base(callback, text, defaultValue, confirmButtonText, cancelButtonText) { }

        protected override bool TryParse(string input, out int result) {
            result = 0;
            return int.TryParse(input, NumberStyles.Integer, CultureInfo.InvariantCulture, out result) && result >= 0;
        }
    }
}
