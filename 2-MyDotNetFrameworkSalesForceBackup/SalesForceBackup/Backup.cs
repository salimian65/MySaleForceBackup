using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using SalesForceBackup.Interfaces;

namespace SalesForceBackup
{
    public class Backup : IDisposable
    {
        private readonly IUploader _uploader;
        private readonly IDownloader _downloader;
        private readonly IErrorHandler _errorHandler;
        private readonly IAppSettings _appSettings;
        private readonly List<string> _filesToDelete = new List<string>();

        public Backup(IUploader uploader, IDownloader downloader, IErrorHandler errorHandler, IAppSettings appSettings)
        {
            _uploader = uploader;
            _downloader = downloader;
            _errorHandler = errorHandler;
            _appSettings = appSettings;
        }

        public async Task Run(IList<string> args)
        {
            _appSettings.AssignValues(args);
            // try
            // {
            var files = await _downloader.Download();
            _filesToDelete.AddRange(files);

            foreach (var file in files.Select(RenameFile))
            {
                _filesToDelete.Add(file);
                _uploader.Upload(file);
            }
            //  }
            //catch (Exception e)
            //{
            //    _errorHandler.HandleError(e);
            ////}
            //finally
            //{

            //}
        }


        private string RenameFile(string file)
        {
            var i = -1;
            string newFile;

            do
            {
                newFile = String.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture),
                    new[] { Path.GetDirectoryName(file), FormatFileName(Path.GetExtension(file), ++i) });
            } while (File.Exists(newFile));

            File.Move(file, newFile);

            return newFile;
        }

        private string FormatFileName(string extension, int i)
        {
            var revision = i <= 0 ? String.Empty : String.Format("-{0}", i);
            var dateName = String.Format("{0}{1}{2}", GetDatetimeString(), revision, extension);
            return dateName;
        }

        private string GetDatetimeString()
        {
            return String.Format("{0}-{1}-{2}_{3}-{4}",
                DateTime.UtcNow.Year.ToString("D4"),
                DateTime.UtcNow.Month.ToString("D2"),
                DateTime.UtcNow.Day.ToString("D2"),
                DateTime.UtcNow.Hour.ToString("D2"),
                DateTime.UtcNow.Minute.ToString("D2"));
        }

        public void Dispose()
        {
            try
            {
                foreach (var file in _filesToDelete.Where(File.Exists))
                {
                    File.Delete(file);
                }
            }
            catch (Exception e)
            {
                _errorHandler.HandleError(e);
            }
        }
    }
}
