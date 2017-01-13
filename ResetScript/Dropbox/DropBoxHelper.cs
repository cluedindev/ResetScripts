using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RestSharp;
using Newtonsoft.Json;

namespace ResetScript.DropBox
{
    public static class DropBoxHelper
    {
        public static string DropBoxApiToken = "";

        public async static Task DeleteDropBoxData()
        {
            var folders = await DropBoxHelper.GetDropBoxFolders("0");
            foreach (var folder in folders.entries)
            {
                //  Delete Channels
                //  Delete Files
                await DropBoxHelper.DeleteDropBoxEntity(folder.id);
                //  Delete Messages
            }
        }

        public async static Task CreateDropBoxData()
        {
            var seedFolders = await DropBoxHelper.LoadSeedFolders();
            var seedFiles = await DropBoxHelper.LoadSeedFiles();
            foreach (var seedFolder in seedFolders)
            {
                await DropBoxHelper.CreateDropBoxFolder(seedFolder);
                //Create Converstations
            }

            foreach (var seedFile in seedFiles)
            {
                var fileInfo = new FileInfo(seedFile);
                await DropBoxHelper.UploadDropBoxFile(seedFile, fileInfo.Name);
                //Create Converstations
            }
        }

        public static async Task<string[]> LoadSeedFolders()
        {
            return Directory.GetDirectories("/Seed/DropBox/Folders");
        }

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/DropBox/Files");
        }

        public static async Task<FolderResponse> GetDropBoxFolders(string path)
        {
            var authEndPoint = new RestClient("https://api.dropboxapi.com/2");
            var authRequest = new RestRequest("files/list_folder", Method.GET);
            authRequest.AddJsonBody(new FolderPost() { path = path, include_deleted = true, include_has_explicit_shared_members = true, include_media_info = true, recursive = true});
            var result = await authEndPoint.ExecuteTaskAsync<FolderResponse>(authRequest);
            if (result.Data == null)
                return new FolderResponse();

            return result.Data;
        }

        public static async Task CreateDropBoxFolder(string name)
        {
            var authEndPoint = new RestClient("https://api.box.com/2.0");
            var authRequest = new RestRequest("folders", Method.POST);
            authRequest.AddParameter("name", name);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task<bool> UploadDropBoxFile(string path, string fileName)
        {
            var authEndPoint = new RestClient("https://content.dropboxapi.com/2/");
            var authRequest = new RestRequest("files/upload", Method.POST);
            authRequest.RequestFormat = DataFormat.Json;
            authRequest.AddHeader("Content-Type", "multipart/form-data");
            authRequest.AddJsonBody(new DropboxUploadPost() { path = path, autorename = false, mode = "", mute = false});

            var result = await authEndPoint.ExecuteTaskAsync<DropboxUploadResponse>(authRequest);
            if (result.Data != null)
                return true;

            return false;
        }

        public static async Task DeleteDropBoxEntity(string path)
        {
            var authEndPoint = new RestClient("https://api.dropboxapi.com/2");
            var authRequest = new RestRequest("files/delete", Method.DELETE);
            authRequest.AddJsonBody(new DropboxFileDeletePost() { path = path });
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

       

        public class DropboxUploadResponse
        {
            public string name { get; set; }
            public string id { get; set; }
            public string client_modified { get; set; }
            public string server_modified { get; set; }
            public string rev { get; set; }
            public int size { get; set; }
            public string path_lower { get; set; }
            public string path_display { get; set; }
            public SharingInfo sharing_info { get; set; }
            public List<PropertyGroup> property_groups { get; set; }
            public bool has_explicit_shared_members { get; set; }
            public string content_hash { get; set; }
        }

        public class DropboxUploadPost
        {
            public string path { get; set; }
            public string mode { get; set; }
            public bool autorename { get; set; }
            public bool mute { get; set; }
        }

        public class DropboxFileDeletePost
        {
            public string path { get; set; }
        }

        public class FolderPost
        {
            public string path { get; set; }
            public bool recursive { get; set; }
            public bool include_media_info { get; set; }
            public bool include_deleted { get; set; }
            public bool include_has_explicit_shared_members { get; set; }
        }

        public class SharingInfo
        {
            public bool read_only { get; set; }
            public string parent_shared_folder_id { get; set; }
            public string modified_by { get; set; }
            public bool? traverse_only { get; set; }
            public bool? no_access { get; set; }
        }

        public class Field
        {
            public string name { get; set; }
            public string value { get; set; }
        }

        public class PropertyGroup
        {
            public string template_id { get; set; }
            public List<Field> fields { get; set; }
        }

        public class Entry
        {
            public string name { get; set; }
            public string id { get; set; }
            public string client_modified { get; set; }
            public string server_modified { get; set; }
            public string rev { get; set; }
            public int size { get; set; }
            public string path_lower { get; set; }
            public string path_display { get; set; }
            public SharingInfo sharing_info { get; set; }
            public List<PropertyGroup> property_groups { get; set; }
            public bool has_explicit_shared_members { get; set; }
            public string content_hash { get; set; }
        }

        public class FolderResponse
        {
            public List<Entry> entries { get; set; }
            public string cursor { get; set; }
            public bool has_more { get; set; }
        }
    }
}
