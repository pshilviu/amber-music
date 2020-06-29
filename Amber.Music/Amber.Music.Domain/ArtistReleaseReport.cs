using System;
using System.Collections.Generic;

namespace Amber.Music.Domain
{
    public class ArtistReleaseReport
    {
        public class ReleaseRow
        {
            public int Year { get; set; }

            public int Releases { get; set; }
        }

        public Guid ArtistId { get; set; }

        public IReadOnlyCollection<ReleaseRow> YearlyReleases { get; set; } = new List<ReleaseRow>();
    }
}
