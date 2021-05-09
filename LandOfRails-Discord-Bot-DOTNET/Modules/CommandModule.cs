using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;
using LandOfRails_Discord_Bot_DOTNET.Models;

namespace LandOfRails_Discord_Bot_DOTNET.Modules
{
    public class CommandModule : ModuleBase<SocketCommandContext>
    {
        private lordiscordbotContext context = new();
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            EmbedBuilder embedBuilder = new EmbedBuilder().WithTitle(user.ToString()).WithColor(64, 224, 208);
            User dbUser = null;
            foreach (User contextUser in context.Users) if (contextUser.MemberId == (long)user.Id) dbUser = contextUser;

            embedBuilder.AddField("Messagecount", dbUser != null ? dbUser.MessageCount : "-", true);
            await ReplyAsync(null, false, embedBuilder.Build());
        }
    }
}
