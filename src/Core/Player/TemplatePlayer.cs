using Nekres.RotationTrainer.Core.UI.Controls;
using Nekres.RotationTrainer.Player.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Blish_HUD.Controls;
using Microsoft.Xna.Framework.Input;

namespace Nekres.RotationTrainer.Player {
    internal class TemplatePlayer : IDisposable
    {
        private bool           _disposed;

        private CancellationTokenSource _cancelToken;

        internal TemplatePlayer() {
            
        }

        public void Play(Rotation rotation) {

            if (_cancelToken != null) {
                _cancelToken.Cancel();
                _cancelToken.Dispose();
            }

            if (RotationTrainerModule.Instance.ActionBindings.Values.Any(x => x.Value.PrimaryKey == Keys.None && x.Value.ModifierKeys == ModifierKeys.None)) {
                ScreenNotification.ShowNotification("Key bindings need to be assigned.");
                return;
            }

            _cancelToken = new CancellationTokenSource();

            var current     = new Thread(() => PlayRotation(rotation));
            current.Start();
        }

        private void PlayRotation(Rotation rotation) {
            var abilities = new List<Ability>(rotation);
            foreach (var ability in abilities) {
                var hint = AbilityHint.ShowHint(ability);
                while (!hint.Completed) 
                {
                    if (_disposed || _cancelToken.IsCancellationRequested) {
                        hint.Dispose();
                        return;
                    }
                }
                hint.Dispose();
            }
        }

        public void Dispose() {
            _disposed = true;
            _cancelToken?.Dispose();
        }
    }
}