using System;
using System.Collections.Generic;

namespace Chess
{
    public class Board
    {
        public ChessPiece[,] Squares { get; private set; }
        public (int fromRow, int fromColumn, int toRow, int toColumn)? LastMove { get; set; } // en passant

        public Board()
        {
            Squares = new ChessPiece[8, 8];
            LastMove = null;
        }

        public void ClearBoard()
        {
            Squares = new ChessPiece[8, 8];
            LastMove = null;
        }

        public void PlacePiece(ChessPiece piece, int row, int column)
        {
            if (IsValidPosition(row, column))
            {
                Squares[row, column] = piece;
            }
        }

        public bool IsValidPosition(int row, int column)
        {
            // true if row and column are between 0 and 7 otherwise its not good
            return row >= 0 && row < 8 && column >= 0 && column < 8;
        }

        public bool IsValidMove(ChessPiece piece, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (piece == null || !IsValidPosition(toRow, toColumn))
            {
                return false;
            }

            var targetPiece = Squares[toRow, toColumn];

            if (targetPiece != null && targetPiece.Color == piece.Color)
            {
                return false; // can't capture your own piece
            }

            switch (piece.Type)
            {
                case PieceType.Pawn:
                    return IsValidPawnMove(piece, fromRow, fromColumn, toRow, toColumn);
                case PieceType.Knight:
                    return IsValidKnightMove(fromRow, fromColumn, toRow, toColumn);
                case PieceType.Bishop:
                    return IsValidBishopMove(fromRow, fromColumn, toRow, toColumn);
                case PieceType.Rook:
                    return IsValidRookMove(fromRow, fromColumn, toRow, toColumn);
                case PieceType.Queen:
                    return IsValidQueenMove(fromRow, fromColumn, toRow, toColumn);
                case PieceType.King:
                    return IsValidKingMove(fromRow, fromColumn, toRow, toColumn);
                default:
                    return false;
            }
        }

        private bool IsValidPawnMove(ChessPiece piece, int fromRow, int fromColumn, int toRow, int toColumn)
        {
            int direction;

            if (piece.Color == PieceColor.White)
            {
                direction = -1;
            }

            else
            {
                direction = 1;
            }

            int startRow;

            if (piece.Color == PieceColor.White)
            {
                startRow = 6;
            }

            else
            {
                startRow = 1;
            }

            // moving forward
            if (fromColumn == toColumn)
            {
                if (Squares[toRow, toColumn] != null)
                {
                    return false; // can't move forward if theres a piece in that square
                }

                if (fromRow + direction == toRow)
                {
                    return true; // move forward by 1
                }

                if (fromRow == startRow && fromRow + 2 * direction == toRow && Squares[fromRow + direction, fromColumn] == null)
                {
                    return true; // move by 2 from the starting position
                }
            }

            // capturing diagonally or en passant
            if (Math.Abs(fromColumn - toColumn) == 1 && fromRow + direction == toRow)
            {
                var targetPiece = Squares[toRow, toColumn];

                if (targetPiece != null && targetPiece.Color != piece.Color)
                {
                    return true; // regular capture
                }

                // check for en passant
                if (LastMove.HasValue)
                {
                    var (lastFromRow, lastFromColumn, lastToRow, lastToColumn) = LastMove.Value;
                    if (Squares[fromRow, toColumn] != null && Squares[fromRow, toColumn].Type == PieceType.Pawn &&
                        fromRow == lastToRow && toColumn == lastToColumn && lastFromRow == fromRow + 2 * direction)
                    {
                        return true; // En Passant capture
                    }
                }
            }

            return false;
        }

        private bool IsValidKnightMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            int rowDiff = Math.Abs(fromRow - toRow);
            int colDiff = Math.Abs(fromColumn - toColumn);
            return rowDiff * colDiff == 2; // has to move in an L shape. product of row and col has to be 2 (2*1 or 1*2)
        }

        private bool IsValidBishopMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (Math.Abs(fromRow - toRow) != Math.Abs(fromColumn - toColumn))
            {
                return false; // has to move diagonally so absolute difference in rows and cols has to be equal
            }

            int rowDirection = (toRow - fromRow) / Math.Abs(toRow - fromRow);
            int colDirection = (toColumn - fromColumn) / Math.Abs(toColumn - fromColumn);

