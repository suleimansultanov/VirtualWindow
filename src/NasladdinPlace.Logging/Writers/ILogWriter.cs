using System.Threading.Tasks;
using NasladdinPlace.Logging.Models;

namespace NasladdinPlace.Logging.Writers
{
    public interface ILogWriter
    {
        Task Write(Log log);
    }
}
