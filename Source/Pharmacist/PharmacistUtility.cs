// Karel Kroeze
// PharmacistUtility.cs
// 2017-02-11

using RimWorld;
using Verse;

namespace Pharmacist
{
    public enum InjurySeverity
    {
        Internal,
        External,
        Disease,
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
            var hediffs = patient.health.hediffSet.hediffs;
            var severity = InjurySeverity.Internal;

            for (int i = 0; i < hediffs.Count; i++)
            {
                Hediff_Injury injury = hediffs[i] as Hediff_Injury;
                if (injury != null && injury.TendableNow )
                {
                    if ( injury.BleedRate > 1e-4f && severity <= InjurySeverity.External )
                    {
                        // bleeds - let's assume that means it can cause infections.
                        severity = InjurySeverity.External;
                    }
                    if ( injury.def.PossibleToDevelopImmunity() )
                    {
                        // can be immune - let's assume that means it's a disease/infection
                        severity = InjurySeverity.Disease;
                        break;
                    }
                }
            }

            // nothing bleeds - let's assume that means it's fairly harmless.
            return severity;
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
    }
}