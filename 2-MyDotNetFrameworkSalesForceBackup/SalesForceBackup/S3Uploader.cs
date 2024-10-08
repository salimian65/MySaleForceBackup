﻿using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using SalesForceBackup.Interfaces;
using System;
using System.IO;

namespace SalesForceBackup
{
    /// <summary>
    /// Uploads files to AWS S3.
    /// </summary>
    public class S3Uploader : IUploader
    {
        private readonly IAppSettings _appSettings;
        private readonly IErrorHandler _errorHandler;
        public S3Uploader(IAppSettings appSettings, IErrorHandler errorHandler)
        {
            //_appSettings = TinyIoCContainer.Current.Resolve<IAppSettings>();
            //_errorHandler = TinyIoCContainer.Current.Resolve<IErrorHandler>();
            _appSettings = appSettings;
            _errorHandler = errorHandler;
        }

        /// <summary>
        /// Uploads a file to S3.
        /// </summary>
        /// <param name="filePath">The full filename and path of the file to upload.</param>
        public void Upload(string filePath)
        {
            var filename = Path.GetFileName(filePath);
            try
            {
                var credentials = new BasicAWSCredentials(_appSettings.Get(AppSettingKeys.AwsAccessKey), _appSettings.Get(AppSettingKeys.AwsSecretKey));
                var region = RegionEndpoint.GetBySystemName(_appSettings.Get(AppSettingKeys.AwsRegion));
                using (var client = new AmazonS3Client(credentials, region))
                {
                    var request = new PutObjectRequest
                    {
                        BucketName = _appSettings.Get(AppSettingKeys.S3Bucket),
                        Key = String.Join("/", new[] { _appSettings.Get(AppSettingKeys.S3Folder), filename }),
                        FilePath = filePath
                    };
                    Console.WriteLine("Uploading {0} to AWS S3...", filename);
                    client.PutObject(request);
                }
            }
            catch (AmazonS3Exception e) when (e.ErrorCode != null && (e.ErrorCode.Equals("InvalidAccessKeyId") || e.ErrorCode.Equals("InvalidSecurity")))
            {

                _errorHandler.HandleError(e, (int)ExitCode.AwsCredentials, "Check the provided AWS Credentials");

            }
            catch (AmazonS3Exception e)
            {

                _errorHandler.HandleError(e, (int)ExitCode.AwsS3Error, String.Format("Error occurred. Message:'{0}' when writing an object", e.Message));

            }
        }
    }
}
