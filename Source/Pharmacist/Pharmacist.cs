using System.Collections.Generic;
using System.Reflection;
using Harmony;
using RimWorld;
using Verse;

namespace Pharmacist
{
    public class Pharmacist : Mod
    {

        public static Dictionary<Pair<Population, InjurySeverity>, MedicalCareCategory> medicalCare;
        
        public Pharmacist( ModContentPack content ) : base( content )
        {
            var harmony = HarmonyInstance.Create("fluffy.rimworld.pharmacist");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            SetDefaults();
        }

        public void SetDefaults()
        {
            medicalCare = new Dictionary<Pair<Population, InjurySeverity>, MedicalCareCategory>();

            // colonists
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Minor), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Major), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.LifeThreathening), MedicalCareCategory.Best);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Operation), MedicalCareCategory.Best);

            // guests
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Minor), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Major), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.LifeThreathening), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);

            // prisoners
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Minor), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Major), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.LifeThreathening), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);

            // animals
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Minor), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Major), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.LifeThreathening), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);
        }
    }
}
