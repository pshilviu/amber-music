using System.Threading.Tasks;

namespace Amber.Music.Domain.Services
{
    public interface ILyricsService
    {
        Task<string> SearchAsync(string artist, string title);
    }
}
