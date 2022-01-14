using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
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
using PdfSharp;
using PdfSharp.Drawing;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

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
                CardView newCard = new CardView();
                card.Numbers.Shuffle();
                foreach (int numb in card.Numbers) {
                    double scale = rnd.Next(7,12) / 10.0;
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

        private async void DoExport(object sender, RoutedEventArgs e) {
            ProgExport.Maximum = Cards.Count();
            ProgExport.Value = 0;

            PdfDocument document = new PdfDocument();
            document.Info.Title = "Created with PDFsharp";

            if(!System.IO.Directory.Exists("temp")) {
                System.IO.Directory.CreateDirectory("temp");
            }

            for(int i = 0; i < Cards.Count; i++) {
                if(i == 110) break;

                PdfPage page = document.AddPage();
                page.Width = "65mm"; //223;
                page.Height = "97mm"; //344;

                RenderTargetBitmap renderTarget = new RenderTargetBitmap(446*3, 666*3, 300, 300, PixelFormats.Pbgra32);
                VisualBrush sourceBrush = new VisualBrush(cardList.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem);
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(446, 666446*1,492)));
                drawingContext.Close();

                renderTarget.Render(drawingVisual);

                byte[] imageArray;
                using(FileStream stream = new FileStream($"temp/{i}.jpg", FileMode.OpenOrCreate)){
                    JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
                    jpgEncoder.QualityLevel = 100;
                    jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));
                    using (MemoryStream outputStream = new MemoryStream())
                    {
                        jpgEncoder.Save(outputStream);
                        imageArray = outputStream.ToArray();
                    }
                    foreach (byte digit in imageArray)
                    {
                        stream.WriteByte(digit);
                    }
                    stream.Close();
                }

                XGraphics gfx = XGraphics.FromPdfPage(page);
                XImage image = XImage.FromFile($"temp/{i}.jpg");
                gfx.DrawImage(image, 0, 0, 180, 269);


                ProgExport.Value = i+1;
                await System.Threading.Tasks.Task.Delay(10);
            }





            document.Save("test.pdf");
        }
    }
}
