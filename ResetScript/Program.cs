using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ResetScript.Box;
using ResetScript.Gmail;
using ResetScript.GoogleCalendar;
using ResetScript.GoogleContacts;
using ResetScript.GoogleDrive;
using ResetScript.Slack;
using ResetScript.DropBox;
using ResetScript.Hubspot;
using ResetScript.Twitter;
using ResetScript.ZenDesk;
using RestSharp;
using Newtonsoft.Json;
using System.IO;
using Nito.AsyncEx;

namespace ResetScript
{
    class Program
    {
        /// <summary>Mains the asynchronous.</summary>
        /// <param name="args">The arguments.</param>
        static async void MainAsync(string[] args)
        {
            //Show deletion progress
            //If failure, tell user to manually cleanup.

            //Delete Everything
            //Slack
            await SlackHelper.DeleteSlackData();
            await TrelloHelper.DeleteTrelloData();
            await BoxHelper.DeleteBoxData();
            await DropBoxHelper.DeleteDropBoxData();
            await HubspotHelper.DeleteHubspotData();
            await GmailHelper.DeleteGmailData();
            await GoogleCalendarHelper.DeleteGoogleCalendarData();
            await GoogleContactsHelper.DeleteGoogleContactsData();
            await GoogleDriveHelper.DeleteGoogleDriveData();
            await TwitterHelper.DeleteTwitterData();
            await ZenDeskHelper.DeleteZenDeskData();
            //Hubspot
            //  Delete Customers
            //  Delete Copmanies

            //

            //CluedIn
            //  Delete everything but the account

            //Seed Tools
            //Slack
            //      Create Legal, Marketing, Sales, Support and Engineering Channels
            await SlackHelper.CreateSlackData();
            await TrelloHelper.CreateTrelloData();
            await BoxHelper.CreateBoxData();
            await DropBoxHelper.CreateDropBoxData();
            await HubspotHelper.CreateHubspotData();
            await GmailHelper.CreateGmailData();
            await GoogleCalendarHelper.CreateGoogleCalendarData();
            await GoogleContactsHelper.CreateGoogleContactsData();
            await GoogleDriveHelper.CreateGoogleDriveData();
            await TwitterHelper.CreateTwitterData();
            await ZenDeskHelper.CreateZenDeskData();
            //      Upload Files

            //Clear Demo Folder
            //  Clear Local Folder that may have been changed locally.

            //Seed Demo Folder
        }

        static void Main(string[] args)
        {
            AsyncContext.Run(() => MainAsync(args));
        }
    }
}
