using Azure.Identity;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System.Reflection.Metadata;
using System.Text;

namespace StoreWFUICore.Data
{
    /// <summary>
    /// Data Repository Interface
    /// </summary>
    public interface IFWDataService
    {
        public Root FWRoot { get; set; }
        public void SaveDataToBlob();
    }

    /// <summary>
    /// Data Repository Class 
    /// </summary>
    public class FWDataService : IFWDataService
    {
        public Root FWRoot { get; set; }

        private string metadataStore = String.Empty;
        private string metadataStoreContainer = String.Empty;
        private string metadataStoreBlob = String.Empty;
        private readonly IConfiguration Configuration;

        public FWDataService(IConfiguration configuration)
        {
            Configuration = configuration;
            metadataStore = Configuration["MetadataStore"];
            metadataStoreContainer = Configuration["MetadataStoreContainer"];
            metadataStoreBlob = Configuration["MetadataStoreBlob"];
            this.LoadDataFromBlob();
        }

        private void LoadDataFromBlob()
        {
            var blobServiceClient = new BlobServiceClient(new Uri(string.Format("https://{0}.blob.core.windows.net", metadataStore)),new DefaultAzureCredential());
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(metadataStoreContainer);
            BlobClient blobClient = containerClient.GetBlobClient(metadataStoreBlob);
            var response = blobClient.DownloadContent();
            BinaryData data = response.Value.Content;
            var blobContents = Encoding.UTF8.GetString(data);
            if (string.IsNullOrEmpty(blobContents) == false && string.IsNullOrWhiteSpace(blobContents) == false)
            {
                FWRoot = JsonConvert.DeserializeObject<Root>(blobContents);
            }
            

            if (FWRoot == null)
            {
                FWRoot = new Root();
                FWRoot.storagefirewalls = new List<Storagefirewall>();
            }
        }

        public void SaveDataToBlob()
        {
            string jsonRoot = String.Empty;
            if (FWRoot != null && FWRoot.storagefirewalls.Count > 0)
            {
                jsonRoot = JsonConvert.SerializeObject(FWRoot);
                var blobServiceClient = new BlobServiceClient(new Uri(string.Format("https://{0}.blob.core.windows.net", metadataStore)), new DefaultAzureCredential());
                BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(metadataStoreContainer);
                BlobClient blobClient = containerClient.GetBlobClient(metadataStoreBlob);
                var content = Encoding.UTF8.GetBytes(jsonRoot);
                using (var ms = new MemoryStream(content))
                    blobClient.Upload(ms, true);
            }
        }
    }

    public class Root
    {
        public List<Storagefirewall> storagefirewalls { get; set; }
    }

    public class Storagefirewall
    {
        public string subId { get; set; }
        public string rgName { get; set; }
        public string accountName { get; set; }
        public bool isPrivate { get; set; }
        public string firewallIPs { get; set; }
    }

    public static class FWExtensions
    {
        public static string GetKey(this Storagefirewall storagefirewall)
        {
            return string.Format("{0}-{1}-{2}", storagefirewall.subId.Trim(), storagefirewall.rgName.Trim(), storagefirewall.accountName.Trim());
        }
    }
}
