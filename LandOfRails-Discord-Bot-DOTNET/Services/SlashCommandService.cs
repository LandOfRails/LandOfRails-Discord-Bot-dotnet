using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using LandOfRails_Discord_Bot_DOTNET.Modules;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.Extensions.DependencyInjection;

namespace LandOfRails_Discord_Bot_DOTNET.Services
{
    class SlashCommandService
    {

        private readonly DiscordSocketClient _discord;
        private readonly IServiceProvider _services;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public SlashCommandService(IServiceProvider services)
        {
            _discord = services.GetRequiredService<DiscordSocketClient>();
            _services = services;
            _factory = new DbContextFactory<lordiscordbotContext>(services, new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());

            _discord.Ready += DiscordOnReady;
            _discord.InteractionCreated += DiscordOnInteractionCreated;
        }

        private async Task DiscordOnInteractionCreated(SocketInteraction arg)
        {
            if (!arg.User.IsBot && !arg.User.IsWebhook)
            {
                lordiscordbotContext context = _factory.CreateDbContext();
                bool userFound = false;
                foreach (User contextUser in context.Users.AsQueryable()
                    .Where(contextUser => contextUser.MemberId == (long)arg.User.Id))
                {
                    userFound = true;
                    contextUser.InteractionCount += 1;
                    break;
                }

                if (!userFound)
                {
                    context.Users.Add(new User
                    {
                        DiscordName = arg.User.Username,
                        MemberId = (long)arg.User.Id,
                        InteractionCount = 1
                    });
                }

                await context.SaveChangesAsync();
                await context.DisposeAsync();
            }
        }

        private async Task DiscordOnReady()
        {
            new ModpackCommands(_discord, _factory);
        }

        public void register()
        {
           
        }
    }
}
