using System.Text.Json;
using GodLeaks.Application.Interfaces;
using GodLeaks.Domain.Entities.LeakSightAPI;
using Microsoft.Extensions.Configuration;

namespace GodLeaks.Infrastructure.LeakSightAPI;

public class LeakSight : ILeakSight
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        Converters = { new StringOrNumberConverter() }
    };

    private readonly HttpClient _httpClient;
    private readonly string _token;

    public LeakSight(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _token = configuration["APIKEY"] ?? string.Empty;
        if (string.IsNullOrWhiteSpace(_token))
        {
            throw new InvalidOperationException("LeakSight API key is not configured.");
        }
    }

    public Task<Username.Root> SearchUsername(string username) =>
        SearchAsync<Username.Root>("username", username);

    public Task<Email.Root> SearchEmail(string email) =>
        SearchAsync<Email.Root>("email", email);

    public Task<CPF.Root> SearchCPF(string cpf) =>
        SearchAsync<CPF.Root>("cpf", cpf);

    public Task<Placa.Root> SearchPlaca(string placa) =>
        SearchAsync<Placa.Root>("placa", placa);

    public Task<Nome.Root> SearchNome(string nome) =>
        SearchAsync<Nome.Root>("nome", nome);

    public Task<Password.Root> SearchPassword(string password) =>
        SearchAsync<Password.Root>("password", password);

    public Task<Url.Root> SearchUrl(string url) =>
        SearchAsync<Url.Root>("url", url);

    public Task<Telefone.Root> SearchPhone(string phone) =>
        SearchAsync<Telefone.Root>("number", phone);

    private async Task<T> SearchAsync<T>(string endpoint, string query)
    {
        var url = $"osint/{endpoint}?token={Uri.EscapeDataString(_token)}&text={Uri.EscapeDataString(query)}";
        var response = await _httpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        var json = await response.Content.ReadAsStringAsync();
        return JsonSerializer.Deserialize<T>(json, JsonOptions)
               ?? throw new JsonException("LeakSight returned an empty response.");
    }
}