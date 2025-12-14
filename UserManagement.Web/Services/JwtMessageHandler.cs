using System.Net.Http.Headers;
using Blazored.LocalStorage;

namespace UserManagement.Web.Services;

public class JwtMessageHandler(ILocalStorageService localStorage):DelegatingHandler
{
   private const string JwtTokenKey = "jwtToken";

   private HashSet<string> _blackListedUrls =
   [
      "/login", "/logout"
   ];

   protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
   {
      var uri = request.RequestUri!.AbsolutePath;
       
     if (_blackListedUrls.Any(url => uri.Contains(url, StringComparison.OrdinalIgnoreCase))) 
     {
         return await base.SendAsync(request, cancellationToken);
     }

     var token = await localStorage.GetItemAsStringAsync(JwtTokenKey, cancellationToken);

     if (!string.IsNullOrWhiteSpace(token))
     {
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
     }
     return await base.SendAsync(request, cancellationToken);
   } 
}