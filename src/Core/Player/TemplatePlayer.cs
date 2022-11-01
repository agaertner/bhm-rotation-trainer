using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Blish_HUD;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Input;
using Nekres.RotationTrainer.Core.UI.Models;

namespace Nekres.RotationTrainer.Player {
    internal class TemplatePlayer : IDisposable
    {
        private bool           _disposed;

        private CancellationTokenSource _cancelToken;

        internal TemplatePlayer() {
            
        }

        public void Play(TemplateModel.BuildRotation rotation) {

            if (_cancelToken != null) {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }

            var healthPoolBtn = new HealthPoolButton {
                Parent = GameService.Graphics.SpriteScreen,
                Text = "Stop Practicing"
            };
            healthPoolBtn.Click += (o, _) => {
                _cancelToken?.Cancel();
                ((HealthPoolButton)o).Dispose();
            };

            if (RotationTrainerModule.Instance.ActionBindings.Values.Any(x => x.Value.PrimaryKey == Keys.None && x.Value.ModifierKeys == ModifierKeys.None)) {
                ScreenNotification.ShowNotification("Key bindings need to be assigned.");
                return;
            }

            _cancelToken = new CancellationTokenSource();

            var current = new Thread(() => PlayRotation(rotation));
            current.Start();
        }

        private void PlayRotation(TemplateModel.BuildRotation rotation) {
            var opener = new List<Ability>(rotation.Opener);
            for (int i = 0; !_disposed && !_cancelToken.IsCancellationRequested && i < opener.Count; i++) {
                this.PlayAbility(opener[i]);
            }
            if (!rotation.Loop.Any()) {
                return;
            }
            var loop = new List<Ability>(rotation.Loop);
            while (!_disposed || !_cancelToken.IsCancellationRequested) {
                for (int i = 0; !_disposed && !_cancelToken.IsCancellationRequested && i < loop.Count; i++) {
                    if (this.PlayAbility(loop[i]))
                    {
                        continue;
                    }
                    return;
                }
            }
        }

        private bool PlayAbility(Ability ability) {
            var hint = AbilityHint.ShowHint(ability);
            while (!hint.Completed) {
                if (!_disposed && !_cancelToken.IsCancellationRequested) {
                    continue;
                }
                hint.Dispose();
                return false;
            }
            hint.Dispose();
            return true;
        }

        public void Dispose() {
            _disposed = true;
            _cancelToken?.Dispose();
        }
    }
}