using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chess
{
    public enum PieceType { King, Queen, Rook, Bishop, Knight, Pawn }
    public enum PieceColor { White, Black }

    public class ChessPiece
    {
        public PieceType Type { get; private set; }
        public PieceColor Color { get; private set; }

        public ChessPiece(PieceType type, PieceColor color)
        {
            Type = type;
            Color = color;
        }
    }
}