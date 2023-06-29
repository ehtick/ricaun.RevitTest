using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    //internal static class Mapper
    //{
    //    public static object ConvertValueToPropertyInfo(string valueToConvert, PropertyInfo propertyInfo)
    //    {
    //        var type = propertyInfo.PropertyType;
    //        if (type == typeof(int))
    //        {
    //            if (int.TryParse(valueToConvert, out int value))
    //                return value;
    //        }
    //        else if (type == typeof(double))
    //        {
    //            try
    //            {
    //                return double.Parse(valueToConvert.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
    //            }
    //            catch { }
    //        }
    //        else if (type == typeof(bool))
    //        {
    //            var value = valueToConvert.ToLower();
    //            return value.Equals("true");
    //        }
    //        else if (type == typeof(XmlBool))
    //        {
    //            var value = valueToConvert.ToLower();
    //            return (XmlBool)value.Equals("true");
    //        }
    //        return valueToConvert;
    //    }
    //    public static bool IsValueEqual(object sourceValue, object destinationValue)
    //    {
    //        if (sourceValue is null)
    //        {
    //            return destinationValue is null;
    //        }

    //        return sourceValue.Equals(destinationValue);
    //    }
    //    public static bool IsPropertyInfoEqualKey(string keyName, PropertyInfo propertyInfo)
    //    {
    //        var keys = keyName.Split('.').Reverse();
    //        var name = propertyInfo.GetCustomAttributeName();
    //        var declaringType = propertyInfo.DeclaringType;
    //        foreach (var key in keys)
    //        {
    //            Debug.WriteLine($"{key} => {name}");
    //            if (key.Equals(name, StringComparison.InvariantCultureIgnoreCase) == false)
    //                return false;

    //            if (declaringType is null) break;
    //            name = declaringType.GetCustomAttributeName();
    //            declaringType = declaringType.DeclaringType;
    //            if (name.EndsWith("Model")) name = name.Substring(0, name.Length - 5);
    //        }

    //        return true;
    //    }
    //    public static T Map<T>(T destination, IDictionary<string, string> dictionary) where T : class
    //    {
    //        if (dictionary is null)
    //            return destination;

    //        Type destinationObjectType = destination.GetType();
    //        PropertyInfo[] destinationPropList = destinationObjectType.GetProperties();

    //        foreach (PropertyInfo destinationPropInfo in destinationPropList)
    //        {
    //            if (destinationPropInfo.GetGetMethod().IsVirtual) continue;
    //            foreach (var keyValue in dictionary)
    //            {
    //                if (IsPropertyInfoEqualKey(keyValue.Key, destinationPropInfo))
    //                {
    //                    try
    //                    {
    //                        var sourceValue = ConvertValueToPropertyInfo(keyValue.Value, destinationPropInfo);
    //                        var destinationValue = destinationPropInfo.GetValue(destination, null);
    //                        //AdapterLogger.Logger.Warning($"{keyValue.Key}: {sourceValue}");
    //                        var isValueIsDiferent = !IsValueEqual(sourceValue, destinationValue);
    //                        if (isValueIsDiferent)
    //                        {
    //                            //AdapterLogger.Logger.Error($"{keyValue.Key}: {sourceValue} >> {destinationPropInfo.Name}: {destinationValue} | {isValueIsDiferent}");
    //                            Debug.WriteLine($"{keyValue.Key}: {sourceValue} >> {destinationPropInfo.Name}: {destinationValue} | {isValueIsDiferent}");
    //                            destinationPropInfo.SetValue(destination, sourceValue, null);
    //                        }
    //                        break;
    //                    }
    //                    catch { }
    //                }
    //            }
    //        }

    //        return destination;
    //    }

    //    #region private
    //    private static string GetCustomAttributeName(this MemberInfo memberInfo)
    //    {
    //        try
    //        {
    //            var customPropertyNames = new[] { "ElementName", "DisplayName", "Name" };
    //            foreach (var customAttribute in memberInfo.GetCustomAttributes())
    //            {
    //                var type = customAttribute.GetType();
    //                foreach (var propertyName in customPropertyNames)
    //                {
    //                    if (type?.GetProperty(propertyName) is PropertyInfo propertyInfo)
    //                    {
    //                        var value = propertyInfo.GetValue(customAttribute) as string;
    //                        if (!string.IsNullOrEmpty(value)) return value;
    //                    }
    //                }
    //            }
    //        }
    //        catch { }
    //        return memberInfo.Name;
    //    }
    //    #endregion
    //}
}
