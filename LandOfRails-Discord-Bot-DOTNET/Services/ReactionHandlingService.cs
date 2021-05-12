using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Discord_Bot_DOTNET.Services
{
    public class ReactionHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public ReactionHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _factory = new DbContextFactory<lordiscordbotContext>(services,
                new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());

            _discord.ReactionAdded += DiscordOnReactionAdded;
            _discord.ReactionRemoved += DiscordOnReactionRemoved;
            _discord.ReactionsCleared += DiscordOnReactionsCleared;
        }

        private async Task DiscordOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {
            var message = await arg1.GetOrDownloadAsync();

            if (arg3.User.IsSpecified && !arg3.User.Value.IsBot && !arg3.User.Value.IsWebhook)
            {
                lordiscordbotContext context = _factory.CreateDbContext();
                bool userFound = false;
                foreach (User contextUser in context.Users.AsQueryable()
                    .Where(contextUser => contextUser.MemberId == (long)arg3.UserId))
                {
                    userFound = true;
                    contextUser.ReactionCount += 1;
                    break;
                }

                if (!userFound)
                {
                    context.Users.Add(new User
                    {
                        DiscordName = arg3.User.Value.Username,
                        MemberId = (long)arg3.UserId,
                        ReactionCount = 1
                    });
                }
                await context.SaveChangesAsync();

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

        private async Task DiscordOnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2, SocketReaction arg3)
        {

        }

        private async Task DiscordOnReactionsCleared(Cacheable<IUserMessage, ulong> arg1, ISocketMessageChannel arg2)
        {

        }

        public void register()
        {

        }
    }
}
