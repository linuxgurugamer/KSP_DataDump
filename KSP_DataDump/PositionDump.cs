using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Experimental.PlayerLoop;
using static KSP_DataDump.DataDump;
#if false
namespace KSP_DataDump
{
    public class KSP_DataDumpModule : PartModule
    {
        [KSPEvent(guiName = "Dump 3dWorldPos", guiActive = true)]
        void DumpAllWorldPos()
        {
            for (int i = 0; i < FlightGlobals.Vessels.Count; i++)
            {
                Log.Info(" vessel: " + FlightGlobals.Vessels[i].GetDisplayName() + ", GetWorldPos3D: " + FlightGlobals.Vessels[i].GetWorldPos3D());
                if (FlightGlobals.Vessels[i].rootPart == null)
                    Log.Info("FlightGlobals.Vessels[i].rootPart is null");
                else
                {
                    if (FlightGlobals.Vessels[i].rootPart.transform == null)
                        Log.Info("FlightGlobals.Vessels[i].rootPart.transform is null");
                    Log.Info("rootPart.transform.position: " + FlightGlobals.Vessels[i].rootPart.transform.position +
                    ", localPosition: " + FlightGlobals.Vessels[i].rootPart.transform.localPosition
                    );
                }
            }
        }

    }
}
#endif