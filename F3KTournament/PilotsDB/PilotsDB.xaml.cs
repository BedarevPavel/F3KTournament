using F3KTournament.Code;
using F3KTournament.Helpers;
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

namespace F3KTournament.PilotsDB
{
    /// <summary>
    /// Логика взаимодействия для PilotsDB.xaml
    /// </summary>
    public partial class PilotsDB : Window
    {
        public PilotsDBViewModel vm;
        public PilotsDB()
        {
            InitializeComponent();
            vm = (PilotsDBViewModel)this.DataContext;

            searchBox.GotFocus +=RemoveText;
            searchBox.LostFocus += AddText;
            searchBox.Text = "Введите ФИО, город или номер для поиска...";
            searchBox.TextChanged += TextBox_TextChanged;
        }

        public void RemoveText(object sender, EventArgs e)
        {
            searchBox.TextChanged -= TextBox_TextChanged;
            searchBox.Text = "";
            searchBox.TextChanged += TextBox_TextChanged;
        }

        public void AddText(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(searchBox.Text))
            {
                searchBox.TextChanged -= TextBox_TextChanged;
                searchBox.Text = "Введите ФИО, город или номер для поиска...";
                searchBox.TextChanged += TextBox_TextChanged;
            }
        }

        //void OnComboboxTextChanged(object sender, RoutedEventArgs e)
        //{
        //    CB.IsDropDownOpen = true;
        //    // убрать selection, если dropdown только открылся
        //    var tb = (TextBox)e.OriginalSource;
        //    tb.Select(tb.SelectionStart + tb.SelectionLength, 0);
        //    CollectionView cv = (CollectionView)CollectionViewSource.GetDefaultView(CB.ItemsSource);
        //    cv.Filter = s =>
        //        ((string)s).IndexOf(CB.Text, StringComparison.CurrentCultureIgnoreCase) >= 0;
        //}

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (vm.IsDataChanged)
            {
                var dRes = MessageBox.Show($"Сохранить внесённые изменеия?", "", MessageBoxButton.YesNo, MessageBoxImage.Question);
                if (dRes == MessageBoxResult.Yes)
                {
                    var pilotsXMLPath = System.IO.Path.Combine(Common.AppDataCatalogPath, Constants.AppFiles.AllPilots);
                    SavedDataHelper.SaveToXML(pilotsXMLPath, vm.PilotsList);
                }
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var vm = (PilotsDBViewModel)this.DataContext;
            vm.LoadPilotList();
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = searchBox.Text.ToLower();
            vm.PilotsList = vm.PilotsSource.Where(p => 
                                                p.FIO.ToLower().Contains(searchText) ||
                                                p.City.ToLower().Contains(searchText) ||
                                                p.ID_FAI.ToLower().Contains(searchText) ||
                                                p.LicNum.ToLower().Contains(searchText))
                                          .ToList();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            Close();
        }
    }
}
