using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using System;
using System.Collections.Generic;

namespace Amber.Music.Services
{
    public class CacheService : ICacheService
    {
        private static readonly Dictionary<Guid, Dictionary<Guid, ArtistWork>> _works = new Dictionary<Guid, Dictionary<Guid, ArtistWork>>();

        private static readonly Dictionary<Guid, ArtistWorkReport> _reports = new Dictionary<Guid, ArtistWorkReport>();

        private static readonly Dictionary<Guid, List<ArtistRelease>> _releases = new Dictionary<Guid, List<ArtistRelease>>();

        public Dictionary<Guid, ArtistWorkReport> Reports => _reports;

        public Dictionary<Guid, Dictionary<Guid, ArtistWork>> Works => _works;

        public Dictionary<Guid, List<ArtistRelease>> Releases => _releases;

        private readonly object _worksLock = new object();

        private readonly object _collectionLock = new object();

        private readonly object _reportsLock = new object();

        private readonly object _releasesLock = new object();

        public void AddReport(Guid id, ArtistWorkReport report)
        {
            lock (_reportsLock)
            {
                if (!_reports.ContainsKey(id))
                {
                    _reports.Add(id, report);
                }
            }
        }

        public void InitializeArtistWorks(Guid id)
        {
            lock (_worksLock)
            {
                if (!_works.ContainsKey(id))
                {
                    _works.Add(id, new Dictionary<Guid, ArtistWork>());
                }
            }
        }

        public void AddArtistWork(Guid artistId, ArtistWork work)
        {
            var artistWorks = _works[artistId];

            lock (_collectionLock)
            {
                if (!_works.ContainsKey(work.Id))
                {
                    artistWorks.Add(work.Id, work);
                }
            }
        }

        public void InitializeArtistReleases(Guid artistId)
        {
            lock (_releasesLock)
            {
                if (!_releases.ContainsKey(artistId))
                {
                    _releases.Add(artistId, new List<ArtistRelease>());
                }
            }
        }

        public void AddReleases(Guid artistId, IEnumerable<ArtistRelease> release)
        {
            InitializeArtistReleases(artistId);
            _releases[artistId].AddRange(release);
        }
    }
}
