using System.Collections.Generic;
using System.Threading.Tasks;

namespace SalesForceBackup.Interfaces
{
    public interface IDownloader
    {
        Task<List<string>> Download();
    }
}
