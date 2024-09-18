namespace MySaleForceBackup
{
    public enum ExitCode
    {
        Normal = 0,
        Unknown = -1,
        ConfigurationError = 10,
        AwsCredentials = 20,
        AwsS3Error = 21,
        AzureError = 30
    }
}
