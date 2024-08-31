namespace SalesForceBackup.Interfaces
{
    public interface IAppSettings
    {
        int age { get; set; }
        string Get(string key);
        void Set(string key, string value);
    }
}
