using System;
using System.Diagnostics;
using System.Reflection;

namespace ricaun.RevitTest.Shared.Mappers
{
    /// <summary>
    /// Mapper
    /// </summary>
    public static class Mapper
    {
        /// <summary>
        /// PropertyInfoEqual
        /// </summary>
        /// <param name="sourcePropInfo"></param>
        /// <param name="destinationPropInfo"></param>
        /// <returns></returns>
        public delegate bool PropertyInfoEqual(PropertyInfo sourcePropInfo, PropertyInfo destinationPropInfo);
        /// <summary>
        /// ValueEqual
        /// </summary>
        /// <param name="sourceValue"></param>
        /// <param name="destinationValue"></param>
        /// <returns></returns>
        public delegate bool ValueEqual(object sourceValue, object destinationValue);
        /// <summary>
        /// NotifyPropertyChanged
        /// </summary>
        /// <param name="propertyName"></param>
        /// <param name="value"></param>
        public delegate void NotifyPropertyChanged(string propertyName, object value);
        /// <summary>
        /// MapPropertyInfoEqual
        /// </summary>
        public static PropertyInfoEqual MapPropertyInfoEqual { get; set; } = IsPropertyInfoEqual;
        /// <summary>
        /// MapValueEqual
        /// </summary>
        public static ValueEqual MapValueEqual { get; set; } = IsValueEqual;
        /// <summary>
        /// Map
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="destination"></param>
        /// <param name="notifyPropertyChanged"></param>
        /// <returns></returns>
        public static T Map<T>(object source, T destination, NotifyPropertyChanged notifyPropertyChanged = null) where T : class
        {
            if (source == null)
                return destination;

            Type destinationObjectType = destination.GetType();
            PropertyInfo[] destinationPropList = destinationObjectType.GetProperties();

            Type sourceObjectType = source.GetType();
            PropertyInfo[] sourcePropList = sourceObjectType.GetProperties();

            foreach (PropertyInfo destinationPropInfo in destinationPropList)
            {
                if (destinationPropInfo.GetGetMethod().IsVirtual) continue;
                foreach (PropertyInfo sourcePropInfo in sourcePropList)
                {
                    if (MapPropertyInfoEqual(sourcePropInfo, destinationPropInfo))
                    {
                        try
                        {
                            var sourceValue = sourcePropInfo.GetValue(source, null);
                            var destinationValue = destinationPropInfo.GetValue(destination, null);
                            if (!MapValueEqual(sourceValue, destinationValue))
                            {
                                Debug.WriteLine($"Map: {sourcePropInfo.Name}: {sourceValue} >> {destinationPropInfo.Name}: {destinationValue} | {!IsValueEqual(sourceValue, destinationValue)}");
                                destinationPropInfo.SetValue(destination, sourceValue, null);
                                notifyPropertyChanged?.Invoke(destinationPropInfo.Name, sourceValue);
                            }
                            break;
                        }
                        catch { }
                    }
                }
            }

            return destination;
        }
        private static bool IsPropertyInfoEqual(PropertyInfo sourcePropInfo, PropertyInfo destinationPropInfo)
        {
            return sourcePropInfo.Name.IndexOf(destinationPropInfo.Name, StringComparison.InvariantCultureIgnoreCase) != -1;
        }
        private static bool IsValueEqual(object sourceValue, object destinationValue)
        {
            if (sourceValue is null)
            {
                return destinationValue is null;
            }

            return sourceValue.Equals(destinationValue);
        }
    }
}