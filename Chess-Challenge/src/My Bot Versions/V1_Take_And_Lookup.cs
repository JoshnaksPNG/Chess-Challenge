using ChessChallenge.API;
using System.Collections.Generic;

public class V1_Bot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 330, 500, 900, 10000 };

    // Lookup for Piece Square Tables
    //Dictionary <PieceType, int[]> PSTableLookup = GeneratePSTable();
    int[,] PSTableLookup = 
    {
        { // Pawn Table
            0,  0,  0,  0,
            5, 10, 10,-20,
            5, -5,-10,  0,
            0,  0,  0, 20,
            5,  5, 10, 25,
           10, 10, 20, 30,
           50, 50, 50, 50,
            0,  0,  0,  0
        },
        { // Knight Table
            -50,-40,-30,-30,
            -40,-20,  0,  5,
            -30,  5, 10, 15,
            -30,  0, 15, 20,
            -30,  5, 15, 20,
            -30,  0, 10, 15,
            -40,-20,  0,  0,
            -50,-40,-30,-30
        },
        { // Bishop Table
            -20,-10,-10,-10,
            -10,  5,  0,  0,
            -10, 10, 10, 10,
            -10,  0, 10, 10,
            -10,  5,  5, 10,
            -10,  0,  5, 10,
            -10,  0,  0,  0,
            -20,-10,-10,-10
        },
        { // Rook Table
            0,  0,  0,  5,
           -5,  0,  0,  0,
           -5,  0,  0,  0,
           -5,  0,  0,  0,
           -5,  0,  0,  0,
           -5,  0,  0,  0,
            5, 10, 10, 10,
            0,  0,  0,  0
        },
        { // Queen Table
            -20,-10,-10, -5,
            -10,  0,  0,  0,
            -10,  5,  5,  5,
              0,  0,  5,  5,
             -5,  0,  5,  5,
            -10,  0,  5,  5,
            -10,  0,  0,  0,
            -20,-10,-10, -5
        },
        { // King Table
             20, 30, 10,  0,
             20, 20,  0,  0,
            -10,-20,-20,-20,
            -20,-30,-30,-40,
            -30,-40,-40,-50,
            -30,-40,-40,-50,
            -30,-40,-40,-50,
            -30,-40,-40,-50
        }
    };

    public Move Think(Board board, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();

        // Queue to get move with lowest score
        PriorityQueue<Move, int> moveQueue = new PriorityQueue<Move, int>();

        Move bestMove = moves[0];
        int bestScore = int.MinValue;

        // Think through every move
        foreach (Move move in moves) 
        {
            // Always go for checkmate
            if (MoveIsCheckmate(board, move))
            {
                return move;
            }

            // Initialize Score
            int score = 0;

            // Add Score for Captured Piece
            int capturedType = (int)move.CapturePieceType;
            score += pieceValues[capturedType];

            // Get Table Index from Position
            int tableIdx = move.TargetSquare.Index;

            if (board.IsWhiteToMove)
            {
                tableIdx -= 63;
                tableIdx *= -1;
            }
            int x = tableIdx % 8;
            int y = tableIdx / 8;

            // Clamp x to 0-3 
            if (x > 3)
            {
                x -= 7;
                x *= -1;
            }

            int ind = y * 4 + x;

            // Add Score for Position after move
            if (capturedType != 0)
            {
                score += PSTableLookup[capturedType - 1, y * 4 + x];
            }
                
            

            // Add move to Queue
            moveQueue.Enqueue(move, score );
            if(score > bestScore) 
            {
                bestScore = score;
                bestMove = move;
            }

        }

        return bestMove;
    }

    // Test if move gives checkmate (From EvilBot.cs)
    bool MoveIsCheckmate(Board board, Move move)
    {
        board.MakeMove(move);
        bool isMate = board.IsInCheckmate();
        board.UndoMove(move);
        return isMate;
    }
}