using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mlie;
using RimWorld;
using UnityEngine;
using Verse;
// ReSharper disable StringIndexOfIsCultureSpecific.1

namespace SyrTraitValue;

public class TraitValueCore : Mod
{
    public static int traitCount = 0;
    private static readonly Dictionary<string, int> newTraitValues = new();
    private static string currentVersion;
    public static TraitValueCore Instance;

    private Vector2 scrollPosition = new(0f, 0f);

    public TraitValueCore(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<TraitValueSettings>();
        currentVersion = VersionFromManifest.GetVersionFromModMetaData(content.ModMetaData);
    }

    internal TraitValueSettings Settings { get; }

    public override string SettingsCategory()
    {
        return "TraitValueSettingsCategory".Translate();
    }

    public override void DoSettingsWindowContents(Rect inRect)
    {
        checked
        {
            var listingStandard = new Listing_Standard();
            listingStandard.Begin(inRect);
            listingStandard.CheckboxLabeled("SyrTraitValue_NoNotice".Translate(),
                ref Settings.noNotice, "SyrTraitValue_NoNoticeTooltip".Translate());
            listingStandard.CheckboxLabeled("SyrTraitValue_EnableColors".Translate(),
                ref Settings.enableColors, "SyrTraitValue_EnableColorsTooltip".Translate());
            listingStandard.CheckboxLabeled("SyrTraitValue_UseBestColor".Translate(),
                ref Settings.useBestColor, "SyrTraitValue_UseBestColorTooltip".Translate());

            var colorButton1 = new Rect(inRect.x, listingStandard.CurHeight, inRect.width * 0.25f, 44f);
            var colorButton2 = new Rect(inRect.x + (inRect.width * 0.25f), listingStandard.CurHeight,
                inRect.width * 0.25f, 44f);
            var colorButton3 = new Rect(inRect.x + (inRect.width * 0.5f), listingStandard.CurHeight,
                inRect.width * 0.25f, 44f);
            var colorButton4 = new Rect(inRect.x + (inRect.width * 0.75f), listingStandard.CurHeight,
                inRect.width * 0.25f, 44f);
            if (buttonImageLabeled(colorButton1, "SyrTraitValue_BestColor".Translate(),
                    "SyrTraitValue_BestColorTooltip".Translate(), Static_Textures.bestColorButton, 128f, 24f))
            {
                Find.WindowStack.Add(new Dialog_ColorPicker(Settings.bestTraitColor, delegate(Color color)
                {
                    Settings.bestTraitColor = color;
                    Static_Textures.bestColorButton =
                        SolidColorMaterials.NewSolidColorTexture(Settings.bestTraitColor);
                    Static_Textures.bestColorButton.Apply(false);
                }));
            }

            if (buttonImageLabeled(colorButton2, "SyrTraitValue_GoodColor".Translate(),
                    "SyrTraitValue_GoodColorTooltip".Translate(), Static_Textures.goodColorButton, 128f, 24f))
            {
                Find.WindowStack.Add(new Dialog_ColorPicker(Settings.goodTraitColor, delegate(Color color)
                {
                    Settings.goodTraitColor = color;
                    Static_Textures.goodColorButton =
                        SolidColorMaterials.NewSolidColorTexture(Settings.goodTraitColor);
                    Static_Textures.goodColorButton.Apply(false);
                }));
            }

            if (buttonImageLabeled(colorButton3, "SyrTraitValue_NeutralColor".Translate(),
                    "SyrTraitValue_NeutralColorTooltip".Translate(), Static_Textures.neutralColorButton, 128f, 24f))
            {
                Find.WindowStack.Add(new Dialog_ColorPicker(Settings.neutralTraitColor, delegate(Color color)
                {
                    Settings.neutralTraitColor = color;
                    Static_Textures.neutralColorButton =
                        SolidColorMaterials.NewSolidColorTexture(Settings.neutralTraitColor);
                    Static_Textures.neutralColorButton.Apply(false);
                }));
            }

            if (buttonImageLabeled(colorButton4, "SyrTraitValue_BadColor".Translate(),
                    "SyrTraitValue_BadColorTooltip".Translate(), Static_Textures.badColorButton, 128f, 24f))
            {
                Find.WindowStack.Add(new Dialog_ColorPicker(Settings.badTraitColor, delegate(Color color)
                {
                    Settings.badTraitColor = color;
                    Static_Textures.badColorButton =
                        SolidColorMaterials.NewSolidColorTexture(Settings.badTraitColor);
                    Static_Textures.badColorButton.Apply(false);
                }));
            }

            var outRect = inRect;
            outRect.height = outRect.height - listingStandard.CurHeight - 44f - 110f;
            outRect.y = outRect.y + listingStandard.CurHeight + 44f;
            var viewRect = inRect.GetInnerRect();
            viewRect.height = 27f * traitCount * 0.34f;
            var rowRect1 = new Rect(viewRect.x, viewRect.y, viewRect.width * 0.33f, 24f);
            var rowRect2 = new Rect(viewRect.x + (viewRect.width * 0.33f), viewRect.y, viewRect.width * 0.33f, 24f);
            var rowRect3 = new Rect(viewRect.x + (viewRect.width * 0.67f), viewRect.y, viewRect.width * 0.33f, 24f);

            Widgets.BeginScrollView(outRect, ref scrollPosition, viewRect);
            var i = 0;
            var threshold = Mathf.CeilToInt(traitCount * 0.333f);
            // ReSharper disable once CollectionNeverUpdated.Local
            var buffers = new Dictionary<TraitDegreeData, string>();
            foreach (var t in TraitValueUtility.AllTraits)
            {
                foreach (var tdd in t.degreeDatas)
                {
                    i++;
                    buffers.TryGetValue(tdd, out var buffer);
                    var trait = new Trait(t, tdd.degree, true);
                    if (i <= threshold)
                    {
                        textFieldNumericLabeled(rowRect1, trait.Label, traitDesc(tdd, t),
                            ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree)
                                .value, ref buffer, -99, 99);
                        rowRect1.y = rowRect1.yMax + 2f;
                    }
                    else if (i <= threshold * 2)
                    {
                        textFieldNumericLabeled(rowRect2, trait.Label, traitDesc(tdd, t),
                            ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree)
                                .value, ref buffer, -99, 99);
                        rowRect2.y = rowRect2.yMax + 2f;
                    }
                    else
                    {
                        textFieldNumericLabeled(rowRect3, trait.Label, traitDesc(tdd, t),
                            ref t.GetModExtension<TraitValueExtension>().traitValues.Find(dv => dv.degree == tdd.degree)
                                .value, ref buffer, -99, 99);
                        rowRect3.y = rowRect3.yMax + 2f;
                    }
                }
            }

