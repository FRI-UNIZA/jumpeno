using JumpenoWebassembly.Shared.ErrorHandling;
using JumpenoWebassembly.Shared.Models;
using System;
using System.Collections.Generic;
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

        public async Task SubmitError(Error errorContent)
        {
            await _httpClient.PutAsJsonAsync("api/error/submitError", errorContent);
        }

        public async Task<List<Error>> ReceiveErrorLog()
        { 
            var result = await _httpClient.GetAsync("api/error/receiveErrorLog");
            if (result == null) return null;
            return await result.Content.ReadFromJsonAsync<List<Error>>();
        }
    }
}
