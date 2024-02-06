using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.UI;
using ricaun.NUnit;
using ricaun.NUnit.Models;
using ricaun.RevitTest.Shared;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class ApsApplicationPipeTest
    {
        public static TestAssemblyModel ApsApplicationCheckTest(UIApplication uiapp, PipeTestServer PipeTestServer, TestRequest testRequest)
        {
            try
            {
                var testPathFile = testRequest.TestPathFile;
                var testFilterNames = testRequest.TestFilters ?? new string[] { };

                // Revit User not connected.
                if (Autodesk.Revit.ApplicationServices.Application.IsLoggedIn == false)
                {
                    var exceptionNeedLoggedIn = new Exception("There is no user connected to Revit.");
                    Log.WriteLine(exceptionNeedLoggedIn.Message);
                    PipeTestServer.Update((response) =>
                    {
                        response.Info = exceptionNeedLoggedIn.Message;
                    });

                    var isPreviewRelease = ApplicationPreviewUtils.IsPreviewRelease();
                    if (isPreviewRelease)
                    {
                        const string PreviewReleaseMessage = "Preview Release allow Revit user not logged.";
                        Log.WriteLine(PreviewReleaseMessage);
                        PipeTestServer.Update((response) =>
                        {
                            response.Info = PreviewReleaseMessage;
                        });
                    }
                    else
                    {
                        return TestEngine.Fail(testPathFile, exceptionNeedLoggedIn, testFilterNames);
                    }
                }

                // Request user to connect with 'ricaun.Auth' and Autodesk Platform Service.
                if (ApsApplication.IsConnected == false)
                {
                    const string RequestUserToConnecteMessage = "The user is not connected with 'ricaun.Auth' and Autodesk Platform Service. 'ricaun.Auth' opened in Revit and waiting for the login...";
                    Log.WriteLine(RequestUserToConnecteMessage);

                    PipeTestServer.Update((response) =>
                    {
                        response.Info = RequestUserToConnecteMessage;
                    });

                    ApsApplicationView.OpenApsView(true);
                }

                // Check user is connected with 'ricaun.Auth' and Autodesk Platform Service.
                if (ApsApplication.IsConnected == false)
                {
                    const string ErrorUserNotConnectedMessage = "The user is not connected with 'ricaun.Auth' and Autodesk Platform Service.";
                    Log.WriteLine(ErrorUserNotConnectedMessage);

                    var exceptionNeedAuth = new Exception(ErrorUserNotConnectedMessage);
                    return TestEngine.Fail(testPathFile, exceptionNeedAuth, testFilterNames);
                }

                // Validation Aps Application
                if (ApsApplication.IsConnected == true)
                {
                    var task = Task.Run(async () =>
                    {
                        return await ApsApplicationCheck.Check();
                    });
                    var apsResponse = task.GetAwaiter().GetResult();
                    if (apsResponse is not null && apsResponse.isValid == false)
                    {
                        var exceptionNotValid = new Exception($"The user is not valid, {apsResponse?.message}");
                        return TestEngine.Fail(testPathFile, exceptionNotValid, testFilterNames);
                    }
                }

                // Validate the Aps User is equals to the Revit User
                if (ApsApplication.IsConnected == true)
                {
                    //var task = Task.Run(async () =>
                    //{
                    //    return await ApsApplication.ApsApplication.EnsureApsUserHaveOpenId();
                    //});
                    //var userHaveOpenId = task.GetAwaiter().GetResult();
                    //if (userHaveOpenId == false)
                    //{
                    //    var exceptionUserWithoutOpenId = new Exception($"The user was disconnected, reconnect the user to 'ricaun.Auth' and Autodesk Platform Service.");
                    //    return TestEngine.Fail(message.TestPathFile, exceptionUserWithoutOpenId, testFilterNames);
                    //}

                    var revitLoginUserId = uiapp.Application.LoginUserId;
                    var apsLoginUserId = ApsApplication.LoginUserId;
                    var isSameUserId = apsLoginUserId.Equals(revitLoginUserId);
                    var isRevitLoginUserIdEmpty = string.IsNullOrEmpty(revitLoginUserId);

                    if (isRevitLoginUserIdEmpty)
                    {
                        Log.WriteLine($"Revit Login UserId is empty.");
                    }

                    if (isSameUserId == false && isRevitLoginUserIdEmpty == false)
                    {
                        const string UserConnectedIsDifferentMessage = "The user connected to Revit is different from the user connected to 'ricaun.Auth'.";

                        Log.WriteLine($"ApsApplication: '{revitLoginUserId}' != '{apsLoginUserId}'");
                        Log.WriteLine(UserConnectedIsDifferentMessage);

                        var exceptionDifferentLoginUserId = new Exception(UserConnectedIsDifferentMessage);
                        return TestEngine.Fail(testPathFile, exceptionDifferentLoginUserId, testFilterNames);
                    }
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
