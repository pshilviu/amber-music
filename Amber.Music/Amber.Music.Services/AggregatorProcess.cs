using Amber.Music.Domain;
using Amber.Music.Domain.Services;
using Dasync.Collections;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Amber.Music.Services
{
    public class AggregatorProcess
    {
        private readonly ILyricsService _lyricsService;
        private readonly IArtistService _artistService;
        private readonly IWordCounterService _wordCounterService;

        public AggregatorProcess(
            ILyricsService lyricsService,
            IArtistService artistService,
            IWordCounterService wordCounterService)
        {
            _lyricsService = lyricsService ?? throw new ArgumentNullException(nameof(lyricsService));
            _artistService = artistService ?? throw new ArgumentNullException(nameof(artistService));
            _wordCounterService = wordCounterService ?? throw new ArgumentNullException(nameof(wordCounterService));
        }

        public async Task<ArtistWorkReport> AggregateData(Guid artistId)
        {
            // Get artist albums/works
            var artist = await _artistService.GetArtistAsync(artistId);

            artist.Works = await _artistService.GetArtistWorksAsync(artistId);

            int sum = 0, validValuesCount = 0;
            ArtistWork minWorkInfo = null, maxWorkInfo = null;
            var syncLock = new object();

            await artist.Works.ParallelForEachAsync(async (work) =>
            {
                // retrieve lyrics
                work.Lyrics = await _lyricsService.SearchAsync(artist.Name, work.Title);

                // calculate number of words
                work.WordCount = _wordCounterService.Count(work.Lyrics);

                // TODO: persist to DB

                // skip from the report songs that we couldn't find lyrics for
                if (work.WordCount == 0)
                {
                    return;
                }

                lock (syncLock)
                {
                    // keep the sum and number of values to calculate the average
                    sum += work.WordCount;
                    validValuesCount++;

                    // keep info about the song with shortest number of words
                    if (minWorkInfo == null || work.WordCount < minWorkInfo.WordCount)
                    {
                        minWorkInfo = work;
                    }

                    // keep info about the song with maximum number of words
                    if (maxWorkInfo == null || work.WordCount > maxWorkInfo.WordCount)
                    {
                        maxWorkInfo = work;
                    }
                }
            });

            var report = new ArtistWorkReport
            {
                ArtistId = artistId,
                Name = artist.Name,
                AverageWords = sum / validValuesCount,
                MinWords = minWorkInfo,
                MaxWords = maxWorkInfo
            };

            // using https://www.mathsisfun.com/data/standard-deviation-formulas.html
            var stdDevSum = artist.Works
                .Where(x => x.WordCount > 0)
                .Sum(x => Math.Pow(x.WordCount - report.AverageWords, 2));

            report.Variance = stdDevSum / validValuesCount;

            report.StandardDeviation = Math.Sqrt(report.Variance);

            return report;
        }
    }
}
