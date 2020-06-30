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

namespace DobbleCreator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged
    {
        public List<int> availibleChars;

        public ObservableCollection<Card> Cards { get; set; } = new ObservableCollection<Card>();

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
            availibleChars = new List<int>();
            for (int i = count; i <= MaxSymbols; i++)
                availibleChars.Add(i);

            try
            {
                int c = 1;

                Card firstcard = new Card();
                for (int i = 1; i <= count; i++)
                    firstcard.Add(i);
                Cards.Add(firstcard);

                int next = count+1;
                for (int i = 0; i <= count-1; i++)
                {
                    Card card = new Card();
                    card.Add(1);

                    for (int i2 = 1; i2 <= count - 1; i2++)
                    {
                        card.Add((count - 1) + (count - 1) * (i - 1) + (i2 + 1));
                    }

                    //for (int x = 0; x < count - 1; x++)
                    //{
                    //    card.Add(next);
                    //    next++;
                    //}
                    Cards.Add(card);
                    c++;
                }

                for (int set = 1; set <= count-1; set++) // Sets durchgehen
                {
                    for (int sIndex = 1; sIndex <= count-1; sIndex++) //Für das Setz x Karten erstellen
                    {
                        Card card = new Card();
                        card.Add(set + 1);



                        for (int numb = 1; numb <= count - 1; numb++)
                        {

                            int xyz;
                            xyz = (count + 1) + ((count - 1) * (numb - 1)) + (((set - 1) * (numb - 1) + (sIndex - 1)) % (count - 1));


                            Debug.WriteLine($"{c}: {set} - {sIndex} - {numb} = {xyz}");

                            card.Add(xyz);
                        }

                        //for (int index = 1; index < count; index++) //Für die Karte x stellen auffüllen
                        //{
                        //    int charac = GetFreeNumb(card);
                        //    if (charac == -1)
                        //    {
                        //        MessageBox.Show("Fehler in der Berechnung!", "Fehler 0x04");
                        //        Cards.Add(card);
                        //        return;
                        //    }
                        //    card.Add(charac);
                        //}

                        Cards.Add(card);
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






            //List<Card> firstrow = new List<Card>();
            //int next = count;
            //for(int i = 0; i < count-1; i++)
            //{
            //    Card card = new Card();
            //    card.AddIcon(0);

            //    for (int x = 0; x < count - 1; x++)
            //    {
            //        card.AddIcon(next);
            //        next++;
            //    }
            //    firstrow.Add(card);
            //}

            //foreach (Card card in firstrow)
            //    Cards.Add(card);

            //for (int i = 1; i < count; i++)
            //{
            //    for (int x = 1; x < count; x++)
            //    {
            //        Card card = new Card();
            //        card.AddIcon(i);

            //        for(int y = 1; y < count; y++)
            //        {
            //            card.Icons.Add(firstrow[x-1].Icons[y-1]);
            //        }
            //    }
            //}


        }


        private int GetFreeNumb(Card card)
        {
            int output = -1;

            List<int> usedChars = new List<int>();
            foreach (int ch in card.Numbers)
                if (!usedChars.Contains(ch))
                    usedChars.Add(ch);

            List<int> toAdd = new List<int>();
            foreach(int usedCh in usedChars)
            {
                IEnumerable<Card> cards = Cards.Where(c => c.Numbers.Contains(usedCh));
                foreach (Card c in cards)
                    foreach (int ch in c.Numbers)
                        if (!usedChars.Contains(ch) && !toAdd.Contains(ch))
                            toAdd.Add(ch);
            }
            usedChars.AddRange(toAdd);

            int charac = -1;
            try
            {
                 charac = availibleChars.Last(ch => !usedChars.Contains(ch));
            }
            catch { }

            return charac;
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
    }
}
