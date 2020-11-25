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
        public string modName;
        public string moduleName;
        public System.Type type;
        public bool enabled;
        public PartModule module;
        public bool multipleModules;
        public Module(string modName, string moduleName, System.Type type)
        {
            this.modName = modName;
            this.moduleName = moduleName;
            this.type = type;

            enabled = false;
            multipleModules = false;
        }

        public string ModuleToString()
        {
            return ((modName != null ? modName : "null" )+ ":" + type.ToString() + ":" + enabled.ToString() + ":" +
                (moduleName != null ? moduleName : "null"));
        }

        string Key { get { return modName + "." + moduleName; } }

        string CommonKey { get { return "PART." + moduleName; } }

        public string ActiveKey
        {
            get
            {
                if (DataDump.selectedModsAppliesToAll)
                    return CommonKey;
                else
                    return Key;
            }
        }

        public string ModuleInfoKey(int i)
        {
            return moduleName + (i == 0 ? "" : "-2");
        }

        static public void GetModuleList()
        {
            List<AvailablePart> loadedParts = Utils.GetPartsList();

            foreach (AvailablePart part in loadedParts)
            {
                if (part == null)
                    continue;
                string partModName;
                if (DataDump.selectedModsAppliesToAll)
                    partModName = "PART";
                else
                    partModName = Utils.FindPartMod(part);
                if (partModName != "")
                {
                    if (!ActiveLists.modList.ContainsKey(partModName))
                    {
                        ActiveLists.modList.Add(partModName, new DataValue(false));
                    }
                }

                Module.CheckPartForModules(partModName, part);
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
                    Log.Error(string.Format("{0} has a null moduleName, skipping it", part.name));
                    continue;
                }


                string usefulModuleName = UsefulModuleName(fullName);
                //Module mod = new Module(modName, usefulModuleName, a);
                Module mod = new Module(modName, fullName, a);

                mod.module = module;
                if (!ActiveLists.activeModuleList.ContainsKey(mod.ActiveKey))
                {
                    // Needs to do another "new" here to have a unique entry inthe commonModulesList
                    mod = new Module(modName, fullName, a);
                    ActiveLists.activeModuleList.Add(mod.ActiveKey, mod);
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
