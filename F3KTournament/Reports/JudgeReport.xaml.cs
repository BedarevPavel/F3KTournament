using System;
using System.Collections.Generic;
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

namespace F3KTournament.Reports
{
    /// <summary>
    /// Interaction logic for JudgeReport.xaml
    /// </summary>
    public partial class JudgeReport : Window
    {
        public JudgeReport(DataTable ReportData)
        {
            InitializeComponent();
            this.reportGrid.ItemsSource = ReportData.DefaultView;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void reportGrid_AutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (!e.Column.Header.ToString().Contains(Constants.Columns.Pilot))
            {
                e.Column.CellStyle = (Style)FindResource("CenterCellStyle");
            }
        }
    }
}
