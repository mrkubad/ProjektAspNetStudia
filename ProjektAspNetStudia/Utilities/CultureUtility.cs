using System.Collections.Generic;
using System.Globalization;
using System.IO.Pipes;
using System.Linq;
using System.Threading;

namespace ProjektAspNetStudia.Utilities
{
    public static class CultureUtility
    {
        public static bool SetCurrentThreadCulture(string language)
        {
            var supportedLangs = new List<string> {"en-us", "pl-pl"}; // for now hardcoded
            var lang = language.ToLower();

            if (supportedLangs.Any(l => l.Equals(lang)))
            {
                var specificCulture = CultureInfo.CreateSpecificCulture(language);
                Thread.CurrentThread.CurrentCulture = specificCulture;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(specificCulture.Name);

                return true;
            }

            return false;
        }

        
    }
}
