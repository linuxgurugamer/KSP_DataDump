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
            GUI.enabled = !propertiesVisible;

            GUILayout.BeginHorizontal();
            moduleScrollPos = GUILayout.BeginScrollView(moduleScrollPos);
            foreach (var m in ActiveLists.AllModulesList)
            {
                if ( m.Value.modName == activeMod)
                {
                    if (modSearchStr == "" || m.Value.type.Name.Contains(modSearchStr, StringComparison.OrdinalIgnoreCase))
                    {
                        Module activeModule;
                        if (!ActiveLists.activeModuleList.TryGetValue(m.Value.ActiveKey, out activeModule))
                            Log.Error("active module not found");

                        GUILayout.BeginHorizontal();
                        // GUI.enabled = !m.Value.enabled;
                        if (GUILayout.Button(activeModule.type.Name, activeModule.enabled ? buttonGreenStyle : GUI.skin.button))
                        {
                            if (!rememberField)
                                fieldSearchStr = "";
                            activeModule.enabled = true;
                            if (activeModule.enabled)
                            {
                                propertiesVisible = true;
                                Property.GetProperties(activeModule.modName, activeModule);

                                posFieldsDumpWindow.x = posModuleDataDumpWindow.x;
                                posFieldsDumpWindow.y = posModuleDataDumpWindow.y + posModuleDataDumpWindow.height;
                            }
                        }
                        //GUI.enabled = true;
                        if (activeModule.enabled)
                        {
                            GUILayout.FlexibleSpace();
                            if (GUILayout.Button("X", buttonRedStyle))
                                activeModule.enabled = false;
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
