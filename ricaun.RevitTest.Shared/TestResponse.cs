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
        public string Text { get; set; }
        public TestModel Test { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }

    //public class TestResponse
    //{
    //    public bool IsBusy { get; set; } = true;
    //    public string Text { get; set; }
    //    public TestModel Test { get; set; }
    //    public override string ToString()
    //    {
    //        return this.JsonSerialize();
    //    }
    //}
}