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

        public Dictionary<Guid, ArtistWorkReport> Reports => _reports;

        public Dictionary<Guid, Dictionary<Guid, ArtistWork>> Works => _works;

        private object worksLock = new object();

        private object collectionLock = new object();

        private object reportsLock = new object();

        public void AddReport(Guid id, ArtistWorkReport report)
        {
            lock (reportsLock)
            {
                if (!_reports.ContainsKey(id))
                {
                    _reports.Add(id, report);
                }
            }
        }

        public void InitializeArtistWorks(Guid id)
        {
            lock (worksLock)
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

            lock (collectionLock)
            {
                if (!_works.ContainsKey(work.Id))
                {
                    artistWorks.Add(work.Id, work);
                }
            }
        }
    }
}
