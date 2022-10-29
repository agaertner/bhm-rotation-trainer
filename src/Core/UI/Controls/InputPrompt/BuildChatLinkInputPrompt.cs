using Gw2Sharp.ChatLinks;
using System;

namespace Nekres.RotationTrainer.Core.UI.Controls {
    internal sealed class BuildChatLinkInputPrompt : InputPrompt<BuildChatLink, BuildChatLinkInputPrompt>
    {
        public BuildChatLinkInputPrompt(Action<bool, BuildChatLink> callback, string text, string defaultValue, string confirmButtonText, string cancelButtonText) : base(callback, text, defaultValue, confirmButtonText, cancelButtonText) { }

        protected override bool TryParse(string input, out BuildChatLink result) {
            result = null;
            return Gw2ChatLink.TryParse(input, out var link) && link.TryCast(out result);
        }

    }
}
