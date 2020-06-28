using System;
using System.Threading.Tasks;

namespace Amber.Music.Domain
{
    public interface IAggregatorProcess
    {
        Task<ArtistWorkReport> AggregateDataAsync(Guid artistId);
    }
}
