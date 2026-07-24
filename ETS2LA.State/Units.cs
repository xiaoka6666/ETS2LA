namespace ETS2LA.State;

using System.ComponentModel;
using System.Reflection;

public enum UnitType
{
    Speed,
    Distance,
    Liquid,
    Weight,
    Temperature,
    Pressure
}

public enum Units
{
    [Description("公制")]
    Metric,
    [Description("英制")]
    Imperial,
    [Description("科学")]
    Scientific
}

public static class UnitConversions
{
    public static float ToScientificUnits(UnitType type, float value, Units fromUnits)
    {
        switch (type)
        {
            case UnitType.Speed:
                switch (fromUnits)
                {
                    case Units.Metric:
                        return value / 3.6f; // km/h to m/s
                    case Units.Imperial:
                        return value * 0.44704f; // mph to m/s
                    case Units.Scientific:
                        return value; // already in m/s
                }
                break;
            case UnitType.Distance:
                switch (fromUnits)
                {
                    case Units.Metric:
                        return value; // already in m
                    case Units.Imperial:
                        return value * 0.3048f; // ft to m
                    case Units.Scientific:
                        return value; // already in m
                }
                break;
            case UnitType.Liquid:
                switch (fromUnits)
                {
                    case Units.Metric:
                        return value; // already in liters
                    case Units.Imperial:
                        return value * 4.54609f; // gallons to liters
                    case Units.Scientific:
                        return value; // already in liters
                }
                break;
            case UnitType.Weight:
                switch (fromUnits)       
                {
                    case Units.Metric:
                        return value; // already in kg
                    case Units.Imperial:
                        return value * 0.453592f; // lbs to kg
                    case Units.Scientific:
                        return value; // already in kg
                }
                break;
            case UnitType.Temperature:
                switch (fromUnits)
                {
                    case Units.Metric:
                        return value + 273.15f; // Celsius to Kelvin
                    case Units.Imperial:
                        return (value - 32) * 5 / 9 + 273.15f; // Fahrenheit to Kelvin
                    case Units.Scientific:
                        return value; // already in Kelvin
                }
                break;
            case UnitType.Pressure:
                switch (fromUnits)
                {                    
                    case Units.Metric:
                        return value * 100f; // bar to Pa
                    case Units.Imperial:
                        return value * 6894.76f; // psi to Pa
                    case Units.Scientific:
                        return value; // already in Pa
                }
                break;
        }
        throw new NotImplementedException($"Conversion for {type} from {fromUnits} is not implemented.");
    }

    public static float FromScientificUnits(UnitType type, float value, Units toUnits)
    {
        switch (type)
        {
            case UnitType.Speed:
                switch (toUnits)
                {
                    case Units.Metric:
                        return value * 3.6f; // m/s to km/h
                    case Units.Imperial:
                        return value / 0.44704f; // m/s to mph
                    case Units.Scientific:
                        return value; // already in m/s
                }
                break;
            case UnitType.Distance:
                switch (toUnits)
                {
                    case Units.Metric:
                        return value; // already in m
                    case Units.Imperial:
                        return value / 0.3048f; // m to ft
                    case Units.Scientific:
                        return value; // already in m
                }
                break;
            case UnitType.Liquid:
                switch (toUnits)
                {
                    case Units.Metric:
                        return value; // already in liters
                    case Units.Imperial:
                        return value / 4.54609f; // liters to gallons
                    case Units.Scientific:
                        return value; // already in liters
                }
                break;
            case UnitType.Weight:
                switch (toUnits)       
                {
                    case Units.Metric:
                        return value; // already in kg
                    case Units.Imperial:
                        return value / 0.453592f; // kg to lbs
                    case Units.Scientific:
                        return value; // already in kg
                }
                break;
            case UnitType.Temperature:
                switch (toUnits)
                {
                    case Units.Metric:
                        return value - 273.15f; // Kelvin to Celsius
                    case Units.Imperial:
                        return (value - 273.15f) * 9 / 5 + 32; // Kelvin to Fahrenheit
                    case Units.Scientific:
                        return value; // already in Kelvin
                }
                break;
            case UnitType.Pressure:
                switch (toUnits)
                {                    
                    case Units.Metric:
                        return value / 100f; // Pa to bar
                    case Units.Imperial:
                        return value / 6894.76f; // Pa to psi
                    case Units.Scientific:
                        return value; // already in Pa
                }
                break;
        }
        throw new NotImplementedException($"Conversion for {type} to {toUnits} is not implemented.");
    }

