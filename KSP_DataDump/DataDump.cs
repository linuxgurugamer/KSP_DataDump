using System.Collections.Generic;
using UnityEngine;
using ToolbarControl_NS;
using ClickThroughFix;
using KSP.UI.Screens;
using KSP.Localization;
using System.CodeDom;

namespace KSP_DataDump
{
    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
    public class DataDump : MonoBehaviour
    {
        static internal ToolbarControl toolbarControl = null;

        const float WIDTH = 300;
        const float MODULES_WIDTH = 400;
        const float MIN_HEIGHT = 400;
        float MAX_HEIGHT = 800;

        internal const string MODID = "DataDump_ns";
        internal const string MODNAME = "KSP_DataDump";
        bool modVisible = false;
        bool moduleVisible = false;
        bool propertiesVisible = false;
        bool initted = false;
        GUIStyle labelTextColor;
        Color origTextColor;
        Color origBackgroundColor;
        private Rect posModDataDumpWindow = new Rect(300, 50, WIDTH, MIN_HEIGHT);
        private Rect posModuleDataDumpWindow = new Rect(450, 450, MODULES_WIDTH, MIN_HEIGHT);
        private Rect posFieldsDumpWindow = new Rect(450, 450, MODULES_WIDTH, MIN_HEIGHT);

        void Awake()
        {
            Log.Info("Awake");

        }

        void Start()
        {
            Log.Info("Start");
            AddToolbarButton();
        }

        void AddToolbarButton()
        {
            if (toolbarControl == null)
            {
                toolbarControl = gameObject.AddComponent<ToolbarControl>();
                toolbarControl.AddToAllToolbars(GUIButtonToggle, GUIButtonToggle,
                    ApplicationLauncher.AppScenes.SPH |
                    ApplicationLauncher.AppScenes.VAB |
                    ApplicationLauncher.AppScenes.SPACECENTER,
                    MODID,
                    "dataDumpButton",
                    "KSP_DataDump/PluginData/DataDump-38",
                    "KSP_DataDump/PluginData/DataDump-24",
                    MODNAME
                );
            }
        }

        void GUIButtonToggle()
        {
            modVisible = !modVisible;
            if (modVisible)
                Module.GetModuleList();
        }

