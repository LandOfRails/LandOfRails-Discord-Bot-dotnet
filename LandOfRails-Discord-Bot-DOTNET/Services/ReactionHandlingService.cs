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
    public class ReactionHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public ReactionHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _factory = new DbContextFactory<lordiscordbotContext>(services, new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());

            _discord.ReactionAdded += DiscordOnReactionAdded;
            _discord.ReactionRemoved += DiscordOnReactionRemoved;
            _discord.ReactionsCleared += DiscordOnReactionsCleared;
        }

        private async Task DiscordOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {
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
                await context.DisposeAsync();
            }
        }

        private async Task DiscordOnReactionRemoved(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)
        {

        }

        private async Task DiscordOnReactionsCleared(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2)
        {

        }

        public void register()
        {

        }
    }
}
