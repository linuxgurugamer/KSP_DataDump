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

            }

        }
    }
}
