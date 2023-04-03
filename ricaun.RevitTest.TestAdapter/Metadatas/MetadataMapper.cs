using ricaun.RevitTest.TestAdapter.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    public static class MetadataMapper
    {
        private static bool IsPropertyInfoEqualAttribute(AssemblyMetadataAttribute attribute, PropertyInfo propertyInfo)
        {
            var keys = attribute.Key.Split('.').Reverse();
            var name = propertyInfo.Name;

            foreach (var key in keys)
            {
                if (key.Equals(name, StringComparison.InvariantCultureIgnoreCase) == false)
                    return false;
                name = propertyInfo.DeclaringType.Name;
                if (name.EndsWith("Model")) name = name.Substring(0, name.Length - 5);
            }

            return true;
        }
        private static object GetValue(AssemblyMetadataAttribute attribute, PropertyInfo propertyInfo)
        {
            Debug.WriteLine(propertyInfo.PropertyType);
            var type = propertyInfo.PropertyType;
            if (type == typeof(int))
            {
                if (int.TryParse(attribute.Value, out int value))
                    return value;
            }
            else if (type == typeof(double))
            {
                try
                {
                    return double.Parse(attribute.Value.Replace(',', '.'), System.Globalization.CultureInfo.InvariantCulture);
                }
                catch { }
            }
            else if (type == typeof(bool))
            {
                var value = attribute.Value.ToLower();
                return value.Equals("true");
            }
            else if (type == typeof(XmlBool))
            {
                var value = attribute.Value.ToLower();
                return (XmlBool)value.Equals("true");
            }
            return attribute.Value;
        }
        private static bool IsValueEqual(object sourceValue, object destinationValue)
        {
            if (sourceValue is null)
            {
                return destinationValue is null;
            }

            return sourceValue.Equals(destinationValue);
        }
        public static T Map<T>(IEnumerable<AssemblyMetadataAttribute> attributes, T destination) where T : class
        {
            if (attributes == null)
                return destination;

            Type destinationObjectType = destination.GetType();
            PropertyInfo[] destinationPropList = destinationObjectType.GetProperties();

            foreach (PropertyInfo destinationPropInfo in destinationPropList)
            {
                if (destinationPropInfo.GetGetMethod().IsVirtual) continue;
                foreach (AssemblyMetadataAttribute attribute in attributes)
                {
                    if (IsPropertyInfoEqualAttribute(attribute, destinationPropInfo))
                    {
                        try
                        {
                            var sourceValue = GetValue(attribute, destinationPropInfo);
                            var destinationValue = destinationPropInfo.GetValue(destination, null);
                            if (!IsValueEqual(sourceValue, destinationValue))
                            {
                                Debug.WriteLine($"{attribute.Key}: {sourceValue} >> {destinationPropInfo.Name}: {destinationValue} | {!IsValueEqual(sourceValue, destinationValue)}");
                                destinationPropInfo.SetValue(destination, sourceValue, null);
                            }
                            break;
                        }
                        catch { }
                    }
                }
            }

            return destination;
        }
    }
}
