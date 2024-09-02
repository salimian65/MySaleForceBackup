using Microsoft.WindowsAzure.Storage.Auth;
using Microsoft.WindowsAzure.Storage.Blob;
using SalesForceBackup.Interfaces;
using System;
using System.IO;

namespace SalesForceBackup
{
    /// <summary>
    /// Uploads backup files to Azure Blob storage.
    /// </summary>
    public class AzureBlobUploader : IUploader
    {

        private readonly IAppSettings _appSettings;
        private readonly IErrorHandler _errorHandler;

        public AzureBlobUploader(IAppSettings appSettings, IErrorHandler errorHandler)
        {
            //_appSettings = TinyIoCContainer.Current.Resolve<IAppSettings>();
            //_errorHandler = TinyIoCContainer.Current.Resolve<IErrorHandler>();
            _appSettings = appSettings;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// Uploads a file to Azure Blob.
        /// </summary>
        /// <param name="filePath">The full filename and path of the file to upload.</param>
        public void Upload(string filePath)
        {
            var blobEndpoint = new Uri(_appSettings.Get(AppSettingKeys.AzureBlobEndpoint));
            var accountName = _appSettings.Get(AppSettingKeys.AzureAccountName);
            var accountKey = _appSettings.Get(AppSettingKeys.AzureSharedKey);
            var containerName = _appSettings.Get(AppSettingKeys.AzureContainer);
            ///TODO use AddressProvider
            var blobName = String.Join("/", new[] { _appSettings.Get(AppSettingKeys.AzureFolder), Path.GetFileName(filePath) });

            try
            {
                var blobClient = new CloudBlobClient(blobEndpoint, new StorageCredentials(accountName, accountKey));
                var container = blobClient.GetContainerReference(containerName);
                container.CreateIfNotExists();
                var blob = container.GetBlockBlobReference(blobName);
                Console.WriteLine("Uploading {0} to Azure...", Path.GetFileName(filePath));
                blob.UploadFromFile(filePath, FileMode.OpenOrCreate);
            }
            catch (Exception e)
            {
                //TODO depend on business 
                _errorHandler.HandleError(e, (int)ExitCode.AzureError, "There was an error uploading the file to Azure.");
                // throw;
            }
        }
    }
}
