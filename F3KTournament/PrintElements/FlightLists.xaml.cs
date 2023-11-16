using F3KTournament.Code;
using F3KTournament.PrintElements.Controls;
using F3KTournament.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace F3KTournament.PrintElements
{
    /// <summary>
    /// Interaction logic for FlightLists.xaml
    /// </summary>
    public partial class FlightLists : Window
    {
        Document document;

        internal Document Document
        {
            get { return document; }
        }

        public Canvas PageCanvas
        {
            get { return Container; }
        }

        public int CardPerPage { get; set; }

        public FlightLists(List<FlightCardViewModel> cardList)
        {
            InitializeComponent();
            document = new Document();

            var pageInxdex = 1;
            document.AddPage(0);
            foreach (var card in cardList)
            {
                var fc = new FlightCard() { DataContext = card };
                if (document.CurrentPage.CardList.Count == 2)
                {
                    document.AddPage(pageInxdex);
                    pageInxdex++;
                }
                    
                document.CurrentPage.CardList.Add(fc);
            }
            
            document.CurrentPageIndex = 0;
            DrawPage();
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();

            if (printDialog.ShowDialog() == true)
            {
                DocumentPaginator paginator = new Paginator(this);
                printDialog.PrintDocument(paginator, "F3K Tournament - полётные листы");
                DrawPage();
            }
        }


        private void DrawPage()
        {
            DrawPage(PageCanvas);
        }

        public void DrawPage(Canvas canvas)
        {
            canvas.Children.Clear();
            Code.Page page = document.CurrentPage;
            int rowHeight = 0;
            foreach (var card in page.CardList)
            {
                Canvas.SetLeft(card, 42);
                Canvas.SetTop(card, 30 + rowHeight);
                if (card.Parent != null)
                {
                    var parent = (Canvas)card.Parent;
                    parent.Children.Remove(card);

                }
                canvas.Children.Add(card);
                rowHeight += 520;
            }

            //var boder = new Border();
            //boder.BorderBrush = Brushes.Black;
            //boder.BorderThickness = new Thickness(3);
            //boder.Width = 793;
            //boder.Height = 20;
            //Canvas.SetLeft(boder, 0);
            //Canvas.SetBottom(boder, 0);
            //canvas.Children.Add(boder);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            document.CurrentPageIndex--;
            currentPageTextBox.Text = (document.CurrentPageIndex + 1).ToString();
            DrawPage();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            document.CurrentPageIndex++;
            currentPageTextBox.Text = (document.CurrentPageIndex + 1).ToString();
            DrawPage();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Application curApp = Application.Current;
            //Window mainWindow = curApp.MainWindow;
            //this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            //this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
            double screenWidth = System.Windows.SystemParameters.PrimaryScreenWidth;
            double screenHeight = System.Windows.SystemParameters.PrimaryScreenHeight;
            double windowWidth = this.Width;
            double windowHeight = this.Height;
            this.Left = (screenWidth / 2) - (windowWidth / 2);
            this.Top = (screenHeight / 2) - (windowHeight / 2);
        }
    }
}
