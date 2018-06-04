// Window_Pharmacist.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;
using static Pharmacist.Constants;
using static Pharmacist.Resources;

namespace Pharmacist
{
    public class MainTabWindow_Pharmacist: MainTabWindow
    {
        public override Vector2 InitialSize => new Vector2( CareSelectorWidth + 2 * Margin, TitleHeight + CareSelectorHeight + 2 * Margin );
        internal static Population[] populations = Enum.GetValues( typeof( Population ) ).Cast<Population>().ToArray();
        internal static InjurySeverity[] severities = Enum.GetValues( typeof( InjurySeverity ) ).Cast<InjurySeverity>().ToArray();
        internal static MedicalCareCategory[] medcares = Enum.GetValues( typeof( MedicalCareCategory ) ).Cast<MedicalCareCategory>().ToArray();

        internal static int CareSelectorWidth => CareSelectorRowLabelWidth + CareSelectorColumnWidth * severities.Length;
        internal static int CareSelectorHeight => RowHeight * ( populations.Length + 1 );
        
        public override void DoWindowContents( Rect canvas )
        {
            // todo: remove (debugging)
            if (PharmacistSettings.medicalCare == null)
                PharmacistSettings.SetDefaults();
            
            Rect titleRect = new Rect( 
                canvas.xMin, 
                canvas.yMin, 
                canvas.width, 
                TitleHeight );
            Rect careSelectorRect = new Rect(
                canvas.xMin,
                titleRect.yMax,
                CareSelectorWidth,
                CareSelectorHeight );

            Text.Font = GameFont.Medium;
            Widgets.Label( titleRect, "Fluffy.Pharmacist.Settings.Title".Translate() );
            Text.Font = GameFont.Small;
            
            DrawCareSelectors( careSelectorRect );
        }

        private void SetMedicalCareFor( Population population )
        {
            
        }

        private void SetMedicalCareFor( InjurySeverity severity )
        {
            
        }

        private void CreateMedicalCareSelectionFloatMenu( Action<MedicalCareCategory> action )
        {
            List<FloatMenuOption> options = new List<FloatMenuOption>();
            foreach ( var category in medcares )
            {
                options.Add( new FloatMenuOption( $"MedicalCareCategory_{category}".Translate(),
                    () => action(category),
                    extraPartWidth: 30,
                    extraPartOnGUI: rect =>
                    {
                        var optionIconRect = new Rect( 0f, 0f, IconSize, IconSize )
                            .CenteredOnXIn( rect )
                            .CenteredOnYIn( rect );
                        GUI.DrawTexture( optionIconRect, medcareGraphics[(int) category] );
                        return false;
                    } ) );
            }

            Find.WindowStack.Add( new FloatMenu( options ) );
        }

        private void DrawCareSelectors( Rect canvas )
        {
            // draw column headers
            var pos = new Vector2( canvas.xMin + CareSelectorRowLabelWidth, canvas.yMin );
            foreach ( var severity in severities )
            {
                var cell = new Rect( pos.x, pos.y, CareSelectorColumnWidth, RowHeight );
                var headerIconRect = new Rect( 0, 0, IconSize, IconSize )
                    .CenteredOnXIn( cell )
                    .CenteredOnYIn( cell );

                TooltipHandler.TipRegion( cell,
                    $"Fluffy.Pharmacist.Severity.{severity}".Translate() + "\n\n" +
                    $"Fluffy.Pharmacist.Severity.{severity}.Tip".Translate() );
                GUI.DrawTexture( headerIconRect, severityTextures[(int) severity] );
                
                
                if ( Widgets.ButtonInvisible( cell ) )
                {
                    CreateMedicalCareSelectionFloatMenu( category =>
                    {
                        foreach ( var population in populations )
                        {
                            PharmacistSettings.medicalCare[population][severity] = category;
                        }
                    } );
                }

                pos.x += CareSelectorColumnWidth;
            }

            pos.x = canvas.xMin;
            pos.y += RowHeight;

            foreach ( var population in populations )
            {
                var populationLabelRect = new Rect( pos.x, pos.y, CareSelectorRowLabelWidth, RowHeight );
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label( populationLabelRect, $"Fluffy.Pharmacist.Population.{population}".Translate() );
                Text.Anchor = TextAnchor.UpperLeft;
                
                if ( Widgets.ButtonInvisible( populationLabelRect ) )
                {
                    CreateMedicalCareSelectionFloatMenu( category =>
                    {
                        foreach ( var severity in severities )
                        {
                            PharmacistSettings.medicalCare[population][severity] = category;
                        }
                    } );
                }
                
                pos.x += CareSelectorRowLabelWidth;

                foreach ( var severity in severities )
                {
                    Rect cell = new Rect(
                        pos.x,
                        pos.y,
                        CareSelectorColumnWidth,
                        RowHeight );
                    Rect iconRect = new Rect( 0, 0, IconSize, IconSize )
                        .CenteredOnXIn( cell )
                        .CenteredOnYIn( cell );

                    Widgets.DrawHighlightIfMouseover( cell );
                    GUI.DrawTexture( iconRect, medcareGraphics[(int) PharmacistSettings.medicalCare[population][severity]] );

                    if ( Widgets.ButtonInvisible( cell ) )
                    {
                        CreateMedicalCareSelectionFloatMenu( category => PharmacistSettings.medicalCare[population][severity] = category );
                    }

                    pos.x += CareSelectorColumnWidth;
                }

                pos.x = canvas.xMin;
                pos.y += RowHeight;
            }
        }
    }
}