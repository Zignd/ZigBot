using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Net.Http;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZigBot.Types;
using System.Threading;
using System.Diagnostics;
using ZigBot.Commands;

namespace ZigBot
{
    public class Bot
    {
        #region Fields

        private readonly string botUsername;
        private readonly string basicPath;
        private readonly string filePath;
        private readonly Dictionary<string, ICommand> commands;

        #endregion

        #region Properties

        private int? OffsetForNextRequest
        {
            get
            {
                return File.Exists(this.filePath) ? (int?)Convert.ToInt32(File.ReadAllText(this.filePath)) : null;
            }
            set
            {
                File.WriteAllText(this.filePath, value.HasValue ? value.Value.ToString() : String.Empty);
            }
        }

        #endregion

        #region Constructor Method

        public Bot(string authenticationToken, string filePath)
        {
            this.basicPath = String.Format("bot{0}/", authenticationToken);
            this.filePath = filePath;

            this.botUsername = this.GetMe().Username;

            this.commands = new Dictionary<string, ICommand>
            {
                { "/replyMe", new TestMessageCommand() },
                { "/dicio", new DicioCommand() }
            };
        }

        #endregion

        #region Public Methods

        public void Test()
        {
            //HtmlWeb web = new HtmlWeb();
            //HtmlDocument doc = web.Load("http://www.dicio.com.br/fuleco/");

            //var a = this.HtmlFind(doc.DocumentNode.ChildNodes, "h1",
            //    new KeyValuePair<string, string>("itemprop", "name"));

            //var b = this.HtmlFind(doc.DocumentNode.ChildNodes, "p",
            //    new KeyValuePair<string, string>("itemprop", "description"),
            //    new KeyValuePair<string, string>("id", "significado"),
            //    new KeyValuePair<string, string>("class", "name"));
        }

        public void Start()
        {
            Stopwatch timer;
            int counter = 1;

            while (true)
            {
                timer = Stopwatch.StartNew();
                
                var updates = this.GetUpdates(this.OffsetForNextRequest);

                foreach (var update in updates)
                {
                    string[] commandPieces = update.Message.Text != null ? this.ExtractCommandPieces(update.Message.Text) : null;

                    if (commandPieces == null)
                    {
                        continue;
                    }

                    ICommand requestedCommand;

                    if (this.commands.TryGetValue(commandPieces[0], out requestedCommand))
                    {
                        requestedCommand.Execute(this, update, commandPieces);
                    }
                }

                Console.Clear();
                Console.WriteLine("Numer of requests to getUpdates: " + counter++);
                Console.WriteLine("Time spent to handle the last loop: " + timer.Elapsed.TotalSeconds);

                timer.Stop();
            }
        }

        public User GetMe()
        {
            var data = this.Request("getMe");

            if ((bool)data["ok"])
            {
                return JsonConvert.DeserializeObject<User>(data["result"].ToString());
            }
            else
            {
                throw new Exception("Error on performing request to getMe.");
            }
        }

        public List<Update> GetUpdates(int? offset = null, int? limit = null, int? timeout = null)
        {
            #region Query String

            StringBuilder query = new StringBuilder();

            if (offset.HasValue)
            {
                query.AppendFormat("offset={0}", offset.Value);
            }

            if (limit.HasValue)
            {
                if (query.Length > 0)
                {
                    query.Append("&");
                }

                query.AppendFormat("limit={0}", limit.Value);
            }

            if (timeout.HasValue)
            {
                if (query.Length > 0)
                {
                    query.Append("&");
                }

                query.AppendFormat("timeout={0}", timeout.Value);
            }

            #endregion

            var data = this.Request("getUpdates", query.ToString());

            if ((bool)data["ok"])
            {
                if (data["result"].HasValues)
                {
                    this.OffsetForNextRequest = Convert.ToInt32(data["result"].Last["update_id"]) + 1;
                }

                return JsonConvert.DeserializeObject<List<Update>>(data["result"].ToString());
            }
            else
            {
                throw new Exception("Error on performing request to getUpdate.");
            }
        }

        // TODO: Implementar reply_markup
        public void SendMessage(int chatId, string text, bool? disableWebPagePreview = null, int? replyToMessageId = null, string replyMarkup = null)
        {
            StringBuilder query = new StringBuilder();

            query.AppendFormat("chat_id={0}&", chatId);
            query.AppendFormat("text={0}", text);

            if (disableWebPagePreview.HasValue)
            {
                query.AppendFormat("&disable_web_page_preview={0}", disableWebPagePreview.Value);
            }

            if (replyToMessageId.HasValue)
            {
                query.AppendFormat("&reply_to_message_id={0}", replyToMessageId.Value);
            }

            if (replyMarkup != null)
            {
                query.AppendFormat("&reply_markup={0}", replyMarkup);
            }

            var data = this.Request("sendMessage", query.ToString());
        }

        #endregion

        #region Private Methods

        private JObject Request(string apiMethod, string queryString = null)
        {
            using (HttpClient client = new HttpClient())
            using (Task<HttpResponseMessage> taskHttpResponseMessage = client.GetAsync(this.BuildRequestUri(apiMethod, queryString)))
            {
                taskHttpResponseMessage.Wait();

                using (HttpResponseMessage response = taskHttpResponseMessage.Result)
                using (var taskString = response.Content.ReadAsStringAsync())
                {
                    taskString.Wait();

                    return JObject.Parse(taskString.Result);
                }
            }
        }

        private Uri BuildRequestUri(string apiMethod, string queryString = null)
        {
            UriBuilder builder = new UriBuilder();
            builder.Scheme = "https";
            builder.Host = "api.telegram.org";
            builder.Path = String.Format("{0}{1}", this.basicPath, apiMethod);
            
            if (queryString != null)
            {
                builder.Query = queryString;
            }   

            return builder.Uri;
        }

        private string[] ExtractCommandPieces(string rawCommand)
        {
            string[] commandPieces = rawCommand.Split(new string[] { " " }, StringSplitOptions.RemoveEmptyEntries);

            if (commandPieces[0].Contains('@'))
            {
                if (commandPieces[0].Substring(commandPieces[0].IndexOf('@') + 1).Equals(this.botUsername))
                {
                    commandPieces[0] = commandPieces[0].Substring(0, commandPieces[0].IndexOf('@'));
                }
                else
                {
                    commandPieces = null;
                }
            }

            return commandPieces;
        }

        #endregion

        //private JArray ExtractUpdatesToHandle(JArray updates)
        //{
        //    if (File.Exists(this.filePath))
        //    {
        //        int lastHandledUpdateId = Convert.ToInt32(File.ReadAllText(this.filePath));

        //        updates = (from update in updates
        //                   where lastHandledUpdateId == Convert.ToInt32(update["update_id"])
        //                   let updatesToHandle = updates.Skip(updates.IndexOf(update) + 1).ToArray()
        //                   select JArray.FromObject(updatesToHandle)).SingleOrDefault();
        //    }

        //    return updates;
        //}
    }
}
