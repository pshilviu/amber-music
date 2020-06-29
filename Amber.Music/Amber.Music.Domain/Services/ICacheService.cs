using System;
using System.Collections.Generic;

namespace Amber.Music.Domain.Services
{
    public interface ICacheService
    {
        /// <summary>
        /// ArtistId, Report
        /// </summary>
        Dictionary<Guid, ArtistWorkReport> Reports { get; }

        /// <summary>
        /// ArtistId, Release
        /// </summary>
        Dictionary<Guid, List<ArtistRelease>> Releases { get; }

        /// <summary>
        /// ArtistId, (WorkId, ArtistWork/Song)
        /// </summary>
        Dictionary<Guid, Dictionary<Guid, ArtistWork>> Works { get; }

        void AddReport(Guid id, ArtistWorkReport report);

        void InitializeArtistWorks(Guid id);

        void AddArtistWork(Guid artistId, ArtistWork work);

        void AddReleases(Guid artistId, IEnumerable<ArtistRelease> release);
    }
}
