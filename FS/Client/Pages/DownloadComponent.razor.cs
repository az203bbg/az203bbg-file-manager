using Microsoft.AspNetCore.Components;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Blob;
using Microsoft.JSInterop;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileExplorer.Client.Pages
{
    public class DownloadComponentBase: ComponentBase
    {
        [Inject]
        IJSRuntime JsRuntime { get; set; }

        [Inject]
        HttpClient Http { get; set; }

        [Parameter]
        public string FileUrl { get; set; }

        [Parameter]
        public string UserId { get; set; }

        protected async void Download()
        {
            Console.WriteLine(FileUrl);
            var cred = await GetConnectionStringWithSasAsync();
            Console.WriteLine("cred");
            var account = CloudStorageAccount.Parse(cred);
            Console.WriteLine("account created");
            var blobClient = account.CreateCloudBlobClient();
            Console.WriteLine("blobclient created");
            var blob = await blobClient.GetBlobReferenceFromServerAsync(new Uri(FileUrl));
            Console.WriteLine("blob");
            var byteArray = new byte[blob.Properties.Length];
            await blob.DownloadToByteArrayAsync(byteArray, 0);

            Console.WriteLine("byte array");
            await FileUtil.SaveAs(JsRuntime, blob.Name, byteArray);
            Console.WriteLine("SaveAs");
        }

        private async Task<string> GetConnectionStringWithSasAsync()
        {
            var str = await Http.GetStringAsync($"sas?fileUri={FileUrl}");
            return str;
        }
    }
}
