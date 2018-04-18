// Settings.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace Pharmacist
{
    public class PharmacistSettings: WorldComponent
    {
        public static MedicalCare medicalCare;
        
        public PharmacistSettings( World world ) : base( world )
        {
            SetDefaults();
        }

        public class MedicalCare: IExposable
        {
            private Dictionary<Population, PopulationCare> _populationCare = new Dictionary<Population, PopulationCare>();
            public PopulationCare this[ Population index ]
            {
                get
                {
                    PopulationCare populationCare;
                    if ( !_populationCare.TryGetValue( index, out populationCare ) )
                    {
                        Log.Warning( $"Medical Care for {index} not initialized, using Best." );
                        populationCare = PopulationCare.Default;
                        _populationCare[index] = populationCare;
                    }
                    return populationCare;
                }
                internal set { _populationCare[index] = value; }
            }

            public void ExposeData()
            {
                Scribe_Collections.Look( ref _populationCare, "Populations", LookMode.Value, LookMode.Deep );
            }
        }
        
        public class PopulationCare: IExposable
        {
            private Dictionary<InjurySeverity, MedicalCareCategory> _populationCare;
                
            public PopulationCare( MedicalCareCategory minor, MedicalCareCategory major,
                MedicalCareCategory lifethreatening, MedicalCareCategory operation )
            {
                _populationCare = new Dictionary<InjurySeverity, MedicalCareCategory>();
                _populationCare.Add( InjurySeverity.Minor, minor );
                _populationCare.Add( InjurySeverity.Major, major );
                _populationCare.Add( InjurySeverity.LifeThreathening, lifethreatening );
                _populationCare.Add( InjurySeverity.Operation, operation );
            }

            public PopulationCare()
            {
                _populationCare = new Dictionary<InjurySeverity, MedicalCareCategory>();
            }
                
            public static PopulationCare Default => new PopulationCare( 
                MedicalCareCategory.Best, 
                MedicalCareCategory.Best,
                MedicalCareCategory.Best, 
                MedicalCareCategory.Best );

            public MedicalCareCategory this[ InjurySeverity index ]
            {
                get => _populationCare[index];
                set => _populationCare[index] = value;
            }

            public void ExposeData()
            {
                Scribe_Collections.Look( ref _populationCare, "MedicalCare", LookMode.Value, LookMode.Value );
            }
        }
        
        public static void SetDefaults()
        {
            medicalCare = new MedicalCare();
            medicalCare[Population.Colonist] = new PopulationCare( 
                MedicalCareCategory.HerbalOrWorse,
                MedicalCareCategory.NormalOrWorse, 
                MedicalCareCategory.Best, 
                MedicalCareCategory.Best );
            
            medicalCare[Population.Guest] = new PopulationCare( 
                MedicalCareCategory.NoMeds,
                MedicalCareCategory.HerbalOrWorse, 
                MedicalCareCategory.NormalOrWorse, 
                MedicalCareCategory.NormalOrWorse );
            
            medicalCare[Population.Prisoner] = new PopulationCare( 
                MedicalCareCategory.NoMeds,
                MedicalCareCategory.HerbalOrWorse, 
                MedicalCareCategory.NormalOrWorse, 
                MedicalCareCategory.NormalOrWorse );
            
            medicalCare[Population.Animal] = new PopulationCare( 
                MedicalCareCategory.NoMeds,
                MedicalCareCategory.HerbalOrWorse, 
                MedicalCareCategory.NormalOrWorse, 
                MedicalCareCategory.NormalOrWorse );
        }
        
        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Deep.Look( ref medicalCare, "PharmacistSettings" );
        }
    }
}