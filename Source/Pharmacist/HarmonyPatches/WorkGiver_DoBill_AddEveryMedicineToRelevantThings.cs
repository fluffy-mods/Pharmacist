// WorkGiver_DoBill_GetMedicalCareCategory.cs
// Copyright Karel Kroeze, 2018-2018

using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Reflection;
using Verse;
using Verse.AI;

namespace Pharmacist.Properties
{
    [HarmonyPatch(typeof(WorkGiver_DoBill), "AddEveryMedicineToRelevantThings")]
    public static class WorkGiver_DoBill_AddEveryMedicineToRelevantThings
    {
        public static bool Prefix(Pawn pawn, Thing billGiver, List<Thing> relevantThings, Predicate<Thing> baseValidator, Map map)
        {
            Pawn patient = billGiver as Pawn;
            //MedicalCareCategory medicalCareCategory = GetMedicalCareCategory(billGiver);
            MedicalCareCategory medicalCareCategory = PharmacistUtility.TendAdvice(patient, InjurySeverity.Operation);
            List<Thing> list = map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine);
            List<Thing> tmpMedicine = (List<Thing>)(typeof(WorkGiver_DoBill).GetField("tmpMedicine", BindingFlags.NonPublic | BindingFlags.Static).GetValue(null));
            tmpMedicine.Clear();
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (medicalCareCategory.AllowsMedicine(thing.def) && baseValidator(thing) && pawn.CanReach(thing, PathEndMode.OnCell, Danger.Deadly))
                {
                    tmpMedicine.Add(thing);
                }
            }
            float inverter = 1f;
            if (patient.BillStack.FirstShouldDoNow.recipe.defName == "Anesthetize") { inverter = -1f; }
            tmpMedicine.SortBy((Thing x) => 0f - (x.GetStatValue(StatDefOf.MedicalPotency) * inverter), (Thing x) => x.Position.DistanceToSquared(billGiver.Position));
            relevantThings.AddRange(tmpMedicine);
            tmpMedicine.Clear();
            return false;
        }
    }
}
