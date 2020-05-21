using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_DataDump
{
    public class Utils
    {
        private static UrlDir.UrlConfig[] configs = null;

        static public string FindPartMod(AvailablePart part)
        {
            if (configs == null)
                configs = GameDatabase.Instance.GetConfigs("PART");

            //Log.Info("ModFilterWindow.FindPartMod, part.name: " + part.name);
            UrlDir.UrlConfig config = Array.Find<UrlDir.UrlConfig>(configs, (c => (part.name == c.name.Replace('_', '.').Replace(' ', '.'))));
            if (config == null)
            {
                config = Array.Find<UrlDir.UrlConfig>(configs, (c => (part.name == c.name)));
                if (config == null)
                    return "";
            }
            var id = new UrlDir.UrlIdentifier(config.url);
            return id[0];
        }

        public static List<AvailablePart> GetPartsList()
        {
            List<AvailablePart> loadedParts = new List<AvailablePart>();
            loadedParts.AddRange(PartLoader.LoadedPartsList); // make a copy we can manipulate

            // these two parts are internal and just serve to mess up our lists and stuff
            AvailablePart kerbalEVA = null;
            AvailablePart flag = null;
            foreach (var part in loadedParts)
            {
                if (part.name.Contains("kerbalEVA"))
                    kerbalEVA = part;
                else if (part.name == "flag")
                    flag = part;
            }

            // still need to prevent errors with null refs when looking up these parts though
            if (kerbalEVA != null)
            {
                loadedParts.Remove(kerbalEVA);
            }
            if (flag != null)
            {
                loadedParts.Remove(flag);
            }
            return loadedParts;
        }

    }
}
