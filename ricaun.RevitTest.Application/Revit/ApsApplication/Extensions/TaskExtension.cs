using System.Threading.Tasks;

namespace ricaun.RevitTest.Application.Revit.ApsApplication
{
    public static class TaskExtension
    {
        public static async void RunAndForget(this Task task)
        {
            try
            {
                await task;
            }
            catch { }
        }
    }
}
