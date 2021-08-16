// Karel Kroeze
// HealthAIUtility_FindBestMedicine.cs
// 2017-02-11

using System.Linq;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace Pharmacist {
    [HarmonyPatch(typeof(HealthAIUtility), nameof(HealthAIUtility.FindBestMedicine))]
    public class HealthAIUtility_FindBestMedicine {
        public static bool Prefix(Pawn healer, Pawn patient, bool onlyUseInventory, ref Thing __result) {

            // get lowest of pawn care settings & pharmacy settings
            MedicalCareCategory pharmacistAdvice = PharmacistUtility.TendAdvice( patient );
            if (pharmacistAdvice <= MedicalCareCategory.NoMeds) {
                __result = null;
                return false;
            }

            // check count required
            int countRequired = Medicine.GetMedicineCountToFullyHeal(patient);
            if (countRequired <= 0) {
                __result = null;
                return false;
            }

            float potencyGetter(Thing t) {
                return t.def.GetStatValueAbstract(StatDefOf.MedicalPotency);
            }
            bool allowedPredicate(Thing m) {
                return !m.IsForbidden(healer)
                    && m.def.IsMedicine
                    && pharmacistAdvice.AllowsMedicine(m.def)
                    && healer.CanReserve(m, 10, 1);
            }

            // check pockets first
            // note: vanilla actually selects the medicine with the _lowest_ potency,
            // I have to assume that that is not intentional.
            //
            // thanks to KennethSammael for adding this check in the unofficial 1.3 update,
            // this code is adapted from his changes.
            var allowedMedicine = healer.inventory.innerContainer.Where(allowedPredicate);
            if (allowedMedicine.Any()) {
                __result = allowedMedicine.MaxBy(potencyGetter);

            }
            if (__result is not null || onlyUseInventory) {
                return false;
            }

            // search for best meds
            __result = GenClosest.ClosestThing_Global_Reachable(
                patient.Position,
                patient.Map,
                patient.Map.listerThings.ThingsInGroup(ThingRequestGroup.Medicine),
                PathEndMode.ClosestTouch,
                TraverseParms.For(healer),
                9999f,
                allowedPredicate,
                potencyGetter);
            return false;
        }
    }
}
