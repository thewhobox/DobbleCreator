using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DobbleCreator
{
    /// <summary>
    /// Interaktionslogik für ViewCards.xaml
    /// </summary>
    public partial class ViewCards : Window
    {
        public ObservableCollection<CardView> Cards { get; set; } = new ObservableCollection<CardView>();

        public ViewCards()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        public void SetCards(List<Card> cards, int maxSymbols)
        {
            Dictionary<int, BitmapImage> images = new Dictionary<int, BitmapImage>();
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;
            int fails = 0;

            var files = System.IO.Directory.GetFiles("Images/");
            files.Shuffle();

            int count = 0;
            foreach(string file in files)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(System.IO.Path.Combine(currentPath, file), UriKind.Absolute);
                img.EndInit();
                images.Add(count, img);
                count++;
            }

            Random rnd = new Random();
            foreach(Card card in cards)
            {
                /*bool flag = false;

                foreach (int numb in card.Numbers)
                    if (numb > maxSymbols)
                        flag = true;

                if (flag)
                {
                    fails++;
                    continue;
                }
                */

                CardView newCard = new CardView();
                card.Numbers.Shuffle();
                foreach (int numb in card.Numbers) {
                    double scale = 1.0;
                    int rotation = rnd.Next(0, 359);
                    newCard.Images.Add(new ImageInfo(images[numb], rotation, scale));
                }
                Cards.Add(newCard);
            }

            if(fails != 0)
            {
                MessageBox.Show("Es konnten " + fails + " Karten nicht erstellt werden...", "Fehler 0x08");
            }
        }
    }
}
