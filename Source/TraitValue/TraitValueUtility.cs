using System.Collections.Generic;
using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace SyrTraitValue;

public static class TraitValueUtility
{
    private static int bestTraitValue;

    private static int worstTraitValue;

    public static readonly List<TraitDef> AllTraits = [];

    private static readonly List<TraitDegreeData> allTraitDegreeDatas = [];

    public static readonly Dictionary<string, int> OriginalTraitValues = new();

    public static void CacheTraitDefs()
    {
        AllTraits.Clear();
        allTraitDegreeDatas.Clear();
        foreach (var item in DefDatabase<TraitDef>.AllDefs.Where(td => td?.degreeDatas != null)
                     .OrderBy(def => def.label))
        {
            AllTraits.Add(item);
            foreach (var degreeData in item.degreeDatas)
            {
                allTraitDegreeDatas.Add(degreeData);
            }
        }

        TraitValueCore.traitCount = allTraitDegreeDatas.Count;
    }

    public static void CacheXMLValues()
    {
        foreach (var allTrait in AllTraits)
        {
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension?.traitValues == null)
            {
                continue;
            }

            foreach (var traitDegreeData in allTrait.degreeDatas)
            {
                OriginalTraitValues.Add($"{allTrait.defName}, {traitDegreeData.degree}",
                    modExtension.traitValues.Find(dv => dv.degree == traitDegreeData.degree).value);
            }
        }
    }

    public static void LoadSavedValues(bool reset)
    {
        if (reset)
        {
            foreach (var originalTraitValue in OriginalTraitValues)
            {
                var key2 = originalTraitValue.Key.Split(',');
                if (key2.Length != 2)
                {
                    Log.Message($"{originalTraitValue.Key} is not correctly formatted.");
                }
                else
                {
                    var modExtension = AllTraits.Find(t => t.defName == key2[0]).GetModExtension<TraitValueExtension>();
                    if (modExtension != null)
                    {
                        modExtension.traitValues.Find(tv => tv.degree == ParseHelper.FromString<int>(key2[1])).value =
                            originalTraitValue.Value;
                    }
                }
            }

            return;
        }

        foreach (var changedTraitValue in TraitValueCore.Instance.Settings.changedTraitValues)
        {
            var key = changedTraitValue.Key.Split(',');
            var modExtension2 = AllTraits.Find(t => t.defName == key[0]).GetModExtension<TraitValueExtension>();
            if (modExtension2 != null)
            {
                modExtension2.traitValues.Find(tv => tv.degree == ParseHelper.FromString<int>(key[1])).value =
                    changedTraitValue.Value;
            }
        }
    }

    public static void ColorTraitLabels()
    {
        foreach (var allTrait in AllTraits)
        {
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension == null)
            {
                continue;
            }

            foreach (var traitDegreeData in allTrait.degreeDatas)
            {
                foreach (var traitValue in modExtension.traitValues)
                {
                    var value = traitValue.value;
                    if (value > bestTraitValue)
                    {
                        bestTraitValue = value;
                    }

                    if (value < worstTraitValue)
                    {
                        worstTraitValue = value;
                    }
                }

                if (modExtension.traitValues.NullOrEmpty() || traitDegreeData.label.Contains("<color=#"))
                {
                    continue;
                }

                traitDegreeData.label =
                    $"{ValueColor(modExtension.traitValues.Find(dv => dv.degree == traitDegreeData.degree).value,
                        out _)}{traitDegreeData.label.CapitalizeFirst()}</color>";
                if (traitDegreeData.labelFemale != null)
                {
                    traitDegreeData.labelFemale =
                        $"{ValueColor(modExtension.traitValues.Find(dv => dv.degree == traitDegreeData.degree).value,
                            out _)}{traitDegreeData.labelFemale.CapitalizeFirst()}</color>";
                }
            }
        }
    }

    public static void UncolorTraitLabels()
    {
        foreach (var allTrait in AllTraits)
        {
            foreach (var degreeData in allTrait.degreeDatas)
            {
                var label = degreeData.label;
                if (label.Contains("<color=#"))
                {
                    // ReSharper disable once StringIndexOfIsCultureSpecific.1
                    degreeData.label = label.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                }

                if (degreeData.labelFemale == null)
                {
                    continue;
                }

                var labelFemale = degreeData.labelFemale;
                if (label.Contains("<color=#"))
                {
                    // ReSharper disable once StringIndexOfIsCultureSpecific.1
                    degreeData.labelFemale = labelFemale.Remove(label.IndexOf("<color=#"), 15).Replace("</color>", "");
                }
            }
        }
    }

    public static string ValueColor(int value, out Color color)
    {
        var num = bestTraitValue * 0.9f;
        if (value >= num && TraitValueCore.Instance.Settings.useBestColor)
        {
            var t = value / (float)bestTraitValue;
            color = Color.Lerp(TraitValueCore.Instance.Settings.goodTraitColor,
                TraitValueCore.Instance.Settings.bestTraitColor, t);
            return colorToRichText(color);
        }

        if (value >= 0)
        {
            var t2 = value / (TraitValueCore.Instance.Settings.useBestColor ? num : bestTraitValue);
            color = Color.Lerp(TraitValueCore.Instance.Settings.neutralTraitColor,
                TraitValueCore.Instance.Settings.goodTraitColor, t2);
            return colorToRichText(color);
        }

        var t3 = value / (float)worstTraitValue;
        color = Color.Lerp(TraitValueCore.Instance.Settings.neutralTraitColor,
            TraitValueCore.Instance.Settings.badTraitColor, t3);
        return colorToRichText(color);
    }

    private static string colorToRichText(Color color)
    {
        return
            $"<color=#{Mathf.RoundToInt(color.r * 255f):X2}{Mathf.RoundToInt(color.g * 255f):X2}{Mathf.RoundToInt(color.b * 255f):X2}>";
    }

    private static string getTraitsModName(TraitDef t)
    {
        return t.modContentPack == null ? "Unknown mod (trait defined with a patch?)" : t.modContentPack.Name;
    }

    public static void CountTraits()
    {
        var totalDegreeData = 0;
        var totalTraitValues = 0;
        var recache = false;
        foreach (var allTrait in AllTraits)
        {
            if (allTrait == null)
            {
                Log.Warning(
                    "Encountered a null-value when iterating over traits in the game, perhaps some other mod forcefully removed it after load.");
                recache = true;
                continue;
            }


            var count = allTrait.degreeDatas?.Count ?? 0;

            totalDegreeData += count;
            var modExtension = allTrait.GetModExtension<TraitValueExtension>();
            if (modExtension is { traitValues: not null } && count > 0)
            {
                var traitValues = modExtension.traitValues.Count;

                totalTraitValues += traitValues;
                if (count != traitValues)
                {
                    Log.Warning(
                        $"{getTraitsModName(allTrait)}: TraitDef {allTrait.defName} has {count} degrees, but {traitValues} values. You should configure one value per trait degree data.");
                    modExtension.traitValues.AddRange(from tdd in allTrait.degreeDatas
                        where modExtension.traitValues.FirstOrDefault(tv => tv.degree == tdd.degree) == null
                        select new DegreeValue(tdd.degree, 0));
                }

                foreach (var traitValue in modExtension.traitValues)
                {
                    var value = traitValue.value;
                    if (value > bestTraitValue)
                    {
                        bestTraitValue = value;
                    }

                    if (value < worstTraitValue)
                    {
                        worstTraitValue = value;
                    }
                }
            }
            else
            {
                if (!TraitValueCore.Instance.Settings.noNotice)
                {
                    Log.Warning(
                        $"{getTraitsModName(allTrait)}: TraitDef{allTrait.defName} does not have trait values correctly set up for integration with [SYR] Trait Values. You can ask the mod author politely to add integration, but accept if they don't want to.");
                }

                try
                {
                    allTrait.modExtensions = (allTrait.modExtensions ?? []).Append(
                        new TraitValueExtension
                        {
                            traitValues = allTrait.degreeDatas.Select(tdd => new DegreeValue(tdd.degree, 0)).ToList()
                        }).ToList();
                }
                catch
                {
                    // ignored
                }
            }
        }

        if (recache)
        {
            CacheTraitDefs();
        }

        Log.Message(
            $"[SYR] Trait Value Framework || Traits: {totalDegreeData} - Traits with an assigned value: {totalTraitValues}");
    }
}