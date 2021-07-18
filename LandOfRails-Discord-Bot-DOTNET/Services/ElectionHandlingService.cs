﻿using System; using System.Collections.Generic; using System.Linq; using System.Runtime.InteropServices; using System.Threading.Tasks; using Discord; using Discord.WebSocket; using LandOfRails_Discord_Bot_DOTNET.Models; using Microsoft.EntityFrameworkCore; using Microsoft.EntityFrameworkCore.Internal; using Microsoft.Extensions.DependencyInjection;  namespace LandOfRails_Discord_Bot_DOTNET.Services {     public class ElectionHandlingService     {         private readonly DiscordSocketClient _discord;         private readonly IServiceProvider _services;         private readonly DbContextFactory<lordiscordbotContext> _factory;          private readonly Dictionary<string, (ulong channelID, ulong roleID)> serverList = new()         {             { "Traincraft", (836008800853950464, 438074536508784640) },             { "Immersive Railroading", (836009186373533716, 456916096587530241) },             { "Real Train Mod", (836009241855918180, 529727596942983187) },             { "Zora no Densha", (836009278439424030, 709848394725851211) },         };          public ElectionHandlingService(IServiceProvider services)         {             _discord = services.GetRequiredService<DiscordSocketClient>();             _services = services;             _factory = new DbContextFactory<lordiscordbotContext>(services,                 new DbContextOptions<lordiscordbotContext>(), new DbContextFactorySource<lordiscordbotContext>());              _discord.ReactionAdded += DiscordOnReactionAdded;         }          private async Task DiscordOnReactionAdded(Cacheable<IUserMessage, ulong> arg1, Cacheable<IMessageChannel, ulong> arg2, SocketReaction arg3)         {             var message = await arg1.GetOrDownloadAsync();         }          public void register()         {             var context = _factory.CreateDbContext();             foreach (Poll poll in context.Polls.AsQueryable().Where(x => x.TeamVoting && (x.EndDatetime - DateTime.Now).TotalDays < 22))                 foreach (var tuple in serverList.Where(tuple => poll.Question.Contains(tuple.Key))) startElectionProcess(tuple);             context.DisposeAsync();         }          private async void startElectionProcess(KeyValuePair<string, (ulong channelID, ulong roleID)> server)         {             var context = _factory.CreateDbContext();             var textChannel = _discord.GetGuild(394112479283904512).GetTextChannel(server.Value.channelID);             await textChannel.SendMessageAsync($@"--- @{_discord.GetGuild(394112479283904512).GetRole(server.Value.roleID)}  **Aufgepasst liebe servername Teamler**   Am DATUM finden die COUNT_WAHLENten Wahlen statt!  -  Nominiert nun die Leute die du in der Leitung sehen möchtest mit:  **!nominate Discordname** z.B. !nominate MarkenJaden oder !nominate @MarkenJaden#7787   Nominierte Teamler müssen der Nominierung anschließend noch per Privatnachricht zustimmen.  -  **Nominiert sind aktuell:**  - @MEMBER");         }          public async Task FinishPoll(Poll poll)         {             var textChannel = _discord.GetGuild(394112479283904512).GetTextChannel((ulong)poll.TextChannelId);             if (await textChannel.GetMessageAsync((ulong)poll.MessageId) is not IUserMessage message)             {                 Console.WriteLine("Poll could not be finished. Message might be null.");                 return;             }             await message.RemoveAllReactionsAsync();             await message.ModifyAsync(properties =>             {                 properties.Embed = message.Embeds.First().ToEmbedBuilder().WithColor(Color.Green).WithFooter("Beendet").Build();             });             var context = _factory.CreateDbContext();             context.Polls.AsQueryable().First(x => x.Equals(poll)).Finished = true;             await context.SaveChangesAsync();             await context.DisposeAsync();         }     } } 