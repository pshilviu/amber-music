﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amber.Music.Domain.Services
{
    public interface IArtistService
    {
        Task<SearchResult<ArtistSearch>> FindArtistsAsync(string query, int? limit = null, int? offset = null);

        Task<ArtistSearch> GetArtistAsync(Guid id);

        Task<IReadOnlyCollection<ArtistWork>> GetArtistWorksAsync(Guid id);

        Task<IReadOnlyCollection<ArtistRelease>> GetArtistReleaseAsync(Guid id);
    }
}
