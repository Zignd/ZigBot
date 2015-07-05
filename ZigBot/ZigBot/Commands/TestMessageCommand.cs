using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZigBot.Commands.Interfaces;
using ZigBot.Types;

namespace ZigBot.Commands
{
    public class TestMessageCommand : ICommand
    {
        public void Execute(Bot bot, Update update, string[] commandPieces)
        {
            bot.SendMessage(update.Message.Chat.Id, String.Format("Hi there {0}!\n\nxD", update.Message.From.FirstName), replyToMessageId: update.Message.MessageId);
        }
    }
}
