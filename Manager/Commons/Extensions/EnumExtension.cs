using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Manager.Commons.Extensions;

public static class EnumExtension
{
    public static string GetDescription<TEnumType>(this TEnumType enumType) where TEnumType : Enum
    {
        var fieldInfo = enumType.GetType().GetField(enumType.ToString());

        return fieldInfo?
            .GetCustomAttributes(typeof(DescriptionAttribute), false)
            .Cast<DescriptionAttribute>()
            .FirstOrDefault()?.Description ?? string.Empty;
    }

    public static TEnumType ToEnum<TEnumType>(this int enumValue) where TEnumType : Enum
        => (TEnumType)Enum.ToObject(typeof(TEnumType), enumValue);

    public static void ToEnum<TEnumType>(this string enumValue) where TEnumType : Enum
    {
        TEnumType foo = (TEnumType)Enum.Parse(typeof(TEnumType), enumValue);

        if (!Enum.IsDefined(typeof(TEnumType), foo) && !foo.ToString().Contains(","))
            throw new InvalidOperationException($"{enumValue} is not an underlying value of the {nameof(TEnumType)} enumeration.");
    }

    public static IEnumerable<int> CollectEnumValues<T>() where T : Enum
     => typeof(T).GetEnumValues().Cast<object>().TakeWhile(value => value is not null).Cast<int>().ToList();
}

