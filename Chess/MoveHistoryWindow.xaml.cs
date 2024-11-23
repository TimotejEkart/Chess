using System.Collections.ObjectModel;
using System.Windows;

namespace Chess
{
    public partial class MoveHistoryWindow : Window
    {
        public MoveHistoryWindow(ObservableCollection<Move> moveHistory)
        {
            InitializeComponent();
            MoveHistoryDataGrid.ItemsSource = moveHistory;
        }
    }
}
