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
            foreach (var m in ActiveLists.activePropertyList)
            {
                if ((selectedModsAppliesToAll || m.Value.modname == activeMod) && m.Value.moduleName == activeModule.moduleName) // && m.Value.fieldsFromReflection != null)
                {
                    baseFieldInfo = m.Value.fields; // FromReflection;

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
                        if (s.Name[0] != '_' && (fieldSearchStr == "" || s.Name.Contains(fieldSearchStr, StringComparison.OrdinalIgnoreCase)))
                        {
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
