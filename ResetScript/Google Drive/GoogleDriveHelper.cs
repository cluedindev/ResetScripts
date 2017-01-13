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

namespace ResetScript.GoogleDrive
{
    public static class GoogleDriveHelper
    {
        public static string GoogleDriveApiToken = "";

        public async static Task DeleteGoogleDriveData()
        {
            var folders = await GoogleDriveHelper.GetGoogleDriveFolders("0");
            foreach (var folder in folders.files)
            {
                //  Delete Channels
                //  Delete Files
                await GoogleDriveHelper.DeleteGoogleDriveEntity(folder.id);
                //  Delete Messages
            }
        }

        public async static Task CreateGoogleDriveData()
        {
            var seedFolders = await GoogleDriveHelper.LoadSeedFolders();
            var seedFiles = await GoogleDriveHelper.LoadSeedFiles();
            foreach (var seedFolder in seedFolders)
            {
                await GoogleDriveHelper.CreateGoogleDriveFolder(seedFolder);
                //Create Converstations
            }

            foreach (var seedFile in seedFiles)
            {
                var fileInfo = new FileInfo(seedFile);
                await GoogleDriveHelper.UploadGoogleDriveFile(seedFile, fileInfo.Name);
                //Create Converstations
            }
        }

        public static async Task<string[]> LoadSeedFolders()
        {
            return Directory.GetDirectories("/Seed/GoogleDrive/Folders");
        }

        public static async Task<string[]> LoadSeedFiles()
        {
            return Directory.GetFiles("/Seed/GoogleDrive/Files");
        }

        public static async Task<FileResponse> GetGoogleDriveFolders(string path)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/drive/v3");
            var authRequest = new RestRequest("files", Method.GET);
            var result = await authEndPoint.ExecuteTaskAsync<FileResponse>(authRequest);
            if (result.Data == null)
                return new FileResponse();

            return result.Data;
        }


        public static async Task UploadGoogleDriveFile(string userId, string body)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/drive/v3");
            var authRequest = new RestRequest("files", Method.POST);
            authRequest.AddParameter("uploadType", "multipart");
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task CreateGoogleDriveFolder(string name)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/drive/v3");
            var authRequest = new RestRequest("files", Method.POST);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public static async Task DeleteGoogleDriveEntity(string path)
        {
            var authEndPoint = new RestClient("https://www.googleapis.com/drive/v3");
            var authRequest = new RestRequest("files/fileId", Method.DELETE);
            var result = await authEndPoint.ExecuteTaskAsync(authRequest);
        }

        public class SharingUser
        {
            public string kind { get; set; }
            public string displayName { get; set; }
            public string photoLink { get; set; }
            public bool me { get; set; }
            public string permissionId { get; set; }
            public string emailAddress { get; set; }
        }

        public class Owner
        {
            public string kind { get; set; }
            public string displayName { get; set; }
            public string photoLink { get; set; }
            public bool me { get; set; }
            public string permissionId { get; set; }
            public string emailAddress { get; set; }
        }

        public class LastModifyingUser
        {
            public string kind { get; set; }
            public string displayName { get; set; }
            public string photoLink { get; set; }
            public bool me { get; set; }
            public string permissionId { get; set; }
            public string emailAddress { get; set; }
        }

        public class Capabilities
        {
            public bool canEdit { get; set; }
            public bool canComment { get; set; }
            public bool canShare { get; set; }
            public bool canCopy { get; set; }
            public bool canReadRevisions { get; set; }
        }

        public class Thumbnail
        {
            public string mimeType { get; set; }
        }

        public class ContentHints
        {
            public Thumbnail thumbnail { get; set; }
            public string indexableText { get; set; }
        }

        public class Location
        {
            public double latitude { get; set; }
            public double longitude { get; set; }
            public double altitude { get; set; }
        }

        public class ImageMediaMetadata
        {
            public int width { get; set; }
            public int height { get; set; }
            public int rotation { get; set; }
            public Location location { get; set; }
            public string time { get; set; }
            public string cameraMake { get; set; }
            public string cameraModel { get; set; }
            public double exposureTime { get; set; }
            public double aperture { get; set; }
            public bool flashUsed { get; set; }
            public double focalLength { get; set; }
            public int isoSpeed { get; set; }
            public string meteringMode { get; set; }
            public string sensor { get; set; }
            public string exposureMode { get; set; }
            public string colorSpace { get; set; }
            public string whiteBalance { get; set; }
            public double exposureBias { get; set; }
            public double maxApertureValue { get; set; }
            public int subjectDistance { get; set; }
            public string lens { get; set; }
        }

        public class VideoMediaMetadata
        {
            public int width { get; set; }
            public int height { get; set; }
            public long durationMillis { get; set; }
        }

        public class File
        {
            public string kind { get; set; }
            public string id { get; set; }
            public string name { get; set; }
            public string mimeType { get; set; }
            public string description { get; set; }
            public bool starred { get; set; }
            public bool trashed { get; set; }
            public bool explicitlyTrashed { get; set; }
            public List<string> parents { get; set; }
            public List<string> spaces { get; set; }
            public int version { get; set; }
            public string webContentLink { get; set; }
            public string webViewLink { get; set; }
            public string iconLink { get; set; }
            public string thumbnailLink { get; set; }
            public bool viewedByMe { get; set; }
            public SharingUser sharingUser { get; set; }
            public List<Owner> owners { get; set; }
            public LastModifyingUser lastModifyingUser { get; set; }
            public bool shared { get; set; }
            public bool ownedByMe { get; set; }
            public Capabilities capabilities { get; set; }
            public bool viewersCanCopyContent { get; set; }
            public bool writersCanShare { get; set; }
            public string folderColorRgb { get; set; }
            public string originalFilename { get; set; }
            public string fullFileExtension { get; set; }
            public string fileExtension { get; set; }
            public string md5Checksum { get; set; }
            public int size { get; set; }
            public int quotaBytesUsed { get; set; }
            public string headRevisionId { get; set; }
            public ContentHints contentHints { get; set; }
            public ImageMediaMetadata imageMediaMetadata { get; set; }
            public VideoMediaMetadata videoMediaMetadata { get; set; }
            public bool isAppAuthorized { get; set; }
        }

        public class FileResponse
        {
            public string kind { get; set; }
            public string nextPageToken { get; set; }
            public List<File> files { get; set; }
        }
    }
}
