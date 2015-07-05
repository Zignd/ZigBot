using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZigBot.Types;

namespace ZigBot.Commands
{
    public class DicioCommand : ICommand
    {
        public void Execute(Bot bot, Types.Update update, string[] commandPieces)
        {
            if (commandPieces.Length < 2)
            {
                bot.SendMessage(
                    chatId: update.Message.Chat.Id,
                    text: "O comando foi utilizado de maneira incorreta.\nSiga o seguinte formato na próxima vez: \"/dicio <palavra>\".",
                    disableWebPagePreview: false,
                    replyToMessageId: update.Message.MessageId);
                return;
            }

            string palavraBusca = commandPieces[1];

            HtmlWeb web = new HtmlWeb
            {
                OverrideEncoding = Encoding.GetEncoding("iso-8859-1")
            };

            HtmlDocument doc = web.Load(String.Format("http://www.dicio.com.br/pesquisa.php?q={0}", palavraBusca));

            bool paginaDefinicaoPalavra = doc.DocumentNode.SelectNodes("//div[@id='content']//h1[@itemprop='name']") != null;
            
            if (!paginaDefinicaoPalavra)
            {
                bool paginaMultiplasPalavras = doc.DocumentNode.SelectNodes("//*[@id='enchant']/a") == null && doc.DocumentNode.SelectNodes("//*[@id='content']/div[1]/p/text()[1]") == null;

                if (paginaMultiplasPalavras)
                {
                    string pathPaginaPrimeiraPalavra = doc.DocumentNode.SelectNodes("//div[@id='content']//ul[@id='resultados']/li/a")[0].Attributes["href"].Value;
                    doc = web.Load(String.Format("http://www.dicio.com.br{0}", pathPaginaPrimeiraPalavra));
                }
                else
                {
                    bot.SendMessage(
                        chatId: update.Message.Chat.Id,
                        text: String.Format("Não foi possível encontrar o significado de \"{0}\".", palavraBusca),
                        replyToMessageId: update.Message.MessageId);

                    return;
                }
            }
            
            // TODO: Descobrir uma maneira menos porca de limpar esse HTML.    
            string palavra = doc.DocumentNode.SelectNodes("//*[@id='content']/div[1]/h1")[0].InnerText;
            string significado = doc.DocumentNode.SelectNodes("//*[@id='significado']")[0].InnerHtml.Replace("<br>", "\n");
            string definicao = doc.DocumentNode.SelectNodes("//*[@id='content']/div[1]/p[2]")[0].InnerHtml
                .Replace("<br>", "\n")
                .Replace("<b>", "")
                .Replace("</b>", "")
                .Replace("<span class=\"sep\">", "")
                .Replace("</span>", "")
                .Replace("</a>", "");

            bool onceAgain;
            do
            {
                onceAgain = false;

                var anchorTagStartIndex = definicao.IndexOf("<a href=\"");
                var anchorTagEndIndex = definicao.IndexOf("/\">");

                if (anchorTagStartIndex != -1 && anchorTagEndIndex != -1)
                {
                    var anchorTag = definicao.Substring(anchorTagStartIndex, (anchorTagEndIndex + 3) - anchorTagStartIndex);
                    definicao = definicao.Replace(anchorTag, "");

                    onceAgain = true;
                }
            } while (onceAgain);

            bot.SendMessage(
                chatId: update.Message.Chat.Id,
                text: String.Format("Significado de {0}:\n{1}\n\nDefinição de {2}:\n{3}", palavra, significado, palavra, definicao),
                disableWebPagePreview: false,
                replyToMessageId: update.Message.MessageId);
        }
    }
}
