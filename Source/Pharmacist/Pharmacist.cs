using System.Reflection;
using Harmony;
using Verse;

namespace Pharmacist
{
    public class Pharmacist : Mod
    {
        public Pharmacist( ModContentPack content ) : base( content )
        {
            HarmonyInstance.DEBUG = true;
            var harmony = HarmonyInstance.Create("fluffy.rimworld.pharmacist");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
