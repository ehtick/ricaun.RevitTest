using Autodesk.Revit.UI;
using System;

namespace ricaun.RevitTest.Application.Revit.Utils
{
    /// <summary>
    /// DialogBoxResolver resolve the dialog box by DialogId and override the result to cancel.
    /// </summary>
    /// <remarks>The resolver only works before Revit initialize.</remarks>
    public class DialogBoxResolver : IDisposable
    {
        private readonly UIControlledApplication application;
        private readonly string[] TaskDialogIds;
        public DialogBoxResolver(UIControlledApplication application)
        {
            this.application = application;
            this.TaskDialogIds = new string[]
            {
                "TaskDialog_License_Current_Status_Demo_Mode",  // Viewer Mode
                "TaskDialog_DNSMTemplate"                       // Revit Warning
            };
        }

        /// <summary>
        /// Initialize the events to resolve the dialog box. 
        /// </summary>
        /// <remarks>The Initialize is ignored if the <see cref="Autodesk.Revit.UI.UIControlledApplication.IsLateAddinLoading"/>.</remarks>
        public virtual void Initialize()
        {
            if (this.application.IsLateAddinLoading) return;
            Dispose();
            this.application.DialogBoxShowing += OnDialogBoxShowing;
            this.application.ControlledApplication.ApplicationInitialized += OnApplicationInitialized;
        }

        /// <summary>
        /// Dispose the events.
        /// </summary>
        public virtual void Dispose()
        {
            this.application.DialogBoxShowing -= OnDialogBoxShowing;
            this.application.ControlledApplication.ApplicationInitialized -= OnApplicationInitialized;
        }

        private void OnDialogBoxShowing(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
        {
            if (TaskDialogIds is null) return;
            foreach (var dialogId in TaskDialogIds)
            {
                if (dialogId == e.DialogId)
                {
                    Console.WriteLine($"{nameof(DialogBoxResolver)}: [{e.DialogId}]");
                    e.OverrideResult((int)TaskDialogResult.Cancel);
                    return;
                }
            }
        }
        private void OnApplicationInitialized(object sender, Autodesk.Revit.DB.Events.ApplicationInitializedEventArgs e)
        {
            Dispose();
        }
    }

}