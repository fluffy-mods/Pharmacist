// WorkGiver_DoBill_GetMedicalCareCategory.cs
// Copyright Karel Kroeze, 2018-2018

using HarmonyLib;
using RimWorld;
using Verse;

namespace Pharmacist.Properties
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), "GetMedicalCareCategory")]
    public static class WorkGiver_DoBill_GetMedicalCareCategory
    {
        public static bool Prefix( Thing billGiver, ref MedicalCareCategory __result )
        {
            Pawn pawn = billGiver as Pawn;
            if ( pawn == null )
            {
                // because this is the fallback vanilla uses...
                __result = MedicalCareCategory.Best;
            }
            else
            {
                // assumption: bills on people === operations
                __result = PharmacistUtility.TendAdvice( pawn, InjurySeverity.Operation );
            }
            
            // always cancel execution of vanilla method.
            return false;
        }
    }
}