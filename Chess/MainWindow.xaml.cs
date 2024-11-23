using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;

namespace Chess
{

    public partial class MainWindow : Window
    {
        private Board board;
        private ChessPiece selectedPiece;
        private (int row, int column)? selectedPosition;
        private PieceColor currentPlayer;
        private ObservableCollection<Move> moveHistory;
        private int currentMoveNumber = 1;
        private string currentTheme = "Default";

        public MainWindow()
        {
            InitializeComponent();
            board = new Board();
            moveHistory = new ObservableCollection<Move>();
            SetupInitialBoard();
            currentPlayer = PieceColor.White;
            CurrentPlayerText.Text = $"{currentPlayer}'s Turn";
        }

        private void SetupInitialBoard()
        {
            board.ClearBoard();
            ChessBoard.Children.Clear();

            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    var square = new Border
                    {
                        Background = GetSquareBackground(i, j, currentTheme),
                        BorderBrush = Brushes.Black,
                        BorderThickness = new Thickness(1),
                        Name = $"{(char)('A' + j)}{8 - i}"
                    };

                    square.MouseDown += Square_MouseDown;
                    ChessBoard.Children.Add(square);
                }
            }

            PlacePiece(new ChessPiece(PieceType.Rook, PieceColor.White), 7, 0);
            PlacePiece(new ChessPiece(PieceType.Knight, PieceColor.White), 7, 1);
            PlacePiece(new ChessPiece(PieceType.Bishop, PieceColor.White), 7, 2);
            PlacePiece(new ChessPiece(PieceType.Queen, PieceColor.White), 7, 3);
            PlacePiece(new ChessPiece(PieceType.King, PieceColor.White), 7, 4);
            PlacePiece(new ChessPiece(PieceType.Bishop, PieceColor.White), 7, 5);
            PlacePiece(new ChessPiece(PieceType.Knight, PieceColor.White), 7, 6);
            PlacePiece(new ChessPiece(PieceType.Rook, PieceColor.White), 7, 7);

            for (int i = 0; i < 8; i++)
            {
                PlacePiece(new ChessPiece(PieceType.Pawn, PieceColor.White), 6, i);
            }

            PlacePiece(new ChessPiece(PieceType.Rook, PieceColor.Black), 0, 0);
            PlacePiece(new ChessPiece(PieceType.Knight, PieceColor.Black), 0, 1);
            PlacePiece(new ChessPiece(PieceType.Bishop, PieceColor.Black), 0, 2);
            PlacePiece(new ChessPiece(PieceType.Queen, PieceColor.Black), 0, 3);
            PlacePiece(new ChessPiece(PieceType.King, PieceColor.Black), 0, 4);
            PlacePiece(new ChessPiece(PieceType.Bishop, PieceColor.Black), 0, 5);
            PlacePiece(new ChessPiece(PieceType.Knight, PieceColor.Black), 0, 6);
            PlacePiece(new ChessPiece(PieceType.Rook, PieceColor.Black), 0, 7);

