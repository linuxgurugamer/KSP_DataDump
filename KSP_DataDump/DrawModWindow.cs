using UnityEngine;

namespace KSP_DataDump
{
    partial class DataDump
    {
        Vector2 modScrollPos, moduleScrollPos, propertiesScrollPos;
        void DrawModWindow(int id)
        {
            GUILayout.BeginHorizontal();
            GUI.enabled = !moduleVisible;

            modScrollPos = GUILayout.BeginScrollView(modScrollPos);
            if (GUILayout.Button(selectedModsAppliesToAll ? "Selected Modules Applies To All Parts" : "Selected Modules Only"))
                activeLists.SetActiveModuleList(!selectedModsAppliesToAll);
            if (GUILayout.Button("Part Attributes"))
            {
                partAttrVisible = !partAttrVisible;
                if (!rememberPartAttrSearchStr)
                    partAttrSearchStr = "";
                posPartAttrDataDumpWindow.x = posModDataDumpWindow.x + posModDataDumpWindow.width;
                posPartAttrDataDumpWindow.y = posModDataDumpWindow.y;
            }
            foreach (var m in ActiveLists.modList)
            {
                GUILayout.BeginHorizontal();
                //GUI.enabled = !m.Value.enabled && !rememberPartAttrSearchStr;
                if (GUILayout.Button(m.Key, m.Value.enabled ? buttonGreenStyle : GUI.skin.button))
                {
                    if (!rememberMod)
                        modSearchStr = "";
                    m.Value.enabled = true;
                    if (m.Value.enabled)
                    {
                        moduleVisible = true;

                        activeMod = m.Key;
                        //GetModuleList(true);
                        posModuleDataDumpWindow.x = posModDataDumpWindow.x + posModDataDumpWindow.width;
                        posModuleDataDumpWindow.y = posModDataDumpWindow.y;
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
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Export"))
            {
                ExportData script = gameObject.AddComponent<ExportData>();
            }
            if (GUILayout.Button("Cancel"))
            {
                modVisible = false;
                moduleVisible = false;
            }
            GUILayout.EndHorizontal();
            GUI.enabled = true;
            GUI.DragWindow();
        }

    }
}
