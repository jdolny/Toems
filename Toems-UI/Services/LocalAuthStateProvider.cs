using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;
using System.Text.Json.Serialization;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;


public class LocalAuthStateProvider : AuthenticationStateProvider
{
    //private readonly HttpClient _httpClient;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILocalStorageService _protectedSessionStorage;
    private readonly string _tokenEndpoint = "/token";
    private readonly AuthenticationState _anonymous = new(new ClaimsPrincipal(new ClaimsIdentity()));

    public LocalAuthStateProvider(IHttpClientFactory httpClientFactory,ILocalStorageService protectedSessionStorage)
    {
        //_httpClient = httpClient;

        _httpClientFactory = httpClientFactory;
        _protectedSessionStorage = protectedSessionStorage;
    }


    
    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var authStateResult = await _protectedSessionStorage.GetItemAsync<AuthState>("authState");
            var claims = authStateResult.Claims.Select(c => new Claim(c.Type, c.Value));
            
            if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= long.Parse(claims.FirstOrDefault(c => c.Type == "Expires")?.Value))
            {
                await LogoutAsync();
                return _anonymous;
            }
            
            var identity = new ClaimsIdentity(claims, "LocalAuth");
            var user = new ClaimsPrincipal(identity);
            return new AuthenticationState(user);

        }
        catch (Exception ex)
        {

        }

        return _anonymous;
    }

    public async Task<long> GetTokenExpiration()
    {
        try
        {
            var authStateResult = await _protectedSessionStorage.GetItemAsync<AuthState>("authState");
            var claims = authStateResult.Claims.Select(c => new Claim(c.Type, c.Value));
            return long.Parse(claims.FirstOrDefault(c => c.Type == "Expires")?.Value);
        }
        catch (Exception ex)
        {
            return 0;
        }
    }

    public async Task<string> GetToken()
    {
        var authStateResult = await _protectedSessionStorage.GetItemAsync<AuthState>("authState");
        var claims = authStateResult.Claims.Select(c => new Claim(c.Type, c.Value));
        var token = claims.Where(x => x.Type.Equals("AccessToken", StringComparison.InvariantCultureIgnoreCase)).Select(x => x.Value).FirstOrDefault();
        return token;
    }

    public async Task<LoginResult> LoginAsync(string username, string password)
    {
        var client = _httpClientFactory.CreateClient("API");

        List<KeyValuePair<string, string>> keyValues = new List<KeyValuePair<string, string>>();
        keyValues.Add(new KeyValuePair<string, string>("username", username));
        keyValues.Add(new KeyValuePair<string, string>("password", password));
        keyValues.Add(new KeyValuePair<string, string>("grant_type", "password"));

        HttpContent content = new FormUrlEncodedContent(keyValues);


        try
        {
            var response = await client.PostAsync(_tokenEndpoint, content);
            if (response.IsSuccessStatusCode)
            {
                var tokenResponse = await response.Content.ReadFromJsonAsync<DtoToken>();
              
                if (!string.IsNullOrEmpty(tokenResponse?.access_token))
                {
                    var claims = new[]
                    {
                        new Claim(ClaimTypes.NameIdentifier, username),
                        new Claim(ClaimTypes.Name, username),
                        //new Claim("Membership", tokenResponse.membership),
                        //new Claim("Theme", tokenResponse.theme),
                        new Claim("AccessToken", tokenResponse.access_token),
                        new Claim("Expires", (DateTimeOffset.UtcNow.ToUnixTimeSeconds() + tokenResponse.expires_in).ToString()),

                    };

                    var authState = new AuthState
                    {
                        Claims = claims.Select(c => new ClaimData { Type = c.Type, Value = c.Value }).ToList()
                    };
                    
                    await _protectedSessionStorage.SetItemAsync("authState", authState);
                    var identity = new ClaimsIdentity(claims, "LocalAuth");
                    var user = new ClaimsPrincipal(identity);



                    NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(user)));
                    return new LoginResult { Success = true };
                }
                return new LoginResult { Success = false, ErrorMessage = "No access token received" };
            }
            return new LoginResult { Success = false, ErrorMessage = $"Login failed: {response.ReasonPhrase}" };
        }
        catch (Exception ex)
        {
            return new LoginResult { Success = false, ErrorMessage = $"Login error: {ex.Message}" };
        }
    



    }

    public async Task LogoutAsync()
    {
        await _protectedSessionStorage.RemoveItemAsync("authState");
        NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        
    }






}

public class DtoToken
{
    public string access_token { get; set; }
    public string error_description { get; set; }
    public int expires_in { get; set; }
    public string token_type { get; set; }

    public string username { get; set; }
    public string membership { get; set; }
    public string theme { get; set; }

    public string is_validated { get; set; }
}
public class LoginResult
{
    public bool Success { get; set; }
    public string ErrorMessage { get; set; }
}
public class AuthState
{
    public List<ClaimData> Claims { get; set; }
}
public class ClaimData
{
    public string Type { get; set; }
    public string Value { get; set; }
}

[JsonSourceGenerationOptions(WriteIndented = false)]
[JsonSerializable(typeof(Dictionary<string, object>))]
[JsonSerializable(typeof(AuthState))]
[JsonSerializable(typeof(List<ClaimData>))]
public partial class JwtPayloadContext : JsonSerializerContext { }