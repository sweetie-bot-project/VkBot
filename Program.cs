using ApiAiSDK;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace VkBot {

    struct VkMessage {
        public int Id;
        public string Text;
        public int UserId;
        //public JObject Attachments;
        public MessageFlags Flags;
    }

    class Program : IDisposable {

        const string apiUrl = "https://api.vk.com/method/";
        readonly string vkToken;

        HttpClient client = new HttpClient() {
            Timeout = TimeSpan.FromMinutes(2)
        };

        public void Dispose() {
            client.Dispose();
        }

        Program(string vkToken, string aiToken, SupportedLanguage lang) {
            this.vkToken = vkToken;
            aiConfig = new AIConfiguration(aiToken, lang);
        }

        uint ts;
        string serverUrl;
        string key;
        const string apiVersion = "5.63";
        const string pollingVersion = "2";

        async Task GetPoolingServerAsync() {
            var builder = new UriBuilder(apiUrl);
            builder.Path += "messages.getLongPollServer";
            builder.Query = $"access_token={vkToken}&v={apiVersion}";
            var responce = await client.GetStringAsync(builder.Uri);
            var serverParams = JObject.Parse(responce)["response"];

            ts = (uint)serverParams["ts"];
            serverUrl = (string)serverParams["server"];
            key = (string)serverParams["key"];
        }

        async Task SendMessage(int userId, string text, string[] attachments = null) {
            UriBuilder builder = new UriBuilder(apiUrl);
            builder.Path += "messages.send";
            builder.Query = $"v={apiVersion}&access_token={vkToken}&peer_id={userId}&message={text}";
            if (attachments != null)
                builder.Query += "&attachment=" + string.Join(',', attachments.Select(i => Uri.EscapeDataString(i)));
            var responceRaw = await client.GetStringAsync(builder.Uri);
            if (JObject.Parse(responceRaw)["response"] == null)
                throw new Exception(string.Format("Invalid server response: {0}", responceRaw));
        }

        AIConfiguration aiConfig;

        async Task ProcessMessage(VkMessage msg) {
            aiConfig.SessionId = "vk_" + msg.UserId.ToString();
            var apiAi = new ApiAi(aiConfig);
            var response = apiAi.TextRequest(msg.Text);
            string respmsg;
            if (response.IsError)
                respmsg = "Ошибочка ...";
            else
                respmsg = response.Result.Fulfillment.Speech;
            if (string.IsNullOrEmpty(respmsg))
                respmsg = "Хм...";
            await SendMessage(msg.UserId, respmsg);
        }

        async Task RunAsync(CancellationToken ct) {
            await GetPoolingServerAsync();

            while (!ct.IsCancellationRequested) {
                string updatesRaw;
                try {
                    using (ct.Register(() => { client.CancelPendingRequests(); })) {
                        updatesRaw = await client.GetStringAsync(
                            $"https://{serverUrl}?act=a_check&ts={ts}&key={key}&version={pollingVersion}&wait={client.Timeout.TotalSeconds}");
                    }
                }
                catch (TaskCanceledException) {
                    continue;
                }
                var updates = JObject.Parse(updatesRaw);

                if (updates["failed"] != null) {
                    var code = (int)updates["failed"];
                    switch (code) {
                        case 1:
                            ts = (uint)updates["new_ts"];
                            break;
                        case 2:
                        case 3:
                            await GetPoolingServerAsync();
                            break;
                        default:
                            throw new Exception(string.Format("Unknown error code has been detected in a long poll response: {0}", code));
                    }
                }
                else {
                    ts = (uint)updates["ts"];
                    foreach (var update in (JArray)updates["updates"]) {
                        switch ((LongPollMessageCodes)(int)update[0]) {
                            case LongPollMessageCodes.AddMessage: {
                                    var updateArr = (JArray)update;
                                    var msg = new VkMessage {
                                        Id = (int)updateArr[1],
                                        Text = (string)updateArr[5],
                                        UserId = (int)updateArr[3],
                                        //Attachments = (updateArr.Count > 6) ? (JObject)update[6] : null,
                                        Flags = (MessageFlags)(int)update[2],
                                    };
                                    if (!msg.Flags.HasFlag(MessageFlags.Outbox))
                                        await ProcessMessage(msg);
                                    break;
                                }
                        }
                    }
                }
            }
        }

        static int Main(string[] args) {
            try {
                using (var prog = new Program(vkToken: args[0], aiToken: args[1], lang: SupportedLanguage.Russian)) {
                    var cts = new CancellationTokenSource();
                    var handler = new ConsoleCancelEventHandler((sender, evargs) => {
                        cts.Cancel();
                        evargs.Cancel = true;
                    });
                    Console.CancelKeyPress += handler;
                    try {
                        prog.RunAsync(cts.Token).GetAwaiter().GetResult();
                    }
                    finally {
                        Console.CancelKeyPress -= handler;
                    }
                }
                return 0;
            }
            catch (Exception e) {
                Console.Error.WriteLine("EXCEPTION: {0}", e);
                return 1;
            }
        }

    }

}
