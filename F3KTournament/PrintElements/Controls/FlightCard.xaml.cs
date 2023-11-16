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
using System.Windows.Threading;

namespace F3KTournament.PrintElements.Controls
{

    public static class ExtensionMethods

    {
        private static Action EmptyDelegate = delegate () { };


        public static void Refresh(this UIElement uiElement)
        {
            uiElement.Dispatcher.Invoke(DispatcherPriority.Render, EmptyDelegate);
        }
    }

    /// <summary>
    /// Interaction logic for FlightCard.xaml
    /// </summary>
    public partial class FlightCard : UserControl
    {
        public FlightCard()
        {
            InitializeComponent();
        }

        private void UserControl_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var ctx = this.DataContext as FlightCardViewModel;

            DrawResultTable(ctx.ColCount1, ctx.RowCount1, ResultGrid1);
            DrawResultTable(ctx.ColCount2, ctx.RowCount2, ResultGrid2);
        }

        private void DrawResultTable(int colCount, int rowCount, GridControl grid)
        {
            for (int i = 0; i < colCount; i++)
            {
                ColumnDefinition col = new ColumnDefinition();
                grid.ColumnDefinitions.Add(col);
            }

            var cellNum = 1;
            for (int i = 0; i < rowCount; i++)
            {
                RowDefinition headRow = new RowDefinition();
                headRow.Height = new GridLength(21);
                grid.RowDefinitions.Add(headRow);

                for (int j = 0; j < colCount; j++)
                {
                    TextBlock numText = new TextBlock();
                    numText.Text = cellNum.ToString();
                    numText.HorizontalAlignment = HorizontalAlignment.Center;
                    numText.VerticalAlignment = VerticalAlignment.Center;
                    numText.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);

                    var rowInx = i;
                    if (i > 0)
                    {
                        rowInx = i * 2;
                    }

                    Grid.SetRow(numText, rowInx);
                    Grid.SetColumn(numText, j);

                    grid.Children.Add(numText);

                    cellNum++;
                }

                RowDefinition row = new RowDefinition();
                grid.RowDefinitions.Add(row);
            }
        }
    }
}
