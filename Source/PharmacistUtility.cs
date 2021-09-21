// Karel Kroeze
// PharmacistUtility.cs
// 2017-02-11

using System.Linq;
using RimWorld;
using Verse;

namespace Pharmacist {
    public enum InjurySeverity {
        Minor,
        Major,
        LifeThreathening,
        Operation
    }

    public enum Population {
        Colonist,
        Prisoner,
        Guest,
        Animal
    }

    public static class PharmacistUtility {
        public static InjurySeverity GetTendSeverity(this Pawn patient) {
            if (!HealthAIUtility.ShouldBeTendedNowByPlayer(patient)) //    .ShouldBeTendedNow( patient ) )
{
                return InjurySeverity.Minor;
            }

            System.Collections.Generic.List<Hediff> hediffs = patient.health.hediffSet.hediffs;
            int ticksToDeathDueToBloodLoss = HealthUtility.TicksUntilDeathDueToBloodLoss( patient );

            // going to die in <6 hours, or any tendable is life threathening
            if (ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 6 ||
                 hediffs.Any(h => h.CurStage?.lifeThreatening ?? false) ||
                 hediffs.Any(NearLethalDisease) || 
                 hediffs.Any(h => IsBloodRot(h) && h.Severity > PharmacistSettings.medicalCare.DiseaseThreshold)
                 ) {
                return InjurySeverity.LifeThreathening;
            }

            // going to die in <12 hours, or any immunity < severity and can be fatal, or death by a thousand cuts imminent
            if (ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 12 ||
                 hediffs.Any(PotentiallyLethalDisease) ||
                 DeathByAThousandCuts(patient) ||
                 hediffs.Any(IsBloodRot)
                 ) {
                return InjurySeverity.Major;
            }

            // otherwise
            return InjurySeverity.Minor;
        }

        private static bool PotentiallyLethalDisease(Hediff h) {
            if (!h.TendableNow()) {
                return false;
            }

            if (h.def.lethalSeverity <= 0f) {
                return false;
            }

            HediffComp_Immunizable compImmunizable = h.TryGetComp<HediffComp_Immunizable>();
            return compImmunizable != null;
        }

        private static bool NearLethalDisease(Hediff h) {
            HediffComp_Immunizable compImmunizable = h.TryGetComp<HediffComp_Immunizable>();
            return PotentiallyLethalDisease(h) &&
                   !compImmunizable.FullyImmune &&
                   h.Severity > PharmacistSettings.medicalCare.DiseaseThreshold &&
                   compImmunizable.Immunity < PharmacistSettings.medicalCare.DiseaseMargin + h.Severity;
        }

        private static bool IsBloodRot(Hediff h)
        {
            return h.def.defName == "BloodRot";
        }

        private static bool DeathByAThousandCuts(Pawn patient) {
            // number of bleeding wounds > threshold
            return patient.health.hediffSet.hediffs.Count(hediff => hediff.Bleeding) >
                   PharmacistSettings.medicalCare.MinorWoundsThreshold;
        }

        public static Population GetPopulation(this Pawn patient) {
            if (patient.RaceProps.Animal) {
                return Population.Animal;
            }

            if (patient.IsColonist) {
                return Population.Colonist;
            }

            if (patient.IsPrisonerOfColony) {
                return Population.Prisoner;
            }

            return Population.Guest;
        }

        public static MedicalCareCategory TendAdvice(Pawn patient) {
            InjurySeverity severity = patient.GetTendSeverity();
            return TendAdvice(patient, severity);
        }

        public static MedicalCareCategory TendAdvice(Pawn patient, InjurySeverity severity) {
            Population population = patient.GetPopulation();

            MedicalCareCategory pharmacist = PharmacistSettings.medicalCare[population][severity];
            MedicalCareCategory playerSetting = patient?.playerSettings?.medCare ?? MedicalCareCategory.Best;

#if DEBUG
            Log.Message(
                "Pharmacist :: Advice" +
                $"\tpatient: {patient?.LabelShort}" +
                $"\tpopulation: {population}" +
                $"\tbloodRot: {patient.health.hediffSet.hediffs.Exists(IsBloodRot)}" +
                $"\tseverity: {severity}" +
                $"\tplayerSettings: {playerSetting}" +
                $"\tpharmacist: {pharmacist}");
#endif

            // return lowest
            if (pharmacist < playerSetting) {
                return pharmacist;
            }

            return playerSetting;
        }
    }
}
