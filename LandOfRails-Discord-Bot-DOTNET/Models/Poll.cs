using System;
using System.Collections.Generic;

#nullable disable

namespace LandOfRails_Discord_Bot_DOTNET.Models
{
    public partial class Poll
    {
        public Poll()
        {
            PollOptions = new HashSet<PollOption>();
        }

        public int Id { get; set; }
        public long MessageId { get; set; }
        public long? MemberId { get; set; }
        public string Question { get; set; }
        public DateTime StartDatetime { get; set; }
        public DateTime EndDatetime { get; set; }
        public bool TeamVoting { get; set; }

        public virtual User Member { get; set; }
        public virtual ICollection<PollOption> PollOptions { get; set; }
    }
}
