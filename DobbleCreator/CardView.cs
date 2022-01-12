using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace DobbleCreator
{
    public class CardView : INotifyPropertyChanged
    {
        public int Index { get; set; }
        public int MaxColumns
        {
            get { return (int) Math.Ceiling(Math.Sqrt(Images.Count)); }
        }
        public ObservableCollection<ImageInfo> Images { get; set; } = new ObservableCollection<ImageInfo>();
        public event PropertyChangedEventHandler PropertyChanged;

        public CardView()
        {
            Images.CollectionChanged += Numbers_CollectionChanged;
        }

        private void Numbers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("MaxColumns"));
        }
    }
}
