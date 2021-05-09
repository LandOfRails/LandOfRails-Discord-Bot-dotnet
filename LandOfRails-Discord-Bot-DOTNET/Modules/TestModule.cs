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
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        private lordiscordbotContext context = new lordiscordbotContext();
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
            User test = null;
            foreach (User contextUser in context.Users) if (contextUser.MemberId == (long) user.Id) test = contextUser; 
            if(test != null) await ReplyAsync($"Messagecount: {test.MessageCount}");
        }
    }
}
