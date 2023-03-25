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
        public int Id { get; set; }
        public string TestPathFile { get; set; }
        public override string ToString()
        {
            return this.JsonSerialize();
        }
    }
    //public class TestRequest
    //{
    //    public int Id { get; set; }
    //    public string TestPathFile { get; set; }
    //    public override string ToString()
    //    {
    //        return this.JsonSerialize();
    //    }
    //}
}