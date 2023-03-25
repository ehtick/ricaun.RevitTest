using NamedPipeWrapper.Json;
using ricaun.NUnit.Models;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ricaun.RevitTest.Shared
{
    public class TestResponse : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
        public bool IsBusy { get; set; } = true;
        public TestModel Test { get; set; }
        public TestAssemblyModel Tests { get; set; }
        public string Info { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}