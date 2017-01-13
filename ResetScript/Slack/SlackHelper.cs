using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using System.IO;

namespace ResetScript.Slack
{
    public static class SlackHelper
    {
        public static string SlackApiToken = "";

        public static async Task<List<Channel>> LoadSeedChannels()
        {
            var files = Directory.GetFiles("/Seed/Slack/Channels");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Channel>>(json));
                }
            }

            return new List<Channel>();
        }

        public static async Task<List<Message>> LoadSeedMessages()
        {
            var files = Directory.GetFiles("/Seed/Slack/Messages");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Message>>(json));
                }
            }

            return new List<Message>();
        }

        public static List<string> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/Slack/Files").ToList();
        }

        public static async Task<bool> DeleteSlackMessage(string ts, string channel)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("chat.delete", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("ts", ts);
            authRequest.AddParameter("channel", channel);

            var result = await authEndPoint.ExecuteTaskAsync<SlackChannelDeleteResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public static async Task<bool> DeleteSlackFiles(string file)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("files.delete", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("file", file);

            var result = await authEndPoint.ExecuteTaskAsync<SlackDeleteResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public static async Task<bool> DeleteSlackChannels(string channel)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("channels.archive", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("channel", channel);

            var result = await authEndPoint.ExecuteTaskAsync<SlackDeleteResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public static async Task<List<Channel>> GetSlackChannels()
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("channels.list", Method.GET);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("exclude_archived", "1");

            var result = await authEndPoint.ExecuteTaskAsync<SlackChannelsResponse>(authRequest);
            if (result.Data.ok)
                return new List<Channel>();

            return result.Data.channels;
        }

        public static async Task<List<File>> GetSlackFiles()
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("files.list", Method.GET);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method

            var result = await authEndPoint.ExecuteTaskAsync<SlackFilesResponse>(authRequest);
            if (result.Data.ok)
                return new List<File>();

            return result.Data.files;
        }

        public static async Task<List<Message>> GetSlackChannelsMessages(string channel)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("channels.history", Method.GET);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("channel", channel);

            var result = await authEndPoint.ExecuteTaskAsync<SlackMessageResponse>(authRequest);
            if (result.Data.ok)
                return new List<Message>();

            return result.Data.messages;
        }


        public static async Task<bool> CreateSlackChannel(string name)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("channels.create", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("name", name);

            var result = await authEndPoint.ExecuteTaskAsync<SlackChannelCreateResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public static async Task<bool> CreateSlackMessage(string text, string channel)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("chat.postMessage", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.AddParameter("channel", channel);
            authRequest.AddParameter("text", text);

            var result = await authEndPoint.ExecuteTaskAsync<SlackMessagePostResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public static async Task<bool> UploadSlackFile(string path, string fileName)
        {
            var authEndPoint = new RestClient("https://slack.com/api");
            var authRequest = new RestRequest("files.upload", Method.POST);
            authRequest.AddParameter("token", SlackApiToken); // adds to POST or URL querystring based on Method
            authRequest.RequestFormat = DataFormat.Json;
            authRequest.AddHeader("Content-Type", "multipart/form-data");
            authRequest.AddFile("file", path);
            authRequest.AddParameter("fileName", fileName);

            var result = await authEndPoint.ExecuteTaskAsync<SlackDeleteResponse>(authRequest);
            if (result.Data.ok)
                return true;

            return false;
        }

        public async static Task DeleteSlackData()
        {
            var channels = await SlackHelper.GetSlackChannels();
            foreach (var channel in channels)
            {
                //  Delete Channels
                await SlackHelper.DeleteSlackChannels(channel.id);
                //  Delete Files
                var files = await SlackHelper.GetSlackFiles();
                foreach (var file in files)
                {
                    //  Delete Sample Files from Slack
                    await SlackHelper.DeleteSlackFiles(file.id);
                }

                //  Delete Messages
                var messages = await SlackHelper.GetSlackChannelsMessages(channel.id);
                foreach (var message in messages)
                {
                    await SlackHelper.DeleteSlackMessage(message.ts, channel.id);
                }
            }
        }

        public async static Task CreateSlackData()
        {
            var seedChannels = await SlackHelper.LoadSeedChannels();
            var seedMessages = await SlackHelper.LoadSeedMessages();
            foreach (var seedChannel in seedChannels)
            {
                await SlackHelper.CreateSlackChannel(seedChannel.name);
                //Create Converstations
                foreach (var seedMessage in seedMessages)
                {
                    await SlackHelper.CreateSlackMessage(seedMessage.text, seedChannel.id);
                }
            }

            var seedFiles = SlackHelper.LoadSeedFiles();
            foreach (var seedFile in seedFiles)
            {
                var file = new FileInfo(seedFile);
                await SlackHelper.UploadSlackFile(seedFile, file.Name);
            }
        }

        public class SlackChannelDeleteResponse
        {
            public bool ok { get; set; }
            public string channel { get; set; }
            public string ts { get; set; }
        }

        public class SlackMessagePostResponse
        {
            public bool ok { get; set; }
            public string channel { get; set; }
            public string ts { get; set; }
        }

        public class SlackDeleteResponse
        {
            public bool ok { get; set; }
        }

        public class Channel
        {
            public string id { get; set; }
            public string name { get; set; }
            public int created { get; set; }
            public string creator { get; set; }
            public bool is_archived { get; set; }
            public bool is_member { get; set; }
            public bool is_general { get; set; }
            public string last_read { get; set; }
            public object latest { get; set; }
            public int unread_count { get; set; }
            public int unread_count_display { get; set; }
            public List<object> members { get; set; }
        }

        public class SlackChannelCreateResponse
        {
            public bool ok { get; set; }
            public Channel channel { get; set; }
        }

        public class Topic
        {
            public string value { get; set; }
            public string creator { get; set; }
            public int last_set { get; set; }
        }

        public class Purpose
        {
            public string value { get; set; }
            public string creator { get; set; }
            public int last_set { get; set; }
        }

        public class SlackChannelsResponse
        {
            public bool ok { get; set; }
            public List<Channel> channels { get; set; }
        }
        public class Reaction
        {
            public string name { get; set; }
            public int count { get; set; }
            public List<string> users { get; set; }
        }

        public class Message
        {
            public string type { get; set; }
            public string ts { get; set; }
            public string user { get; set; }
            public string text { get; set; }
            public bool? is_starred { get; set; }
            public List<Reaction> reactions { get; set; }
            public bool? wibblr { get; set; }
        }

        public class SlackMessageResponse
        {
            public bool ok { get; set; }
            public string latest { get; set; }
            public List<Message> messages { get; set; }
            public bool has_more { get; set; }
        }



        public class File
        {
            public string id { get; set; }
            public int timestamp { get; set; }
            public string name { get; set; }
            public string title { get; set; }
            public string mimetype { get; set; }
            public string filetype { get; set; }
            public string pretty_type { get; set; }
            public string user { get; set; }
            public string mode { get; set; }
            public bool editable { get; set; }
            public bool is_external { get; set; }
            public string external_type { get; set; }
            public int size { get; set; }
            public string url { get; set; }
            public string url_download { get; set; }
            public string url_private { get; set; }
            public string url_private_download { get; set; }
            public string thumb_64 { get; set; }
            public string thumb_80 { get; set; }
            public string thumb_360 { get; set; }
            public string thumb_360_gif { get; set; }
            public int thumb_360_w { get; set; }
            public int thumb_360_h { get; set; }
            public string permalink { get; set; }
            public string edit_link { get; set; }
            public string preview { get; set; }
            public string preview_highlight { get; set; }
            public int lines { get; set; }
            public int lines_more { get; set; }
            public bool is_public { get; set; }
            public bool public_url_shared { get; set; }
            public List<string> channels { get; set; }
            public List<string> groups { get; set; }
            public int num_stars { get; set; }
            public bool is_starred { get; set; }
        }

        public class Paging
        {
            public int count { get; set; }
            public int total { get; set; }
            public int page { get; set; }
            public int pages { get; set; }
        }

        public class SlackFilesResponse
        {
            public bool ok { get; set; }
            public List<File> files { get; set; }
            public Paging paging { get; set; }
        }
    }


}
