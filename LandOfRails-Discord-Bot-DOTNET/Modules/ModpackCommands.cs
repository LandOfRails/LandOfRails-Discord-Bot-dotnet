using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Net;
using Discord.WebSocket;
using LandOfRails_Discord_Bot_DOTNET.Models;
using LandOfRails_Discord_Bot_DOTNET.Utils;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;

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
            AddPermission();
            RemovePermission();
            UpdateImageLink();

            _discord.InteractionCreated += DiscordOnInteractionCreated;
        }

        private async Task DiscordOnInteractionCreated(SocketInteraction arg)
        {
            if (arg is SocketSlashCommand command)
            {
                var context = _factory.CreateDbContext();
                switch (command.Data.Name)
                {
                    case "update-modpack":
                        if (context.Launchers.Any(x =>
                            x.FkMemberId == (long) command.User.Id &&
                            x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value)))
                        {
                            var modpack = GetModpackList().First(x => x.Shortcut.Equals(command.Data.Options.ElementAt(0).Value));
                            var modpackVersion = new Version(modpack.ModpackVersion);
                            var newVersion = new Version(command.Data.Options.ElementAt(2).Value.ToString()!);
                            if (modpackVersion < newVersion)
                            {
                                //TODO
                            }
                            else
                            {
                                var build = modpackVersion.Build == 0 ? modpackVersion.Build : 1;
                                await command.RespondAsync($"Please specify a higher version than {modpackVersion}. For example {modpackVersion.Major}.{modpackVersion.Minor}.{build}");
                            }
                        }
                        else await command.RespondAsync("You don't have permission to use this command for this modpack.");
                        break;
                    case "add-permission":
                        if (command.User.Id == 222733101770604545 || context.Launchers.Any(x =>
                            x.FkMemberId == (long)command.User.Id &&
                            x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value)))
                        {
                            var user = command.Data.Options.ElementAt(1).Value as SocketUser;
                            if (context.Launchers.Any(x => x.FkMemberId == (long) user.Id && x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value.ToString())))
                            {
                                await command.RespondAsync(
                                    $"{user} already has all permissions to the {command.Data.Options.ElementAt(0).Value} modpack.");
                                break;
                            }
                            context.Launchers.Add(new Launcher()
                            {
                                FkMemberId = (long) user.Id,
                                ModpackShortcut = command.Data.Options.ElementAt(0).Value.ToString()
                            });
                            await context.SaveChangesAsync();
                            await command.RespondAsync($"{user} now has all permissions to the {command.Data.Options.ElementAt(0).Value} modpack.");
                        }
                        else await command.RespondAsync("You don't have permission to use this command for this modpack. If you think this is wrong contact MarkenJaden.");

                        break;
                    case "remove-permission":
                        if (command.User.Id == 222733101770604545 || context.Launchers.Any(x =>
                            x.FkMemberId == (long)command.User.Id &&
                            x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value)))
                        {
                            var user = command.Data.Options.ElementAt(1).Value as SocketUser;
                            if (context.Launchers.Any(x => x.FkMemberId == (long)user.Id && x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value.ToString())))
                            {
                                context.Launchers.Remove(context.Launchers.First(x => x.FkMemberId == (long) user.Id));
                                await context.SaveChangesAsync();
                                await command.RespondAsync(
                                    $"{user} was revoked the permissions to the {command.Data.Options.ElementAt(0).Value} modpack.");
                                break;
                            }
                            await command.RespondAsync($"{user} has no permissions to the {command.Data.Options.ElementAt(0).Value} modpack.");
                        }
                        else await command.RespondAsync("You don't have permission to use this command for this modpack. If you think this is wrong contact MarkenJaden.");

                        break;
                    case "update-image-link":
                        if (command.User.Id == 222733101770604545 || context.Launchers.Any(x =>
                            x.FkMemberId == (long)command.User.Id &&
                            x.ModpackShortcut.Equals(command.Data.Options.ElementAt(0).Value)))
                        {
                            if (Uri.TryCreate(command.Data.Options.ElementAt(0).Value.ToString(), UriKind.Absolute, out var uriResult) 
                                && (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps))
                            {
                                //TODO
                            }
                            else await command.RespondAsync("Invalid link! Please provide a valid link to the image.");
                        }
                        else
                        {
                            await command.RespondAsync(
                                "You don't have permission to use this command for this modpack. If you think this is wrong contact MarkenJaden.");
                        }
                        break;
                }

                await context.DisposeAsync();
            }
        }

        private void UpdateModpack()
        {
            var command = new SlashCommandBuilder()
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
                _discord.Rest.CreateGlobalCommand(command.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private void AddPermission()
        {
            var command = new SlashCommandBuilder()
                .WithName("add-permission")
                .WithDescription("Add permission to edit modpack in the LandOfRails Launcher.")
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
                    .WithName("user")
                    .WithDescription("User who should get permissions for the modpack.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                );
            try
            {
                _discord.Rest.CreateGlobalCommand(command.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }

        private void RemovePermission()
        {
            var command = new SlashCommandBuilder()
                .WithName("remove-permission")
                .WithDescription("Remove permission to edit modpack in the LandOfRails Launcher.")
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
                    .WithName("user")
                    .WithDescription("User who should get permissions removed for the modpack.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.User)
                );
            try
            {
                _discord.Rest.CreateGlobalCommand(command.Build());
            }
            catch (ApplicationCommandException exception)
            {
                // If our command was invalid, we should catch an ApplicationCommandException. This exception contains the path of the error as well as the error message. You can serialize the Error field in the exception to get a visual of where your error is.
                var json = JsonConvert.SerializeObject(exception.Error, Formatting.Indented);

                // You can send this error somewhere or just print it to the console, for this example we're just going to print it.
                Console.WriteLine(json);
            }
        }
        
        private void UpdateImageLink()
        {
            var command = new SlashCommandBuilder()
                .WithName("update-image-link")
                .WithDescription("Change the little modpack icon image.")
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
                    .WithName("image-link")
                    .WithDescription("Direct link to the image.")
                    .WithRequired(true)
                    .WithType(ApplicationCommandOptionType.String)
                );
            try
            {
                _discord.Rest.CreateGlobalCommand(command.Build());
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
