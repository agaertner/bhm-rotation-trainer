using System;
using System.Collections.Generic;
using Blish_HUD.Input;

namespace Nekres.RotationTrainer.Player.Models {
    internal class Ability : IDisposable {

        public event EventHandler<EventArgs> Activated;

        public GuildWarsAction Action { get; }

        public int Duration { get; }

        public int Repetitions { get; }

        private KeyBinding _keyBinding;

        public Ability(GuildWarsAction action, KeyBinding keyBinding, int duration = 0, int repetitions = 0) {
            Action = action;
            Duration = duration;
            Repetitions = repetitions;

            _keyBinding           =  keyBinding;
            _keyBinding.Activated += OnActivated;
        }

        private void OnActivated(object o, EventArgs e) {
            Activated?.Invoke(this, EventArgs.Empty);
        }

        public void Dispose() {
            _keyBinding.Activated -= OnActivated;
        }

    }
}
