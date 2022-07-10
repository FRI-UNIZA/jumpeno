using JumpenoWebassembly.Shared.Models;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace JumpenoWebassembly.Client.Services
{
    /// <summary>
    /// Servis pre aktualizovanie udajov hraca
    /// </summary>
    public class ErrorService
    {
        private readonly HttpClient _httpClient;

        public ErrorService(HttpClient http)
        {
            _httpClient = http;
        }

        public async Task SubmitError(string errorContent)
        {
            await _httpClient.PutAsJsonAsync("api/error/submit", errorContent);
        }

        public async Task<string> ReceiveErrorLog()
        {
            var result = await _httpClient.GetAsync("api/error/receive");
            if (result == null) return null;
            return result.Content.ToString();
        }
    }
}
