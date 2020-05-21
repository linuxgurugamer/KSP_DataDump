﻿using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Vectrosity;
using ClickThroughFix;
using UnityEngine.Experimental.XR;
using KSP.Localization;


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

            internal ModuleInfo(string moduleName, int numFields, int startingCol)
            {
                this.moduleName = moduleName;
                this.numFields = numFields;
                this.startingCol = startingCol;
            }

        }
        Dictionary<string, ModuleInfo> moduleInfoList = new Dictionary<string, ModuleInfo>();

        StringBuilder line;
        int colCnt = 0;
        void StartLine(string str)
        {
            line = new StringBuilder(str);
            colCnt = 1;
        }
        void AppendLine(string str)
        {
            if (str == null) str = "";
            str = str.Replace("\"", "'");
            line.Append(",\"" + str + "\"");
            colCnt++;
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
            Log.Info("GetUniqueModuleList");

            StartLine("PartName");
            AppendLine("TechRequired");
            AppendLine("entryCost");
            AppendLine("cost");
            AppendLine("title");
            AppendLine("description");
            AppendLine("mass");
            AppendLine("bulkheadProfiles");
            AppendLine("CrewCapacity");

            foreach (var m in Module.modulesList)
            {
                if (m.Value.enabled)
                {
                    var mod = m.Value;
                    for (int f = 0; f < mod.module.Fields.Count; f++)
                    {
                        Field field = new Field(mod.modName, mod.moduleName, mod.module.Fields[f].name);
                        if (Field.fieldsList.TryGetValue(field.Key, out field))
                        {
                            if (field.enabled)
                            {
                                if (!moduleInfoList.ContainsKey(m.Value.moduleName))
                                {
                                    moduleInfoList.Add(m.Value.moduleName, new ModuleInfo(m.Value.moduleName, 1, colCnt));

                                }
                                else
                                {
                                    moduleInfoList[m.Value.moduleName].numFields++;
                                }
                            
                                AppendLine( m.Value.moduleName + "." + mod.module.Fields[f].name );
                            }
                        }
                    }
                }
            }
            EndLine();
            byte[] bytes = Encoding.ASCII.GetBytes(line.ToString());
            fs.Write(bytes, 0, line.Length);
            foreach (var m in moduleInfoList)
            {
                Log.Info("Module: " + m.Value.moduleName + ", startingCol: " + m.Value.startingCol + ", numFields: " + m.Value.numFields);
            }
        }

        const int MAXCOL = 1000;
        int maxUsedCol = 0;
        FileStream fs;

        void OpenFile()
        {
            if (File.Exists(outputFile))
                File.Delete(outputFile);
            fs = File.OpenWrite(outputFile);
        }

        void GetPartData(AvailablePart part)
        {
            StartLine(part.name);
            AppendLine(part.TechRequired);
            AppendLine(part.entryCost);
            AppendLine(part.cost);
            AppendLine(part.title );
            AppendLine(part.description);
            AppendLine(part.partPrefab.mass);
            AppendLine(part.bulkheadProfiles);
            AppendLine(part.partPrefab.CrewCapacity);

        }
        public void DumpData()
        {
            List<AvailablePart> loadedParts = Utils.GetPartsList();

            foreach (AvailablePart part in loadedParts)
            {
                if (part == null)
                    continue;
                string[] colData = new string[MAXCOL];

                string partModName = Utils.FindPartMod(part);
                if (partModName != "")
                {
                    if (DataDump.modList.ContainsKey(partModName))
                    {
                        for (int i = 0; i < MAXCOL; i++)
                            colData[i] = null;

                        bool partDone = false;
                        foreach (PartModule module in part.partPrefab.Modules)
                        {
                            var a = module.GetType();

                            if (module.moduleName == null)
                                continue;

                            string moduleName = Module.UsefulModuleName(module.moduleName);

                            Module mod = new Module(partModName, moduleName, a);

                            if (Module.modulesList.TryGetValue(mod.Key, out mod))
                            {
                                if (moduleInfoList.ContainsKey(mod.moduleName))
                                {
                                    var m = moduleInfoList[mod.moduleName];
                                    int cnt = 0;
                                    Log.Info("mod.module.Fields.Count: " + mod.module.Fields.Count);
                                    for (int f = 0; f < mod.module.Fields.Count; f++)
                                    {
                                        Field field = new Field(mod.modName, mod.moduleName, mod.module.Fields[f].name);
                                        if (Field.fieldsList.TryGetValue(field.Key, out field))
                                        {
                                            if (field.enabled)
                                            {
                                                Log.Info("Field: " + mod.module.Fields[f].name + ", in col: " + (m.startingCol + cnt + 1).ToString());
                                                //colData[m.startingCol + cnt] = mod.module.Fields[f].name;


                                                Field.fieldsList[field.Key].enabled = GUILayout.Toggle(Field.fieldsList[field.Key].enabled, "");

                                                var str = mod.module.Fields[f].name;
                                                string v = mod.module.Fields[f].GetStringValue(mod.module.Fields[f].host, true);


                                                // colData[m.startingCol + cnt ] += ":" + Localizer.Format(v);
                                                 colData[m.startingCol + cnt ] = Localizer.Format(v);




                                                maxUsedCol = Math.Max(maxUsedCol, m.startingCol + cnt); 
                                                cnt++;
                                                partDone = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        if (partDone)
                        {
                            Log.Info("maxUsedCol: " + maxUsedCol);
                            //StringBuilder line = new StringBuilder("\"" + part.name + "\"");
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
            }
            fs.Close();
        }

        public void Start()
        {

        }
        Rect winPos = new Rect(400, 400, 300, 100);
        public void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            winPos = ClickThruBlocker.GUILayoutWindow(567382457, winPos, GetOutputFile, "KSP DataDump Output");

        }
        string outputFile = "";
 
        public void GetOutputFile(int id)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Enter filename: ");
            outputFile = GUILayout.TextField(outputFile);
            GUILayout.EndHorizontal();
       
            GUILayout.BeginHorizontal();
            if (outputFile == "")
                GUI.enabled = false;
            if (GUILayout.Button("Proceed"))
            {
                OpenFile();
                GetUniqueModuleList();
                DumpData();

                Destroy(this);
            }
            GUI.enabled = true;
            if (GUILayout.Button("Cancel"))
            {
                Destroy(this);
            }
            GUILayout.EndHorizontal();
            GUI.DragWindow();
        }
    }
}
