#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    class Modpack
    {
        public Modpack(string name, string title, string shortcut, string modpackVersion, string minecraftVersion, int organisation, string key, string locationOnServer, string imageUrl, string downloadedImage)
        {
            Name = name;
            Title = title;
            Shortcut = shortcut;
            MinecraftVersion = minecraftVersion;
            ModpackVersion = modpackVersion;
            Organisation = organisation;
            Key = key;
            LocationOnServer = locationOnServer;
            ImageUrl = imageUrl;
            DownloadedImage = downloadedImage;

        }
        public string Name { get; set; }
        public string Title { get; set; }
        public string Shortcut { get; set; }
        public string MinecraftVersion { get; set; }
        public string ModpackVersion { get; set; }

        [JsonIgnore]
        public string CurrentVersion { get; set; }
        public int Organisation { get; set; }
        public string Key { get; set; }
        public string LocationOnServer { get; set; }
        public string ImageUrl { get; set; }
        public string DownloadedImage { get; set; }

        public override string ToString() =>
            "Modpack{" +
            "Name='" + Name + '\'' +
            ", Title='" + Title + '\'' +
            ", Shortcut='" + Shortcut + '\'' +
            ", MinecraftVersion='" + MinecraftVersion + '\'' +
            ", ModpackVersion='" + ModpackVersion + '\'' +
            ", Organisation=" + Organisation +
            ", Key='" + Key + '\'' +
            ", LocationOnServer='" + LocationOnServer + '\'' +
            ", ImageUrl='" + ImageUrl + '\'' +
            ", DownloadedImage='" + DownloadedImage + '\'' +
            '}';

        public override bool Equals(object? obj)
        {
            if (obj is not Modpack modpack) {
                return false;
            }
            return Shortcut.Equals(modpack.Shortcut, StringComparison.CurrentCultureIgnoreCase) || Title.Equals(modpack.Title, StringComparison.CurrentCultureIgnoreCase) || Name.Equals(modpack.Name, StringComparison.CurrentCultureIgnoreCase) || Organisation == modpack.Organisation;
        }
    }
}
