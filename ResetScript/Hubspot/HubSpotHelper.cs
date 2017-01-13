using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.Hubspot
{
    public static class HubspotHelper
    {
        public static string HubspotApiToken = "";

        public async static Task DeleteHubspotData()
        {
            var companies = await HubspotHelper.GetHubspotCompanies();
            foreach (var company in companies)
            {
                //  Delete Channels
                //  Delete Files
                await HubspotHelper.DeleteHubspotEntity(company.companyId, company.portalId);
                //  Delete Messages
            }
        }

        public async static Task CreateHubspotData()
        {
            var seedCompanies = await HubspotHelper.LoadSeedCompany();
            foreach (var seedCompany in seedCompanies)
            {
                await HubspotHelper.CreateHubspotCompany(seedCompany.properties.name.value, seedCompany.properties.description.value);
                //Create Converstations
            }
        }

        public static async Task<List<CompanyReturn>> LoadSeedCompany()
        {
            var files = Directory.GetFiles("/Seed/Hubspot/Companies");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<List<CompanyReturn>>(json));
                }
            }

            return new List<CompanyReturn>();
        }

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/Hubspot/Files");
        }

        public static async Task<List<CompanyReturn>> GetHubspotCompanies()
        {
            var authEndPoint = new RestClient("https://api.hubapi.com/companies/v2");
            var authRequest = new RestRequest("companies/paged?hapikey=demo&properties=name&properties=website&limit=2", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<List<CompanyReturn>>(authRequest);
            if (result.Data == null)
                return new List<CompanyReturn>();

            return result.Data;
        }

        public static async Task CreateHubspotCompany(string name, string description)
        {
            var authEndPoint = new RestClient("https://api.hubapi.com/companies/v2/");
            var authRequest = new RestRequest("companies?hapikey=demo", Method.POST);
            authRequest.AddJsonBody(new Company() { properties = new List<Property>() { new Property() { name = "name", value = name }, new Property() { name = "description", value = name } } });

            var result = await authEndPoint.ExecuteTaskAsync<CompanyReturn>(authRequest);
        }

        public static async Task<bool> DeleteHubspotEntity(int id, int portalId)
        {
            var authEndPoint = new RestClient("https://api.hubapi.com/companies/v2");
            var authRequest = new RestRequest("companies/" + id + "?hapikey=demo&portalId=" + portalId, Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync<HubSpotDeleteResponse>(authRequest);
            if (result.Data.deleted)
                return true;

            return false;
        }

        public class HubSpotDeleteResponse
        {
            public int companyId { get; set; }
            public bool deleted { get; set; }
        }

        public class Property
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class Company
        {
            public List<Property> properties { get; set; }
        }

        public class Version
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public List<object> sourceVid { get; set; }
        }

        public class Description
        {
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public object sourceId { get; set; }
            public List<Version> versions { get; set; }
        }

        public class Version2
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public List<object> sourceVid { get; set; }
        }

        public class Name
        {
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public object sourceId { get; set; }
            public List<Version2> versions { get; set; }
        }

        public class Version3
        {
            public string name { get; set; }
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public List<object> sourceVid { get; set; }
        }

        public class Createdate
        {
            public string value { get; set; }
            public long timestamp { get; set; }
            public string source { get; set; }
            public object sourceId { get; set; }
            public List<Version3> versions { get; set; }
        }

        public class Properties
        {
            public Description description { get; set; }
            public Name name { get; set; }
            public Createdate createdate { get; set; }
        }

        public class CompanyReturn
        {
            public int portalId { get; set; }
            public int companyId { get; set; }
            public bool isDeleted { get; set; }
            public Properties properties { get; set; }
        }
    }
}
