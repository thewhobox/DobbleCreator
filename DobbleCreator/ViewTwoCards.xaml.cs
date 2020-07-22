using System;
using System.Collections.Generic;
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
    /// Interaktionslogik für ViewTwoCards.xaml
    /// </summary>
    public partial class ViewTwoCards : Window
    {
        private List<CardView> Cards { get; set; } = new List<CardView>();
        private Random rnd = new Random();

        public ViewTwoCards()
        {
            InitializeComponent();
        }

        public void SetCards(List<Card> cards)
        {
            Dictionary<int, BitmapImage> images = new Dictionary<int, BitmapImage>();
            string currentPath = AppDomain.CurrentDomain.BaseDirectory;

            var files = System.IO.Directory.GetFiles("Images/");
            files.Shuffle();

            int count = 0;
            foreach (string file in files)
            {
                BitmapImage img = new BitmapImage();
                img.BeginInit();
                img.UriSource = new Uri(System.IO.Path.Combine(currentPath, file), UriKind.Absolute);
                img.EndInit();
                images.Add(count, img);
                count++;
            }

            foreach (Card card in cards)
            {
                CardView newCard = new CardView();
                card.Numbers.Shuffle();
                foreach (int numb in card.Numbers)
                    newCard.Images.Add(images[numb]);
                Cards.Add(newCard);
            }
            NeuMischen();
        }

        private void RunShuffle(object sender, RoutedEventArgs e)
        {
            NeuMischen();
        }

        private void NeuMischen()
        {
            Solution.Source = null;

            int card1 = rnd.Next(0, Cards.Count - 1);
            int card2 = card1;

            while(card1 == card2)
            {
                card2 = rnd.Next(0, Cards.Count - 1);
            }

            Card1.DataContext = Cards[card1];
            Card2.DataContext = Cards[card2];
        }

        private void ShowSolution(object sender, RoutedEventArgs e)
        {
            CardView card1 = Card1.DataContext as CardView;
            CardView card2 = Card2.DataContext as CardView;


            for(int i = 0; i < card1.Images.Count; i++)
            {
                for (int x = 0; x < card2.Images.Count; x++)
                {
                    if(card1.Images[i] == card2.Images[x])
                    {
                        Solution.Source = card1.Images[i];
                        return;
                    }
                }
            }

            MessageBox.Show("Es wurde keine Übereinstimmung gefunden...", "Fehler 0x07");
        }
    }
}
