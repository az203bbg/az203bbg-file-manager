using FileExplorer.Client.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FileExplorer.Server.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ListController : ControllerBase
    {
        public string ConnString { get; set; }
        public ListController(IConfiguration conf)
        {
            ConnString = conf["BlobStorageConnectionString"];
        }

        // TODO: Implement pagination
        [HttpGet]
        public async Task<List<BlobModel>> Get(string userId)
        {
            var account = CloudStorageAccount.Parse(ConnString);
            var blobClient = account.CreateCloudBlobClient();
            var containerName = GetContainerNameByUserId(userId);
            var container = blobClient.GetContainerReference(containerName);

            BlobContinuationToken token = null;
            var list = new List<CloudBlockBlob>();
            do
            {
                var blobs = await container.ListBlobsSegmentedAsync(token);
                list.AddRange(blobs.Results.OfType<CloudBlockBlob>());
                token = blobs.ContinuationToken;
            } 
            while (token != null);

            return list.Select(x => new BlobModel() {Container = x.Container.Name, Name = x.Name, Uri = x.Uri.ToString() }).ToList();
        }

        private string GetContainerNameByUserId(string userId)
        {
            return "usercontainer";
        }
    }
}