using System;

namespace Amber.Music.Domain
{
    public class ArtistWork
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public string Lyrics { get; set; }

        public int WordCount { get; set; } = 0;
    }
}
