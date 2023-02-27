using System;
using System.Collections.Generic;
using System.Linq;

namespace Autodesk.Revit.DB
{
    /// <summary>
    /// RevitParameters
    /// </summary>
    public static class RevitParameters
    {
        private static List<object> recordParameters = new List<object>();

        /// <summary>
        /// AddParameter
        /// </summary>
        /// <param name="parameters"></param>
        public static void AddParameter(params object[] parameters)
        {
            foreach (var parameter in parameters)
                recordParameters.Add(parameter);
        }

        /// <summary>
        /// Parameters
        /// </summary>
        public static object[] Parameters => GetParameters(recordParameters.ToArray());

        private static object[] GetParameters(params object[] parameters)
        {
            var list = new List<object>();
            list.AddRange(parameters);
            return list.ToArray();
        }
    }
}
