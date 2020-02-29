using System.Reflection;
using HarmonyLib;
using Verse;

namespace Pharmacist
{
    public class Pharmacist : Mod
    {
        public Pharmacist( ModContentPack content ) : base( content )
        {
#if DEBUG
            Harmony.DEBUG = true;
#endif
            var harmony = new Harmony("fluffy.pharmacist" );
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
