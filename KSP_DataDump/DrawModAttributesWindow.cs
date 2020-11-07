using UnityEngine;
using KSP.Localization;
using System;
using System.Collections.Generic;

namespace KSP_DataDump
{
    partial class DataDump
    {
        void DrawModAttributesWindow(int d)
        {
          
            GUILayout.BeginHorizontal();
           List<FldInfo> baseFieldInfo = null;

            propertiesScrollPos = GUILayout.BeginScrollView(propertiesScrollPos);
            foreach (var m in Property.propertyList)
            {
                if (m.Value.modname == activeMod && m.Value.moduleName == activeModule.moduleName) // && m.Value.fieldsFromReflection != null)
                {
                    baseFieldInfo = m.Value.fields; // FromReflection;

                    //Log.Info("m.Value.moduleName: " + m.Value.moduleName + ", Field: " + str + ", fields.Cnt: " + m.Value.fields.Count);
                    foreach (var s in m.Value.fields ) //FromReflection)
                    {
                        Field existingField = null;
                        Field field = new Field(activeMod, activeModule.moduleName, s.Name);
                        if (!Field.fieldsList.TryGetValue(field.Key, out existingField))
                        {
                            Field.fieldsList.Add(field.Key, field);
                        }
                        else
                            field = existingField;
                        if (s.Name[0] != '_' && (fieldSearchStr == "" || s.Name.Contains(fieldSearchStr, StringComparison.OrdinalIgnoreCase)))
                        {
                            Log.Info("Name: " + s.Name);

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

            fieldSearchStr = GUILayout.TextField(fieldSearchStr, GUILayout.Width(90));
            rememberField = GUILayout.Toggle(rememberField, "Remember");
            if (GUILayout.Button("Clear"))
                fieldSearchStr = "";
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                propertiesVisible = false;
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