        void OnGUI()
        {
            GUI.skin = HighLogic.Skin;
            if (!initted)
            {
                labelTextColor = new GUIStyle("Label");
                labelTextColor.fontStyle = FontStyle.Bold;
                //labelTextColor.fontSize++;
                origTextColor = labelTextColor.normal.textColor;
                origBackgroundColor = GUI.backgroundColor;


                initted = true;
            }
            if (modVisible)
            {
                posModDataDumpWindow = ClickThruBlocker.GUILayoutWindow(56783457, posModDataDumpWindow, DrawModList, "KSP DataDump Mod Selection",
                   GUILayout.Width(WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

            }
            if (moduleVisible)
            {
                posModuleDataDumpWindow = ClickThruBlocker.GUILayoutWindow(56783458, posModuleDataDumpWindow, DrawPartModuleList, "KSP DataDump Module Selection", GUILayout.Width(MODULES_WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

            }
            if (propertiesVisible)
            {
                posFieldsDumpWindow = ClickThruBlocker.GUILayoutWindow(56783459, posFieldsDumpWindow, DrawFieldsList, "KSP DataDump Field Selection", GUILayout.Width(MODULES_WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

            }
        }
        Vector2 modScrollPos, moduleScrollPos, propertiesScrollPos;
        void DrawModList(int id)
        {
            GUILayout.BeginHorizontal();
            modScrollPos = GUILayout.BeginScrollView(modScrollPos);
            GUI.enabled = !moduleVisible;
            foreach (var m in modList)
            {
                GUILayout.BeginHorizontal();
                /*m.Value.enabled = */ GUILayout.Toggle(m.Value.enabled, "");
                //GUI.enabled = m.Value.enabled & !moduleVisible; ;
                if (GUILayout.Button(m.Key))
                {
                    m.Value.enabled = !m.Value.enabled;
                    if (m.Value.enabled)
                    {
                        moduleVisible = true;

                        activeMod = m.Key;
                        //GetModuleList(true);
                        posModuleDataDumpWindow.x = posModDataDumpWindow.x + posModDataDumpWindow.width;
                        posModuleDataDumpWindow.y = posModDataDumpWindow.y;
                    }
                }
                GUI.enabled = true;
                GUILayout.EndHorizontal();
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUI.enabled = !moduleVisible;

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

        void DrawPartModuleList(int id)
        {
            GUILayout.BeginHorizontal();
            moduleScrollPos = GUILayout.BeginScrollView(moduleScrollPos);
            foreach (var m in Module.modulesList)
            {
                if (m.Value.modName == activeMod)
                {
                    GUILayout.BeginHorizontal();
                    /*m.Value.enabled = */ GUILayout.Toggle(m.Value.enabled, "");
                   //GUI.enabled = m.Value.enabled && !propertiesVisible;
                    if (GUILayout.Button(m.Value.type.Name, GUILayout.Width(200)))
                    {
                        m.Value.enabled = !m.Value.enabled;
                        if (m.Value.enabled)
                        {
                            propertiesVisible = true;
                            Property.GetProperties(m.Value.modName, m.Value);

                            posFieldsDumpWindow.x = posModuleDataDumpWindow.x;
                            posFieldsDumpWindow.y = posModuleDataDumpWindow.y + posModuleDataDumpWindow.height;
                        }
                    }
                    GUI.enabled = true;
                    GUILayout.FlexibleSpace();
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();
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

        void DrawFieldsList(int d)
        {
            GUILayout.BeginHorizontal();
            BaseFieldList baseFieldList = null;
            propertiesScrollPos = GUILayout.BeginScrollView(propertiesScrollPos);
            foreach (var m in Property.propertyList)
            {
                if (m.Value.modname == activeMod && m.Value.moduleName == activeModule.moduleName && m.Value.fields != null)
                {
                    baseFieldList = m.Value.fields;
                    string str = m.Value.name;

                    if (m.Value.fields != null)
                    {

                        foreach (var s in m.Value.fields)
                        {
                            Field existingField = null;
                            Field field = new Field(activeMod, activeModule.moduleName, s.name);
                            if (!Field.fieldsList.TryGetValue(field.Key, out existingField))
                            {
                                Field.fieldsList.Add(field.Key, field);
                            }
                            else
                                field = existingField;

                            GUILayout.BeginHorizontal();
                            /* Field.fieldsList[field.Key].enabled = */GUILayout.Toggle(Field.fieldsList[field.Key].enabled, "");

                            str = s.name;
                            string v = s.GetStringValue(s.host, true);
                            //GUILayout.Label(str + " : " + Localizer.Format(v));
                             if (GUILayout.Button(str + " : " + Localizer.Format(v)))
                                Field.fieldsList[field.Key].enabled = !Field.fieldsList[field.Key].enabled;
                           GUILayout.EndHorizontal();

                        }
                    }
                }
            }
            GUILayout.EndScrollView();
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("OK"))
            {
                propertiesVisible = false;
            }

            if (GUILayout.Button("Select All"))
            {
                SetFieldValue(baseFieldList, Value.True);
            }
            if (GUILayout.Button("Select None"))
            {
                SetFieldValue(baseFieldList, Value.False);

            }
            if (GUILayout.Button("Toggle all"))
            {
                SetFieldValue(baseFieldList, Value.Toggle);

            }
            GUILayout.EndHorizontal();

            GUI.DragWindow();
        }

        enum Value { False, True, Toggle };
        void SetFieldValue(BaseFieldList baseFieldList, Value v)
        {
            foreach (var s in baseFieldList)
            {
                Field existingField = null;
                Field field = new Field(activeMod, activeModule.moduleName, s.name);
                if (!Field.fieldsList.TryGetValue(field.Key, out existingField))
                {
                    Log.Error("Impossible error 1");
                }
                else
                    field = existingField;
                switch (v)
                {
                    case Value.False:
                        Field.fieldsList[field.Key].enabled = false;
                        break;
                    case Value.True:
                        Field.fieldsList[field.Key].enabled = true;
                        break;
                    case Value.Toggle:
                        Field.fieldsList[field.Key].enabled = !Field.fieldsList[field.Key].enabled;
                        break;

                }
            }

        }

        static public string activeMod;
        static public Module activeModule;


        public class DataValue
        {
            public bool enabled;
            public DataValue(bool e)
            {
                enabled = e;
            }
        }

        public static Dictionary<string, DataValue> modList = new Dictionary<string, DataValue>();






    }
}

