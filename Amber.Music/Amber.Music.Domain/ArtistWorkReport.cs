using System;

namespace Amber.Music.Domain
{
    public class ArtistWorkReport
    {
        public Guid ArtistId { get; set; }

        public string Name { get; set; }

        public int AverageWords { get; set; }

        public double Variance { get; set; }

        public double StandardDeviation { get; set; }

        public ArtistWork MinWords { get; set; }

        public ArtistWork MaxWords { get; set; }
    }
}
