using HarmonyLib;

namespace USLModule.Utils
{
    public class Patcher
    {
        public static void DoPatching()
        {
            var harmony = new Harmony("com.usl.patch");
            harmony.PatchAll();
        }
    }
}