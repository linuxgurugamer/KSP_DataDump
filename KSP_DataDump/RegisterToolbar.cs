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
    }
}
