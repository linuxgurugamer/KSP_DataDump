
using System;
using System.Collections.Generic;
using System.Reflection;
using static KSP_DataDump.DataDump;


namespace KSP_DataDump
{
    public class FldInfo
    {
        public string Name;
        public Type FieldType;

        public FldInfo(string n, Type t)
        {
            Name = n;
            FieldType = t;
        }

    }
    public class Property
    {
        public string modname;
        public string moduleName;
        public bool enabled;
        //public BaseFieldList fields;

        //public FieldInfo[] fieldsFromReflection;
        public List<FldInfo> fields;
        //public SortedDictionary<string, Field> fieldList;

        public Property(string modname, string moduleName)
        {
            this.modname = modname;
            this.moduleName = moduleName;
            // this.fields = null;
            // this.fieldsFromReflection = null;
            enabled = false;
            fields = new List<FldInfo>();
        }
        private string Key { get { return modname + "." + moduleName; } }
        private string CommonKey { get { return "PART." + moduleName; } }

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

        static public string GetKey(string modname, string moduleName)
        {
            //return modname + "." + moduleName;
            if (DataDump.selectedModsAppliesToAll)
                return "PART." + moduleName;
            else
                return modname + "." + moduleName;
        }


        static public void GetProperties(string modName, Module module)
        {
            DataDump.activeModule = module;

            var fields = module.type.GetFields(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.Static);
            Property p = new Property(modName, module.moduleName);

            if (!ActiveLists.activePropertyList.ContainsKey(p.ActiveKey))
                ActiveLists.activePropertyList.Add(p.ActiveKey, p);

            foreach (var f in fields)
                p.fields.Add(new FldInfo(f.Name, f.FieldType));

            foreach (PropertyInfo prop in module.type.GetProperties(BindingFlags.FlattenHierarchy |
                                    BindingFlags.Instance |
                                    BindingFlags.Public |
                                    BindingFlags.Static))
                p.fields.Add(new FldInfo(prop.Name, prop.PropertyType));
        }

        static public void GetPartProperties()
        {
            AvailablePart part = new AvailablePart();
            var partType = part.GetType();

            DataDump.activeMod = "PART";
            DataDump.activeModule = new Module("PART", "PART", partType);

            var fields = DataDump.activeModule.type.GetFields(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.GetProperty |
                                                            BindingFlags.Static);
            Property p = new Property("PART", "PART");
            if (!ActiveLists.activePropertyList.ContainsKey(p.Key))
            {
                ActiveLists.activePropertyList.Add(p.Key, p);
            }
            foreach (var f in fields)
                p.fields.Add(new FldInfo(f.Name, f.FieldType));

            // p.fieldsFromReflection = fields;

            foreach (PropertyInfo prop in DataDump.activeModule.type.GetProperties(BindingFlags.FlattenHierarchy |
                                                BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.Static))
                p.fields.Add(new FldInfo(prop.Name, prop.PropertyType));
        }


    }
}
