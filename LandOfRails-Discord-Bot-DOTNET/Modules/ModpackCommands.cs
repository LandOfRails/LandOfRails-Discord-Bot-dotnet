using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using LandOfRails_Discord_Bot_DOTNET.Utils;
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
                        var context = _factory.CreateDbContext();
                        if (context.Launchers.Any(x =>
                            x.FkMemberId == (long) command.User.Id &&
                            x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value)))
                        {
                            var modpack = GetModpackList().First(x => x.Shortcut.Equals(command.Data.Options.ElementAt(0).Value));
                            var modpackVersion = new Version(modpack.ModpackVersion);
                            var newVersion = new Version(command.Data.Options.ElementAt(2).Value.ToString()!);
                            if (modpackVersion < newVersion)
                            {

                            }
                            else
                            {
                                await command.RespondAsync($"Please specify a higher version than {modpackVersion}. For example {modpackVersion.IncrementBuild()}");
                            }
                        }
                        else await command.RespondAsync("You don't have permission to use this command for this modpack.");
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
                .AddOption(new SlashCommandOptionBuilder()
                    .WithName("server")
                    .WithDescription("The server which belongs to the modpack.")
                    .WithRequired(true)
                    .AddChoice("Traincraft", "tc")
                    .AddChoice("Immersive Railroading", "ir")
                    .AddChoice("Real Train Mod", "rtm")
                    .AddChoice("Zora no Densha", "znd")
                    .WithType(ApplicationCommandOptionType.String)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("download-link")
                    .WithDescription("Direct download link to modpack.zip")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                ).AddOption(new SlashCommandOptionBuilder()
                    .WithName("version")
                    .WithDescription("The version to which the modpack should be updated.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                );

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

        private List<Modpack> GetModpackList() => JsonConvert.DeserializeObject<List<Modpack>>(File.ReadAllText("/var/www/launcher/ModpackList.json"));
    }
}
