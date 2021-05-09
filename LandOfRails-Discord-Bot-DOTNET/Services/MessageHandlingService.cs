using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Discord_Bot_DOTNET.Services
{
    public class MessageHandlingService
    {
        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public MessageHandlingService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _factory = new DbContextFactory<lordiscordbotContext>(services,
                new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());

            _discord.MessageReceived += DiscordOnMessageReceived;
        }

        public void register()
        {

        }

        private async Task DiscordOnMessageReceived(SocketMessage arg)
        {
            lordiscordbotContext context = _factory.CreateDbContext();
            foreach (User contextUser in context.Users)
            {
                if (contextUser.MemberId != (long)arg.Author.Id) continue;
                contextUser.MessageCount += 1;
                break;
            }
            context.SaveChangesAsync();
        }
    }
}
