using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace Pharmacist
{
    public class Pharmacist : Mod
    {

        
        public Pharmacist( ModContentPack content ) : base( content )
        {
            var harmony = HarmonyInstance.Create("fluffy.rimworld.pharmacist");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}
