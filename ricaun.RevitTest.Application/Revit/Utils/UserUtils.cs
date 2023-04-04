using Autodesk.Revit.UI;
using System;

namespace ricaun.RevitTest.Application.Revit.Utils
{
    public static class UserUtils
    {
        public static bool IsNotValid(UIApplication uiapp)
        {
            return !IsValid(uiapp);
        }
        public static bool IsValid(UIApplication uiapp)
        {
            return uiapp.Application.Username.Equals("ricaun");
        }
    }
}
