using F3KTournament.DataModel;
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
using System.Windows.Shapes;

namespace F3KTournament.PrintElements
{
    /// <summary>
    /// Interaction logic for PrintTeamScores.xaml
    /// </summary>
    public partial class PrintTeamScores : Window
    {
        public PrintTeamScores(List<TeamScore> data)
        {
            InitializeComponent();

            DocViewer.Document.TextAlignment = TextAlignment.Center;

            foreach (var team in data)
            {
                var rowsGroup = new TableRowGroup();
                rowsGroup.Background = Brushes.LightGray;
                rowsGroup.FontWeight = FontWeights.Bold;
                rowsGroup.FontFamily = new FontFamily("Verdana");
                pTable.RowGroups.Add(rowsGroup);

                var headRow = new TableRow();

                var uiBlock = new BlockUIContainer();
                uiBlock.Padding = new Thickness(0, 5, 0, 5);
                var textBlock = new TextBlock();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                //textBlock.TextWrapping = TextWrapping.NoWrap;
                textBlock.Text = $"{team.Place} - {team.Team} ({team.TotalScores})";

                Grid grid = new Grid();
                //grid.Height = 40;
                grid.VerticalAlignment = VerticalAlignment.Center;
                RowDefinition rowDefin = new RowDefinition();
                rowDefin.Height = new GridLength(1.0, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDefin);
                grid.Children.Add(textBlock);

                uiBlock.Child = grid;

                var cell = new TableCell(uiBlock);

                cell.ColumnSpan = 2;
                cell.BorderThickness = new Thickness(0, 2, 1, 0);
                cell.BorderBrush = Brushes.Black;

                headRow.Cells.Add(cell);

                rowsGroup.Rows.Add(headRow);

                var bodyRowsGroup = new TableRowGroup();
                bodyRowsGroup.FontFamily = new FontFamily("Verdana");
                bodyRowsGroup.FontSize = 12;
                pTable.RowGroups.Add(bodyRowsGroup);

                foreach (var pilot in team.Pilots)
                {
                    var nRow = new TableRow();

                    var textBlock2 = new TextBlock()
                    {
                        Text = pilot.Pilot.FullName,
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    var p = new Paragraph();
                    p.Inlines.Add(textBlock2);

                    var cellP = new TableCell(p);
                    cellP.BorderThickness = new Thickness(0, 2, 1, 0);
                    cellP.Padding = new Thickness(3, 5, 3, 5);
                    cellP.BorderBrush = Brushes.Black;

                    nRow.Cells.Add(cellP);

                    var textBlock3 = new TextBlock()
                    {
                        Text = pilot.TotalScores.ToString(),
                        VerticalAlignment = VerticalAlignment.Center,
                    };

                    var p2 = new Paragraph();
                    p2.Inlines.Add(textBlock3);

                    var cellS = new TableCell(p2);
                    cellS.BorderThickness = new Thickness(0, 2, 1, 0);
                    cellS.Padding = new Thickness(3, 5, 3, 5);
                    cellS.BorderBrush = Brushes.Black;

                    nRow.Cells.Add(cellS);

                    bodyRowsGroup.Rows.Add(nRow);
                }

            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            PrintDialog printDialog = new PrintDialog();
            if (printDialog.ShowDialog() == true)
            {
                this.Hide();
                FlowDocument doc = DocViewer.Document;
                doc.PageHeight = printDialog.PrintableAreaHeight;
                doc.PageWidth = printDialog.PrintableAreaWidth;
                doc.ColumnWidth = printDialog.PrintableAreaWidth;
                doc.ColumnGap = 0;


                printDialog.PrintDocument(
                    ((IDocumentPaginatorSource)doc).DocumentPaginator,
                    "A Flow Document");
                this.Close();
            }
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
