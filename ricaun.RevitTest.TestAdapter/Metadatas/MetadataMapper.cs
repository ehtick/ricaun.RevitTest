using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter.Metadatas
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
}
