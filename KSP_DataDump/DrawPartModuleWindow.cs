using UnityEngine;
using ToolbarControl_NS;
using ClickThroughFix;
using KSP.UI.Screens;
using KSP.Localization;
using System.Reflection;
using System;

namespace KSP_DataDump
{
    partial class DataDump
    {
        void DrawPartModuleWindow(int id)
        {
            GUILayout.BeginHorizontal();
            moduleScrollPos = GUILayout.BeginScrollView(moduleScrollPos);
            foreach (var m in Module.modulesList)
            {
                if (/* selectedModsAppliesToAll || */ m.Value.modName == activeMod)
                {
                    if (modSearchStr == "" || m.Value.type.Name.Contains(modSearchStr, StringComparison.OrdinalIgnoreCase))
                    {
                        GUILayout.BeginHorizontal();
                       // GUI.enabled = !m.Value.enabled;
                        if (GUILayout.Button(m.Value.type.Name, m.Value.enabled ? buttonGreenStyle : GUI.skin.button))
                        {
                            if (!rememberField)
                                fieldSearchStr = "";
                            m.Value.enabled = true;
                            if (m.Value.enabled)
                            {
                                propertiesVisible = true;
                                Property.GetProperties(m.Value.modName, m.Value);

                                posFieldsDumpWindow.x = posModuleDataDumpWindow.x;
                                posFieldsDumpWindow.y = posModuleDataDumpWindow.y + posModuleDataDumpWindow.height;
                            }
                        }
                        //GUI.enabled = true;
                        if (m.Value.enabled)
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X", buttonRedStyle))
                                m.Value.enabled = false;
                        }
                        else
                            GUILayout.FlexibleSpace();
                        GUILayout.EndHorizontal();
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Search: ");

            modSearchStr = GUILayout.TextField(modSearchStr, GUILayout.Width(90));
            rememberMod = GUILayout.Toggle(rememberMod, "Remember");
            if (GUILayout.Button("Clear"))
                modSearchStr = "";
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUI.enabled = !propertiesVisible;
            if (GUILayout.Button("OK"))
            {
                moduleVisible = false;

            }

            if (GUILayout.Button("Cancel"))
                moduleVisible = false;
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.DragWindow();
        }

    }
}