    public static string GetUnitName(UnitType type, Units units)
    {
        switch (type)
        {
            case UnitType.Speed:
                switch (units)
                {
                    case Units.Metric:
                        return "公里/小时";
                    case Units.Imperial:
                        return "英里/小时";
                    case Units.Scientific:
                        return "米/秒";
                }
                break;
            case UnitType.Distance:
                switch (units)
                {
                    case Units.Metric:
                        return "米";
                    case Units.Imperial:
                        return "英尺";
                    case Units.Scientific:
                        return "米";
                }
                break;
            case UnitType.Liquid:
                switch (units)
                {
                    case Units.Metric:
                        return "升";
                    case Units.Imperial:
                        return "加仑";
                    case Units.Scientific:
                        return "升";
                }
                break;
            case UnitType.Weight:
                switch (units)       
                {
                    case Units.Metric:
                        return "千克";
                    case Units.Imperial:
                        return "磅";
                    case Units.Scientific:
                        return "千克";
                }
                break;
            case UnitType.Temperature:
                switch (units)
                {
                    case Units.Metric:
                        return "°C";
                    case Units.Imperial:
                        return "°F";
                    case Units.Scientific:
                        return "K";
                }
                break;
            case UnitType.Pressure:
                switch (units)
                {                    
                    case Units.Metric:
                        return "bar";
                    case Units.Imperial:
                        return "psi";
                    case Units.Scientific:
                        return "帕斯卡";
                }
                break;
        }
        throw new NotImplementedException($"Unit name for {type} in {units} is not implemented.");
    }

    public static string GetUnitAbbreviation(UnitType type, Units units)
    {
        switch (type)
        {
            case UnitType.Speed:
                switch (units)
                {
                    case Units.Metric:
                        return "km/h";
                    case Units.Imperial:
                        return "mph";
                    case Units.Scientific:
                        return "m/s";
                }
                break;
            case UnitType.Distance:
                switch (units)
                {
                    case Units.Metric:
                        return "m";
                    case Units.Imperial:
                        return "ft";
                    case Units.Scientific:
                        return "m";
                }
                break;
            case UnitType.Liquid:
                switch (units)
                {
                    case Units.Metric:
                        return "L";
                    case Units.Imperial:
                        return "gal";
                    case Units.Scientific:
                        return "L";
                }
                break;
            case UnitType.Weight:
                switch (units)       
                {
                    case Units.Metric:
                        return "kg";
                    case Units.Imperial:
                        return "lbs";
                    case Units.Scientific:
                        return "kg";
                }
                break;
            case UnitType.Temperature:
                switch (units)
                {
                    case Units.Metric:
                        return "°C";
                    case Units.Imperial:
                        return "°F";
                    case Units.Scientific:
                        return "K";
                }
                break;
            case UnitType.Pressure:
                switch (units)
                {                    
                    case Units.Metric:
                        return "bar";
                    case Units.Imperial:
                        return "psi";
                    case Units.Scientific:
                        return "Pa";
                }
                break;
        }
        throw new NotImplementedException($"Unit abbreviation for {type} in {units} is not implemented.");
    }
}

/// <summary>
/// 为枚举提供中文显示名称的扩展方法
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// 获取枚举值的显示名称（优先读取 Description 特性，否则返回枚举名称）
    /// </summary>
    public static string GetDisplayName(this Enum value)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<DescriptionAttribute>();
        return attr?.Description ?? value.ToString();
    }
}