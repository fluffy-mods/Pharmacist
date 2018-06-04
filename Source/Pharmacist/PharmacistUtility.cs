// Karel Kroeze
// PharmacistUtility.cs
// 2017-02-11

using System.Text;
using RimWorld;
using Verse;

namespace Pharmacist
{
    public enum InjurySeverity
    {
        Minor,
        Major,
        LifeThreathening,
        Operation
    }

    public enum Population
    {
        Colonist,
        Prisoner,
        Guest,
        Animal
    }

    public static class PharmacistUtility
    {
        public static InjurySeverity GetTendSeverity( this Pawn patient )
        {
            if ( !HealthAIUtility.ShouldBeTendedNow( patient ) )
                return InjurySeverity.Minor;
            
            var hediffs = patient.health.hediffSet.hediffs;
            var ticksToDeathDueToBloodLoss = HealthUtility.TicksUntilDeathDueToBloodLoss( patient );
            
            // going to die in <6 hours, or any tendable is life threathening
            if ( ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 6 || hediffs.Any( h => h.CurStage?.lifeThreatening ?? false ) )
                return InjurySeverity.LifeThreathening;
            
            // going to die in <12 hours, or any immunity < severity and can be fatal
            if ( ticksToDeathDueToBloodLoss <= GenDate.TicksPerHour * 12 ||
                 hediffs.Any( PotentiallyLethalDisease ) )
                return InjurySeverity.Major;
            
            // otherwise
            return InjurySeverity.Minor;
        }

        private static bool PotentiallyLethalDisease( Hediff h )
        {
            if ( h.def.lethalSeverity <= 0f )
                return false;
            var compImmunizable = h.TryGetComp<HediffComp_Immunizable>();
            if ( compImmunizable == null )
                return false;
            return !compImmunizable.FullyImmune && compImmunizable.Immunity < h.Severity;
        }
        
        public static Population GetPopulation( this Pawn patient )
        {
            if ( patient.RaceProps.Animal )
                return Population.Animal;
            if (patient.IsColonist)
                return Population.Colonist;
            if ( patient.IsPrisonerOfColony )
                return Population.Prisoner;
            return Population.Guest;
        }
        
        public static MedicalCareCategory TendAdvice( Pawn patient )
        {
            InjurySeverity severity = patient.GetTendSeverity();
            return TendAdvice( patient, severity );
        }

        public static MedicalCareCategory TendAdvice( Pawn patient, InjurySeverity severity )
        {
            Population population = patient.GetPopulation();
            
            var pharmacist = PharmacistSettings.medicalCare[population][severity];
            var playerSetting = patient?.playerSettings?.medCare ?? MedicalCareCategory.Best;
            
            #if DEBUG
            Log.Message(
                "Pharmacist :: Advice" +
                $"\n\tpatient: {patient?.LabelShort}" +
                $"\n\tpopulation: {population}" +
                $"\n\tseverity: {severity}" +
                $"\n\tplayerSettings: {playerSetting}" +
                $"\n\tpharmacist: {pharmacist}" );
            #endif
            
            // return lowest
            if ( pharmacist < playerSetting )
                return pharmacist;
            return playerSetting;
        }
    }
}