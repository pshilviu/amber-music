using Amber.Music.Domain.Services;
using Amber.Music.Services;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Amber.Music.Api.Services
{
    public class LyricsService : ILyricsService
    {
        private readonly HttpClient _httpClient;
        private readonly LyricsApiOptions _lyricsApiOptions;

        private class LyricsResponse
        {
            [JsonPropertyName("lyrics")]
            public string Lyrics { get; set; }
        }

        public LyricsService(
            HttpClient httpClient,
            LyricsApiOptions lyricsApiOptions)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _lyricsApiOptions = lyricsApiOptions ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<string> SearchAsync(string artist, string title)
        {
            if (string.IsNullOrEmpty(artist))
            {
                throw new ArgumentNullException(nameof(artist));
            }

            if (string.IsNullOrEmpty(title))
            {
                throw new ArgumentNullException(nameof(title));
            }

            var url = $"{_lyricsApiOptions.Endpoint}/{artist}/{title}";
            var response = await _httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null;
                }

                throw new InvalidOperationException($"Could not retrieve lyrics ({response.StatusCode}) : {response.ReasonPhrase}");
            }

            var responseBody = await response.Content.ReadAsStringAsync();

            var result = JsonSerializer.Deserialize<LyricsResponse>(responseBody);

            return result.Lyrics;
        }
    }
}
