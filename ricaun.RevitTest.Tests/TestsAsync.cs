using Autodesk.Revit.UI;
using NUnit.Framework;
using ricaun.Revit.Async;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Tests
{
    public class TestRevitTask
    {
        [Test]
        public void Initialize()
        {
            RevitTask.Initialize();
        }
    }

    public class TestsAsync
    {
        [Test]
        public async Task TestAsync_MacroManager()
        {
            await RevitTask.Run((uiapp) =>
            {
                uiapp.PostCommand(RevitCommandId.LookupPostableCommandId(PostableCommand.MacroManager));
            });
            var inContext = await RevitTask.Run((uiapp) => { return InContext(uiapp); });
            Assert.IsTrue(inContext);
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
