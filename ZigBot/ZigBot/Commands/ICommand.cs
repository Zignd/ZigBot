using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZigBot.Types;

namespace ZigBot.Commands
{
    public interface ICommand
    {
        void Execute(Bot bot, Update update, string[] commandPieces);
    }
}
