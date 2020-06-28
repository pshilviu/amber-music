using System;
using System.Collections.Generic;

namespace Amber.Music.Domain
{
    public class ArtistSearch
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        public string SortName { get; set; }

        public string Type { get; set; }

        public string Area { get; set; }

        public IReadOnlyCollection<ArtistWork> Works { get; set; } = new List<ArtistWork>();
    }
}
