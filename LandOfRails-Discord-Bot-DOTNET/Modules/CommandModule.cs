using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LandOfRails_Discord_Bot_DOTNET.Models;
using Newtonsoft.Json.Linq;

namespace LandOfRails_Discord_Bot_DOTNET.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle("Stats of " + user).WithColor(64, 224, 208);
            lordiscordbotContext context = new();
            var dbUser = context.Users.AsQueryable().First(x => x.MemberId == (long)user.Id);

            embedBuilder.AddField("Messagecount", dbUser != null ? dbUser.MessageCount : "-", true);
            await ReplyAsync(null, false, embedBuilder.Build());
        }
        [Command("rank", true)]
        [Alias("ranks", "ranking", "top")]
        public async Task RankingAsync()
        {
            EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle("Ranking").WithColor(64, 224, 208);
            lordiscordbotContext context = new();
            var dbUser = context.Users.AsQueryable().First(x => x.MemberId == (long)Context.User.Id);

            var topUsers = context.Users.AsQueryable().OrderByDescending(x => x.MessageCount).Take(5).ToList();
            await context.DisposeAsync();
            for (int i = 0; i < topUsers.Count; i++) embedBuilder.AddField((i + 1) + ". " + topUsers[i].DiscordName, $"Messages: {topUsers[i].MessageCount}\nReactions: {topUsers[i].ReactionCount}", true);

            embedBuilder.AddField("Your messagecount", dbUser != null ? dbUser.MessageCount : "-");
            embedBuilder.AddField("Your reactioncount", dbUser != null ? dbUser.ReactionCount : "-", true);
            await ReplyAsync(null, false, embedBuilder.Build());
        }

        [Command("vote")]
        public async Task startPoll(bool anonym, int hoursTilEnd, [Remainder] string question)
        {
            var end = new DateTimeOffset(DateTime.Now).AddHours(hoursTilEnd);
            EmbedBuilder builder = new EmbedBuilder().WithColor(Color.Gold).WithAuthor(anonym ? "Anonym" : Context.User.Username).WithTitle(question).WithDescription("\uD83D\uDC4D Ja! = 0 \n \n \uD83D\uDC4E Nein! = 0 \n \n \u270A Mir egal... = 0").WithFooter("Endet")
                .WithTimestamp(end);

            var message = await Context.Guild.GetTextChannel(581602258102517760).SendMessageAsync(null, false, builder.Build());
            await message.AddReactionsAsync(new[] { new Emoji("\uD83D\uDC4D"), new Emoji("\uD83D\uDC4E"), new Emoji("\u270A") });

            lordiscordbotContext context = new();
            Poll poll = new Poll
            {
                MessageId = (long)message.Id,
                Member = context.Users.AsQueryable().First(x => x.MemberId == (long)Context.User.Id),
                Question = question,
                StartDatetime = DateTime.Now,
                EndDatetime = end.DateTime,
                TeamVoting = false
            };
            ICollection<PollOption> pollOptions = new List<PollOption>();
            pollOptions.Add(new PollOption { VoteOption = "Ja!", EmojiUnicode = "\uD83D\uDC4D", FkPoll = poll, Votes = 0 });
            pollOptions.Add(new PollOption { VoteOption = "Nein!", EmojiUnicode = "\uD83D\uDC4E", FkPoll = poll, Votes = 0 });
            pollOptions.Add(new PollOption { VoteOption = "Mir egal...", EmojiUnicode = "\u270A", FkPoll = poll, Votes = 0 });
            poll.PollOptions = pollOptions;
            context.Polls.Add(poll);
            await context.SaveChangesAsync();
            await context.DisposeAsync();
        }
    }
}
