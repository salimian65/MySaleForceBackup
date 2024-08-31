using System.Collections.Generic;

namespace SalesForceBackup.Interfaces
{
    public interface IAppSettings
    {
        int age { get; set; }
        string Get(string key);
        void Set(string key, string value);
        void AssignValues(IList<string> args);
    }
}
