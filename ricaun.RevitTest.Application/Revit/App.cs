using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using NamedPipeWrapper;
using Revit.Busy;
using ricaun.NUnit;
using ricaun.Revit.Async;
using ricaun.Revit.UI;
using System;

namespace ricaun.RevitTest.Application.Revit
{
    [AppLoader]
    public class App : IExternalApplication
    {
        public App()
        {
            CosturaUtility.Initialize();
        }

        private static RibbonPanel ribbonPanel;
        private static RibbonItem ribbonItem;
        public Result OnStartup(UIControlledApplication application)
        {
            TestEngine.Initialize();

            RevitBusyControl.Initialize(application);
            RevitBusyControl.Control.PropertyChanged += RevitBusyControlPropertyChanged;
            RevitTask.Initialize(application);

            RevitParameters.AddParameter(
                application,
                application.ControlledApplication,
                application.GetUIApplication(),
                application.ControlledApplication.GetApplication());

            ribbonPanel = application.CreatePanel("");
            ribbonItem = ribbonPanel.CreatePushButton<Commands.Command>("RevitTest");
            UpdateLargeImageBusy(ribbonItem, RevitBusyControl.Control);

#if DEBUG
            ribbonPanel.GetRibbonPanel().CustomPanelTitleBarBackground = System.Windows.Media.Brushes.Salmon;
#endif

            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            ribbonPanel?.Remove();
            return Result.Succeeded;
        }

        private void RevitBusyControlPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var control = sender as RevitBusyService;
            UpdateLargeImageBusy(ribbonItem, control);
        }

        private static void UpdateLargeImageBusy(RibbonItem ribbonItem, RevitBusyService control)
        {
            if (ribbonItem is null) return;
            const string LargeImageIsBusy = "/UIFrameworkRes;component/ribbon/images/close.ico";
            const string LargeImageNoBusy = "/UIFrameworkRes;component/ribbon/images/add.ico";
            if (control.IsRevitBusy)
                ribbonItem.SetLargeImage(LargeImageIsBusy);
            else
                ribbonItem.SetLargeImage(LargeImageNoBusy);
        }

    }

}