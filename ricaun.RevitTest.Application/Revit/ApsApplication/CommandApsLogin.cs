using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    [Transaction(TransactionMode.Manual)]
    public class CommandApsLogin : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            ApsApplication.Login().RunAndForget();

            return Result.Succeeded;
        }
    }

    [Transaction(TransactionMode.Manual)]
    public class CommandApsLogout : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            ApsApplication.Logout().RunAndForget();

            return Result.Succeeded;
        }
    }
}
