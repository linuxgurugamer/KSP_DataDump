using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using ClickThroughFix;
using KSP.Localization;
using System.Reflection;
using System.Text.RegularExpressions;
using static KSP_DataDump.DataDump;

using System.Linq;

namespace KSP_DataDump
{
    public class ExportData : MonoBehaviour
    {
        // First get list of unique modules
        // Then build a CSV, putting data for each one into the same columns

        public class ModInfo
        {
            public string moduleName;
            public int numFields;
            public int index;
            public ModInfo(string moduleName)
            {
                this.moduleName = moduleName;
            }
        }

        internal class ModuleInfo
        {
            internal string moduleName;
            internal int numFields;
            internal int startingCol;
            internal bool secondModule;
            internal ModuleInfo(string moduleName, int numFields, int startingCol, int moduleNum)
            {
                this.moduleName = moduleName;
                this.numFields = numFields;
                this.startingCol = startingCol;
                secondModule = (moduleNum == 1);
            }

        }
        Dictionary<string, ModuleInfo> moduleInfoList = new Dictionary<string, ModuleInfo>();

        StringBuilder line, headerLine;
        int colCnt = 0;
        void StartLine(string str)
        {
            Log.Info("StartLine: " + str);
            line = new StringBuilder(str);
            colCnt = 1;
        }
        void AppendLine(string str)
        {
            if (str == null) str = "";
            str = str.Replace("\"", "'");
            line.Append(",\"" + str + "\"");
            colCnt++;
            Log.Info("AppendLine, str: " + str);
        }
        void AppendLine(int i)
        {
            AppendLine(i.ToString());
        }
        void AppendLine(float f)
        {
            AppendLine(f.ToString());
        }
        void EndLine()
        {
            line.Append("\n");
        }
        public void GetUniqueModuleList()
        {
            bool started = false;

#if DEBUG
            foreach (var f in ActiveLists.activeFieldsList)
                if (f.Value.enabled)
                    Log.Info("fieldsList, key: " + f.Key + ", enabled: " + f.Value.enabled);
#endif

            DataDump.activeMod = "PART";
            AvailablePart part = new AvailablePart();
            var a = part.GetType();
            Module partmod = new Module("PART", "PART", a);
            //StartLine("");
            //started = true;



            if (ActiveLists.activePropertyList.TryGetValue(Property.GetKey(partmod.modName, partmod.moduleName), out Property p))
            {
                foreach (var s in p.fields) //FromReflection)
                {
                    if (!started)
                    {
                        StartLine(partmod.moduleName + "." + s.Name);
                        started = true;
                        for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
                        {
                            if (DataDump.partAttrs[(int)partAttr - 1] != null && DataDump.partAttrs[(int)partAttr - 1].enabled)
                            {
                                if (partAttr == PartAttrEnum.DimensionsInfo)
                                {
                                    AppendLine("X");
                                    AppendLine("Y");
                                    AppendLine("Z");
                                }
                                else
                                {
                                    AppendLine(PartAttrStr[(int)partAttr - 1] /* + "-pre" */);
                                }
                            }
                        }
                    }
                    Field field = new Field(partmod.modName, partmod.moduleName, s.Name);
                    if (ActiveLists.activeFieldsList.TryGetValue(field.ActiveKey, out field))
                    {
                        if (field.enabled)
                        {
                            if (!moduleInfoList.ContainsKey(partmod.moduleName))
                            {
                                moduleInfoList.Add(partmod.modName, new ModuleInfo(partmod.moduleName, 1, colCnt, 0));
                            }
                            else
                            {
                                moduleInfoList[partmod.moduleName].numFields++;
                            }
                            if (!started)
                            {
                            }
                            else
                            {
                                var str = partmod.moduleName;
                                if (str.Length > 6 && str.Substring(0, 5) == "Module")
                                    str = str.Substring(6);
                                AppendLine(str + "." + s.Name);
                            }
                        }
                    }
                }
            }

            Dictionary<string, string> colHeader = new Dictionary<string, string>();

            foreach (var m in ActiveLists.activeModuleList)
            {
                if (m.Value.enabled)
                {
                    Module mod = m.Value;
                    for (int i = 0; i < (mod.multipleModules ? 2 : 1); i++)
                    {
                        Log.Info("mod.multipleModules: " + mod.multipleModules);
                        if (ActiveLists.activePropertyList.TryGetValue(Property.GetKey(mod.modName, mod.moduleName), out p))
                        {
                            foreach (var s in p.fields) //FromReflection)
                            {
                                Field field = new Field(mod.modName, mod.moduleName, s.Name);
                                if (ActiveLists.activeFieldsList.TryGetValue(field.ActiveKey, out field))
                                {
                                    if (field.enabled)
                                    {
                                        if (mod.moduleName == null)
                                            Log.Error("moduleName 2 is null");
                                        if (!moduleInfoList.ContainsKey(mod.ModuleInfoKey(i)))
                                        {
                                            moduleInfoList.Add(mod.ModuleInfoKey(i), new ModuleInfo(mod.moduleName, 1, colCnt, i));

                                        }
                                        else
                                        {
                                            moduleInfoList[mod.ModuleInfoKey(i)].numFields++;
                                        }
                                        string str = mod.moduleName + (i==0?"":"-2")+ "." + s.Name;
                                        if (!colHeader.ContainsKey(str))
                                        {
                                            colHeader.Add(str, str);
                                            if (!started)
                                            {
                                                StartLine(m.Value.moduleName + "." + s.Name);
                                                started = true;
                                            }
                                            else
                                                AppendLine(m.Value.moduleName + "." + s.Name);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            if (started)
                EndLine();
            if (!onefilePerMod)
            {
                byte[] bytes = Encoding.ASCII.GetBytes(line.ToString());
                fs.Write(bytes, 0, line.Length);
#if false
                foreach (var m in moduleInfoList)
                {
                    Log.Info("Module: " + m.Value.moduleName + ", startingCol: " + m.Value.startingCol + ", numFields: " + m.Value.numFields);
                }
                Log.Info("headerLine: " + line.ToString());
#endif
            }
            else
            {
                headerLine = line;
                Log.Info("headerLine: " + headerLine.ToString());
            }
        }

        const int MAXCOL = 1000;
        int maxUsedCol = 0;
        FileStream fs = null;

        void OpenFile(string outputFile)
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            fs = File.OpenWrite(outputFile);
        }

        void GetPartData(AvailablePart part)
        {
            bool started = false;
            Vector3 pg = new Vector3();
            StartLine(part.name);
            started = true;
            if (partAttrs[(int)PartAttrEnum.DimensionsInfo - 1] != null && partAttrs[(int)PartAttrEnum.DimensionsInfo - 1].enabled)
            {
                //
                // Now get the part dimensions: x,y,z
                //
                List<Bounds> list = new List<Bounds>();
                if (!(part.partPrefab.Modules.GetModule<LaunchClamp>(0) != null))
                {

                    Bounds[] partRendererBounds = PartGeometryUtil.GetPartRendererBounds(part.partPrefab);
                    int num = partRendererBounds.Length;
                    for (int j = 0; j < num; j++)
                    {
                        Bounds bounds2 = partRendererBounds[j];
                        Bounds bounds3 = bounds2;
                        bounds3.size *= part.partPrefab.boundsMultiplier;
                        Vector3 size = bounds3.size;
                        bounds3.Expand(part.partPrefab.GetModuleSize(size, ModifierStagingSituation.CURRENT));
                        list.Add(bounds2);
                    }
                }

                pg = PartGeometryUtil.MergeBounds(list.ToArray(), part.partPrefab.transform.root).size;
#if false
                if (!started)
                    StartLine(pg.x.ToString("F3"));
                else
                    AppendLine(pg.x.ToString("F3"));
                AppendLine(pg.y.ToString("F3"));
                AppendLine(pg.z.ToString("F3"));
                started = true;
#endif
#if false
                string resources = "";
                foreach (AvailablePart.ResourceInfo r in part.resourceInfos)
                {
                    if (r.resourceName != "ElectricCharge" && r.resourceName != "Ablator")
                        resources += r.resourceName + ",";
                }
                if (resources != "")
                {

                    Log.Info("part: " + part.name + ", part.title: " + part.title + ", descr: " + part.description.Replace(",", ".") +
                        ", mass: " + part.partPrefab.mass + ", techRequired: " + part.TechRequired +
                        ", height x,y,z: " + pg.x.ToString() + ", " + pg.y.ToString() + ", " + pg.z.ToString() + "," + resources);
                }
#endif
            }

            DataDump.activeMod = "PART";
            var a = part.GetType();
            bool b = false;
            Module partmod = new Module("PART", "PART", a);
            // ConfigNode partNode = part.partConfig.GetNode("PART");
            //Log.Info("partConfig: " + part.partConfig);
            if (ActiveLists.activePropertyList.TryGetValue(Property.GetKey(partmod.modName, partmod.moduleName), out Property p))
            {
                foreach (FldInfo s in p.fields) //FromReflection)
                {
                    if (!b)
                    {
                        //string value = part.partConfig.GetValue(s.Name);


                        b = true;
                        for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
                        {
                            if (DataDump.partAttrs[(int)partAttr - 1] != null && DataDump.partAttrs[(int)partAttr - 1].enabled)
                            {
                                if (partAttr == PartAttrEnum.DimensionsInfo)
                                {
                                    AppendLine(pg.x.ToString("F3"));
                                    AppendLine(pg.y.ToString("F3"));
                                    AppendLine(pg.z.ToString("F3"));
                                }
                                else
                                {
                                    string str = "n/a";
                                    str = "";
                                    if (!part.partConfig.TryGetValue(partAttr.ToString(), ref str))
                                    {
                                        str = "";
                                        Log.Error("data not found");
                                    }

                                    //str = part.partConfig.GetValue(partAttr.ToString());

                                    AppendLine(str);
                                }
                            }
                        }
                    }

                    Field field = new Field(partmod.modName, partmod.moduleName, s.Name);
                    if (s.Name == "entryCost")
                        s.Name = "_entryCost";
                    if (ActiveLists.activeFieldsList.TryGetValue(field.ActiveKey, out field))
                    {
                        if (field != null && field.enabled)
                        {

                            string value = part.partConfig.GetValue(s.Name);
                            if (value == null || value == "")
                            {
                                Type fieldsType = typeof(AvailablePart);
                                // Get an array of FieldInfo objects.
                                FieldInfo[] fields = fieldsType.GetFields(BindingFlags.FlattenHierarchy |
                                                            BindingFlags.Instance |
                                                            BindingFlags.Public | BindingFlags.NonPublic |
                                                            BindingFlags.Static);
                                foreach (var f in fields)
                                {
                                    string v = "";


                                    var fieldtype = s.FieldType.ToString();
                                    string t = "System.Collections.Generic";
                                    if (fieldtype.StartsWith(t))
                                        fieldtype = fieldtype.Substring(t.Length + 1);


                                    if (f.Name == s.Name)
                                    {
                                        switch (fieldtype)
                                        {
                                            case "System.Single":
                                                v = "float";
                                                value = ((float)f.GetValue(part)).ToString();
                                                break;
                                            case "System.Double":
                                                v = "double";
                                                value = ((double)f.GetValue(part)).ToString();
                                                break;
                                            case "System.Int32":
                                                v = "int";
                                                value = ((int)f.GetValue(part)).ToString();
                                                break;
                                            case "System.UInt32":
                                                v = "uint";
                                                value = ((uint)f.GetValue(part)).ToString();
                                                break;
                                            case "System.String":
                                                v = "string";
                                                value = (string)f.GetValue(part);
                                                if (s.Name == "description")
                                                {
                                                    value = StripHtml(value);
                                                    if (value.Length > maxLen && s.Name == "description")
                                                        value = value.Substring(0, maxLen);
                                                    value = value.Replace("\n", " ").Replace("\r", " ");
                                                }
                                                break;
                                            case "System.Boolean":
                                                v = "boolean";
                                                value = ((bool)f.GetValue(part)).ToString();
                                                break;
                                            case "UnityEngine.Vector3":
                                                v = "Vector3";
                                                value = ((Vector3)f.GetValue(part)).ToString();
                                                break;

                                        }
                                        break;
                                    }
                                    Log.Info("v: " + v);
                                }
                            }
                            if (!started)
                            {
                            }
                            else
                                AppendLine(value);


                        }
                    }
                    else
                    {
                        Log.Error("GetPartData, not found");
                    }
                }
            }
        }

        private string StripHtml(string source)
        {
            string output;

            //get rid of HTML tags
            output = Regex.Replace(source, "<[^>]*>", string.Empty);

            //get rid of multiple blank lines
            output = Regex.Replace(output, @"^\s*$\n", string.Empty, RegexOptions.Multiline);

            return output;
        }

        public void DumpData()
        {
            string currentMod = "";
            List<AvailablePart> loadedParts = Utils.GetPartsList();

            foreach (AvailablePart part in loadedParts)
            {
                if (part == null || part.name.Length >= 9 && part.name.Substring(0, 9) == "kerbalEVA")
                    continue;
                string[] colData = new string[MAXCOL];
                string lastModuleName = "";
                int multiple = 0;
                string partModName = Utils.FindPartMod(part);
                if (partModName != "")
                {
                    // Check to see if this part module is selected
                    if (ActiveLists.modList.TryGetValue(partModName, out DataValue dv) && dv.enabled)
                    {
                        for (int i = 0; i < MAXCOL; i++)
                            colData[i] = null;
                        lastModuleName = "";
                        multiple = 0;

                        // Go through all the modules on the part.  Sort it to keep multiple (ie:  ModuleEnginesFx) sequential
                        var orderedList = part.partPrefab.Modules;
                        foreach (PartModule partModule in orderedList)
                        {
                            if (partModule.moduleName == null)
                                continue;
                            multiple += (partModule.moduleName == lastModuleName) ? 1 : 0;
                            lastModuleName = partModule.moduleName;

                            var partModType = partModule.GetType();
                            Module mod = new Module(partModName, partModule.moduleName, partModType);


                            //string usefulModuleName = Module.UsefulModuleName(module.moduleName);
                            //Module mod = new Module(partModName, usefulModuleName, a);

                            // Go through the list of selected part modules
                            if (ActiveLists.activeModuleList.TryGetValue(mod.ActiveKey, out mod))
                            {
                                if (moduleInfoList.ContainsKey(mod.ModuleInfoKey(multiple)))
                                {
                                    var m = moduleInfoList[mod.ModuleInfoKey(multiple)];
                                    int cnt = 0;
                                    if (ActiveLists.activePropertyList.TryGetValue(Property.GetKey(mod.modName, mod.moduleName), out Property p))
                                    {
                                        foreach (FldInfo s in p.fields) //FromReflection)
                                        {

                                            Field field = new Field(mod.modName, mod.moduleName, s.Name);
                                            if (ActiveLists.activeFieldsList.TryGetValue(field.ActiveKey, out field))
                                            {
                                                if (field.enabled)
                                                {
                                                    ActiveLists.activeFieldsList[field.ActiveKey].enabled = GUILayout.Toggle(ActiveLists.activeFieldsList[field.ActiveKey].enabled, "");
                                                    string v = "unknownData";
                                                    if (part.partConfig != null)
                                                    {
                                                        var moduleNodes = part.partConfig.GetNodes("MODULE");
                                                        if (moduleNodes == null)
                                                            Log.Error("moduleNodes is null");
                                                        string lastModuleNodeName = "";
                                                        foreach (ConfigNode moduleNode in moduleNodes)
                                                        {
                                                            var name = moduleNode.GetValue("name");

                                                            string v1 = "";
                                                            moduleNode.TryGetValue(s.Name, ref v1);


                                                            if ((name == partModule.moduleName && multiple == 0) ||
                                                                (name == partModule.moduleName && multiple == 1 && partModule.moduleName == lastModuleNodeName))
                                                            {
                                                                moduleNode.TryGetValue(s.Name, ref v);
                                                                break;
                                                            }

                                                            lastModuleNodeName = name;
                                                        }
                                                    }

                                                    colData[m.startingCol + cnt] = Localizer.Format(v);

                                                    maxUsedCol = Math.Max(maxUsedCol, m.startingCol + cnt);
                                                    cnt++;
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (onefilePerMod)
                        {
                            if (currentMod != partModName)
                            {
                                if (fs != null)
                                    fs.Close();
                                outputFile = partModName + ".csv";
                                OpenFile(outputFile);
                                if (allOutputfiles != "")
                                    allOutputfiles += ", " + outputFile;
                                else
                                    allOutputfiles = outputFile;
                                byte[] bytes2 = Encoding.ASCII.GetBytes(headerLine.ToString());
                                fs.Write(bytes2, 0, headerLine.Length);
                                currentMod = partModName;
                            }
                        }
                        GetPartData(part);

                        for (int f = colCnt; f <= maxUsedCol; f++)
                        {
                            if (colData[f] != null)
                                AppendLine(colData[f]);
                            else
                                AppendLine("");

                        }
                        EndLine();
                        byte[] bytes = Encoding.ASCII.GetBytes(line.ToString());
                        fs.Write(bytes, 0, line.Length);

                    }
                }
            }
            fs.Close();

        }
#if false
        Dictionary<string, Module> activeModuleList;
        SortedDictionary<string, Property> activePropertyList = null;

        void SetActiveModuleList()
        {
            if (selectedModsAppliesToAll)
                activeModuleList = Module.commonModulesList;
            else
                activeModuleList = Module.modulesList;
            if (selectedModsAppliesToAll)
                activePropertyList = Property.commonPropertyList;
            else
                activePropertyList = activePropertyList;
        }
#endif

        public void Start()
        {
            //SetActiveModuleList();
            foreach (var m in ActiveLists.activeModuleList)
            {
                if (m.Value.enabled)
                {
                    outputFile = "ExportedData.csv";
                }
            }
        }
        Rect winPos = new Rect(400, 400, 500, 150);

        bool completed = false;
        public void OnGUI()
        {
            //GUI.skin = HighLogic.Skin;
            if (!completed)
                winPos = ClickThruBlocker.GUILayoutWindow(567382457, winPos, GetOutputFile, "KSP DataDump Output", DataDump.window);
            else
                winPos = ClickThruBlocker.GUILayoutWindow(57382457, winPos, ExportCompleted, "KSP DataDump Output", DataDump.window);
        }
        string outputFile = "DataDump";
        string maxLenStr = "80";
        int maxLen = 80;
        string allOutputfiles = "";
        bool onefilePerMod = false;

        public void GetOutputFile(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter maximum description length:");
            string tmpMaxLenStr = GUILayout.TextField(maxLenStr);
            if (int.TryParse(tmpMaxLenStr, out maxLen))
            {
                maxLenStr = tmpMaxLenStr;
            }
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (onefilePerMod)
                GUI.enabled = false;
            GUILayout.Label("Enter filename: ");
            outputFile = GUILayout.TextField(outputFile);
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUILayout.BeginHorizontal();
            onefilePerMod = GUILayout.Toggle(onefilePerMod, "Make a unique file for each selected mod");
            GUILayout.EndHorizontal();
            if (outputFile.Length < 4 || outputFile.ToLower().Substring(outputFile.Length - 4) != ".csv")
                outputFile += ".csv";

            GUILayout.BeginHorizontal();
            if (outputFile == ".csv" || outputFile == "")
                GUI.enabled = false;
            if (GUILayout.Button("Proceed"))
            {
                if (!onefilePerMod)
                    OpenFile(outputFile);
                GetUniqueModuleList();
                DumpData();
                completed = true;
                //Destroy(this);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                Destroy(this);
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }

        public void ExportCompleted(int id)
        {
            GUILayout.BeginVertical();
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (onefilePerMod)
                outputFile = allOutputfiles;
            GUILayout.Label("Output file: " + outputFile);
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export completed.  File(s): " + outputFile + "\nClick to continue", GUILayout.Height(40)))
            {
                Destroy(this);
            }
            GUILayout.EndHorizontal();
            GUILayout.FlexibleSpace();
            GUILayout.EndVertical();
            GUI.DragWindow();
        }
    }
}
