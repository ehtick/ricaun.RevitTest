using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.Revit.UI;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;

namespace ricaun.RevitTest.Application.Revit.Application
{
    public static class ApplicationValidUser
    {
        /// <summary>
        /// Check if user is valid before run tests.
        /// </summary>
        /// <param name="PipeTestServer"></param>
        /// <param name="testRequest"></param>
        /// <returns>Return null if is valid user.</returns>
        /// <remarks>Check if user is loggin or if is running in the PreviewRelease.</remarks>
        public static TestAssemblyModel ApplicationCheckTest(PipeTestServer PipeTestServer, TestRequest testRequest)
        {
            try
            {
                var testPathFile = testRequest.TestPathFile;
                var testFilterNames = testRequest.TestFilters ?? new string[] { };

                var isPreviewReleaseOrLoggedIn = RevitApplicationPreview.IsPreviewReleaseOrLoggedIn;
                if (isPreviewReleaseOrLoggedIn)
                {
                    var isPreviewRelease = RevitApplicationPreview.IsPreviewRelease;
                    if (isPreviewRelease)
                    {
                        const string PreviewReleaseMessage = "Preview Release allow Revit user not logged.";
                        Log.WriteLine(PreviewReleaseMessage);
                        PipeTestServer.Update((response) =>
                        {
                            response.Info = PreviewReleaseMessage;
                        });
                    }
                    return null;
                }
                else
                {
                    var exceptionNeedLoggedIn = new Exception("There is no user connected to Revit.");

                    Log.WriteLine(exceptionNeedLoggedIn.Message);
                    PipeTestServer.Update((response) =>
                    {
                        response.Info = exceptionNeedLoggedIn.Message;
                    });

                    return TestEngine.Fail(testPathFile, exceptionNeedLoggedIn, testFilterNames);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
            return null;
        }
    }
}
