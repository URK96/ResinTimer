using ResinTimer.Resources;

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;

namespace ResinTimer
{
    public class License
    {
        public string LibraryName { get; set; }
        public string LicenseType { get; set; }

        public string GetLicenseContent()
        {
            const string ROOTCLASS = "ResinTimer";
            string path = string.Empty;
            string content = string.Empty;

            var assembly = IntrospectionExtensions.GetTypeInfo(typeof(LicenseViewer)).Assembly;

            switch (LicenseType)
            {
                case "Syncfusion License":
                    path = $"{ROOTCLASS}.License_Syncfusion.txt";
                    break;
                case "MIT License - Microsoft Corporation":
                    path = $"{ROOTCLASS}.License_MIT_Microsoft.txt";
                    break;
                default:
                    return string.Empty;
            }

            using (var reader = new StreamReader(assembly.GetManifestResourceStream(path)))
            {
                content = reader.ReadToEnd();
            }

            return content;
        }
    }
}
