using Microsoft.Win32;
using OfficeOpenXml;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using F3KTournament.Helpers;
using F3KTournament.ViewModel;
using System.Collections.Specialized;
using System.Data;
using F3KTournament.DataModel;
using System.ComponentModel;
using System.Reflection;
using F3KTournament.Code;
using F3KTournament.ValueConverters;

namespace F3KTournament
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }


        #region http://www.dotnetcurry.com/wpf/677/wpf-data-grid-row-drag-drop

        //private void TasksGrid_Drop(object sender, DragEventArgs e)
        //{
        //    if (rowIndex < 0)
        //        return;
        //    int index = this.GetCurrentRowIndex(e.GetPosition);
        //    if (index < 0)
        //        return;
        //    if (index == rowIndex)
        //        return;
        //    if (index == productsDataGrid.Items.Count - 1)
        //    {
        //        MessageBox.Show("This row-index cannot be drop");
        //        return;
        //    }
        //    ProductCollection productCollection = Resources["ProductList"] as ProductCollection;
        //    Product changedProduct = productCollection[rowIndex];
        //    productCollection.RemoveAt(rowIndex);
        //    productCollection.Insert(index, changedProduct);
        //}



        //private void TasksGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        //{
        //    rowIndex = GetCurrentRowIndex(e.GetPosition);
        //    if (rowIndex < 0)
        //        return;
        //    productsDataGrid.SelectedIndex = rowIndex;
        //    Product selectedEmp = productsDataGrid.Items[rowIndex] as Product;
        //    if (selectedEmp == null)
        //        return;
        //    DragDropEffects dragdropeffects = DragDropEffects.Move;
        //    if (DragDrop.DoDragDrop(productsDataGrid, selectedEmp, dragdropeffects)
        //                        != DragDropEffects.None)
        //    {
        //        productsDataGrid.SelectedItem = selectedEmp;
        //    }
        //}

        //private bool GetMouseTargetRow(Visual theTarget, GetPosition position)
        //{
        //    Rect rect = VisualTreeHelper.GetDescendantBounds(theTarget);
        //    Point point = position((IInputElement)theTarget);
        //    return rect.Contains(point);
        //}

        //private DataGridRow GetRowItem(int index)
        //{
        //    if (productsDataGrid.ItemContainerGenerator.Status
        //            != GeneratorStatus.ContainersGenerated)
        //        return null;
        //    return productsDataGrid.ItemContainerGenerator.ContainerFromIndex(index)
        //                                                    as DataGridRow;
        //}

        //private int GetCurrentRowIndex(GetPosition pos)
        //{
        //    int curIndex = -1;
        //    for (int i = 0; i < productsDataGrid.Items.Count; i++)
        //    {
        //        DataGridRow itm = GetRowItem(i);
        //        if (GetMouseTargetRow(itm, pos))
        //        {
        //            curIndex = i;
        //            break;
        //        }
        //    }
        //    return curIndex;
        //}

        //private void InitAppData()
        //{
        //    //_pilotList = new List<Pilot>();

        //    // если есть сохранённые данные, то в таблицу подгружаем их
        //    // в таком случае необходимо реализовать возможность добавлять упражнения из справочника

        //    var excelHelper = new ExcelHelper();
        //    _tasksList = excelHelper.GetTasksList();
        //    //заполним туры
        //    for (int i = 1; i <= _tasksList.Count; i++)
        //    {
        //        _tasksList[i - 1].Round = i;
        //    }
        //}
        #endregion

        private void NumGroupsTextBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            try
            {
                Convert.ToInt32(e.Text);
            }
            catch
            {
                ((TextBox)sender).Text = "1";
                e.Handled = true;
            }
        }

        private bool isManualEditCommit;
        private void DataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (!isManualEditCommit)
            {
                isManualEditCommit = true;
                DataGrid grid = (DataGrid)sender;
                grid.CommitEdit(DataGridEditingUnit.Row, true);
                isManualEditCommit = false;
            }
        }
        private void DataGrid_TaskScoresAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var colHeader = e.Column.Header.ToString();
            if (!colHeader.Contains(Constants.Columns.Pilot))
            {
                e.Column.CellStyle = (Style)FindResource("CenterCellStyle");
            }
            else
            {
                e.Column.CellStyle = (Style)FindResource("ReadOnlyCellStyle");
            }

            // меняем стили ячеек которые можно редактировать
            if (colHeader.IndexOf(Constants.Columns.SubTask) > -1)
            {
                e.Column.CellStyle = (Style)FindResource("CanEditCellStyle");
            }

            if (colHeader.IndexOf(Constants.Columns.SubTask) > -1
             || colHeader.IndexOf(Constants.Columns.TotalTime) > -1)
            {
                var converter = new RoundSCoreValueConverter();
                DataGridTextColumn dgtc = e.Column as DataGridTextColumn;
                (dgtc.Binding as Binding).Converter = converter;
            }

            if (colHeader.IndexOf(Constants.Columns.SubTask) == -1 && colHeader.IndexOf(Constants.Columns.Penalty) == -1)
            {
                e.Column.IsReadOnly = true;
            }

            if (colHeader.ToString().IndexOf(Constants.Columns.Group) > -1)
            {
                e.Column.Visibility = Visibility.Hidden;
            }

            var dataGrid = sender as CustomDataGrid;
            if (dataGrid != null)
            {
                var source = dataGrid.ItemsSource as BindingListCollectionView;
                if (source != null)
                {
                    var dataView = source.SourceCollection as DataView;
                    if (dataView != null)
                    {
                        var table = dataView.Table;
                        var columnsHeaders = dataGrid.ColumnHeaders;
                        var currentTaskHeaders = columnsHeaders.SingleOrDefault(t => t.TaskIndex == table.Namespace);
                        if (currentTaskHeaders != null)
                        {
                            var currentHeader = e.Column.Header.ToString();
                            if (currentTaskHeaders.Headers.ContainsKey(currentHeader))
                            {
                                e.Column.Header = currentTaskHeaders.Headers[currentHeader];
                            }
                        }
                    }
                }


            }
        }

        private void DataGrid_TotalScoresAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var colHeader = e.Column.Header.ToString();
            if (!colHeader.Contains(Constants.Columns.Pilot))
            {
                e.Column.CellStyle = (Style)FindResource("CenterCellStyle");
            }

            if (colHeader == Constants.Columns.Percent)
            {
                DataGridTextColumn col = e.Column as DataGridTextColumn;
                col.Binding = new Binding(e.PropertyName) { StringFormat = "{0}%" };
            }

            var dataGrid = sender as CustomDataGrid;
            if (dataGrid != null)
            {
                var columnsHeaders = dataGrid.ColumnHeaders;
                var currentHeader = e.Column.Header.ToString();
                if (columnsHeaders[0].Headers.ContainsKey(currentHeader))
                {
                    e.Column.Header = columnsHeaders[0].Headers[currentHeader];
                    var col = e.Column as DataGridTextColumn;
                    col.ElementStyle = (Style)FindResource("ForseScoreStyle");
                }
            }
        }

        private void flightMatrixGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            var colHeader = e.Column.Header.ToString();
            if (!colHeader.Contains(Constants.Columns.Pilot))
            {
                e.Column.CellStyle = (Style)FindResource("CenterCellStyle");
            }
        }

        private void DataGrid_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Key.Delete == e.Key)
            {
                var msgRes = MessageBox.Show("Действительно хотите удалить пилота?", "Удаление пилота", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (msgRes != MessageBoxResult.Yes)
                {
                    e.Handled = true;
                }
            }
        }

        private void Tabs_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0)
            {
                if (e.RemovedItems[0] is TabItem tab && tab.Name == "PilotsTab")
                {
                    var pilots = pilotsDg.ItemsSource as ObservableCollection<Pilot>;
                    var tmpPilots = new List<string>();
                    foreach (var p in pilots)
                    {
                        if (tmpPilots.Any(t => t == p.FullName))
                        {
                            MessageBox.Show($"ФИО пилотов должно быть уникальным,\nповторяется ФИО:{p.FullName}", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
                            e.Handled = true;
                            return;
                        }
                        else
                        {
                            tmpPilots.Add(p.FullName);
                        }
                    }
                }
            }
        }
    }
}
