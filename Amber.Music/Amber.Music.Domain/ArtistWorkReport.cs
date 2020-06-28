using System;

namespace Amber.Music.Domain
{
    public class ArtistWorkReport
    {
        public Guid ArtistId { get; set; }

        public string Name { get; set; }

        public int TotalSongs { get; set; } = 0;

        public int SongsConsidered { get; set; } = 0;

        public int AverageWords { get; set; } = 0;

        public double Variance { get; set; } = 0;

        public double StandardDeviation { get; set; } = 0;

        public ArtistWork MinWords { get; set; }

        public ArtistWork MaxWords { get; set; }
    }
}
