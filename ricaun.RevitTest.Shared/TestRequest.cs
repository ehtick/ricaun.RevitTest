using NamedPipeWrapper.Json;
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
        public string TestPathFile { get; set; }
        public string TestFilter { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
}