using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;


namespace KSP_DataDump
{
    public class Property
    {
        public string modname;
        public string moduleName;
        public string name;
        public Type type;
        public string attribute;
        public bool enabled;
        public BaseFieldList fields;
        //public SortedDictionary<string, Field> fieldList;

        public Property(string modname, string moduleName, string name, Type type)
        {
            this.modname = modname;
            this.moduleName = moduleName;
            this.name = name;
            this.type = type;
            this.attribute = type.Attributes.ToString();
            this.fields = null;
            enabled = false;
        }
        public string Key { get { return modname + "." + name; } }

        static public SortedDictionary<string, Property> propertyList = new SortedDictionary<string, Property>();
        static public void GetProperties(string modName, Module module)
        {
            DataDump.activeMod = modName;
            DataDump.activeModule = module;

            foreach (PropertyInfo prop in module.type.GetProperties(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public |
                                                            BindingFlags.Static))
            {
                Property p = new Property(modName, module.moduleName, prop.Name, prop.PropertyType);
                if (prop.Name == "Fields")
                {
                    p.fields = (BaseFieldList)module.module.Fields;
                }
                if (!propertyList.ContainsKey(p.Key))
                {
                    propertyList.Add(p.Key, p);
                }
            }
        }

    }

}
