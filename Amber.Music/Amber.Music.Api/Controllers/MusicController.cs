using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Amber.Music.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Amber.Music.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : ControllerBase
    {
        private readonly ILogger<MusicController> _logger;

        private static SearchResult<ArtistSearch> _searchResults;

        private readonly IArtistService _artistService;
        private readonly ILyricsService _lyricsService;
        private readonly AggregatorProcess _aggregatorProcess;

        public MusicController(
            ILogger<MusicController> logger,
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

        [HttpGet]
        [Route("report")]
        public Task<ArtistWorkReport> GetArtistWorkReport(Guid id)
        {
            return _aggregatorProcess.AggregateData(id);
        }
    }
}
