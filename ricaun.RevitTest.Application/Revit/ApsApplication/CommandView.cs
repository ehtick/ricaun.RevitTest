using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using ricaun.Auth.Aps.UI;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    [Transaction(TransactionMode.Manual)]
    public class CommandView : IExternalCommand, IExternalCommandAvailability
    {
        private static ApsView apsView;
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elementSet)
        {
            UIApplication uiapp = commandData.Application;

            if (apsView is null)
            {
                apsView = new ApsView(Autodesk.Windows.ComponentManager.ApplicationWindow);
                apsView.SetApsConfiguration(ApsApplication.ClientId, ApsApplication.ClientPort, ApsApplication.ClientScopes);
                apsView.Closed += (s, e) => { apsView = null; };
                apsView.Show();
            }
            apsView?.Activate();

            return Result.Succeeded;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            return true;
        }
    }
}
