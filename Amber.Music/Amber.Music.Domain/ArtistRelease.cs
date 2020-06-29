using System;

namespace Amber.Music.Domain
{
    public class ArtistRelease
    {
        public Guid Id { get; set; }

        public string Title { get; set; }

        public DateTime? Date { get; set; }
    }
}
