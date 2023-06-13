using System;
using System.Collections.Generic;
using System.Linq;

namespace ricaun.RevitTest.TestAdapter.Models
{
    /// <summary>
    /// TestAssemblyModel
    /// </summary>
    public class TestAssemblyModel : TestModel
    {
        /// <summary>
        /// FileName
        /// </summary>
        public string FileName { get; set; }
        /// <summary>
        /// Version
        /// </summary>
        public string Version { get; set; }
        /// <summary>
        /// SuccessHate
        /// </summary>
        public double SuccessHate => GetSuccessHate();

        /// <summary>
        /// Tests
        /// </summary>
        public List<TestTypeModel> Tests { get; set; } = new();

        /// <summary>
        /// Test Count
        /// </summary>
        public int TestCount => Tests.SelectMany(e => e.Tests).Count();

        private double GetSuccessHate()
        {
            var tests = Tests.SelectMany(e => e.Tests);
            var count = tests.Count();
            var success = Success ? 1.0 : 0;
            return count == 0 ? success : Math.Round(tests.Where(e => e.Success).Count() / (double)count, 2);
        }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name} {TestCount} {Success} {Math.Round(SuccessHate * 100, 2)}% {Time}ms";
        }

        /// <summary>
        /// Show in the Console
        /// </summary>
        public string AsString()
        {
            var text = $"{this}\n";
            foreach (var test in this.Tests)
            {
                text += $"\t{test}\n";
                foreach (var t in test.Tests)
                {
                    text += $"\t\t{t}\n";
                }
            }
            return text;
        }
    }

    /// <summary>
    /// TestTypeModel
    /// </summary>
    public class TestTypeModel : TestModel
    {
        /// <summary>
        /// Tests
        /// </summary>
        public List<TestModel> Tests { get; set; } = new();

        /// <summary>
        /// Test Count
        /// </summary>
        public int TestCount => Tests.Count;

        /// <summary>
        /// SuccessHate
        /// </summary>
        public double SuccessHate => GetSuccessHate();
        private double GetSuccessHate()
        {
            var count = Tests.Count;
            var success = Success ? 1.0 : 0;
            return count == 0 ? success : Math.Round(Tests.Where(e => e.Success).Count() / (double)count, 2);
        }
    }

    /// <summary>
    /// Test Model
    /// </summary>
    public class TestModel
    {
        /// <summary>
        /// Test Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Test Alias
        /// </summary>
        public string Alias { get; set; }

        /// <summary>
        /// Test FullName
        /// </summary>
        public string FullName { get; set; }

        /// <summary>
        /// Test Success?
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// Test Skipped
        /// </summary>
        public bool Skipped { get; set; } = false;

        /// <summary>
        /// Message Output
        /// </summary>
        public string Message { get; set; } = "";

        /// <summary>
        /// Console Output
        /// </summary>
        public string Console { get; set; } = "";

        /// <summary>
        /// Time in milliseconds
        /// </summary>
        public double Time { get; set; }

        /// <summary>
        /// ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var result = Skipped ? "Skipped" : Success ? "Passed" : "Failed";
            var name = string.IsNullOrEmpty(Alias) ? Name : Alias != Name ? $"{Alias}[{Name}]" : Alias;
            var message = Message;
            //var message = (!Message.Contains("Exception:")) ? Message : Message.Substring(0, Message.IndexOf("Exception:") + "Exception".Length);
            return $"{name}\t {result}\t {message}".Replace("\n", " ").Replace("\r", " ");
        }
    }
}
