using UnityEngine;
using ToolbarControl_NS;

namespace KSP_DataDump
{
    [KSPAddon(KSPAddon.Startup.MainMenu, true)]
    public class RegisterToolbar : MonoBehaviour
    {
        void Start()
        {
            ToolbarControl.RegisterMod(DataDump.MODID, DataDump.MODNAME);
        }
        void OnGUI()
        {
            if (DataDump.buttonGreenStyle == null)
            {
                DataDump.buttonGreenStyle = new GUIStyle(GUI.skin.button);
                DataDump.buttonGreenStyle.normal.textColor = Color.green; 
                DataDump.buttonGreenStyle.hover.textColor = Color.green;

                DataDump.buttonRedStyle = new GUIStyle(GUI.skin.button);
                DataDump.buttonRedStyle.normal.textColor = Color.red;
                DataDump.buttonRedStyle.hover.textColor = Color.red;

                DataDump.window = new GUIStyle(HighLogic.Skin.window);
                DataDump.window.active.background = DataDump.window.normal.background;

                DataDump.tex = DataDump.window.normal.background; //.CreateReadable();
                var pixels = DataDump.tex.GetPixels32();

                for (int i = 0; i < pixels.Length; ++i)
                    pixels[i].a = 255;

                DataDump.tex.SetPixels32(pixels); DataDump.tex.Apply();
                DataDump.window.active.background =
                    DataDump.window.focused.background =
                    DataDump.window.normal.background = DataDump.tex;

                DataDump.labelTextColor = new GUIStyle("Label");
                DataDump.labelTextColor.fontStyle = FontStyle.Bold;
                //labelTextColor.fontSize++;
                DataDump.origTextColor = DataDump.labelTextColor.normal.textColor;
                DataDump.origBackgroundColor = GUI.backgroundColor;


            }

        }
    }
}
