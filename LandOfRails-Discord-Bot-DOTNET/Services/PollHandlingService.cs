using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Discord_Bot_DOTNET.Services
{
    public class PollHandlingService
    {
        private static DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public PollHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _factory = new DbContextFactory<lordiscordbotContext>(services,
                new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());

            _discord.ReactionAdded += DiscordOnReactionAdded;
        }

        private async Task DiscordOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var message = await arg1.GetOrDownloadAsync();

            if (arg3.User.IsSpecified && !arg3.User.Value.IsBot && !arg3.User.Value.IsWebhook)
            {
                lordiscordbotContext context = _factory.CreateDbContext();

                //Check if there is an active poll
                if (context.Polls.AsQueryable().Any(x =>
                    x.EndDatetime.CompareTo(DateTime.Now) > 0 && x.MessageId == (long)arg3.MessageId))
                {
                    await message.RemoveReactionAsync(arg3.Emote, arg3.UserId);
                    var poll = context.Polls.AsQueryable().Include(x => x.PollOptions).First(x => x.MessageId == (long)arg3.MessageId);
                    if (poll.PollOptions.Any(x => x.EmojiUnicode.Equals(arg3.Emote.Name)))
                    {
                        //clicked option
                        var pollOption = poll.PollOptions.First(x => x.EmojiUnicode.Equals(arg3.Emote.Name));

                        //check if clicked on other option -> remove
                        foreach (var pollVotedRegister in context.PollVotedRegisters.Include(x => x.FkPollOptions).Where(pollVotedRegister => pollVotedRegister.FkMemberId == (long)arg3.UserId).Where(pollVotedRegister => pollVotedRegister.FkPollOptions.FkPollId == poll.Id))
                        {
                            pollVotedRegister.FkPollOptions.Votes -= 1;
                            context.PollVotedRegisters.Remove(pollVotedRegister);
                        }

                        pollOption.Votes += 1;
                        context.PollVotedRegisters.Add(new PollVotedRegister { FkMemberId = (long)arg3.UserId, FkPollOptions = pollOption });
                        await context.SaveChangesAsync();

                        string description = poll.PollOptions.Aggregate(string.Empty, (current, option) => current + $"{option.EmojiUnicode} {option.VoteOption} = {option.Votes} \n \n ");
                        await message.ModifyAsync(properties =>
                        {
                            properties.Embed = message.Embeds.First().ToEmbedBuilder().WithDescription(description).Build();
                        });
                        await context.DisposeAsync();
                    }
                }
            }
        }

        public async void register()
        {
            var context = _factory.CreateDbContext();
            foreach (Poll poll in context.Polls.AsQueryable().Include(x => x.PollOptions).Where(x => x.EndDatetime.CompareTo(DateTime.Now) > 0))
            {
                Task.Delay(poll.EndDatetime.TimeOfDay).ContinueWith(_ => FinishPoll(poll));
            }
        }

        public static async void FinishPoll(Poll poll)
        {
            var message = _discord.GetGuild(394112479283904512).GetTextChannel((ulong) poll.TextChannelId).GetMessageAsync((ulong) poll.MessageId).Result as SocketUserMessage;
            await message.RemoveAllReactionsAsync();
            await message.ModifyAsync(properties =>
            {
                properties.Embed = message.Embeds.First().ToEmbedBuilder().WithColor(Color.Green).Build();
            });
        }
    }
}
