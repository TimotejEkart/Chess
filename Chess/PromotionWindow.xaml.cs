using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Chess
{
    public partial class PromotionWindow : Window
    {
        private PieceColor color;
        private int currentIndex;
        private PieceType[] pieces = { PieceType.Queen, PieceType.Rook, PieceType.Bishop, PieceType.Knight };

        public PieceType SelectedPieceType { get; private set; }

        public PromotionWindow(PieceColor color)
        {
            InitializeComponent();
            this.color = color;
            currentIndex = 0;
            UpdatePieceDisplay();
        }

        private void UpdatePieceDisplay()
        {
            var piece = pieces[currentIndex];
            PieceImage.Source = new BitmapImage(new Uri(GetPieceImagePath(piece)));
            PieceName.Text = piece.ToString();
        }

        private string GetPieceImagePath(PieceType piece)
        {
            return $"pack://application:,,,/Images/{color}_{piece}.png";
        }

        private void PreviousPiece_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = (currentIndex - 1 + pieces.Length) % pieces.Length;
            UpdatePieceDisplay();
        }

        private void NextPiece_Click(object sender, RoutedEventArgs e)
        {
            currentIndex = (currentIndex + 1) % pieces.Length;
            UpdatePieceDisplay();
        }

        private void Confirm_Click(object sender, RoutedEventArgs e)
        {
            SelectedPieceType = pieces[currentIndex];
            DialogResult = true;
        }

        protected override void OnClosing(System.ComponentModel.CancelEventArgs e)
        {
            if (DialogResult != true)
            {
                e.Cancel = true; 
                MessageBox.Show("You must select a piece to promote the pawn.");
            }
            base.OnClosing(e);
        }
    }
}
