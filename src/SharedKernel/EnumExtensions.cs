using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Image_guesser.SharedKernel;

public static class EnumExtensions
{
    public static string GetDisplayName(this Enum enumValue)
    {
        // Get the Display attribute on the enum field if it exists
        var fieldInfo = enumValue.GetType().GetField(enumValue.ToString());
        var displayAttribute = fieldInfo?.GetCustomAttribute<DisplayAttribute>();

        // Return the Name from the Display attribute, or just the enum value if not found
        return displayAttribute?.Name ?? enumValue.ToString();
    }
}
