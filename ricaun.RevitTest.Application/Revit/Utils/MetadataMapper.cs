using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.Application.Revit
{
    internal static class MetadataMapper
    {
        public static T Map<T>(T destination, IEnumerable<AssemblyMetadataAttribute> attributes, string prefix = null) where T : class
        {
            if (attributes is null)
                return destination;

            var metadataDictionary = attributes
                .GroupBy(attr => attr.Key)
                .ToDictionary(group => group.Key, group => group.Last().Value);

            return MapperKey.Map(destination, metadataDictionary, prefix);
        }
    }
    internal static class MapperKey
    {
        public static T Map<T>(T destination, IDictionary<string, string> dictionary, string prefix = null)
        {
            if (dictionary is null)
                return destination;

            foreach (var kvp in Mapper.ProcessProperties(destination, prefix))
            {
                var sourceKey = kvp.Key;
                var sourcePropertyInfo = kvp.PropertyInfo;
                var sourceValue = sourcePropertyInfo.GetValue(kvp.Object);

                Debug.WriteLine($"{sourceKey} - {sourcePropertyInfo} - {sourceValue}");

                if (dictionary.TryGetValue(sourceKey, out string valueToConvert))
                {
                    var destinationValue = Mapper.ConvertValueToPropertyInfo(valueToConvert, sourcePropertyInfo);
                    if (!Mapper.IsValueEqual(sourceValue, destinationValue))
                    {
                        try
                        {
                            destinationValue = Convert.ChangeType(destinationValue, sourcePropertyInfo.PropertyType);
                            sourcePropertyInfo.SetValue(kvp.Object, destinationValue);
                            Debug.WriteLine($"\t{sourceKey}: {sourceValue} >> {sourcePropertyInfo.Name}: {destinationValue}");
                        }
                        catch (Exception ex){ Debug.WriteLine(ex); }
                    }
                }
            }

            return destination;
        }
        public static IEnumerable<string> GetNames<T>(T destination)
        {
            return Mapper.ProcessProperties(destination).Select(e => e.Key).ToList();
        }
        public static class Mapper
        {
            public static string[] CustomAttributeNames = new[] { "ElementName", "DisplayName", "Name" };
            public static Dictionary<Type, Func<string, object>> MapperConvert;
            public static object ConvertToLong(string valueToConvert)
            {
                if (long.TryParse(valueToConvert.Replace(',', '.').Split('.')[0], out var value))
                    return value;
                return valueToConvert;
            }
            public static object ConvertToULong(string valueToConvert)
            {
                if (ulong.TryParse(valueToConvert.Replace(',', '.').Split('.')[0], out var value))
                    return value;
                return valueToConvert;
            }
            public static object ConvertToDouble(string valueToConvert)
            {
                try
                {
                    return double.Parse(valueToConvert.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
                return valueToConvert;
            }
            public static object ConvertToBool(string valueToConvert)
            {
                var value = valueToConvert.ToLower();
                return value.Equals("true") || value.Equals("1");
            }
            static Mapper()
            {
                MapperConvert = new Dictionary<Type, Func<string, object>>();
                MapperConvert.Add(typeof(ulong), ConvertToULong);
                MapperConvert.Add(typeof(uint), ConvertToULong);
                MapperConvert.Add(typeof(ushort), ConvertToULong);
                MapperConvert.Add(typeof(long), ConvertToLong);
                MapperConvert.Add(typeof(int), ConvertToLong);
                MapperConvert.Add(typeof(short), ConvertToLong);
                MapperConvert.Add(typeof(double), ConvertToDouble);
                MapperConvert.Add(typeof(float), ConvertToDouble);
                MapperConvert.Add(typeof(bool), ConvertToBool);
            }

            #region PropertyInformation
            public class PropertyInformation
            {
                public string Key { get; }
                public object Object { get; }
                public PropertyInfo PropertyInfo { get; }

                public PropertyInformation(string key, object obj, PropertyInfo propertyInfo)
                {
                    Key = key;
                    Object = obj;
                    PropertyInfo = propertyInfo;
                }
            }
            public static IEnumerable<PropertyInformation> ProcessProperties(object obj, string prefix = null)
            {
                if (obj == null)
                    yield break;

                if (string.IsNullOrEmpty(prefix))
                    prefix = string.Empty;

                Type objectType = obj.GetType();
                PropertyInfo[] properties = objectType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

                foreach (PropertyInfo property in properties)
                {
                    var propertyName = GetCustomAttributeName(property);
                    string propertyKey = string.IsNullOrEmpty(prefix) ? propertyName : $"{prefix}.{propertyName}";

                    if (property.PropertyType.IsClass && property.PropertyType != typeof(string))
                    {
                        object nestedPropertyValue = property.GetValue(obj);
                        if (nestedPropertyValue is null)
                        {
                            try
                            {
                                nestedPropertyValue = Activator.CreateInstance(property.PropertyType);
                                property.SetValue(obj, nestedPropertyValue);
                            }
                            catch { }
                        }
                        foreach (var nestedProperty in ProcessProperties(nestedPropertyValue, propertyKey))
                        {
                            yield return nestedProperty;
                        }
                    }
                    else
                    {
                        yield return new PropertyInformation(propertyKey, obj, property);
                    }
                }
            }
            #endregion
            public static string GetCustomAttributeName(MemberInfo memberInfo)
            {
                try
                {
                    var customPropertyNames = CustomAttributeNames;
                    foreach (var customAttribute in memberInfo.GetCustomAttributes())
                    {
                        var type = customAttribute.GetType();
                        foreach (var propertyName in customPropertyNames)
                        {
                            if (type?.GetProperty(propertyName) is PropertyInfo propertyInfo)
                            {
                                var value = propertyInfo.GetValue(customAttribute) as string;
                                if (!string.IsNullOrEmpty(value)) return value;
                            }
                        }
                    }
                }
                catch { }
                return memberInfo.Name;
            }
            public static object ConvertValueToPropertyInfo(string valueToConvert, PropertyInfo propertyInfo)
            {
                var type = propertyInfo.PropertyType;
                if (MapperConvert.TryGetValue(type, out var converter))
                {
                    return converter.Invoke(valueToConvert);
                }
                return valueToConvert;
            }
            public static bool IsValueEqual(object sourceValue, object destinationValue)
            {
                if (sourceValue is null)
                {
                    return destinationValue is null;
                }

                return sourceValue.Equals(destinationValue);
            }
        }
    }
}