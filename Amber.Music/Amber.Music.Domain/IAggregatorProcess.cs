using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Amber.Music.Domain
{
    public interface IAggregatorProcess
    {
        Task<ArtistWorkReport> AggregateDataAsync(Guid artistId);

        ArtistReleaseReport CompileReleasesPerYearReport(Guid artistId, IEnumerable<ArtistRelease> releases);
    }
}
