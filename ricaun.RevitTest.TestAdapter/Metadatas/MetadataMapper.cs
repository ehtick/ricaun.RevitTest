using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace ricaun.RevitTest.TestAdapter.Metadatas
{
    internal static class MetadataMapper
    {
        public static T Map<T>(T destination, IEnumerable<AssemblyMetadataAttribute> attributes) where T : class
        {
            if (attributes is null)
                return destination;

            var metadataDictionary = attributes
                .ToDictionary(e => e.Key, e => e.Value, StringComparer.OrdinalIgnoreCase);

            return MapperKey.Map(destination, metadataDictionary);
        }
    }
}
