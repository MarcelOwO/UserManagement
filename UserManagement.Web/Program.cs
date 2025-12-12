using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using MudBlazor.Services;
using UserManagement.Web;
using UserManagement.Web.Servicse;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient
{
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress),
});

builder.Services.AddMudServices();

builder.Services.AddSingleton<LoginService>();
builder.Services.AddSingleton<ApiService>();

var app = builder.Build();

await app.RunAsync();
