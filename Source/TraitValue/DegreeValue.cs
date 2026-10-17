using System.Xml;
using Verse;

namespace SyrTraitValue;

public class DegreeValue
{
    public int degree;

    public int value;

    public DegreeValue()
    {
    }

    public DegreeValue(int degree, int value)
    {
        this.degree = degree;
        this.value = value;
    }

    public void LoadDataFromXmlCustom(XmlNode xmlRoot)
    {
        if (xmlRoot.ChildNodes.Count != 1)
        {
            Log.Error("");
            return;
        }

        var array = xmlRoot.FirstChild?.Value?.Split(',');
        if (array is { Length: 1 })
        {
            degree = 0;
            value = ParseHelper.FromString<int>(array[0]);
        }
        else if (array is not { Length: 2 })
        {
            Log.ErrorOnce(
                $"Trait values need two numbers separated by a comma. Or a single number. Wrong value: {xmlRoot.FirstChild?.Value}",
                16205552);
        }
        else
        {
            degree = ParseHelper.FromString<int>(array[0]);
            value = ParseHelper.FromString<int>(array[1]);
        }
    }
}