using Gw2Sharp.ChatLinks;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Linq;

namespace Nekres.RotationTrainer.Core.Services.Persistance
{
    internal class RawTemplate
    {
        [JsonProperty("title")] 
        public string Title { get; set; }

        [JsonProperty("patch")] 
        public DateTime Patch { get; set; }

        [JsonProperty("template")] 
        public string Template { get; set; }

        [JsonProperty("rotation")] 
        public Rotation Rotation { get; set; }

        [JsonProperty("utilityKeys")] 
        public int[] UtilityKeys { get; set; }

        public BuildChatLink GetBuildChatLink()
        {
            if (!string.IsNullOrEmpty(this.Template) && Gw2ChatLink.TryParse(this.Template, out var chatLink))
            {
                return (BuildChatLink)chatLink;
            }
            return null;
        }

        public void Save()
        {
            var json = JsonConvert.SerializeObject(this, Formatting.Indented);
            var title = Path.GetInvalidFileNameChars().Aggregate(this.Title, (current, c) => current.Replace(c, '-'));
            var path = Path.Combine(RotationTrainerModule.Instance.DirectoriesManager.GetFullDirectoryPath("special_forces"), title);
            File.WriteAllText(path + ".json", json);
        }
    }

    internal class Rotation
    {
        [JsonProperty("opener")] 
        public string Opener { get; set; }

        [JsonProperty("loop")] 
        public string Loop { get; set; }
    }
}