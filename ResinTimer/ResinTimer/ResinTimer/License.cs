using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.Text;

namespace ResinTimer
{
    public class License
    {
        public string LibraryName { get; set; }
        public string LicenseType { get; set; }

        public string GetLicenseContent()
        {
            string content;

            switch (LicenseType)
            {
                case "Syncfusion License":
                    content = AppResources.License_Syncfusion;
                    break;
                case "MIT License - Microsoft Corporation":
                    content = AppResources.License_MIT_Microsoft;
                    break;
                default:
                    content = string.Empty;
                    break;
            }

            return content;
        }
    }
}
