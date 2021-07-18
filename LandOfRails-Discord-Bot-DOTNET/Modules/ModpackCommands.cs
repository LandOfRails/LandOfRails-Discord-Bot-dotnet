using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Engines;

namespace LandOfRails_Discord_Bot_DOTNET.Modules
{
    class ModpackCommands
    {
        private readonly DiscordSocketClient _discord;
        private readonly DbContextFactory<lordiscordbotContext> _factory;

        public ModpackCommands(DiscordSocketClient _discord, DbContextFactory<lordiscordbotContext> _factory)
        {
            this._discord = _discord;
            this._factory = _factory;

            UpdateModpack();

            _discord.InteractionCreated += DiscordOnInteractionCreated;
        }

        private async Task DiscordOnInteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                switch (command.Data.Name)
                {
                    case "update-modpack":
                        break;
                    default:
                        break;
                }
            }
        }

        private void UpdateModpack()
        {
            var updateModpackCommand = new SlashCommandBuilder()
                .WithName("update-modpack")
                .WithDescription("Updates a specific modpack to a new version in the LandOfRails Launcher.")
                .AddOption("server", ApplicationCommandOptionType.String, "The server which belongs to the modpack.",true, false, null, new []{new ApplicationCommandOptionChoiceProperties(){Name = "Traincraft",Value = "TC"}, new ApplicationCommandOptionChoiceProperties() { Name = "Immersive Railroading", Value = "IR" }, new ApplicationCommandOptionChoiceProperties() { Name = "Real Train Mod", Value = "RTM" }, new ApplicationCommandOptionChoiceProperties() { Name = "Zora no Densha", Value = "ZnD" } });

            try
            {
                _discord.Rest.CreateGlobalCommand(updateModpackCommand.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

    }
}
