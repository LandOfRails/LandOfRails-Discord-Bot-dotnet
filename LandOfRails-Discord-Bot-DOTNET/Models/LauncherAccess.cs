using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class LauncherAccess
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public string ModpackShortcut { get; set; }
    }
}
