using F3KTournament.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace F3KTournament.Code
{
    public class CustomDataGrid : DataGrid
    {
        public List<TaskHeaders> ColumnHeaders
        {
            get { return (List<TaskHeaders>)GetValue(ColumnHeadersProperty); }
            set { SetValue(ColumnHeadersProperty, value); }
        }

        public static readonly DependencyProperty ColumnHeadersProperty = DependencyProperty.Register("ColumnHeaders", typeof(List<TaskHeaders>), typeof(CustomDataGrid), new FrameworkPropertyMetadata(null));


    }
}