            Widgets.EndScrollView();

            listingStandard.Gap(inRect.height - 120f);
            listingStandard.GapLine(30f);
            var rectDefaultSettings = listingStandard.GetRect(30f);
            if (currentVersion != null)
            {
                GUI.contentColor = Color.gray;
                Widgets.Label(rectDefaultSettings.LeftHalf(),
                    "SyrTraitValue_CurrentModVersion".Translate(currentVersion));
                GUI.contentColor = Color.white;
            }

            TooltipHandler.TipRegion(rectDefaultSettings.RightHalf(), "RestoreToDefaultSettings".Translate());
            if (Widgets.ButtonText(rectDefaultSettings.RightHalf(), "ResetBinding".Translate()))
            {
                Settings.enableColors = true;
                Settings.useBestColor = true;
                TraitValueUtility.LoadSavedValues(true);
                Settings.changedTraitValues.Clear();
                Settings.bestTraitColor = Color.cyan;
                Static_Textures.bestColorButton = SolidColorMaterials.NewSolidColorTexture(Color.cyan);
                Static_Textures.bestColorButton.Apply(false);
                Settings.goodTraitColor = Color.green;
                Static_Textures.goodColorButton = SolidColorMaterials.NewSolidColorTexture(Color.green);
                Static_Textures.goodColorButton.Apply(false);
                Settings.neutralTraitColor = Color.yellow;
                Static_Textures.neutralColorButton = SolidColorMaterials.NewSolidColorTexture(Color.yellow);
                Static_Textures.neutralColorButton.Apply(false);
                Settings.badTraitColor = Color.red;
                Static_Textures.badColorButton = SolidColorMaterials.NewSolidColorTexture(Color.red);
                Static_Textures.badColorButton.Apply(false);
            }