            for (int i = 0; i < 8; i++)
            {
                PlacePiece(new ChessPiece(PieceType.Pawn, PieceColor.Black), 1, i);
            }
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to start a new game?", "New Game", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                SetupInitialBoard();
                currentPlayer = PieceColor.White;
                CurrentPlayerText.Text = $"{currentPlayer}'s Turn";
                moveHistory.Clear();
                currentMoveNumber = 1;
            }
        }

        private void MoveHistoryButton_Click(object sender, RoutedEventArgs e)
        {
            var moveHistoryWindow = new MoveHistoryWindow(moveHistory);
            moveHistoryWindow.Show();
        }

        private void PlacePiece(ChessPiece piece, int row, int column)
        {
            board.PlacePiece(piece, row, column);

            var border = GetSquareBorder(row, column);

            if (border != null)
            {
                var image = new Image
                {
                    Source = new BitmapImage(new Uri(GetPieceImagePath(piece))),
                    Stretch = Stretch.Uniform
                };

                border.Child = image;
            }
        }

        private Border GetSquareBorder(int row, int column)
        {
            string squareName = $"{(char)('A' + column)}{8 - row}";
            return ChessBoard.Children.OfType<Border>().FirstOrDefault(b => b.Name == squareName);
        }

        private string GetPieceImagePath(ChessPiece piece)
        {
            return $"pack://application:,,,/Images/{piece.Color}_{piece.Type}.png";
        }

        private void Square_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var border = sender as Border;

            if (border != null)
            {
                string squareName = border.Name;
                int column = squareName[0] - 'A';
                int row = 8 - int.Parse(squareName[1].ToString());

                var clickedPiece = board.Squares[row, column];

                if (selectedPiece == null)
                {
                    if (clickedPiece != null && clickedPiece.Color == currentPlayer)
                    {
                        SelectPiece(clickedPiece, row, column, border);
                    }
                }

                else
                {
                    if (selectedPosition.HasValue && selectedPosition.Value.row == row && selectedPosition.Value.column == column)
                    {
                        ClearSelection();
                    }

                    else if (clickedPiece != null && clickedPiece.Color == currentPlayer)
                    {
                        ClearSelection();
                        SelectPiece(clickedPiece, row, column, border);
                    }

                    else
                    {
                        var (fromRow, fromColumn) = selectedPosition.Value;

                        if (IsValidMoveWithCheck(fromRow, fromColumn, row, column))
                        {
                            MovePiece(fromRow, fromColumn, row, column);
                            ClearSelection();

                            if (IsCheckmate(GetOpponent(currentPlayer)))
                            {
                                MessageBox.Show($"{currentPlayer} wins by checkmate!");
                                return;
                            }

                            if (IsStalemate(GetOpponent(currentPlayer)))
                            {
                                MessageBox.Show("The game is a stalemate!");
                                return;
                            }

                            SwitchPlayer();

                            if (IsCheck(currentPlayer))
                            {
                                MessageBox.Show($"{currentPlayer} is in check!");
                            }
                        }

                        else
                        {
                            MessageBox.Show("Invalid move!");
                        }
                    }
                }
            }
        }

        private void SelectPiece(ChessPiece piece, int row, int column, Border border)
        {
            selectedPiece = piece;
            selectedPosition = (row, column);
            border.BorderBrush = (SolidColorBrush)FindResource("SelectedBorderBrush");
            HighlightValidMoves(row, column);
        }

        private void HighlightValidMoves(int row, int column)
        {
            ClearHighlights();

            var piece = board.Squares[row, column];

            if (piece != null)
            {
                for (int targetRow = 0; targetRow < 8; targetRow++)
                {
                    for (int targetColumn = 0; targetColumn < 8; targetColumn++)
                    {
                        if (board.IsValidMove(piece, row, column, targetRow, targetColumn))
                        {
                            var border = GetSquareBorder(targetRow, targetColumn);

                            if (border != null)
                            {
                                border.Background = (SolidColorBrush)FindResource("ValidMoveBrush");
                            }
                        }
                    }
                }
            }
        }

        private void ClearHighlights()
        {
            foreach (var child in ChessBoard.Children)
            {
                if (child is Border border)
                {
                    int row = 8 - int.Parse(border.Name[1].ToString());
                    int column = border.Name[0] - 'A';
                    border.Background = GetSquareBackground(row, column, currentTheme);
                }
            }
        }

        private Brush GetOriginalColor(int row, int column)
        {
            return GetSquareBackground(row, column, currentTheme);
        }

        private void ClearSelection()
        {
            ClearHighlights();

            if (selectedPosition.HasValue)
            {
                var (row, column) = selectedPosition.Value;
                var border = GetSquareBorder(row, column);

                if (border != null)
                {
                    border.BorderBrush = Brushes.Black;
                }
            }

            selectedPiece = null;
            selectedPosition = null;
        }

        private void MovePiece(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            var piece = board.Squares[fromRow, fromColumn];

            if (piece != null)
            {
                string move = GenerateMoveNotation(piece, fromRow, fromColumn, toRow, toColumn);

                if (currentPlayer == PieceColor.White)
                {
                    moveHistory.Add(new Move
                    {
                        MoveNumber = currentMoveNumber,
                        WhiteMove = move,
                        BlackMove = string.Empty
                    });
                }

                else
                {
                    moveHistory.Last().BlackMove = move;
                    currentMoveNumber++;
                }


                if (piece.Type == PieceType.Pawn && Math.Abs(fromColumn - toColumn) == 1 && board.Squares[toRow, toColumn] == null)
                {
                    var direction = piece.Color == PieceColor.White ? -1 : 1;
                    var capturedRow = toRow - direction;
                    board.Squares[capturedRow, toColumn] = null;

                    var capturedBorder = GetSquareBorder(capturedRow, toColumn);
                    if (capturedBorder != null)
                    {
                        capturedBorder.Child = null;
                    }
                }

                if (piece.Type == PieceType.King && Math.Abs(fromColumn - toColumn) == 2)
                {
                    int rookFromColumn = toColumn == 6 ? 7 : 0;
                    int rookToColumn = toColumn == 6 ? 5 : 3;
                    var rook = board.Squares[fromRow, rookFromColumn];

                    board.Squares[fromRow, rookFromColumn] = null;
                    board.Squares[fromRow, rookToColumn] = rook;

                    var rookFromBorder = GetSquareBorder(fromRow, rookFromColumn);
                    var rookToBorder = GetSquareBorder(fromRow, rookToColumn);

                    if (rookFromBorder != null) rookFromBorder.Child = null;

                    if (rookToBorder != null)
                    {
                        var image = new Image
                        {
                            Source = new BitmapImage(new Uri(GetPieceImagePath(rook))),
                            Stretch = Stretch.Uniform
                        };

                        rookToBorder.Child = image;
                    }
                }

                board.Squares[fromRow, fromColumn] = null;
                board.Squares[toRow, toColumn] = piece;

                var fromBorder = GetSquareBorder(fromRow, fromColumn);
                var toBorder = GetSquareBorder(toRow, toColumn);

                if (fromBorder != null) fromBorder.Child = null;

                if (toBorder != null)
                {
                    var image = new Image
                    {
                        Source = new BitmapImage(new Uri(GetPieceImagePath(piece))),
                        Stretch = Stretch.Uniform
                    };

                    toBorder.Child = image;
                }

                if (piece.Type == PieceType.Pawn && (toRow == 0 || toRow == 7))
                {
                    PromotePawn(piece, toRow, toColumn);
                }

                board.LastMove = (fromRow, fromColumn, toRow, toColumn);
            }
        }



        private string GenerateMoveNotation(ChessPiece piece, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            string pieceNotation = piece.Type == PieceType.Pawn ? "" : piece.Type.ToString().Substring(0, 1);
            string fromSquare = $"{(char)('A' + fromColumn)}{8 - fromRow}";
            string toSquare = $"{(char)('A' + toColumn)}{8 - toRow}";

            string captureSymbol = board.Squares[toRow, toColumn] != null ? "x" : "";
            if (piece.Type == PieceType.Pawn && captureSymbol == "x")
            {
                pieceNotation = ((char)('A' + fromColumn)).ToString();
            }

            return $"{pieceNotation}{captureSymbol}{toSquare}";
        }

        private void PromotePawn(ChessPiece pawn, int row, int column)
        {
            var promotionWindow = new PromotionWindow(pawn.Color);

            if (promotionWindow.ShowDialog() == true)
            {
                var newPieceType = promotionWindow.SelectedPieceType;
                var newPiece = new ChessPiece(newPieceType, pawn.Color);
                board.Squares[row, column] = newPiece;
                PlacePiece(newPiece, row, column);
            }
        }

        private void SwitchPlayer()
        {
            currentPlayer = currentPlayer == PieceColor.White ? PieceColor.Black : PieceColor.White;
            CurrentPlayerText.Text = $"{currentPlayer}'s Turn";
        }

        private PieceColor GetOpponent(PieceColor player)
        {
            return player == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }

        private bool IsCheckmate(PieceColor player)
        {
            if (!IsCheck(player))
            {
                return false;
            }

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var piece = board.Squares[row, column];

                    if (piece != null && piece.Color == player)
                    {
                        for (int targetRow = 0; targetRow < 8; targetRow++)
                        {
                            for (int targetColumn = 0; targetColumn < 8; targetColumn++)
                            {
                                if (IsValidMoveWithCheck(row, column, targetRow, targetColumn))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool IsStalemate(PieceColor player)
        {
            if (IsCheck(player))
            {
                return false;
            }

            // checks for at least one legal move from the current player
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var piece = board.Squares[row, column];

                    if (piece != null && piece.Color == player)
                    {
                        for (int targetRow = 0; targetRow < 8; targetRow++)
                        {
                            for (int targetColumn = 0; targetColumn < 8; targetColumn++)
                            {
                                if (IsValidMoveWithCheck(row, column, targetRow, targetColumn))
                                {
                                    return false;
                                }
                            }
                        }
                    }
                }
            }

            return true;
        }

        private bool IsCheck(PieceColor player)
        {
            var kingPosition = FindKing(player);

            if (kingPosition == null)
            {
                return false;
            }

            var (kingRow, kingColumn) = kingPosition.Value;
            return IsInCheck(player, kingRow, kingColumn);
        }

        private (int row, int column)? FindKing(PieceColor color)
        {
            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var piece = board.Squares[row, column];

                    if (piece != null && piece.Color == color && piece.Type == PieceType.King)
                    {
                        return (row, column);
                    }
                }
            }

            return null;
        }

        private bool IsInCheck(PieceColor player, int kingRow, int kingColumn)
        {
            var opponent = GetOpponent(player);

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var piece = board.Squares[row, column];

                    if (piece != null && piece.Color == opponent)
                    {
                        if (board.IsValidMove(piece, row, column, kingRow, kingColumn))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private bool IsValidMoveWithCheck(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            var piece = board.Squares[fromRow, fromColumn];

            if (piece == null || !board.IsValidMove(piece, fromRow, fromColumn, toRow, toColumn))
            {
                return false;
            }

            var originalPiece = board.Squares[toRow, toColumn];
            board.Squares[toRow, toColumn] = piece;
            board.Squares[fromRow, fromColumn] = null;

            var kingPosition = piece.Type == PieceType.King ? (toRow, toColumn) : FindKing(piece.Color);
            var stillInCheck = kingPosition.HasValue && IsInCheck(piece.Color, kingPosition.Value.row, kingPosition.Value.column);

            board.Squares[fromRow, fromColumn] = piece;
            board.Squares[toRow, toColumn] = originalPiece;

            return !stillInCheck;
        }

        private void ThemeSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ThemeSelector.SelectedItem is ComboBoxItem selectedItem)
            {
                currentTheme = selectedItem.Content.ToString();
                UpdateBoardStyle(currentTheme);
            }
        }

        private void UpdateBoardStyle(string theme)
        {
            foreach (var child in ChessBoard.Children)
            {
                if (child is Border border)
                {
                    int row = 8 - int.Parse(border.Name[1].ToString());
                    int column = border.Name[0] - 'A';
                    border.Background = GetSquareBackground(row, column, theme);
                }
            }
        }

        private Brush GetSquareBackground(int row, int column, string theme = "Default")
        {
            bool isLightSquare = (row + column) % 2 == 0;

            switch (theme)
            {
                case "Bubblegum":
                    return isLightSquare ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 105, 180)); // White and Bubblegum Pink
                case "Lavender":
                    return isLightSquare ? Brushes.White : new SolidColorBrush(Color.FromRgb(230, 230, 250)); // White and Lavender
                case "Fire":
                    return isLightSquare ? Brushes.White : new SolidColorBrush(Color.FromRgb(255, 69, 0)); // White and Fire Red
                default:
                    return isLightSquare ? Brushes.White : Brushes.Gray; // Default theme: White and Gray
            }
        }
    }
    public class Move : INotifyPropertyChanged
    {
        private string whiteMove;
        private string blackMove;

        public int MoveNumber { get; set; }

        public string WhiteMove
        {
            get => whiteMove;
            set
            {
                if (whiteMove != value)
                {
                    whiteMove = value;
                    OnPropertyChanged(nameof(WhiteMove));
                }
            }
        }

        public string BlackMove
        {
            get => blackMove;
            set
            {
                if (blackMove != value)
                {
                    blackMove = value;
                    OnPropertyChanged(nameof(BlackMove));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
