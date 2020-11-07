using KSP_Log;
using System;
using System.Collections.Generic;
using System.Reflection;


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
        public string Key { get { return modname + "." + moduleName; } }
        static public string GetKey(string modname, string moduleName)
        {
            return modname + "." + moduleName;
        }

        static public SortedDictionary<string, Property> propertyList = new SortedDictionary<string, Property>();

        static public void GetProperties(string modName, Module module)
        {
            DataDump.activeMod = modName;
            DataDump.activeModule = module;

            var fields = module.type.GetFields(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.Static);
            Property p = new Property(modName, module.moduleName);
            if (!propertyList.ContainsKey(p.Key))
                propertyList.Add(p.Key, p);
            foreach (var f in fields)
                p.fields.Add( new FldInfo( f.Name, f.FieldType));
            //p.fieldsFromReflection = fields;

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
            DataDump.activeModule = new Module("PART", "PART",partType)                ;

            var fields = DataDump.activeModule.type.GetFields(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic|
                                                            BindingFlags.GetProperty |
                                                            BindingFlags.Static);
            Property p = new Property("PART", "PART");
            if (!propertyList.ContainsKey(p.Key))
            {
                propertyList.Add(p.Key, p);
            }
            foreach (var f in fields)
                p.fields.Add(new FldInfo( f.Name, f.FieldType));

           // p.fieldsFromReflection = fields;

            foreach (PropertyInfo prop in DataDump.activeModule.type.GetProperties(BindingFlags.FlattenHierarchy |
                                                BindingFlags.Instance |
                                                BindingFlags.Public |
                                                BindingFlags.Static))               
                    p.fields.Add(new FldInfo(prop.Name, prop.PropertyType));
        }


    }
}
