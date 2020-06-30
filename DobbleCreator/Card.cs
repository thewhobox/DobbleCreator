using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text;

namespace DobbleCreator
{
    public class Card : INotifyPropertyChanged
    {
        public ObservableCollection<int> Numbers { get; set; } = new ObservableCollection<int>();
        public event PropertyChangedEventHandler PropertyChanged;

        public string NumberText
        {
            get {
                List<string> xy = new List<string>();

                foreach (int numb in Numbers)
                {
                    string o = "";

                    for (int i = 0; i < MainWindow.MaxNumber.ToString().Length - numb.ToString().Length; i++)
                        o += "0";

                    xy.Add(o + numb);
                }
                return string.Join(" ", xy); 
            }
        }

        public Card()
        {
            Numbers.CollectionChanged += Numbers_CollectionChanged;
        }

        private void Numbers_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs("NumberText"));
        }

        public void Add(int chara)
        {
            Numbers.Add(chara);
        }
    }
}
