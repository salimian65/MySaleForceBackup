using System;

namespace SalesForceBackup.Interfaces
{
    public interface IAddressProvider
    {
        Uri SalesForceBaseAddress();

        string SalesForceSaveAddress(string fileName);
    }
}
