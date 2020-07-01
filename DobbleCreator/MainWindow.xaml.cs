using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Ribbon;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;


/// <summary>
/// Symbole sind von Icons8.de
/// </summary>



namespace DobbleCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<int> availibleChars;

        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();

        private int _progress = 0;
        public int Progress
        {
            get { return _progress; }
            set { _progress = value; Changed("Progress"); }
        }

        private int _maxCount = 0;
        public int MaxCount
        {
            get { return _maxCount; }
            set { _maxCount = value; MaxNumber = value; Changed("MaxCount"); }
        }

        private int _maxCols = 0;
        public int MaxCols
        {
            get { return _maxCols; }
            set { _maxCols = value; Changed("MaxCols"); }
        }

        private int _maxSymbols = 0;
        public int MaxSymbols
        {
            get { return _maxSymbols; }
            set { _maxSymbols = value; Changed("MaxSymbols"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void Changed(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }


        public static int MaxNumber = 0;



        public MainWindow()
        {
            InitializeComponent();
            this.DataContext = this;
        }

        private void RunCreate(object sender, RoutedEventArgs e)
        {
            Cards.Clear();
            BtnTest.IsEnabled = false;
            BtnShow.IsEnabled = false;
            Progress = 0;

            bool isInt = int.TryParse(InCount.Text, out int count);
            if (!isInt)
            {
                MessageBox.Show("Die eingegebene Anzahl an Symbolen ist keine richtige Zahl!", "Fehler 0x01");
                return;
            }
            Debug.WriteLine("Prim: " + IsPrime(count - 1).ToString());
            Debug.WriteLine("Base: " + IsBasePrime(count - 1).ToString());
            if (!IsPrime(count-1) && !IsBasePrime(count-1))
            {
                MessageBoxResult result = MessageBox.Show("Für die Zahl kann kein endlicher Körper erstellt werden. ", "Fehler 0x0" + (!IsPrime(count-1) ? "2":"3"), MessageBoxButton.OKCancel);
                if(result == MessageBoxResult.Cancel)
                    return;
            }
            MaxCount = (count * (count-1)) + 1;
            MaxCols = count - 1;
            MaxSymbols = (int)Math.Pow(count - 1, 2) + count;
            VisProgress.Maximum = MaxCount;

            availibleChars = new List<int>();
            for (int i = count; i <= MaxSymbols; i++)
                availibleChars.Add(i);

            List<Card> tempCards = new List<Card>();

            Task.Run(() =>
            {
                try
                {
                    int c = 0;

                    //Card firstcard = new Card();
                    //for (int i = 1; i <= count; i++)
                    //    firstcard.Add(i);
                    //tempCards.Add(firstcard);
                    //Progress++;

                    int next = count + 1;
                    for (int i = 0; i <= count - 1; i++)
                    {
                        Card card = new Card() { Index = c };
                        card.Add(1);

                        for (int i2 = 1; i2 <= count - 1; i2++)
                        {
                            card.Add((count - 1) + (count - 1) * (i - 1) + (i2 + 1));
                        }
                        tempCards.Add(card);
                        Progress++;
                        c++;
                    }

                    for (int set = 1; set <= count - 1; set++) // Sets durchgehen
                    {
                        for (int sIndex = 1; sIndex <= count - 1; sIndex++) //Für das Setz x Karten erstellen
                        {
                            Card card = new Card() { Index = c };
                            card.Add(set + 1);

                            for (int numb = 1; numb <= count - 1; numb++)
                            {
                                int xyz = (count + 1) + ((count - 1) * (numb - 1)) + (((set - 1) * (numb - 1) + (sIndex - 1)) % (count - 1));
                                card.Add(xyz);
                            }
                            tempCards.Add(card);
                            Progress++;
                            c++;
                            Debug.WriteLine("---");
                        }
                    }
                }
                catch (ArgumentOutOfRangeException ex)
                {
                    MessageBox.Show("Für diese Karten sind nicht genügend Symbole vorhanden.\r\n" + $"Vorhanden: {availibleChars.Count} - Benötigt: {MaxSymbols}", "Fehler 0x05");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Es trat ein unbekannter Fehler auf:\r\n" + ex.Message, "Fehler 0x00");
                }

                Dispatcher.Invoke(() =>
                {
                    DisplayResult(tempCards);
                });
            });
            


            BtnTest.IsEnabled = true;
        }

        private void DisplayResult(List<Card> tempCards)
        {
            foreach (Card card in tempCards)
                Cards.Add(card);
        }



        private void RunTest(object sender, RoutedEventArgs e)
        {
            Progress = 0;

            int maxTests = 0;
            for (int i = Cards.Count; i > 1; i--)
                maxTests += i-1;

            VisProgress.Maximum = maxTests;

            Task.Run(() =>
            {
                for(int i = 0; i < Cards.Count; i++)
                {
                    Card cardA = Cards[i];

                    for(int x = i+1; x < Cards.Count; x++)
                    {
                        if(CompareNumbers(cardA, Cards[x]))
                        {
                            MessageBox.Show($"Test wurde nicht bestanden!\r\nKarte {i} mit Karte {x} haben mehr als eine übereinstimmung!", "Fehler 0x06");
                            return;
                        }
                        Progress++;
                    }
                }

                MessageBox.Show("Test wurde bestanden!", "Erfolgreich");
                Dispatcher.Invoke(() =>
                {
                    BtnShow.IsEnabled = true;
                });
            });
        }

        private bool CompareNumbers(Card a, Card b)
        {
            int sims = 0;

            foreach (int numb in a.Numbers)
                if (b.Numbers.Contains(numb))
                    sims++;

            if (sims != 1) return true;
            sims = 0;

            foreach (int numb in b.Numbers)
                if (a.Numbers.Contains(numb))
                    sims++;

            return sims != 1;
        }

        private bool IsPrime(int input)
        {
            if (input == 1) return true;
            int a = 0;
            for (int i = 1; i < input; i++)
            {
                if (input % i == 0)
                {
                    a++;
                }
            }
            return a == 1;
        }

        private bool IsBasePrime(int input)
        {
            int start = 2;
            int prime = 2;
            bool first = true;

            while (true)
            {
                double result = Math.Pow(prime, start);
                if (result.ToString().Contains(",")) return false;
                if (result > input)
                {
                    if (first)
                        return false;

                    bool flag = false;
                    while (!flag)
                    {
                        prime++;
                        flag = IsPrime(prime);
                    }
                    start = 2;
                    first = true;
                }
                else
                {
                    if (result == input) return true;

                    first = false;
                    start++;
                }
            }
        }

        private void RunShow(object sender, RoutedEventArgs e)
        {
            ViewCards diag = new ViewCards();
            diag.SetCards(Cards.ToList());
            diag.ShowDialog();
        }

        private void RunShow2(object sender, RoutedEventArgs e)
        {

        }
    }
}
