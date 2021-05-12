using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class PollVotedRegister
    {
        public int Id { get; set; }
        public int FkPollOptionsId { get; set; }
        public long FkMemberId { get; set; }

        public virtual User FkMember { get; set; }
        public virtual PollOption FkPollOptions { get; set; }
    }
}
