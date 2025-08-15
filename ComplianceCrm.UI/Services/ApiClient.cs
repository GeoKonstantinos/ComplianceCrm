using static System.Net.WebRequestMethods;

namespace ComplianceCrm.UI.Services;

public sealed class ApiClient
{
    private readonly IHttpClientFactory _factory;
    public ApiClient(IHttpClientFactory factory) => _factory = factory;

    private HttpClient Client => _factory.CreateClient("ComplianceApi");

    // GET helper
    public async Task<T?> GetAsync<T>(string url, CancellationToken ct = default)
        => await Client.GetFromJsonAsync<T>(url, ct);

    // POST helper
    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest body, CancellationToken ct = default)
    {
        var resp = await Client.PostAsJsonAsync(url, body, ct);
        resp.EnsureSuccessStatusCode();
        return await resp.Content.ReadFromJsonAsync<TResponse>(cancellationToken: ct);
    }
    public async Task PutAsync<TRequest>(string url, TRequest data)
    {
        var res = await Client.PutAsJsonAsync(url, data);
        res.EnsureSuccessStatusCode();
    }

    public async Task DeleteAsync(string url)
    {
        var res = await Client.DeleteAsync(url);
        res.EnsureSuccessStatusCode();
    }
}