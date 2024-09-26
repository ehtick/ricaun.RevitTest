using ricaun.NamedPipeWrapper.Json;
using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ricaun.RevitTest.Shared
{
    public class TestRequest : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged
        /// <summary>
        /// PropertyChanged
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] string propertyName = "") =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        #endregion
        public string Info { get; set; }
        public string[] TestFilters { get; set; }
        public string TestPathFile { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}