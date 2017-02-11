// Karel Kroeze
// HealthAIUtility_FindBestMedicine.cs
// 2017-02-11

using System;
using Harmony;
using RimWorld;
using Verse;
using Verse.AI;

namespace Pharmacist
{
    [HarmonyPatch(typeof( HealthAIUtility ))]
    [HarmonyPatch("FindBestMedicine")]
    public class HealthAIUtility_FindBestMedicine
    {
        public static bool Prefix( Pawn healer, Pawn patient, ref Thing __result )
        {
            // get lowest of pawn care settings & pharmacy settings
            MedicalCareCategory pharmacistAdvice = Pharmacist.TendAdvice( patient );
            MedicalCareCategory playerSetting = patient?.playerSettings?.medCare ?? MedicalCareCategory.NoMeds;
            MedicalCareCategory medCare = pharmacistAdvice < playerSetting ? pharmacistAdvice : playerSetting;

            if ( medCare <= MedicalCareCategory.NoMeds )
            {
                __result = null;
                return false;
            }
            
            __result = GenClosest.ClosestThing_Global_Reachable( 
                patient.Position, 
                patient.Map, 
                patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine), 
                PathEndMode.ClosestTouch, 
                TraverseParms.For( healer ), 
                9999f, 
                (m) => !m.IsForbidden( healer ) && medCare.AllowsMedicine( m.def ) && healer.CanReserve(m, 1),
                (m) => m.def.GetStatValueAbstract(StatDefOf.MedicalPotency) );

            return false;
        }
    }
}