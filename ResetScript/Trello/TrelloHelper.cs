using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.Slack
{
    public static class TrelloHelper
    {
        public static string TrelloApiToken = "";

        public async static Task DeleteTrelloData()
        {
            var boards = await TrelloHelper.GetTrelloBoards("");
            foreach (var board in boards)
            {
                //  Delete Channels
                //  Delete Files
                var lists = await TrelloHelper.GetTrelloLists(board.id);
                foreach (var list in lists)
                {
                    //  Delete Sample Files from Slack
                    var cards = await TrelloHelper.GetTrelloCards(list.id);
                    foreach (var card in cards)
                    {
                        await TrelloHelper.DeleteTrelloCards(card.id);
                    }

                    await TrelloHelper.DeleteTrelloLists(list.id);
                }

                await TrelloHelper.DeleteTrelloBoards(board.id);
                //  Delete Messages
            }
        }

        public async static Task CreateTrelloData()
        {
            var seedBoards = await TrelloHelper.LoadSeedBoards();
            var seedLists = await TrelloHelper.LoadSeedLists();
            var seedCards = await TrelloHelper.LoadSeedCards();
            foreach (var seedBoard in seedBoards)
            {
                await TrelloHelper.CreateTrelloBoards(seedBoard.name);
                //Create Converstations
                foreach (var seedList in seedLists)
                {
                    await TrelloHelper.CreateTrelloLists(seedList.name, seedBoard.id);

                    foreach (var seedCard in seedCards)
                    {
                        await TrelloHelper.CreateTrelloCards(seedCard.name, seedList.id, seedCard.desc);
                    }
                }
            }
        }

        public static async Task<List<Board>> LoadSeedBoards()
        {
            var files = Directory.GetFiles("/Seed/Slack/Boards");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Board>>(json));
                }
            }

            return new List<Board>();
        }

        public static async Task<List<List>> LoadSeedLists()
        {
            var files = Directory.GetFiles("/Seed/Slack/Lists");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<List>>(json));
                }
            }

            return new List<List>();
        }

        public static async Task<List<Card>> LoadSeedCards()
        {
            var files = Directory.GetFiles("/Seed/Slack/Cards");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Card>>(json));
                }
            }

            return new List<Card>();
        }

        public static async Task<List<Board>> GetTrelloBoards(string organization)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("organizations/exampleorg/boards?key=[application_key]&token=[optional_auth_token]", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<List<Board>>(authRequest);
            if (result.Data == null)
                return new List<Board>();

            return result.Data;
        }

        public static async Task<List<List>> GetTrelloLists(string board)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("boards/4eea4ffc91e31d1746000046/lists?cards=open&card_fields=name&fields=name&key=[application_key]&token=[optional_auth_token]", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<List<List>>(authRequest);
            if (result.Data == null)
                return new List<List>();

            return result.Data;
        }

        public static async Task<List<Card>> GetTrelloCards(string list)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("lists/4eea4ffc91e31d174600004a/cards?key=[application_key]&token=[optional_auth_token]", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<List<Card>>(authRequest);
            if (result.Data == null)
                return new List<Card>();

            return result.Data;
        }

        public static async Task CreateTrelloCards(string card, string idList, string desc)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("cards", Method.POST);
            authRequest.AddParameter("name", card);
            authRequest.AddParameter("idList", idList);
            authRequest.AddParameter("desc", desc);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task CreateTrelloBoards(string board)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("boards", Method.POST);
            authRequest.AddParameter("name", board);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task CreateTrelloLists(string list, string idBoard)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("lists", Method.POST);
            authRequest.AddParameter("name", list);
            authRequest.AddParameter("idBoard", idBoard);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteTrelloCards(string cardId)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("cards/" + cardId, Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteTrelloLists(string listId)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("boards/4eea4ffc91e31d1746000046/lists?cards=open&card_fields=name&fields=name&key=[application_key]&token=[optional_auth_token]", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteTrelloBoards(string boardId)
        {
            var authEndPoint = new RestClient("https://api.trello.com/1");
            var authRequest = new RestRequest("organizations/exampleorg/boards?key=[application_key]&token=[optional_auth_token]", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public class LabelNames
        {
            public string green { get; set; }
            public string yellow { get; set; }
            public string orange { get; set; }
            public string red { get; set; }
            public string purple { get; set; }
            public string blue { get; set; }
            public string sky { get; set; }
            public string lime { get; set; }
            public string pink { get; set; }
            public string black { get; set; }
        }

        public class Membership
        {
            public string id { get; set; }
            public string idMember { get; set; }
            public string memberType { get; set; }
            public bool unconfirmed { get; set; }
        }

        public class Prefs
        {
            public string permissionLevel { get; set; }
            public string voting { get; set; }
            public string comments { get; set; }
            public string invitations { get; set; }
            public bool selfJoin { get; set; }
            public bool cardCovers { get; set; }
            public bool calendarFeedEnabled { get; set; }
            public string background { get; set; }
            public string backgroundColor { get; set; }
            public object backgroundImage { get; set; }
            public object backgroundImageScaled { get; set; }
            public bool backgroundTile { get; set; }
            public string backgroundBrightness { get; set; }
            public bool canBePublic { get; set; }
            public bool canBeOrg { get; set; }
            public bool canBePrivate { get; set; }
            public bool canInvite { get; set; }
        }

        public class Board
        {
            public string id { get; set; }
            public bool closed { get; set; }
            public object dateLastActivity { get; set; }
            public object dateLastView { get; set; }
            public string desc { get; set; }
            public object descData { get; set; }
            public string idOrganization { get; set; }
            public List<object> invitations { get; set; }
            public bool invited { get; set; }
            public LabelNames labelNames { get; set; }
            public List<Membership> memberships { get; set; }
            public string name { get; set; }
            public bool pinned { get; set; }
            public List<object> powerUps { get; set; }
            public Prefs prefs { get; set; }
            public string shortLink { get; set; }
            public string shortUrl { get; set; }
            public object starred { get; set; }
            public object subscribed { get; set; }
            public string url { get; set; }
        }
       
        public class List
        {
            public string id { get; set; }
            public string name { get; set; }
            public List<Card> cards { get; set; }
        }

        public class Badges
        {
            public int votes { get; set; }
            public bool viewingMemberVoted { get; set; }
            public bool subscribed { get; set; }
            public string fogbugz { get; set; }
            public int checkItems { get; set; }
            public int checkItemsChecked { get; set; }
            public int comments { get; set; }
            public int attachments { get; set; }
            public bool description { get; set; }
            public object due { get; set; }
        }

        public class Card
        {
            public string id { get; set; }
            public Badges badges { get; set; }
            public List<object> checkItemStates { get; set; }
            public bool closed { get; set; }
            public string dateLastActivity { get; set; }
            public string desc { get; set; }
            public object descData { get; set; }
            public object due { get; set; }
            public object email { get; set; }
            public object idAttachmentCover { get; set; }
            public string idBoard { get; set; }
            public List<object> idChecklists { get; set; }
            public List<object> idLabels { get; set; }
            public string idList { get; set; }
            public List<object> idMembers { get; set; }
            public List<object> idMembersVoted { get; set; }
            public int idShort { get; set; }
            public List<object> labels { get; set; }
            public bool manualCoverAttachment { get; set; }
            public string name { get; set; }
            public int pos { get; set; }
            public string shortLink { get; set; }
            public string shortUrl { get; set; }
            public object subscribed { get; set; }
            public string url { get; set; }
        }
    }
}
