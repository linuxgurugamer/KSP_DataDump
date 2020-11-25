using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;
using System;

namespace KSP_DataDump
{
    partial class DataDump
    {
        public enum PartAttrEnum { first, TechRequired, entryCost, cost , category, DimensionsInfo, minimum_drag, maximum_drag, angularDrag, crashTolerance, breakingForce, breakingtorque, maxTemp, mass, Resources, last };
        public static readonly string[] PartAttrStr = { "TechRequired", "entryCost", "cost", "Category", "Dimensions info", "MinDrag", "MaxDrag", "AngularDrag", "CrashTolerance", "BreakingForce", "BreakingTorque", "MaxTemp", "Mass", "Resources"};
        public static Property[] partAttrs = new Property[PartAttrStr.Length];
        public static int maxResources = 3;
        void DrawPartAttributes(int d)
        {

            GUILayout.BeginHorizontal();
           // List<FldInfo> baseFieldInfo = null;

            propertiesScrollPos = GUILayout.BeginScrollView(propertiesScrollPos);
            for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
            {
                if (partAttrs[(int)partAttr - 1] == null)
                    partAttrs[(int)partAttr - 1] = new Property("PART","PART");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button(PartAttrStr[(int)partAttr - 1], partAttrs[(int)partAttr - 1].enabled ? buttonGreenStyle : GUI.skin.button))
                {
                    partAttrs[(int)partAttr - 1].enabled = true;
                }      
                GUILayout.FlexibleSpace();

                if (partAttr == PartAttrEnum.Resources)
                {
                    if (GUILayout.Button("<", GUILayout.Width(20)))
                    {
                        maxResources = Math.Max(1, maxResources - 1);
                    }
                    GUILayout.Label(maxResources.ToString());
                    if (GUILayout.Button(">", GUILayout.Width(20)))
                    {
                        maxResources++;
                    }
                }
                if (partAttrs[(int)partAttr - 1].enabled)
                {
                    if (GUILayout.Button("X", buttonRedStyle))
                        partAttrs[(int)partAttr - 1].enabled = false;
                }
                GUILayout.EndHorizontal();
            }

#if false
            GUILayout.BeginHorizontal();
            GUILayout.Label("=====================================");
            GUILayout.EndHorizontal();



            foreach (var m in ActiveLists.activePropertyList)
            {
                if (m.Value.modname == "PART" && m.Value.fields /* FromReflection */ != null)
                {
                    baseFieldInfo = m.Value.fields; //  FromReflection;

                    foreach (var s in m.Value.fields) //FromReflection)
                    {
                        Field existingField = null;
                        Field field = new Field(activeMod, activeModule.moduleName, s.Name);
                        if (!ActiveLists.activeFieldsList.TryGetValue(field.ActiveKey, out existingField))
                        {
                            ActiveLists.activeFieldsList.Add(field.ActiveKey, field);
                        }
                        else
                            field = existingField;
                        if (!s.Name.Contains("cacheAutoLOC") &&  s.Name[0] != '_' && 
                            (partAttrSearchStr == "" || s.Name.Contains(partAttrSearchStr, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (s.Name == "name")
                                ActiveLists.activeFieldsList[field.ActiveKey].enabled = true;

                            var str = s.Name;
                            string v = "";
                            string data = "";


                            if (!s.Name.Contains("Curve"))
                            {
                                var f = s.FieldType.ToString();
                                string t = "System.Collections.Generic";
                                if (f.StartsWith(t))
                                    f = f.Substring(t.Length + 1);

                                v = f;
                                bool goodToGo = true;

                                goodToGo = GetFieldDescr(f, out v);
                                if (goodToGo)
                                {
                                    GUILayout.BeginHorizontal();
                                    //GUILayout.Toggle(Field.fieldsList[field.Key].enabled, "");
                                    if (GUILayout.Button(str + " : " + Localizer.Format(v) + " : " + data, ActiveLists.activeFieldsList[field.ActiveKey].enabled ? buttonGreenStyle : GUI.skin.button))
                                    {
                                        ActiveLists.activeFieldsList[field.ActiveKey].enabled = !ActiveLists.activeFieldsList[field.ActiveKey].enabled;
                                    }
                                    GUILayout.FlexibleSpace();

                                    GUILayout.EndHorizontal();
                                }
                            }

                        }
                    }
                }
            }
            GUILayout.EndHorizontal();
            GUILayout.EndScrollView();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Search: ");

            partAttrSearchStr = GUILayout.TextField(partAttrSearchStr, GUILayout.Width(90));
            rememberPartAttrSearchStr = GUILayout.Toggle(rememberPartAttrSearchStr, "Remember");
            if (GUILayout.Button("Clear"))
                partAttrSearchStr = "";
            GUILayout.EndHorizontal();
#endif
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                partAttrVisible = false;
            }

            if (GUILayout.Button("Select All"))
            {
                activeMod = "PART";
                activeModule.moduleName = "PART";
                for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
                    partAttrs[(int)partAttr - 1].enabled = true;
                 //   SetFieldValue(baseFieldInfo, Value.True);
            }
            if (GUILayout.Button("Select None"))
            {
                activeMod = "PART";
                activeModule.moduleName = "PART";
                for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
                    partAttrs[(int)partAttr - 1].enabled = false;
                //SetFieldValue(baseFieldInfo, Value.False);

            }
            if (GUILayout.Button("Toggle all"))
            {
                activeMod = "PART";
                activeModule.moduleName = "PART";
                for (var partAttr = PartAttrEnum.first + 1; partAttr < PartAttrEnum.last; partAttr++)
                    partAttrs[(int)partAttr - 1].enabled = !partAttrs[(int)partAttr - 1].enabled;

                //SetFieldValue(baseFieldInfo, Value.Toggle);

            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

    }
}
