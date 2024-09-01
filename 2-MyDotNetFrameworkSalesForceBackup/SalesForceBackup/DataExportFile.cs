﻿namespace SalesForceBackup
{
    class DataExportFile
    {
        public DataExportFile(string FileName, string Url)
        {
            this.FileName = FileName;
            this.Url = Url;
        }

        public string FileName { get; private set; }
        public string Url { get; private set; }
    }
}
