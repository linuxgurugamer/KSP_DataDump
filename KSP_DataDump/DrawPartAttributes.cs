using System.Collections.Generic;
using UnityEngine;
using KSP.Localization;
using System;

namespace KSP_DataDump
{
    partial class DataDump
    {
        public enum PartAttrEnum { first, DimensionsInfo, minimum_drag, maximum_drag, angularDrag, crashTolerance, breakingForce, breakingtorque, maxTemp, mass, category, last };
        public static readonly string[] PartAttrStr = { "Dimensions info", "MinDrag", "MaxDrag", "AngularDrag", "CrashTolerance", "BreakingForce", "BreakingTorque", "MaxTemp", "Mass", "Category", };
        public static Property[] partAttrs = new Property[PartAttrStr.Length];
        void DrawPartAttributes(int d)
        {

            GUILayout.BeginHorizontal();
            List<FldInfo> baseFieldInfo = null;

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

                if (partAttrs[(int)partAttr - 1].enabled)
                {
                    if (GUILayout.Button("X", buttonRedStyle))
                        partAttrs[(int)partAttr - 1].enabled = false;
                }
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            GUILayout.Label("=====================================");
            GUILayout.EndHorizontal();

            foreach (var m in Property.propertyList)
            {
                if (m.Value.modname == "PART" && m.Value.fields /* FromReflection */ != null)
                {
                    baseFieldInfo = m.Value.fields; //  FromReflection;

                    //Log.Info("m.Value.moduleName: " + m.Value.moduleName + ", Field: " + str + ", fields.Cnt: " + m.Value.fields.Count);
                    foreach (var s in m.Value.fields) //FromReflection)
                    {
                        Field existingField = null;
                        Field field = new Field(activeMod, activeModule.moduleName, s.Name);
                        if (!Field.fieldsList.TryGetValue(field.Key, out existingField))
                        {
                            Field.fieldsList.Add(field.Key, field);
                        }
                        else
                            field = existingField;
                        if (!s.Name.Contains("cacheAutoLOC") &&  s.Name[0] != '_' && 
                            (partAttrSearchStr == "" || s.Name.Contains(partAttrSearchStr, StringComparison.OrdinalIgnoreCase)))
                        {
                            if (s.Name == "name")
                                Field.fieldsList[field.Key].enabled = true;

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
                                    if (GUILayout.Button(str + " : " + Localizer.Format(v) + " : " + data, Field.fieldsList[field.Key].enabled ? buttonGreenStyle : GUI.skin.button))
                                    {
                                        Field.fieldsList[field.Key].enabled = !Field.fieldsList[field.Key].enabled;
                                        Log.Info("field.Key: " + field.Key + ": " + Field.fieldsList[field.Key].enabled);
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
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                partAttrVisible = false;
            }

            if (GUILayout.Button("Select All"))
            {
                SetFieldValue(baseFieldInfo, Value.True);
            }
            if (GUILayout.Button("Select None"))
            {
                SetFieldValue(baseFieldInfo, Value.False);

            }
            if (GUILayout.Button("Toggle all"))
            {
                SetFieldValue(baseFieldInfo, Value.Toggle);

            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

    }
}
