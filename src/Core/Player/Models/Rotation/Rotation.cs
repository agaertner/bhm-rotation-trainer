using Nekres.RotationTrainer.Core.Player.Models;
using System;
using System.Collections.Generic;
using Action = Nekres.RotationTrainer.Core.Player.Models.Action;

namespace Nekres.RotationTrainer.Player.Models {
    internal class Rotation : RotationBase<Action> {
        
        public Rotation(IEnumerable<Action> abilities) : base(abilities) {
            /* NOOP */
        }

        public Rotation() : base(new List<Action>()) {
            /* NOOP */
        }

        public static bool TryParse(string rawRotation, out Rotation rotation, int[] utilOrder = null) {
            rotation = null;

            if (string.IsNullOrEmpty(rawRotation)) {
                return false;
            }

            var abilities = new List<Action>();

            string[] actions = rawRotation.Split(new []{ DELIMITER }, StringSplitOptions.None);

            foreach (string expression in actions) {

                if (!Action.TryParse(expression, out var action, utilOrder)) {
                    continue;
                }

                // Add ability to rotation
                abilities.Add(action);
            }

            rotation = new Rotation(abilities);
            return true;
        }

    }
}
