using BepInEx;
using HarmonyLib;
using MoreCompany;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;

namespace NoMoreCompanyLogo
{
    [BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
    [BepInDependency("me.swipez.melonloader.morecompany")]
    public class Plugin : BaseUnityPlugin
    {
        private const string PLUGIN_GUID = "io.daxcess.nomorecompanylogo";
        private const string PLUGIN_NAME = "NoMoreCompanyLogo";
        private const string PLUGIN_VERSION = "1.1.0";

        private void Awake()
        {
            // Plugin startup logic
            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

            Logger.LogInfo($"Plugin {PLUGIN_GUID} is loaded!");
        }
    }

    [HarmonyPatch]
    internal static class MoreCompanyPatches
    {
        [HarmonyPatch(typeof(MenuManagerLogoOverridePatch), nameof(MenuManagerLogoOverridePatch.Awake_Postfix))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> RemoveLogoPatchTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "MenuContainer/MainButtons/HeaderImage"))
                .SetOperandAndAdvance("INVALID OBJECT")
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "MenuContainer/LoadingScreen"))
                .SetOperandAndAdvance("INVALID OBJECT")
                .InstructionEnumeration();
        }

        [HarmonyPatch(typeof(MenuManagerVersionDisplayPatch), nameof(MenuManagerVersionDisplayPatch.Postfix))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> DisableVersionTextOverride(IEnumerable<CodeInstruction> instructions)
        {
            return new CodeMatcher(instructions)
                .MatchForward(false, new CodeMatch(OpCodes.Ldstr, "{0} (MC)"))
                .SetOperandAndAdvance("{0}")
                .InstructionEnumeration();
        }
    }
}
