using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LinqToTwitter;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.Twitter
{
    public static class TwitterHelper
    {
        public static string TwitterApiToken = "";

        public async static Task DeleteTwitterData()
        {
            var auth = new XAuthAuthorizer
            {
                CredentialStore = new XAuthCredentials
                {
                    ConsumerKey = "consumerKey",
                    ConsumerSecret = "consumerSecret",
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            var client = new TwitterContext(auth);
            var statuses = await TwitterHelper.GetTwitterStatuses(client);
            foreach (var status in statuses)
            {
                //  Delete Channels
                //  Delete Files
                await TwitterHelper.DeleteTwitterCards(client, status.ID);
                //  Delete Messages
            }
        }

        public async static Task CreateTwitterData()
        {
            var seedBoards = await TwitterHelper.LoadSeedBoards();
            var auth = new XAuthAuthorizer
            {
                CredentialStore = new XAuthCredentials
                {
                    ConsumerKey = "consumerKey",
                    ConsumerSecret = "consumerSecret",
                    UserName = "YourUserName",
                    Password = "YourPassword"
                }
            };

            var client = new TwitterContext(auth);
            foreach (var seedBoard in seedBoards)
            {
                //  Delete Channels
                //  Delete Files
                await TwitterHelper.CreateTwitterCards(client, seedBoard.Text);
                //  Delete Messages
            }
        }

        public static async Task<List<Status>> LoadSeedBoards()
        {
            var files = Directory.GetFiles("/Seed/Twitter/Status");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Status>>(json));
                }
            }

            return new List<Status>();
        }

        public static async Task<List<Status>> GetTwitterStatuses(TwitterContext client)
        {
            var query = (from tweet in client.Status
                         where tweet.Type == StatusType.Mentions &&
                               tweet.Count == 200
                         select tweet).Skip(200 * 0).Expression;

            return ((IEnumerable<Status>)client.ExecuteAsync<Status>(query, true)).ToList();
        }

        public static async Task CreateTwitterCards(TwitterContext client, string status)
        {
            await client.TweetAsync(status);
        }

        public static async Task DeleteTwitterCards(TwitterContext client, ulong statusId)
        {
            await client.DeleteTweetAsync(statusId);
        }




    }
}
