using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ricaun.RevitTest.TestAdapter.Models
{
    public class TestAssemblyModel
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public int SuccessHate { get; set; }
        public TestTypeModel[] Tests { get; set; }
        public int TestCount { get; set; }
        public string Name { get; set; }
        public object Alias { get; set; }
        public object FullName { get; set; }
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
        public string Console { get; set; }
        public double Time { get; set; }
    }

    public class TestTypeModel
    {
        public TestModel[] Tests { get; set; }
        public int TestCount { get; set; }
        public int SuccessHate { get; set; }
        public string Name { get; set; }
        public string Alias { get; set; }
        public string FullName { get; set; }
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
        public string Console { get; set; }
        public double Time { get; set; }
    }

    public class TestModel
    {
        public string Name { get; set; }
        public string Alias { get; set; }
        public string FullName { get; set; }
        public bool Success { get; set; }
        public bool Skipped { get; set; }
        public string Message { get; set; }
        public string Console { get; set; }
        public double Time { get; set; }
    }

}
