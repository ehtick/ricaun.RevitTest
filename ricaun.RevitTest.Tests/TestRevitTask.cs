using Autodesk.Revit.UI;
using NUnit.Framework;
using ricaun.Revit.UI.Tasks;
using System;
using System.Threading.Tasks;

[assembly: System.Reflection.AssemblyDescription("{\"TestAsync\":\"TestAsync\",\"TimeOut\":60.0}")]

namespace ricaun.RevitTest.Tests
{
    public class TestRevitTask
    {
        static IRevitTask RevitTask;
        [OneTimeSetUp]
        public void Initialize(UIApplication uiapp)
        {
            if (RevitTask is null)
            {
                var revitTask = new RevitTaskService(uiapp);
                revitTask.Initialize();
                RevitTask = revitTask;
            }
        }
        /// <summary>
        /// This method is required to initialize the RevitTask
        /// </summary>
        [Test]
        public void Test_Initialize()
        {
            Assert.IsNotNull(RevitTask);
        }

#if !(NET)
        [Test]
        public async Task TestAsync_MacroManager()
        {
            await RevitTask.Run((uiapp) =>
            {
                uiapp.DialogBoxShowing += DialogBoxShowingForceClose;
                uiapp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.MacroManager));
            });

            await RevitTask.Run((uiapp) =>
            {
                uiapp.DialogBoxShowing -= DialogBoxShowingForceClose;
            });

            var inContext = await RevitTask.Run((uiapp) => { return InContext(uiapp); });
            Assert.IsTrue(inContext);

        }
        private void DialogBoxShowingForceClose(object sender, Autodesk.Revit.UI.Events.DialogBoxShowingEventArgs e)
        {
            var uiapp = sender as UIApplication;
            uiapp.DialogBoxShowing -= DialogBoxShowingForceClose;
            Console.WriteLine($"DialogBoxShowing {e.DialogId}");
            e.OverrideResult((int)TaskDialogResult.Close);
        }
#endif
        [Test]
        public async Task TestAsync_Idling()
        {
            await RevitTask.Run((uiapp) =>
            {
                uiapp.Idling += Uiapp_Idling;
            });

            await RevitTask.Run((uiapp) =>
            {
                uiapp.Idling -= Uiapp_Idling;
            });

            Console.WriteLine(Index);
            Assert.AreEqual(1, Index);
        }

        /// <summary>
        /// This test gonna check if the `IsTestRunning` is working
        /// </summary>
        [Explicit]
        [TestCase(2)]
        public async Task TestAsync_Idling_Timeout(int length)
        {
            var timeout = 0;
            for (int i = 0; i < length; i++)
            {
                try
                {
                    var source = new System.Threading.CancellationTokenSource(200);
                    var cancellationToken = source.Token;
                    await Task.Delay(500);
                    await RevitTask.Run((uiapp) =>
                    {
                        // This never execute
                        TaskDialog.Show("Show", "Close me");
                    }, cancellationToken);
                }
                catch (Exception) { timeout++; }
            }
            Console.WriteLine(timeout);
            Assert.AreEqual(length, timeout);
        }

        private int Index;
        private void Uiapp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
        {
            Index++;
        }

        [Test]
        public async Task TestAsync()
        {
            await Task.Delay(1100);
            Console.WriteLine("Delay");
        }

        [Test]
        public async Task TestAsync(UIApplication uiapp)
        {
            await Task.Delay(100);
            var inContextShouldBeFalse = InContext(uiapp);
            Console.WriteLine(inContextShouldBeFalse);
            Assert.IsFalse(inContextShouldBeFalse);
            var inContext = await RevitTask.Run((app) => { return InContext(app); });
            Console.WriteLine(inContext);
            Assert.IsTrue(inContext);

        }

        private bool InContext(UIApplication uiapp)
        {
            void Uiapp_Idling(object sender, Autodesk.Revit.UI.Events.IdlingEventArgs e)
            {
            }
            try
            {
                uiapp.Idling += Uiapp_Idling;
                uiapp.Idling -= Uiapp_Idling;
                return true;
            }
            catch { }
            return false;
        }
    }
}
