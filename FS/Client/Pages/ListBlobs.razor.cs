using FileExplorer.Client.Models;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace FileExplorer.Client.Pages
{
    public class ListBlobsBase : ComponentBase
    {
        [Inject]
        HttpClient Http { get; set; }

        [Parameter]
        public string FileUrl { get; set; }

        [Parameter]
        public string UserId { get; set; }

        protected List<BlobModel> blobs;

        protected override async Task OnInitializedAsync()
        {
            blobs = await Http.GetJsonAsync<List<BlobModel>>($"list?userId={UserId}");
        }
    }
}
