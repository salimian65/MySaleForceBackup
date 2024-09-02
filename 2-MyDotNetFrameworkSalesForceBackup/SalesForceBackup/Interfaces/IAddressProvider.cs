using System;
using System.Text.RegularExpressions;

namespace SalesForceBackup.Interfaces
{
    public interface IAddressProvider
    {
        Uri SalesForceBaseAddress();

        string SalesForceSaveAddress(string fileName);

        string SalesForceUrlFormater(Match match);
    }
}
