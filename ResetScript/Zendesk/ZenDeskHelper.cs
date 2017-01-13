using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using ZendeskApi_v2;
using ZendeskApi_v2.Models.Tickets;

namespace ResetScript.ZenDesk
{
    public static class ZenDeskHelper
    {
        public static string ZenDeskApiToken = "";

        public async static Task DeleteZenDeskData()
        {
            var client = new ZendeskApi("cluedinhelp.zendesk.com", "username", "password");
            var statuses = await ZenDeskHelper.GetZenDeskStatuses(client);
            foreach (var status in statuses.Tickets)
            {
                //  Delete Channels
                //  Delete Files
                await ZenDeskHelper.DeleteZenDeskCards(client, status);
                //  Delete Messages
            }
        }

        public async static Task CreateZenDeskData()
        {
            var seedBoards = await ZenDeskHelper.LoadSeedBoards();
            var client = new ZendeskApi("cluedinhelp.zendesk.com", "username", "password");
            foreach (var seedBoard in seedBoards)
            {
                //  Delete Channels
                //  Delete Files
                await ZenDeskHelper.CreateZenDeskCards(client, seedBoard);
                //  Delete Messages
            }
        }

        public static async Task<List<Ticket>> LoadSeedBoards()
        {
            var files = Directory.GetFiles("/Seed/ZenDesk/Status");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Ticket>>(json));
                }
            }

            return new List<Ticket>();
        }

        public static async Task<GroupTicketResponse> GetZenDeskStatuses(ZendeskApi client)
        {
            return await client.Tickets.GetAllTicketsAsync();
        }

        public static async Task CreateZenDeskCards(ZendeskApi client, Ticket ticket)
        {
            await client.Tickets.CreateTicketAsync(ticket);
        }

        public static async Task DeleteZenDeskCards(ZendeskApi client, Ticket ticket)
        {
            if (ticket.Id != null)
                await client.Tickets.DeleteAsync(ticket.Id.Value);
        }
    }
}
