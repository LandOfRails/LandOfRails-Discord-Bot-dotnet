using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class User
    {
        public User()
        {
            ElectionLineupByMembers = new HashSet<ElectionLineup>();
            ElectionLineupMembers = new HashSet<ElectionLineup>();
            PollVotedRegisters = new HashSet<PollVotedRegister>();
            Polls = new HashSet<Poll>();
        }

        public long MemberId { get; set; }
        public string DiscordName { get; set; }
        public int MessageCount { get; set; }
        public int ReactionCount { get; set; }

        public virtual ICollection<ElectionLineup> ElectionLineupByMembers { get; set; }
        public virtual ICollection<ElectionLineup> ElectionLineupMembers { get; set; }
        public virtual ICollection<PollVotedRegister> PollVotedRegisters { get; set; }
        public virtual ICollection<Poll> Polls { get; set; }
    }
}
