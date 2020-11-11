using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KSP_DataDump
{
    public class Field
    {
        public string fieldName;
        public bool enabled;

        public string modName;
        public string moduleName;

        public Field(string modName, string moduleName, string fieldName)
        {
            this.fieldName = fieldName;
            this.modName = modName;
            this.moduleName = moduleName;
            enabled = false;
        }

        string Key { get { return modName + "." + moduleName + "." + fieldName; } }
        string CommonKey { get { return "MOD." + moduleName + "." + fieldName; } }

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
    }

}
