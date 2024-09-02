using SalesForceBackup.Interfaces;
using SalesForceBackup.SFDC;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SalesForceBackup
{
    /// <summary>
    /// Downloads backup files from the Salesforce website by scraping the web page.
    /// </summary>
    public class SalesForceWebDownloader : IDownloader
    {
        private readonly IAppSettings _appSettings;
        private readonly IErrorHandler _errorHandler;
        private readonly IAddressProvider _addressProvider;

        public SalesForceWebDownloader(IAppSettings appSettings, IErrorHandler errorHandler, IAddressProvider addressProvider)
        {
            //_appSettings = TinyIoCContainer.Current.Resolve<IAppSettings>();
            //_errorHandler = TinyIoCContainer.Current.Resolve<IErrorHandler>();
            _appSettings = appSettings;
            _errorHandler = errorHandler;
            _addressProvider = addressProvider;
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        }

        //public string[] Download()
        public async Task<List<string>> Download()
        {
            var files = new List<string>();
            // var baseAddress =  new Uri(String.Format("{0}://{1}", _appSettings.Get(AppSettingKeys.Scheme), _appSettings.Get(AppSettingKeys.Host)));
            var baseAddress = _addressProvider.SalesForceBaseAddress();
            //try
            //{
            Console.Write("Connecting to SalesForce.com ... ");
            var sessionId = LogIn();
            Console.WriteLine("\u221A");

            Console.Write("Getting list of export files ... ");
            var exportFiles = await DownloadListOfExportFiles(sessionId);
            Console.WriteLine("\u221A");

            for (int i = 0; i < exportFiles.Count; i++)
            {
                var exportFile = exportFiles[i];
                Console.Write(String.Format("Downloading {0} of {1}: {2} ... ", i + 1, exportFiles.Count, exportFile.FileName));
                await DownloadExportFile(exportFile, baseAddress, sessionId);  //.Wait();
                Console.WriteLine("\u221A");

                // files.Add(String.Join(Path.DirectorySeparatorChar.ToString(CultureInfo.InvariantCulture), new[] { Environment.CurrentDirectory, exportFile.FileName }));
                files.Add(_addressProvider.SalesForceSaveAddress(exportFile.FileName));
            }
            //}
            //catch (Exception e)
            //{
            //    _errorHandler.HandleError(e);
            //}
           
            //return files.ToArray();
            return files;

        }

        private string LogIn()
        {
            var sfClient = new SforceService();
            var username = _appSettings.Get(AppSettingKeys.Username);
            var password = _appSettings.Get(AppSettingKeys.Password) + _appSettings.Get(AppSettingKeys.SecurityToken);
            var currentLoginResult = sfClient.login(username, password);
            CheckloginResult(currentLoginResult);
            // Change the binding to the new endpoint
            // sfClient.Url = currentLoginResult.serverUrl;

            // Create a new session header object and set the session id to that returned by the login
            var sessionId = currentLoginResult.sessionId;
            // sfClient.SessionHeaderValue = new SessionHeader { sessionId = sessionId };
            return sessionId;
            // return Guid.NewGuid().ToString();
        }

        private async Task<List<DataExportFile>> DownloadListOfExportFiles(string sessionId)
        { 
            var page = await DownloadWebpage(_appSettings.Get(AppSettingKeys.DataExportPage), sessionId);
            var matches = GetMatchingItems(page);
            return GetExportFiles(matches);
        }

        private async Task<HttpResponseMessage> DownloadWebpage(string url, string sessionId)
        {
            // var baseAddress = new Uri(String.Format("{0}://{1}", _appSettings.Get(AppSettingKeys.Scheme), _appSettings.Get(AppSettingKeys.Host)));
            var baseAddress = _addressProvider.SalesForceBaseAddress();
            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var message = new HttpRequestMessage(HttpMethod.Get, url);
                message.Headers.Add("Cookie", String.Format("sid={0}", sessionId));
                return await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead);
            }
        }

        private MatchCollection GetMatchingItems(HttpResponseMessage page)
        {
            var regex = new Regex(_appSettings.Get(AppSettingKeys.FilenamePattern), RegexOptions.IgnoreCase);
            var matches = regex.Matches(page.Content.ReadAsStringAsync().Result);
            return matches;
        }

        private List<DataExportFile> GetExportFiles(MatchCollection matches)
        {
            List<DataExportFile> result = new List<DataExportFile>();
            foreach (Match match in matches)
            {
                var fileName = match.Groups[1].ToString().Split(new[] { '&' })[0];
               // var url = String.Format("{0}{1}", _appSettings.Get(AppSettingKeys.DownloadPage), match.ToString().Replace("&amp;", "&"));
                var url = _addressProvider.SalesForceUrlFormater(match);
                result.Add(new DataExportFile(fileName, url.Substring(0, url.Length - 1)));
            }
            return result;
        }

        private async System.Threading.Tasks.Task DownloadExportFile(DataExportFile dataExportFile, Uri baseAddress, string sessionId)
        {
            using (var handler = new HttpClientHandler { UseCookies = false })
            using (var client = new HttpClient(handler) { BaseAddress = baseAddress })
            {
                var message = new HttpRequestMessage(HttpMethod.Get, dataExportFile.Url);
                message.Headers.Add("Cookie", String.Format("sid={0}", sessionId));

                using (HttpResponseMessage response = await client.SendAsync(message, HttpCompletionOption.ResponseHeadersRead))
                using (Stream streamToReadFrom = await response.Content.ReadAsStreamAsync())
                {
                    using (Stream fileStream = File.Open(dataExportFile.FileName, FileMode.Create))
                    {
                        await streamToReadFrom.CopyToAsync(fileStream);
                    }
                }
            }
        }

        private void CheckloginResult(LoginResult loginResult)
        {
            if (loginResult is null)
            {
                throw new Exception("SalesForceWebDownloader cannot login to SalesForce");
            }
        }
    }
}
