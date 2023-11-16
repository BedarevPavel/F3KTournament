using F3KTournament.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
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
    /// Interaction logic for PrintTaskScores.xaml
    /// </summary>
    public partial class PrintTaskScores : Window
    {
        private ObservableCollection<Task> _taskList;
        public PrintTaskScores(ObservableCollection<DataTable> tasksScoresList, int currentRound, ObservableCollection<Task> taskList)
        {
            InitializeComponent();

            _taskList = taskList;
            RoundScoresDDL.ItemsSource = tasksScoresList;
            RoundScoresDDL.DisplayMemberPath = "TableName";
            RoundScoresDDL.SelectedValuePath = "TableName";
            RoundScoresDDL.SelectedValue = currentRound;
        }

        private void InitDoc(DataTable dTable)
        {
            var task = _taskList.Single(t => t.Round == int.Parse(dTable.TableName));

            pTable.RowGroups.Clear();
            pTable.Columns.Clear();

            DocViewer.Document.TextAlignment = TextAlignment.Center;

            var colMaxLength = MainHelper.MeasureString(Constants.Columns.TotalTime);

            //Head
            var headRow = new TableRow();
            //Добавляем столбцы
            for (int i = 0; i < dTable.Columns.Count; i++)
            {
                var col = dTable.Columns[i];

                if (col.ColumnName == Constants.Columns.Group || col.ColumnName == Constants.Columns.Judge)
                {
                    continue;
                }

                var pCol = new TableColumn();
                pTable.Columns.Add(pCol);

                var uiBlock = new BlockUIContainer();
                uiBlock.Padding = new Thickness(0, 5, 0, 5);
                var textBlock = new TextBlock();
                textBlock.HorizontalAlignment = HorizontalAlignment.Center;
                textBlock.VerticalAlignment = VerticalAlignment.Center;
                //textBlock.TextWrapping = TextWrapping.NoWrap;

                if (col.ColumnName.Contains(Constants.Columns.SubTask))
                {
                    var pair = col.ColumnName.Split('-');
                    var indx = 0;
                    int.TryParse(pair[1], out indx);
                    textBlock.Text = task.SubTasks[indx - 1].Title;
                }
                else
                {
                    textBlock.Text = col.ColumnName;
                }

                Grid grid = new Grid();
                grid.Height = colMaxLength.Width + 50;
                grid.VerticalAlignment = VerticalAlignment.Center;
                RowDefinition rowDefin = new RowDefinition();
                rowDefin.Height = new GridLength(1.0, GridUnitType.Star);
                grid.RowDefinitions.Add(rowDefin);
                grid.Children.Add(textBlock);

                uiBlock.Child = grid;
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
                    pCol.Width = new GridLength(maxSize.Width + 10);
                }

                var cell = new TableCell(uiBlock);
                cell.TextAlignment = TextAlignment.Center;
                cell.BorderThickness = new Thickness(0, 2, 1, 0);
                cell.BorderBrush = Brushes.Black;

                headRow.Cells.Add(cell);
            }

            var headRowsGroup = new TableRowGroup();
            headRowsGroup.Background = Brushes.LightGray;
            headRowsGroup.FontWeight = FontWeights.Bold;
            headRowsGroup.FontFamily = new FontFamily("Verdana");
            headRowsGroup.Rows.Add(headRow);

            //Добавляем номет тура название задания перед таблицей

            var titleRowsGroup = new TableRowGroup();
            titleRowsGroup.FontWeight = FontWeights.Bold;
            var titleRow = new TableRow();

            var titleCell = new TableCell(new Paragraph(new Run($"Тур:{dTable.TableName} - {task.Description}") { FontSize = 18 })) { Padding = new Thickness(0, 15, 0, 15) };
            titleCell.ColumnSpan = headRow.Cells.Count;
            titleCell.BorderThickness = new Thickness(0, 2, 1, 0);
            titleCell.BorderBrush = Brushes.Black;
            titleRow.Cells.Add(titleCell);
            titleRowsGroup.Rows.Add(titleRow);
            pTable.RowGroups.Add(titleRowsGroup);

            //Вставляем шапку таблицы
            pTable.RowGroups.Add(headRowsGroup);

            //Группируем данные в таблице по группам пилотов
            var groupedDt = dTable.AsEnumerable().GroupBy(x => x.Field<string>(Constants.Columns.Group)).OrderBy(k => k.Key).ToList();

            for (int g = 0; g < groupedDt.Count; g++)
            {
                //Добавляем раделительную строку с номером группы
                var groupRowsGroup = new TableRowGroup();
                groupRowsGroup.Background = Brushes.LightGray;
                groupRowsGroup.FontWeight = FontWeights.Bold;
                pTable.RowGroups.Add(groupRowsGroup);
                var groupRow = new TableRow();
                var spanCell = new TableCell(new Paragraph(new Run($"Группа №{groupedDt[g].Key}"))) { Padding = new Thickness(0, 3, 0, 3) };
                spanCell.ColumnSpan = headRow.Cells.Count;
                spanCell.BorderThickness = new Thickness(0, 2, 1, 0);
                spanCell.BorderBrush = Brushes.Black;
                groupRow.Cells.Add(spanCell);
                groupRowsGroup.Rows.Add(groupRow);

                //Добавляем группу для текущих строк 
                var bodyRowsGroup = new TableRowGroup();
                bodyRowsGroup.FontFamily = new FontFamily("Verdana");
                bodyRowsGroup.FontSize = 12;
                pTable.RowGroups.Add(bodyRowsGroup);

                List<DataRow> rows = groupedDt[g].OrderByDescending(r=>r[Constants.Columns.Scores]).ToList();

                for (int i = 0; i < rows.Count; i++)
                {
                    var nRow = new TableRow();
                    for (int j = 0; j < dTable.Columns.Count; j++)
                    {
                        var col = dTable.Columns[j];

                        if (col.ColumnName == Constants.Columns.Group || col.ColumnName == Constants.Columns.Judge)
                        {
                            continue;
                        }

                        var val = rows[i][col].ToString();
                        if (col.ColumnName.Contains(Constants.Columns.SubTask) || col.ColumnName.Contains(Constants.Columns.TotalTime))
                        {
                            val = MainHelper.ConvertSecToTimeString(val);
                        }

                        var p = new Paragraph();
                        p.Inlines.Add(new TextBlock()
                        {
                            Text = val,
                            TextWrapping = TextWrapping.NoWrap,
                            VerticalAlignment = VerticalAlignment.Center
                        });

                        var cell = new TableCell(p);
                        cell.BorderThickness = new Thickness(0, 2, 1, 0);
                        cell.BorderBrush = Brushes.Black;

                        if (col.ColumnName == Constants.Columns.Pilot)
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

        private void RoundScoresDDL_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            try
            {
                var selectedTable = e.AddedItems[0] as DataTable;
                InitDoc(selectedTable);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Возникла Ошибка в процессе формирования страницы: {ex}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
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
    }
}
