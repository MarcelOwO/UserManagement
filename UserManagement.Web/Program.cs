using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UserManagement.Web;
using UserManagement.Web.Services;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddMudServices();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddAuthorizationCore();
builder.Services.AddCascadingAuthenticationState();
builder.Services.AddScoped<JwtMessageHandler>();

builder.Services.AddHttpClient("BackendApi", client => { client.BaseAddress = new Uri("http+https://api"); })
    .AddHttpMessageHandler<JwtMessageHandler>()
    .AddServiceDiscovery();

builder.Services.AddScoped(sp => sp
    .GetRequiredService<IHttpClientFactory>()
    .CreateClient("BackendApi")
);

builder.Services.AddSingleton<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddSingleton<ApiService>();

await builder.Build().RunAsync();