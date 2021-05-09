using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace LandOfRails_Discord_Bot_DOTNET.Modules
{
    public class TestModule : ModuleBase<SocketCommandContext>
    {
        [Command("userinfo")]
        public async Task UserInfoAsync(IUser user = null)
        {
            user ??= Context.User;

            await ReplyAsync(user.ToString());
        }
    }
}
