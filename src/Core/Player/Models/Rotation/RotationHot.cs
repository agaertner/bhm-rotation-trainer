using System;
using System.Collections.Generic;

namespace Nekres.RotationTrainer.Core.Player.Models {
    internal class RotationHot : RotationBase<ActionHot> {

        public RotationHot(IEnumerable<ActionHot> abilities) : base(abilities) {
            /* NOOP */
        }

        public RotationHot() : base(new List<ActionHot>()) {
            /* NOOP */
        }

        public static bool TryParse(string rawRotation, out RotationHot rotation, int[] utilOrder = null) {
            rotation = null;

            if (string.IsNullOrEmpty(rawRotation)) {
                return false;
            }

            var abilities = new List<ActionHot>();

            string[] actions = rawRotation.Split(new[] { DELIMITER }, StringSplitOptions.None);

            foreach (string expression in actions) {

                if (!ActionHot.TryParse(expression, out var action, utilOrder)) {
                    continue;
                }

                // Add ability to rotation
                abilities.Add(action);
            }

            rotation = new RotationHot(abilities);
            return true;
        }
    }
}
