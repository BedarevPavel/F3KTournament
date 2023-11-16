using F3KTournament.Code;
using F3KTournament.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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
    /// Interaction logic for PrintTable.xaml
    /// </summary>
    public partial class PrintDataTabe : Window
    {
        public PrintDataTabe(DataTable dTable, string title, TaskHeaders taskHeaders = null, bool shortFio = false)
        {
            InitializeComponent();

            this.Title = title;

            //calc max col text length
            double maxColTextWidth = 0;
            foreach (DataColumn col in dTable.Columns)
            {
                if (shortFio && col.ColumnName == "Пилот")
                {
                    for (int i = 0; i < dTable.Rows.Count; i++)
                    {
                        var val = dTable.Rows[i][col]?.ToString();
                        var varArr = val.Split(' ');
                        dTable.Rows[i][col] = varArr[0];
                    }

                }
                var l = MainHelper.MeasureString(col.ColumnName, true);
                if (l.Width > maxColTextWidth)
                {
                    maxColTextWidth = l.Width;
                }
            }


            DocViewer.Document.TextAlignment = TextAlignment.Center;
            var headRowsGroup = new TableRowGroup();
            headRowsGroup.Background = Brushes.LightGray;
            headRowsGroup.FontWeight = FontWeights.Bold;
            headRowsGroup.FontFamily = new FontFamily("Verdana");
            pTable.RowGroups.Add(headRowsGroup);

            var headRow = new TableRow();
            //Добавляем столбцы
            for (int i = 0; i < dTable.Columns.Count; i++)
            {
                var col = dTable.Columns[i];

                var pCol = new TableColumn();
                pTable.Columns.Add(pCol);

                var uiBlock = new BlockUIContainer();
                uiBlock.Padding = new Thickness(0, 5, 0, 5);
                var textBlock = new TextBlock();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                //textBlock.TextWrapping = TextWrapping.NoWrap;
                textBlock.Text = col.ColumnName;

                Grid grid = new Grid();
                grid.Height = maxColTextWidth + 30;
                grid.VerticalAlignment = VerticalAlignment.Center;
                RowDefinition rowDefin = new RowDefinition();
                rowDefin.Height = new GridLength(1.0, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDefin);
                grid.Children.Add(textBlock);

                uiBlock.Child = grid;
                //uiBlock.Child = textBlock;
                if (col.ColumnName != Constants.Columns.Num && col.ColumnName != Constants.Columns.Pilot)
                {
                    textBlock.LayoutTransform = new RotateTransform(-90);
                }

                if (col.ColumnName == Constants.Columns.Pilot)
                {
                    int maxLength = 0;
                    string maxLengthStr = string.Empty;
                    //Определяем самое длинное имя
                    for (int r = 0; r < dTable.Rows.Count; r++)
                    {
                        var row = dTable.Rows[r];
                        string tStr = row[col.ColumnName].ToString();
                        if (tStr.Length > maxLength)
                        {
                            maxLength = tStr.Length;
                            maxLengthStr = tStr;
                        }
                    }
                    //Расчитываем ширину текста
                    var maxSize = MainHelper.MeasureString(maxLengthStr);
                    if (shortFio)
                    {
                        pCol.Width = new GridLength(maxSize.Width - 10);
                    }
                    else
                    {
                        pCol.Width = new GridLength(maxSize.Width + 10);
                    }
                    
                }

                var cell = new TableCell(uiBlock);

                cell.BorderThickness = new Thickness(0, 2, 1, 0);
                cell.BorderBrush = Brushes.Black;

                headRow.Cells.Add(cell);
            }

            headRowsGroup.Rows.Add(headRow);

            var bodyRowsGroup = new TableRowGroup();
            bodyRowsGroup.FontFamily = new FontFamily("Verdana");
            bodyRowsGroup.FontSize = 12;
            pTable.RowGroups.Add(bodyRowsGroup);

            for (int i = 0; i < dTable.Rows.Count; i++)
            {
                var nRow = new TableRow();
                for (int j = 0; j < dTable.Columns.Count; j++)
                {
                    var val = dTable.Rows[i][j].ToString();
                    //if (dTable.Columns[j].ColumnName == Constants.Columns.Pilot)
                    //{
                    //    var res = string.Empty;
                    //    //Из полной ФИО оставляем только фамилию и инициалы
                    //    var fio = val.Split(' ');
                    //    res = fio[0];
                    //    for (int f = 1; f < fio.Length; f++)
                    //    {
                    //        res = res + $" {fio[f][0]}.";
                    //    }
                    //    val = res;
                    //}
                    var textBlock = new TextBlock()
                    {
                        Text = val,
                        VerticalAlignment = VerticalAlignment.Center,
                    };
                    if (taskHeaders != null && taskHeaders.Headers.ContainsKey(dTable.Columns[j].ColumnName))
                    {
                        if (val.Contains('('))
                        {
                            textBlock.TextDecorations = TextDecorations.Strikethrough;
                        }
                    }
                    var p = new Paragraph();
                    p.Inlines.Add(textBlock);



                    var cell = new TableCell(p);
                    cell.BorderThickness = new Thickness(0, 2, 1, 0);
                    cell.BorderBrush = Brushes.Black;

                    if (dTable.Columns[j].ColumnName == Constants.Columns.Pilot)
                    {
                        cell.TextAlignment = TextAlignment.Left;
                    }
                    else
                    {
                        cell.TextAlignment = TextAlignment.Center;
                    }
                    cell.Padding = new Thickness(3, 5, 3, 5);
                    nRow.Cells.Add(cell);
                }

                bodyRowsGroup.Rows.Add(nRow);
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
