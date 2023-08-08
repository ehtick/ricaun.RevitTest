using Autodesk.Revit.UI;
using NUnit.Framework;
using ricaun.Revit.Async;
using ricaun.Revit.Async.Timeout;
using System;
using System.Threading.Tasks;

[assembly: System.Reflection.AssemblyDescription("{\"TestAsync\":\"TestAsync\",\"TimeOut\":60.0}")]

namespace ricaun.RevitTest.Tests
{
    public class TestRevitTask
    {
        [Test]
        public void Initialize()
        {
            RevitTask.Initialize();
        }

        [Test]
        public async Task TestAsync_MacroManager()
        {
            await RevitTask.Run((uiapp) =>
            {
                uiapp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.MacroManager));
                uiapp.DialogBoxShowing += DialogBoxShowingForceClose;
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
                    await RevitTaskTimeout.RunAsync((uiapp) =>
                    {
                        TaskDialog.Show("Show", "Close me");
                    });
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
