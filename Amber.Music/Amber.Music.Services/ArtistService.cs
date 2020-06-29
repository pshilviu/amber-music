using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Amber.Music.Services;
using MetaBrainz.MusicBrainz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Amber.Music.Api.Services
{
    public class MusicBrainzService : IArtistService
    {
        public MusicBrainzService(ApplicationInfoOptions options)
        {
            options = options ?? throw new ArgumentNullException(nameof(options));

            Query.DefaultUserAgent = $"{options.Name}/{options.Version} ( {options.ContactEmail} )";
        }

        public async Task<SearchResult<ArtistSearch>> FindArtistsAsync(string query, int? limit = 25, int? offset = null)
        {
            if (string.IsNullOrWhiteSpace(query))
            {
                throw new ArgumentNullException(nameof(query), "Artist name must be provided");
            }

            using var mbQuery = new Query();

            var searchResults = await mbQuery.FindArtistsAsync(query, limit, offset);

            return new SearchResult<ArtistSearch>
            {
                Results = searchResults.Results.Select(x => new ArtistSearch
                {
                    Id = x.Item.Id,
                    Name = x.Item.Name,
                    SortName = x.Item.SortName,
                    Type = x.Item.Type,
                    Area = x.Item.Area?.ToString()
                }).ToArray(),
                TotalResults = searchResults.TotalResults,
                Limit = searchResults.Limit ?? limit ?? 25,
                Offset = searchResults.Offset
            };
        }

        public async Task<ArtistSearch> GetArtistAsync(Guid id)
        {
            using var mbQuery = new Query();

            var artist = await mbQuery.LookupArtistAsync(id); // , Include.Works | Include.Releases | Include.ReleaseRelationships

            return new ArtistSearch
            {
                Id = artist.Id,
                Name = artist.Name,
                // Works = artist.Works.Select(x => new ArtistWork { Id = x.Id, Title = x.Title })
            };
        }

        public async Task<IReadOnlyCollection<ArtistWork>> GetArtistWorksAsync(Guid id)
        {
            using var mbQuery = new Query();

            var works = new List<ArtistWork>();

            var current = await mbQuery.BrowseArtistWorksAsync(id, 100);
            while (current != null && current.Results.Count > 0)
            {
                works.AddRange(current.Results.Select(x => new ArtistWork { Id = x.Id, Title = x.Title }));

                current = await current.NextAsync();
            }

            return works;
        }

        public async Task<IReadOnlyCollection<ArtistRelease>> GetArtistReleaseAsync(Guid id)
        {
            using var mbQuery = new Query();

            var releases = new List<ArtistRelease>();

            var current = await mbQuery.BrowseArtistReleasesAsync(id, 100);
            while (current != null && current.Results.Count > 0)
            {
                releases.AddRange(current.Results.Select(x => new ArtistRelease { Id = x.Id, Title = x.Title, Date = x.Date?.NearestDate }));

                current = await current.NextAsync();
            }

            return releases;
        }
    }
}
