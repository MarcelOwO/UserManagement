using System.Net.Http.Json;
using System.Text.RegularExpressions;
using System.Web;
using UserManagement.Core;
using UserManagement.Core.Dto.Group;
using UserManagement.Core.Dto.User;
using UserManagement.Web.Models;

namespace UserManagement.Web.Services;

public class ApiService(HttpClient httpClient)
{
    private const string GroupApiUrl = "api/groups";
    private const string UserApiUrl = "api/users";

    public async Task<PaginatedResult<UserDto>> GetUsers(QueryParameters parameter)
    {
        var query = new List<string>
        {
            $"page={parameter.Page}",
            $"pageSize={parameter.PageSize}"
        };

        if (!string.IsNullOrWhiteSpace(parameter.Search))
        {
            query.Add($"search={HttpUtility.UrlEncode(parameter.Search)}");
        }

        var queryString = string.Join("&", query);
        var requestUrl = $"{UserApiUrl}?{queryString}";

        var response = await httpClient.GetAsync(requestUrl);

        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<PaginatedResult<UserDto>>();

        return result ?? throw new Exception("Failed to deserialize users.");
    }

    public async Task GetUser()
    {
    }

    public async Task CreateUser()
    {
    }

    public async Task UpdateUser()
    {
    }

    public async Task DeleteUser()
    {
    }

    public async Task GetGroups()
    {
    }

    public async Task GetGroup()
    {
    }

    public async Task CreateGroup()
    {
    }

    public async Task UpdateGroup()
    {
    }

    public async Task DeleteGroup()
    {
    }

    public async Task AssignGroup()
    {
    }

    public async Task RemoveGroupAssignment()
    {
    }
}