            listingStandard.End();
        }
    }

    private static bool buttonImageLabeled(Rect rect, string label, string tooltip, Texture2D tex, float width,
        float height)
    {
        Widgets.Label(rect, label);
        TooltipHandler.TipRegion(rect, tooltip);
        var result = Widgets.ButtonImage(new Rect(rect.x, rect.y + 20f, width, height), tex);
        return result;
    }

    private static void textFieldNumericLabeled<T>(Rect rect, string label, string tooltip, ref T val,
        ref string buffer,
        float min = 0f, float max = 1E+09f) where T : struct
    {
        var rectLabel = new Rect(rect.x, rect.y, rect.width * 0.77f, rect.height);
        var rectField = new Rect(rect.x + (rect.width * 0.8f), rect.y, rect.width * 0.2f, rect.height);
        var anchor = Text.Anchor;
        Text.Anchor = TextAnchor.MiddleRight;
        if (val is int value && Instance.Settings.enableColors)
        {
            var originalColor = GUI.color;
            TraitValueUtility.ValueColor(value, out var traitColor);
            GUI.color = traitColor;
            if (label.Contains("<color=#"))
            {
                label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
            }

            Widgets.Label(rectLabel, label);
            TooltipHandler.TipRegion(rectLabel, tooltip);
            GUI.color = originalColor;
        }
        else
        {
            if (label.Contains("<color=#"))
            {
                label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
            }

            Widgets.Label(rectLabel, label);
            TooltipHandler.TipRegion(rectLabel, tooltip);
        }

        Text.Anchor = anchor;
        Widgets.TextFieldNumeric(rectField, ref val, ref buffer, min, max);
    }

    private static string traitDesc(TraitDegreeData tdd, TraitDef t)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(tdd.description);
        var traitThoughts = getPermaThoughts(tdd, t);
        var skillGains = tdd.skillGains.Count > 0;
        var thoughtDefs = traitThoughts as ThoughtDef[] ?? traitThoughts.ToArray();
        var anyThought = thoughtDefs.Any();
        var statOffsets = tdd.statOffsets != null;
        var statFactors = tdd.statFactors != null;
        if (skillGains || anyThought || statOffsets || statFactors)
        {
            stringBuilder.AppendLine();
            stringBuilder.AppendLine();
        }

        if (skillGains)
        {
            foreach (var keyValuePair in tdd.skillGains)
            {
                if (keyValuePair.amount == 0)
                {
                    continue;
                }

                var value = $"    {keyValuePair.skill.skillLabel.CapitalizeFirst()}:   {keyValuePair.amount:+##;-##}";
                stringBuilder.AppendLine(value);
            }
        }

        if (anyThought)
        {
            foreach (var thoughtDef in thoughtDefs)
            {
                stringBuilder.AppendLine("    " + "PermanentMoodEffect".Translate() + " " + thoughtDef.stages[0]
                    .baseMoodEffect.ToStringByStyle(ToStringStyle.Integer, ToStringNumberSense.Offset));
            }
        }

        if (statOffsets && tdd.statOffsets != null)
        {
            foreach (var statOffset in tdd.statOffsets)
            {
                var valueToStringAsOffset = statOffset.ValueToStringAsOffset;
                string value2 = "    " + statOffset.stat.LabelCap + " " + valueToStringAsOffset;
                stringBuilder.AppendLine(value2);
            }
        }

        if (statFactors && tdd.statFactors != null)
        {
            foreach (var statFactor in tdd.statFactors)
            {
                var toStringAsFactor = statFactor.ToStringAsFactor;
                string value3 = "    " + statFactor.stat.LabelCap + " " + toStringAsFactor;
                stringBuilder.AppendLine(value3);
            }
        }

        if (!tdd.hungerRateFactor.Equals(1f))
        {
            var hunger = tdd.hungerRateFactor.ToStringByStyle(ToStringStyle.PercentOne, ToStringNumberSense.Factor);
            string value4 = "    " + "HungerRate".Translate() + " " + hunger;
            stringBuilder.AppendLine(value4);
        }

        if (ModsConfig.RoyaltyActive)
        {
            var allowedMeditationFocusTypes = tdd.allowedMeditationFocusTypes;
            if (!allowedMeditationFocusTypes.NullOrEmpty())
            {
                stringBuilder.AppendLine();
                stringBuilder.AppendLine();
                stringBuilder.AppendLine("EnablesMeditationFocusType".Translate() + ":\n" +
                                         (from f in allowedMeditationFocusTypes
                                             select f.LabelCap.RawText).ToLineList("  - "));
            }
        }

        if (stringBuilder.Length <= 0 || stringBuilder[^1] != '\n')
        {
            return stringBuilder.ToString();
        }

        if (stringBuilder.Length > 1 && stringBuilder[^2] == '\r')
        {
            stringBuilder.Remove(stringBuilder.Length - 2, 2);
        }
        else
        {
            stringBuilder.Remove(stringBuilder.Length - 1, 1);
        }

        return stringBuilder.ToString();
    }

    private static IEnumerable<ThoughtDef> getPermaThoughts(TraitDegreeData tdd, TraitDef t)
    {
        var allThoughts = DefDatabase<ThoughtDef>.AllDefsListForReading;
        int num;
        for (var i = 0; i < allThoughts.Count; i = num + 1)
        {
            if (allThoughts[i].IsSituational && allThoughts[i].Worker is ThoughtWorker_AlwaysActive &&
                allThoughts[i].requiredTraits != null && allThoughts[i].requiredTraits.Contains(t) &&
                (!allThoughts[i].RequiresSpecificTraitsDegree || allThoughts[i].requiredTraitsDegree == tdd.degree))
            {
                yield return allThoughts[i];
            }

            num = i;
        }
    }

    public override void WriteSettings()
    {
        if (!Settings.enableColors)
        {
            TraitValueUtility.UncolorTraitLabels();
        }
        else
        {
            TraitValueUtility.ColorTraitLabels();
        }

        foreach (var t in TraitValueUtility.AllTraits)
        {
            foreach (var degree in t.degreeDatas.Select(degreeData => degreeData.degree))
            {
                var modExtension = t.GetModExtension<TraitValueExtension>();
                if (modExtension == null)
                {
                    continue;
                }

                var key = $"{t.defName}, {degree}";
                if (newTraitValues.ContainsKey(key))
                {
                    newTraitValues[key] = modExtension.traitValues.Find(dv => dv.degree == degree).value;
                }
                else
                {
                    newTraitValues.Add($"{t.defName}, {degree}",
                        modExtension.traitValues.Find(dv => dv.degree == degree).value);
                }
            }
        }

        Settings.changedTraitValues = newTraitValues
            .Where(kvp => TraitValueUtility.OriginalTraitValues[kvp.Key] != kvp.Value)
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
        base.WriteSettings();
    }
}