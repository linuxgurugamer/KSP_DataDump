using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static KSP_DataDump.DataDump;

namespace KSP_DataDump
{
    public class Module
    {
        public static Dictionary<string, Module> modulesList = new Dictionary<string, Module>();

        public string modName;
        public string moduleName;
        public System.Type type;
        public bool enabled;
        public PartModule module;

        public Module(string modName, string moduleName, System.Type type)
        {
            this.modName = modName;
            this.moduleName = moduleName;
            this.type = type;

            enabled = false;
        }

        public string Key
        {
            get
            {
                if (DataDump.selectedModsAppliesToAll)
                    return "PART." + moduleName; 
                else
                    return modName + "." + moduleName;
            }
        }

        static public void GetModuleList()
        {
            List<AvailablePart> loadedParts = Utils.GetPartsList();

            foreach (AvailablePart part in loadedParts)
            {
                if (part == null)
                    continue;
                string partModName = Utils.FindPartMod(part);
                if (partModName != "")
                {
                    if (!DataDump.modList.ContainsKey(partModName))
                    {
                        DataDump.modList.Add(partModName, new DataDump.DataValue(false));
                    }
                }

                Module.CheckPartForModules(partModName, part);

                //Log.Info("partModName: " + partModName);
            }
        }

        static public void CheckPartForModules(string modName, AvailablePart part)
        {
            foreach (PartModule module in part.partPrefab.Modules)
            {
                var a = module.GetType();
                string fullName = module.moduleName;
                if (fullName == null)
                {
                    Log.Info(string.Format("{0} has a null moduleName, skipping it", part.name));
                    continue;
                }
                //if (part.name == "KK_F9demo_mainEngine")
                {
                    Log.Info("CheckPartForModules, part: " + part.name + ", module: " + fullName);
                }


                string usefulModuleName = UsefulModuleName(fullName);
                //Module mod = new Module(modName, usefulModuleName, a);
                Module mod = new Module(modName, fullName, a);

                mod.module = module;
                if (!modulesList.ContainsKey(mod.Key))
                {
                    modulesList.Add(mod.Key, mod);
                }
            }
        }
        static internal string UsefulModuleName(string longName)
        {
            if (longName.StartsWith("Module"))
                return longName.Substring(6);
            if (longName.StartsWith("FXModule"))
                return "FX" + longName.Substring(8);
            return longName;
        }

    }


}
