using System.Collections.Generic;
using UnityEngine;
using ToolbarControl_NS;
using ClickThroughFix;
using KSP.UI.Screens;
using KSP_Log;

namespace KSP_DataDump
{


    [KSPAddon(KSPAddon.Startup.EditorAny, false)]
     public partial class DataDump : MonoBehaviour
    {
        static internal ToolbarControl toolbarControl = null;

        const float WIDTH = 300;
        const float MODULES_WIDTH = 400;
        const float MIN_HEIGHT = 400;
        float MAX_HEIGHT = 800;

        internal const string MODID = "DataDump_ns";
        internal const string MODNAME = "KSP_DataDump";
        bool partAttrVisible = false;
        bool modVisible = false;
        bool moduleVisible = false;
        bool propertiesVisible = false;
        bool initted = false;
        GUIStyle labelTextColor;
        Color origTextColor;
        Color origBackgroundColor;
        private Rect posModDataDumpWindow = new Rect(300, 50, WIDTH, MIN_HEIGHT);
        private Rect posPartAttrDataDumpWindow = new Rect(300, 50, MODULES_WIDTH, MIN_HEIGHT);
        private Rect posModuleDataDumpWindow = new Rect(450, 450, MODULES_WIDTH, MIN_HEIGHT);
        private Rect posFieldsDumpWindow = new Rect(450, 450, MODULES_WIDTH, MIN_HEIGHT);

        string partAttrSearchStr = "";
        string fieldSearchStr = "";
        string modSearchStr = "";
        bool rememberField = false, rememberMod = false;
        bool rememberPartAttrSearchStr = false;
        internal static bool selectedModsAppliesToAll = false;
        public static Log Log;

        //public static bool volumeInfo = false;


        void Awake()
        {
#if DEBUG
            Log = new Log("DataDump", Log.LEVEL.INFO);
#else
            Log = new Log("DataDump", Log.LEVEL.ERROR);
#endif
        }
        void Start()
        {
            Log.Info("Start");
            AddToolbarButton();
            Property.GetPartProperties();
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
        public static Texture2D tex = null;
        public static GUIStyle window;
        public static GUIStyle buttonGreenStyle, buttonRedStyle;
        void OnGUI()
        {
            // GUI.color = Color.grey;
            if (tex == null)
            {
                window = new GUIStyle(HighLogic.Skin.window);
                window.active.background = window.normal.background;

                tex = window.normal.background; //.CreateReadable();
                var pixels = tex.GetPixels32();

                for (int i = 0; i < pixels.Length; ++i)
                    pixels[i].a = 255;

                tex.SetPixels32(pixels); tex.Apply();
                window.active.background =
                    window.focused.background =
                    window.normal.background = tex;
            }
            //GUI.skin = HighLogic.Skin;
            if (!initted)
            {
                labelTextColor = new GUIStyle("Label");
                labelTextColor.fontStyle = FontStyle.Bold;
                //labelTextColor.fontSize++;
                origTextColor = labelTextColor.normal.textColor;
                origBackgroundColor = GUI.backgroundColor;


                initted = true;
            }
            if (partAttrVisible)
            {
                posPartAttrDataDumpWindow = ClickThruBlocker.GUILayoutWindow(56783456, posPartAttrDataDumpWindow, DrawPartAttributes, "KSP Part Attribute Selection", window,
                   GUILayout.Width(MODULES_WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

            }
            else
            {
                if (modVisible)
                {
                    posModDataDumpWindow = ClickThruBlocker.GUILayoutWindow(56783457, posModDataDumpWindow, DrawModWindow, "KSP DataDump Mod Selection", window,
                       GUILayout.Width(WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

                }
                if (moduleVisible)
                {
                    posModuleDataDumpWindow = ClickThruBlocker.GUILayoutWindow(56783458, posModuleDataDumpWindow, DrawPartModuleWindow, "KSP DataDump Module Selection", window, GUILayout.Width(MODULES_WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

                }
                if (propertiesVisible)
                {
                    posFieldsDumpWindow = ClickThruBlocker.GUILayoutWindow(56783459, posFieldsDumpWindow, DrawModAttributesWindow, "KSP DataDump Attribute Selection", window, GUILayout.Width(MODULES_WIDTH), GUILayout.MinHeight(MIN_HEIGHT), GUILayout.MaxHeight(MAX_HEIGHT));

                }
            }
        }


        bool GetFieldDescr(string f, out string v)
        {
            bool goodToGo = true;
            switch (f)
            {
                case "System.Single":
                    v = "float";
                    break;
                case "System.Double":
                    v = "double";
                    break;
                case "System.Int32":
                    v = "int";
                    break;
                case "System.UInt32":
                    v = "uint";
                    break;

                case "System.String":
                    v = "string";
                    break;
                case "System.Boolean":
                    v = "boolean";
                    break;
                case "UnityEngine.Vector3":
                    v = "Vector3";
                    break;
                default:
                    goodToGo = false;
                    v = "bad";
                    break;

            }
            return goodToGo;
        }

        enum Value { False, True, Toggle };
        void SetFieldValue(List<FldInfo> baseFieldList, Value v)
        {
            foreach (var s in baseFieldList)
            {
                Field existingField = null;
                Field field = new Field(activeMod, activeModule.moduleName, s.Name);
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

