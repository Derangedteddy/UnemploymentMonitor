using BepInEx.Logging;
using Game.Common;
using Game;
using HarmonyLib;

namespace UnemploymentMonitor;

[HarmonyPatch]
class Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    public static class SystemOrderPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Postfix(UpdateSystem updateSystem)
        {
            updateSystem.UpdateAt<UnderemploymentSystem>(SystemUpdatePhase.GameSimulation);
            updateSystem.UpdateAt<UnemploymentUISystem>(SystemUpdatePhase.UIUpdate);
            
        }
    }
}

