using System.IO;
using System.Threading.Tasks;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Azure.ResourceManager.Storage;
using Newtonsoft.Json;
using Azure.Identity;
using Azure.ResourceManager;
using Azure.ResourceManager.Storage.Models;
using Azure.Core;
using Azure.ResourceManager.Resources;
using Microsoft.Azure.Management.Storage.Models;

namespace AzureFuncs
{
    public class StorageFWFuncCS
    {
        private readonly ILogger<StorageFWFuncCS> _logger;

        public StorageFWFuncCS(ILogger<StorageFWFuncCS> logger)
        {
            _logger = logger;
        }

        [Function(nameof(StorageFWFuncCS))]
        public async Task Run([BlobTrigger("firewallrules2/storagefw2.json", Connection = "MetadataStoreConnection")] Stream stream, string name)
        {
            try
            {
                using var blobStreamReader = new StreamReader(stream);
                var content = await blobStreamReader.ReadToEndAsync();
                _logger.LogInformation($"C# Blob trigger function Processed blob\n Data: {content}");

                Root root = JsonConvert.DeserializeObject<Root>(content);
                _logger.LogInformation($"C# Blob trigger total rules\n Data: {root.storagefirewalls.Count}");

                ArmClient client = new ArmClient(new DefaultAzureCredential());

                foreach (Storagefirewall storagefirewall in root.storagefirewalls)
                {
                    _logger.LogInformation($"Targeting Storage Resource: {storagefirewall.subId} - {storagefirewall.rgName} - {storagefirewall.accountName}");
                    ResourceIdentifier storageResourceId = StorageAccountResource.CreateResourceIdentifier(storagefirewall.subId, storagefirewall.rgName, storagefirewall.accountName);
                    StorageAccountResource storageAccountResource = client.GetStorageAccountResource(storageResourceId);
                    if (storagefirewall.isPrivate == true)
                    {
                        _logger.LogInformation($"Disabling Public Access to Storage Resource: {storagefirewall.subId} - {storagefirewall.rgName} - {storagefirewall.accountName}");
                        StorageAccountPatch storageAccountPatch = new StorageAccountPatch();
                        storageAccountPatch.PublicNetworkAccess = StoragePublicNetworkAccess.Disabled;
                        storageAccountResource.Update(storageAccountPatch);
                        _logger.LogInformation($"Disabled Public Access to Storage Resource: {storagefirewall.subId} - {storagefirewall.rgName} - {storagefirewall.accountName}");
                    }
                    else
                    {
                        _logger.LogInformation($"Enabling Public Access to Storage Resource: {storagefirewall.subId} - {storagefirewall.rgName} - {storagefirewall.accountName}");
                        StorageAccountPatch storageAccountPatch = new StorageAccountPatch();
                        storageAccountPatch.PublicNetworkAccess = StoragePublicNetworkAccess.Enabled;
                        storageAccountPatch.NetworkRuleSet = new StorageAccountNetworkRuleSet(StorageNetworkDefaultAction.Deny);
                        if (string.IsNullOrEmpty(storagefirewall.firewallIPs) == false && string.IsNullOrWhiteSpace(storagefirewall.firewallIPs) == false)
                        {
                            string[] listOfIps = storagefirewall.firewallIPs.Split(",");
                            foreach (string ip in listOfIps)
                            {
                                storageAccountPatch.NetworkRuleSet.IPRules.Add(new StorageAccountIPRule(ip));
                            }
                        }
                        storageAccountResource.Update(storageAccountPatch);
                        _logger.LogInformation($"Enabled Public Access to Storage Resource: {storagefirewall.subId} - {storagefirewall.rgName} - {storagefirewall.accountName}");
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.LogError($"C# Blob trigger function Exception\n Data: {ex.Message}");
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
}
