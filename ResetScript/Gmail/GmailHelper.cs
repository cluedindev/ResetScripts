using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.Gmail
{
    public static class GmailHelper
    {
        public static string GmailApiToken = "";

        public async static Task DeleteGmailData()
        {
            var companies = await GmailHelper.GetGmailMail("cluedin.matthew.smith@gmail.com");
            foreach (var company in companies.messages)
            {
                //  Delete Channels
                //  Delete Files
                await GmailHelper.DeleteGmailEntity(int.Parse(company.id), "cluedin.matthew.smith@gmail.com");
                //  Delete Messages
            }
        }

        public async static Task CreateGmailData()
        {
            var seedCompanies = await GmailHelper.LoadSeedCompany();
            foreach (var seedCompany in seedCompanies)
            {
                await GmailHelper.CreateGmailMail("cluedin.matthew.smith@gmail.com", seedCompany.raw);
                //Create Converstations
            }
        }

        public static async Task<List<Message>> LoadSeedCompany()
        {
            var files = Directory.GetFiles("/Seed/Gmail/Companies");
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

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/Gmail/Files");
        }

        public static async Task<MessageResponse> GetGmailMail(string userId)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/gmail/v1");
            var authRequest = new RestRequest("users/" + userId + "/messages", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<MessageResponse>(authRequest);
            if (result.Data == null)
                return new MessageResponse();

            return result.Data;
        }

        public static async Task CreateGmailMail(string userId, string body)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/gmail/v1/");
            var authRequest = new RestRequest("users/" + userId + "/messages", Method.POST);
            authRequest.AddParameter("uploadType", "media");
            authRequest.AddJsonBody(new GmailUploadPost() { raw = body });

            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task CreateGmailMailWithAttachment(string userId, string body)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/gmail/v1/");
            var authRequest = new RestRequest("users/" + userId + "/messages", Method.POST);
            authRequest.AddParameter("uploadType", "multipart");
            authRequest.AddJsonBody(new GmailUploadPost() { raw = body });

            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteGmailEntity(int id, string userId)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/gmail/v1");
            var authRequest = new RestRequest("/users/" + userId + "/messages/" + id, Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public class GmailUploadPost
        {
            public string raw { get; set; }
        }

        public class MessageResponse
        {
            public List<Message> messages { get; set; }
            public string nextPageToken { get; set; }
            public int resultSizeEstimate { get; set; }
        }

        public class Header
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Payload
        {
            public string partId { get; set; }
            public string mimeType { get; set; }
            public string filename { get; set; }
            public List<Header> headers { get; set; }
            public List<object> parts { get; set; }
        }

        public class Message
        {
            public string id { get; set; }
            public string threadId { get; set; }
            public List<string> labelIds { get; set; }
            public string snippet { get; set; }
            public long historyId { get; set; }
            public long internalDate { get; set; }
            public Payload payload { get; set; }
            public int sizeEstimate { get; set; }
            public string raw { get; set; }
        }
    }
}
