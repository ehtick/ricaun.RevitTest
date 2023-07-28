using Autodesk.Revit.UI;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace ricaun.RevitTest.Tests
{
    public class TestsAsync
    {
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
            Console.WriteLine(InContext(uiapp));
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
