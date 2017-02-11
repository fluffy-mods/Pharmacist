using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Harmony;
using HugsLib;
using RimWorld;
using Verse;

namespace Pharmacist
{
    public class Pharmacist : ModBase
    {

        private static Dictionary<Pair<Population, InjurySeverity>, MedicalCareCategory> medicalCare;

        public override string ModIdentifier => "Pharmacist";

        public override void Initialize()
        {
            base.Initialize();
            var harmony = HarmonyInstance.Create("fluffy.rimworld.pharmacist");
            harmony.PatchAll(Assembly.GetExecutingAssembly());

            SetDefaults();
            GetOptions();
        }


        public void SetDefaults()
        {
            medicalCare = new Dictionary<Pair<Population, InjurySeverity>, MedicalCareCategory>();

            // colonists
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Internal), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.External), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Disease), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Colonist, InjurySeverity.Operation), MedicalCareCategory.Best);

            // guests
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Internal), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.External), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Disease), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Guest, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);

            // prisoners
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Internal), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.External), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Disease), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Prisoner, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);

            // animals
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Internal), MedicalCareCategory.NoMeds);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.External), MedicalCareCategory.HerbalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Disease), MedicalCareCategory.NormalOrWorse);
            medicalCare.Add(new Pair<Population, InjurySeverity>(Population.Animal, InjurySeverity.Operation), MedicalCareCategory.NormalOrWorse);
        }

        public void GetOptions()
        {
            // todo; implement hugslib options.
        }

        public static MedicalCareCategory TendAdvice( Pawn patient )
        {
            InjurySeverity severity = patient.GetTendSeverity();
            Population population = patient.GetPopulation();
            var key = new Pair<Population, InjurySeverity>( population, severity );
            return medicalCare[key];
        }
    }
}
