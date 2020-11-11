using System;
using System.Collections.Generic;
using static KSP_DataDump.DataDump;

namespace KSP_DataDump
{
    public class ActiveLists
    {
        Dictionary<string, Module> modulesList = new Dictionary<string, Module>();
        Dictionary<string, Module> commonModulesList = new Dictionary<string, Module>();

        SortedDictionary<string, Field> fieldsList = new SortedDictionary<string, Field>();
        SortedDictionary<string, Field> commonFieldsList = new SortedDictionary<string, Field>();


        SortedDictionary<string, Property> propertyList = new SortedDictionary<string, Property>();
        SortedDictionary<string, Property> commonPropertyList = new SortedDictionary<string, Property>();


        static public Dictionary<string, Module> activeModuleList = null;
        static public SortedDictionary<string, Property> activePropertyList = null;
        static public SortedDictionary<string, Field> activeFieldsList = null;

        public static Dictionary<string, DataValue> modList = new Dictionary<string, DataValue>();
        static public Dictionary<string, Module> AllModulesList;
        public ActiveLists()
        {
            SetActiveModuleList(false);
            Property.GetPartProperties();
            Module.GetModuleList();

            SetActiveModuleList(true);
            Property.GetPartProperties();
            Module.GetModuleList();

            AllModulesList = modulesList;
#if false
            Log.Info("ActiveLists");
            Log.Info("modList.Count: " + modList.Values.Count);
            foreach (var mod in modList)
                Log.Info(mod.Key);
            Log.Info("PartModuleLists, commonModulesList.Count: " + commonModulesList.Values.Count);
            foreach (var module in commonModulesList)
                Log.Info(module.Key + "::" +module.Value.ModuleToString());
            Log.Info("PartModuleLists, modulesList.Count: " + modulesList.Values.Count);
            foreach (var  module in modulesList)
                Log.Info(module.Key + "::" + module.Value.ModuleToString());
#endif
        }
        public void SetActiveModuleList(bool b)
        {
            DataDump.selectedModsAppliesToAll = b;

            if (DataDump.selectedModsAppliesToAll)
            {
                activeModuleList = commonModulesList;
                activePropertyList = commonPropertyList;
                activeFieldsList = commonFieldsList;
            }
            else
            {
                activeModuleList = modulesList;
                activePropertyList = propertyList;
                activeFieldsList = fieldsList;
            }
        }


    }
}
