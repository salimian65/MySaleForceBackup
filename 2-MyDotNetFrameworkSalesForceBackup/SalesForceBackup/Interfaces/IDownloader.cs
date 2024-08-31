using System.Threading.Tasks;

namespace SalesForceBackup.Interfaces
{
    public interface IDownloader
    {
      Task<string[]> Download();
    }
}
