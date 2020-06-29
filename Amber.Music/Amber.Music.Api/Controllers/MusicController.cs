using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Amber.Music.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MusicController : ControllerBase
    {
        private readonly IArtistService _artistService;
        private readonly IAggregatorProcess _aggregatorProcess;

        // Used a cache mechanism, but it could be replaced with a db, redis, elastic search, etc.
        private readonly ICacheService _cacheService;

        public MusicController(
            IArtistService artistService,
            IAggregatorProcess aggregatorProcess,
            ICacheService cacheService)
        {
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
            _aggregatorProcess = aggregatorProcess ?? throw new ArgumentNullException(nameof(aggregatorProcess));
            _cacheService = cacheService ?? throw new ArgumentNullException(nameof(cacheService));
        }

        [HttpGet]
        [Route("")]
        public string Index()
        {
            return "Alive";
        }

        [HttpGet]
        [Route("find/{query}")]
        [Route("find/{query}/{limit:int}/{offset:int}")]
        public Task<SearchResult<ArtistSearch>> FindArtists(string query, int? limit = null, int? offset = null)
        {
            return _artistService.FindArtistsAsync($"\"{query}\"", limit, offset);
        }

        [HttpGet]
        [Route("lastsearches")]
        public ArtistSearch[] LastSearches()
        {
            return _cacheService.Reports
                .OrderByDescending(x => x.Value.LastAccessed)
                .Take(5)
                .Select(x => new ArtistSearch { Id = x.Value.ArtistId, Name = x.Value.Name })
                .ToArray();
        }

        [HttpGet]
        [Route("report/{id}")]
        public async Task<ArtistWorkReport> GetArtistWorkReport(Guid id)
        {
            if (!_cacheService.Reports.ContainsKey(id))
            {
                var report = await _aggregatorProcess.AggregateDataAsync(id);
                _cacheService.AddReport(id, report);
            }

            _cacheService.Reports[id].LastAccessed = DateTime.Now;
            return _cacheService.Reports[id];
        }

        [HttpGet]
        [Route("releases/{id}")]
        public async Task<ArtistReleaseReport> GetArtistReleasesReport(Guid id)
        {
            if (!_cacheService.Releases.ContainsKey(id))
            {
                var report = await _artistService.GetArtistReleaseAsync(id);
                _cacheService.AddReleases(id, report);
            }

            return _aggregatorProcess.CompileReleasesPerYearReport(id, _cacheService.Releases[id]);
        }
    }
}
