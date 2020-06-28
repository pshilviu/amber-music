using System.Collections.Generic;

namespace Amber.Music.Domain
{
    public class SearchResult<T>
        where T : class
    {
        public int TotalResults { get; set; }

        public int Limit { get; set; }

        public int Offset { get; set; }

        public IReadOnlyList<T> Results { get; set; }
    }
}