            for (int i = 1; i < Math.Abs(toRow - fromRow); i++)
            {
                if (Squares[fromRow + i * rowDirection, fromColumn + i * colDirection] != null)
                {
                    return false; // path blocked if any square is used by piece
                }
            }

            return true;
        }

        private bool IsValidRookMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            if (fromRow != toRow && fromColumn != toColumn)
            {
                return false; // has to move in a straight line
            }

            int rowDirection;

            if (fromRow == toRow)
            {
                rowDirection = 0;
            }

            else
            {
                rowDirection = (toRow - fromRow) / Math.Abs(toRow - fromRow);
            }

            int colDirection;

            if (fromColumn == toColumn)
            {
                colDirection = 0;
            }

            else
            {
                colDirection = (toColumn - fromColumn) / Math.Abs(toColumn - fromColumn);
            }

            int distance = Math.Max(Math.Abs(toRow - fromRow), Math.Abs(toColumn - fromColumn));

            for (int i = 1; i < distance; i++)
            {
                if (Squares[fromRow + i * rowDirection, fromColumn + i * colDirection] != null)
                {
                    return false; // path blocked if any square is used by piece
                }
            }

            return true;
        }

        private bool IsValidQueenMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            return IsValidBishopMove(fromRow, fromColumn, toRow, toColumn) || IsValidRookMove(fromRow, fromColumn, toRow, toColumn);
        }

        private bool IsValidKingMove(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // normal king move
            if (Math.Max(Math.Abs(fromRow - toRow), Math.Abs(fromColumn - toColumn)) == 1)
            {
                return true;
            }

            // castling
            if (fromRow == toRow && Math.Abs(fromColumn - toColumn) == 2)
            {
                if (CanCastle(fromRow, fromColumn, toRow, toColumn))
                {
                    return true;
                }
            }

            return false;
        }

        private bool CanCastle(int fromRow, int fromColumn, int toRow, int toColumn)
        {
            // making sure it's the king trying to castle
            var piece = Squares[fromRow, fromColumn];
            if (piece.Type != PieceType.King)
            {
                return false;
            }

            // checking if rook or king moved
            if ((piece.Color == PieceColor.White && (fromRow != 7 || (fromColumn != 4 || (toColumn != 2 && toColumn != 6)))) ||
                (piece.Color == PieceColor.Black && (fromRow != 0 || (fromColumn != 4 || (toColumn != 2 && toColumn != 6)))))
            {
                return false;
            }

            int rookColumn = toColumn == 6 ? 7 : 0; // determining which rook we're castling with
            var rook = Squares[fromRow, rookColumn];

            if (rook == null || rook.Type != PieceType.Rook)
            {
                return false;
            }

            // ensuring the spaces between king and rook are empty
            int direction = toColumn == 6 ? 1 : -1;

            for (int i = fromColumn + direction; i != rookColumn; i += direction)
            {
                if (Squares[fromRow, i] != null)
                {
                    return false;
                }
            }

            // ensuring king is not in check and doesn't move through or into check
            for (int i = fromColumn; i != toColumn + direction; i += direction)
            {
                if (IsInCheck(piece.Color, fromRow, i))
                {
                    return false;
                }
            }

            return true;
        }

        private bool IsInCheck(PieceColor player, int kingRow, int kingColumn)
        {
            var opponent = GetOpponent(player);

            for (int row = 0; row < 8; row++)
            {
                for (int column = 0; column < 8; column++)
                {
                    var piece = Squares[row, column];

                    // checking if piece belongs to opponent
                    if (piece != null && piece.Color == opponent)
                    {
                        // check if piece can move to king's position
                        if (IsValidMove(piece, row, column, kingRow, kingColumn))
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        public List<(int, int)> GetValidMoves(ChessPiece piece, int fromRow, int fromColumn)
        {
            var validMoves = new List<(int, int)>();

            for (int toRow = 0; toRow < 8; toRow++)
            {
                for (int toColumn = 0; toColumn < 8; toColumn++)
                {
                    if (IsValidMove(piece, fromRow, fromColumn, toRow, toColumn))
                    {
                        validMoves.Add((toRow, toColumn));
                    }
                }
            }

            return validMoves;
        }

        private PieceColor GetOpponent(PieceColor player)
        {
            return player == PieceColor.White ? PieceColor.Black : PieceColor.White;
        }
    }
}
