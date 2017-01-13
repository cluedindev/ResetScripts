using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.Box
{
    public static class BoxHelper
    {
        public static string BoxApiToken = "";

        public async static Task DeleteBoxData()
        {
            var folders = await BoxHelper.GetBoxFolders("0");
            foreach (var folder in folders.entries)
            {
                //  Delete Channels
                //  Delete Files
                await BoxHelper.DeleteBoxFolder(folder.id);
                await BoxHelper.DeleteBoxFile(folder.id);
                //  Delete Messages
            }
        }

        public async static Task CreateBoxData()
        {
            var seedFolders = await BoxHelper.LoadSeedFolders();
            var seedFiles = await BoxHelper.LoadSeedFiles();
            foreach (var seedFolder in seedFolders.entries)
            {
                await BoxHelper.CreateBoxFolder(seedFolder.name, seedFolder.parent.id, seedFolder.id);
                //Create Converstations
            }

            foreach (var seedFile in seedFiles)
            {
                var fileInfo = new FileInfo(seedFile);
                await BoxHelper.UploadBoxFile(seedFile, fileInfo.Name);
                //Create Converstations
            }
        }
      
        public static async Task<Folder> LoadSeedFolders()
        {
            var files = Directory.GetFiles("/Seed/Box/Folders");
            foreach (var file in files)
            {
                using (var r = new StreamReader(file))
                {
                    string json = await r.ReadToEndAsync();
                    return await Task.Factory.StartNew(() => JsonConvert.DeserializeObject<Folder>(json));
                }
            }

            return new Folder();
        }

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/Box/Files");
        }

        public static async Task<Folder> GetBoxFolders(string folderId)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("folders/FOLDER_ID/items?limit=30&offset=0", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<Folder>(authRequest);
            if (result.Data == null)
                return new Folder();

            return result.Data;
        }

        public static async Task CreateBoxFolder(string name, string parent, string id)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("folders", Method.POST);
            authRequest.AddParameter("name", name);
            authRequest.AddParameter("parent", parent);
            authRequest.AddParameter("id", id);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task<bool> UploadBoxFile(string path, string fileName)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("files.upload", Method.POST);
            authRequest.AddParameter("attributes", ""); // adds to POST or URL querystring based on Method
            authRequest.RequestFormat = DataFormat.Json;
            authRequest.AddHeader("Content-Type", "multipart/form-data");
            authRequest.AddHeader("Content-MD5", path);
            authRequest.AddParameter("name", fileName);
            authRequest.AddParameter("parent", fileName);
            authRequest.AddParameter("id", fileName);

            var result = await authEndPoint.ExecuteTaskAsync<File>(authRequest);
            if (result.Data != null)
                return true;

            return false;
        }

        public static async Task DeleteBoxFile(string fileId)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("files/FILE_ID/trash", Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteBoxFolder(string folderId)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("folders/FOLDER_ID/trash", Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public class Order
        {
            public string by { get; set; }
            public string direction { get; set; }
        }

        public class Folder
        {
            public int total_count { get; set; }
            public List<Entry> entries { get; set; }
            public int offset { get; set; }
            public int limit { get; set; }
            public List<Order> order { get; set; }
        }

        public class Entry2
        {
            public string type { get; set; }
            public string id { get; set; }
            public string sequence_id { get; set; }
            public string etag { get; set; }
            public string name { get; set; }
        }

        public class PathCollection
        {
            public int total_count { get; set; }
            public List<Entry2> entries { get; set; }
        }

        public class CreatedBy
        {
            public string type { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string login { get; set; }
        }

        public class ModifiedBy
        {
            public string type { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string login { get; set; }
        }

        public class OwnedBy
        {
            public string type { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string login { get; set; }
        }

        public class Parent
        {
            public string type { get; set; }
            public string id { get; set; }
            public string sequence_id { get; set; }
            public string etag { get; set; }
            public string name { get; set; }
        }

        public class Entry
        {
            public string type { get; set; }
            public string id { get; set; }
            public string sequence_id { get; set; }
            public string etag { get; set; }
            public string sha1 { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int size { get; set; }
            public PathCollection path_collection { get; set; }
            public string created_at { get; set; }
            public string modified_at { get; set; }
            public object trashed_at { get; set; }
            public object purged_at { get; set; }
            public string content_created_at { get; set; }
            public string content_modified_at { get; set; }
            public CreatedBy created_by { get; set; }
            public ModifiedBy modified_by { get; set; }
            public OwnedBy owned_by { get; set; }
            public object shared_link { get; set; }
            public Parent parent { get; set; }
            public string item_status { get; set; }
        }

        public class File
        {
            public int total_count { get; set; }
            public List<Entry> entries { get; set; }
        }
    }
}
