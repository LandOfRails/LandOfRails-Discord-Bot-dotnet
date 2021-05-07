using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class ElectionLineup
    {
        public int Id { get; set; }
        public long MemberId { get; set; }
        public long ByMemberId { get; set; }

        public virtual User ByMember { get; set; }
        public virtual User Member { get; set; }
    }
}
