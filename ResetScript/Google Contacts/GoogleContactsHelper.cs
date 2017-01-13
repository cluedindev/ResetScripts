using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;
using Google.Contacts;
using Google.GData.Client;
using Google.GData.Contacts;

namespace ResetScript.GoogleContacts
{
    public static class GoogleContactsHelper
    {
        public static string GoogleContactsApiToken = "";

        public async static Task DeleteGoogleContactsData()
        {
            var companies = GoogleContactsHelper.GetGoogleContactsMail();
            foreach (var company in companies)
            {
                //  Delete Channels
                //  Delete Files
                await GoogleContactsHelper.DeleteGoogleContactsEntity(company);
                //  Delete Messages
            }
        }

        public async static Task CreateGoogleContactsData()
        {
            var seedCompanies = await GoogleContactsHelper.LoadSeedCompany();
            foreach (var seedCompany in seedCompanies)
            {
                await GoogleContactsHelper.CreateGoogleContactsEvent(seedCompany);
                //Create Converstations
            }
        }

        public static async Task<List<Contact>> LoadSeedCompany()
        {
            var files = Directory.GetFiles("/Seed/GoogleContacts/Contacts");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<Contact>>(json));
                }
            }

            return new List<Contact>();
        }

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/GoogleContacts/Files");
        }

        public static IEnumerable<Contact> GetGoogleContactsMail()
        {
            var client = new RequestSettings("CluedIn", GoogleContactsApiToken);
            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
            query.NumberToRetrieve = 50000;
            ContactsRequest cr = new ContactsRequest(client);
            Feed<Contact> f = cr.Get<Contact>(query);
            f.AutoPaging = true;

            return f.Entries;
        }

        public static async Task CreateGoogleContactsEvent(Contact contact)
        {
            var client = new RequestSettings("CluedIn", GoogleContactsApiToken);
            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
            query.NumberToRetrieve = 50000;
            ContactsRequest cr = new ContactsRequest(client);
            var f = cr.Insert<Contact>(new Uri(""), contact);
        }

        public static async Task DeleteGoogleContactsEntity(Contact contact)
        {
            var client = new RequestSettings("CluedIn", GoogleContactsApiToken);
            ContactsQuery query = new ContactsQuery(ContactsQuery.CreateContactsUri("default"));
            query.NumberToRetrieve = 50000;
            ContactsRequest cr = new ContactsRequest(client);
            cr.Delete<Contact>(contact);
        }

        public class GoogleContactsUploadPost
        {
            public string raw { get; set; }
        }

        public class DefaultReminder
        {
            public string method { get; set; }
            public int minutes { get; set; }
        }

        public class Creator
        {
            public string id { get; set; }
            public string email { get; set; }
            public string displayName { get; set; }
            public bool self { get; set; }
        }

        public class Organizer
        {
            public string id { get; set; }
            public string email { get; set; }
            public string displayName { get; set; }
            public bool self { get; set; }
        }

        public class Start
        {
            public string timeZone { get; set; }
        }

        public class End
        {
            public string timeZone { get; set; }
        }

        public class OriginalStartTime
        {
            public string timeZone { get; set; }
        }

        public class Attendee
        {
            public string id { get; set; }
            public string email { get; set; }
            public string displayName { get; set; }
            public bool organizer { get; set; }
            public bool self { get; set; }
            public bool resource { get; set; }
            public bool optional { get; set; }
            public string responseStatus { get; set; }
            public string comment { get; set; }
            public int additionalGuests { get; set; }
        }

        public class Gadget
        {
            public string type { get; set; }
            public string title { get; set; }
            public string link { get; set; }
            public string iconLink { get; set; }
            public int width { get; set; }
            public int height { get; set; }
            public string display { get; set; }
        }

        public class Override
        {
            public string method { get; set; }
            public int minutes { get; set; }
        }

        public class Reminders
        {
            public bool useDefault { get; set; }
            public List<Override> overrides { get; set; }
        }

        public class Source
        {
            public string url { get; set; }
            public string title { get; set; }
        }

        public class Attachment
        {
            public string fileUrl { get; set; }
            public string title { get; set; }
            public string mimeType { get; set; }
            public string iconLink { get; set; }
            public string fileId { get; set; }
        }

        public class CalendarEvent
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string id { get; set; }
            public string status { get; set; }
            public string htmlLink { get; set; }
            public string summary { get; set; }
            public string description { get; set; }
            public string location { get; set; }
            public string colorId { get; set; }
            public Creator creator { get; set; }
            public Organizer organizer { get; set; }
            public Start start { get; set; }
            public End end { get; set; }
            public bool endTimeUnspecified { get; set; }
            public List<string> recurrence { get; set; }
            public string recurringEventId { get; set; }
            public OriginalStartTime originalStartTime { get; set; }
            public string transparency { get; set; }
            public string visibility { get; set; }
            public string iCalUID { get; set; }
            public int sequence { get; set; }
            public List<Attendee> attendees { get; set; }
            public bool attendeesOmitted { get; set; }
            public string hangoutLink { get; set; }
            public Gadget gadget { get; set; }
            public bool anyoneCanAddSelf { get; set; }
            public bool guestsCanInviteOthers { get; set; }
            public bool guestsCanModify { get; set; }
            public bool guestsCanSeeOtherGuests { get; set; }
            public bool privateCopy { get; set; }
            public bool locked { get; set; }
            public Reminders reminders { get; set; }
            public Source source { get; set; }
            public List<Attachment> attachments { get; set; }
        }

        public class CalendarEventResponse
        {
            public string kind { get; set; }
            public string etag { get; set; }
            public string summary { get; set; }
            public string description { get; set; }
            public string timeZone { get; set; }
            public string accessRole { get; set; }
            public List<DefaultReminder> defaultReminders { get; set; }
            public string nextPageToken { get; set; }
            public string nextSyncToken { get; set; }
            public List<CalendarEvent> items { get; set; }
        }
    }
}
