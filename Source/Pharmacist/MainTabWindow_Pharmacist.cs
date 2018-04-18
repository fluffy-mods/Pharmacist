// Window_Pharmacist.cs
// Copyright Karel Kroeze, 2018-2018

using System;
using System.Collections.Generic;
using System.Linq;
using Reloader;
using RimWorld;
using UnityEngine;
using Verse;
using static Pharmacist.Constants;
using static Pharmacist.Resources;

namespace Pharmacist
{
    public class MainTabWindow_Pharmacist: MainTabWindow
    {
        public override Vector2 InitialSize => new Vector2( 500, TitleHeight + ( populations.Length + 1 ) * RowHeight + 2 * Margin );
        internal static Population[] populations = Enum.GetValues( typeof( Population ) ).Cast<Population>().ToArray();
        internal static InjurySeverity[] severities = Enum.GetValues( typeof( InjurySeverity ) ).Cast<InjurySeverity>().ToArray();
        internal static MedicalCareCategory[] medcares = Enum.GetValues( typeof( MedicalCareCategory ) ).Cast<MedicalCareCategory>().ToArray();
        
        [ReloadMethod]
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
            Vector2 pos = new Vector2( canvas.xMin, titleRect.yMax );

            Text.Font = GameFont.Medium;
            Widgets.Label( titleRect, "Fluffy.Pharmacist.Settings.Title".Translate() );
            Text.Font = GameFont.Small;
            
            // set up column widths
            int labelWidth = (int)canvas.width / 4;
            int colWidth = (int) ( canvas.width - labelWidth ) / severities.Length;
            
            // draw column headers
            pos.x += labelWidth;
            foreach ( var severity in severities )
            {
                var cell = new Rect( pos.x, pos.y, colWidth, RowHeight );
                var headerIconRect = new Rect( 0, 0, IconSize, IconSize )
                    .CenteredOnXIn( cell )
                    .CenteredOnYIn( cell );
                
                TooltipHandler.TipRegion( cell, $"Fluffy.Pharmacist.Severity.{severity}".Translate() + "\n\n" + $"Fluffy.Pharmacist.Severity.{severity}.Tip".Translate() );
                GUI.DrawTexture( headerIconRect, severityTextures[(int) severity] );
                
                pos.x += colWidth;
            }

            pos.x = canvas.xMin;
            pos.y += RowHeight;

            foreach ( var population in populations )
            {
                Text.Anchor = TextAnchor.MiddleLeft;
                Widgets.Label( new Rect( pos.x, pos.y, labelWidth, RowHeight ),
                    $"Fluffy.Pharmacist.Population.{population}".Translate() );
                pos.x += labelWidth;
                Text.Anchor = TextAnchor.UpperLeft;

                foreach ( var severity in severities )
                {
                    Rect cell = new Rect(
                        pos.x,
                        pos.y,
                        colWidth,
                        RowHeight );
                    Rect iconRect = new Rect( 0, 0, IconSize, IconSize )
                        .CenteredOnXIn( cell )
                        .CenteredOnYIn( cell );
                    
                    Widgets.DrawHighlightIfMouseover( cell );
                    GUI.DrawTexture( iconRect, medcareGraphics[(int)PharmacistSettings.medicalCare[population][severity]]);

                    if ( Widgets.ButtonInvisible( cell ) )
                    {
                        List<FloatMenuOption> options = new List<FloatMenuOption>();
                        foreach ( var category in medcares )
                        {
                            options.Add( new FloatMenuOption( $"MedicalCareCategory_{category}".Translate(),
                                () => PharmacistSettings.medicalCare[population][severity] = category,
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

                    pos.x += colWidth;
                }

                pos.x = canvas.xMin;
                pos.y += RowHeight;
            }
        }
    }
}