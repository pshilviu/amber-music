using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Amber.Music.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Amber.Music.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;

        private static SearchResult<ArtistSearch> _searchResults;

        private readonly IArtistService _artistService;
        private readonly ILyricsService _lyricsService;
        private readonly AggregatorProcess _aggregatorProcess;

        public WeatherForecastController(
            ILogger<WeatherForecastController> logger,
            IArtistService artistService,
            ILyricsService lyricsService,
            AggregatorProcess aggregatorProcess)
        {
            _logger = logger;
            _searchResults = new SearchResult<ArtistSearch>();

            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
            _lyricsService = lyricsService ?? throw new ArgumentNullException(nameof(lyricsService));
            _aggregatorProcess = aggregatorProcess ?? throw new ArgumentNullException(nameof(aggregatorProcess));
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            //return RedirectToAction(nameof(GetLyrics), new { artist = "Jeff Buckley", title = "Demon John" });
            return RedirectToAction(nameof(GetArtistWorkReport), new { id = new Guid("e6e879c0-3d56-4f12-b3c5-3ce459661a8e") }); //cc197bad-dc9c-440d-a5b5-d52ba2e14234
        }

        // Entry point: Search by name (and pagination)...
        // 1 entry => do artist search
        // multiple entries => show results (table + pagination) => on click, do above
        //artist/{query}/find/{limit}/{offset}
        [HttpGet]
        [Route("find")]
        public async Task<SearchResult<ArtistSearch>> FindArtists(string query, int? limit = null, int? offset = null)
        {
            _searchResults = await _artistService.FindArtistsAsync(query, limit, offset);

            return _searchResults;
        }

        // Get artist info
        // Get works
        // Get lyrics for each song

        [HttpGet]
        [Route("works")]
        public Task<IEnumerable<ArtistWork>> GetArtist(Guid id)
        {
            return _artistService.GetArtistWorksAsync(id);
        }

        [HttpGet]
        [Route("lyrics")]
        public Task<string> GetLyrics(string artist, string title)
        {
            return _lyricsService.SearchAsync(artist, title);
        }

        [HttpGet]
        [Route("report")]
        public Task<ArtistWorkReport> GetArtistWorkReport(Guid id)
        {
            return _aggregatorProcess.AggregateData(id);
        }
    }
}
