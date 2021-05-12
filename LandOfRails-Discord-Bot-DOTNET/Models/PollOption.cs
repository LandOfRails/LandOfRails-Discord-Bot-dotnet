using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class PollOption
    {
        public PollOption()
        {
            PollVotedRegisters = new HashSet<PollVotedRegister>();
        }

        public int Id { get; set; }
        public int FkPollId { get; set; }
        public string VoteOption { get; set; }
        public string EmojiUnicode { get; set; }
        public int Votes { get; set; }

        public virtual Poll FkPoll { get; set; }
        public virtual ICollection<PollVotedRegister> PollVotedRegisters { get; set; }
    }
}
