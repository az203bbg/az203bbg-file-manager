using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using System.Net.Http;

namespace FileExplorer.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddBaseAddressHttpClient();

            await builder.Build().RunAsync();
        }
    }

    public static class FileUtil
    {
        public async static Task SaveAs(IJSRuntime js, string filename, byte[] data)
        {
            await js.InvokeAsync<object>(
                "saveAsFile",
                filename,
                Convert.ToBase64String(data));
        }
    }

    public static class Test
    {
        public static IServiceCollection AddHttpClientX(this IServiceCollection serviceCollection)
        {
            return serviceCollection.AddSingleton(s =>
            {
                var handler = new HttpClientHandler()
                {
                    AutomaticDecompression = System.Net.DecompressionMethods.None
                };
                // Creating the URI helper needs to wait until the JS Runtime is initialized, so defer it.
                var navigationManager = s.GetRequiredService<NavigationManager>();
                return new HttpClient(handler)
                {
                    BaseAddress = new Uri(navigationManager.BaseUri),
                };
            });
        }
    }
}
