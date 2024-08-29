namespace MySaleForceBackup.Interfaces
{
    public interface IAppSettings
    {
        string Get(string key);
        void Set(string key, string value);
    }
}
