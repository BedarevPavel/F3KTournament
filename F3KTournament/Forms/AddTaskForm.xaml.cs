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

namespace F3KTournament.Forms
{
    /// <summary>
    /// Interaction logic for AddTaskForm.xaml
    /// </summary>
    public partial class AddTaskForm : Window
    {
        public List<Task> SelectedTasks = new List<Task>();
        public AddTaskForm(List<Task> tasks)
        {
            InitializeComponent();
            TasksGrid.ItemsSource = tasks;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWindow = curApp.MainWindow;
            this.Left = mainWindow.Left + (mainWindow.Width - this.ActualWidth) / 2;
            this.Top = mainWindow.Top + (mainWindow.Height - this.ActualHeight) / 2;
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            if (TasksGrid.SelectedItems.Count > 0)
            {
                foreach (var task in TasksGrid.SelectedItems) 
                {
                    SelectedTasks.Add(task as Task);
                }
                Close();
            }
            else
            {
                MessageBox.Show("Не выбрано задание", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
            }

        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
