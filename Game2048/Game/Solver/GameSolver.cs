/* Implementation based on algorithm from https://github.com/nneonneo/2048-ai */

using Game2048.Game.Models;
using Game2048.Game.Solver.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Game2048.Game.Solver
{
    public class GameSolver : IGameSolver
    {
        #region Constants

        private const ulong ROW_MASK = 0xFFFF;
        private const ulong COL_MASK = 0x000F000F000F000F;

        #endregion

        #region Fields

        //We can perform state lookups one row at a time by using arrays with 65536 entries.
        //Move tables. Each row or compressed column is mapped to (oldrow^newrow) assuming row/col 0.
        //Thus, the value is 0 if there is no move, and otherwise equals a value that can easily be
        //xor'ed into the current board state to update the board.
        private ulong[] rowsMovedLeft = new ulong[ushort.MaxValue + 1];
        private ulong[] rowsMovedRight = new ulong[ushort.MaxValue + 1];
        private ulong[] columnsMovedUp = new ulong[ushort.MaxValue + 1];
        private ulong[] columnsMovedDown = new ulong[ushort.MaxValue + 1];
        private double[] heuristics = new double[ushort.MaxValue + 1];
        private HeuristicWeight heuristicWeight;
        private int depthLimit = 3;

        #endregion

        /// <summary>
        /// Initializes a new instance of the <see cref="GameSolver"/> class.
        /// </summary>
        /// <param name="weightProvider">The weight provider.</param>
        public GameSolver(IHeuristicWeightProvider weightProvider)
        {
            heuristicWeight = weightProvider.GetWeight();

        }

        /// <summary>
        /// Transposes the specified x.
        /// </summary>
        /// <param name="x">The x.</param>
        /// <returns></returns>
        private ulong Transpose(ulong x)
        {
            // Transpose rows/columns in a board:
            //   0123       048c
            //   4567  -->  159d
            //   89ab       26ae
            //   cdef       37bf
            ulong a1 = x & 0xF0F00F0FF0F00F0F;
            ulong a2 = x & 0x0000F0F00000F0F0;
            ulong a3 = x & 0x0F0F00000F0F0000;
            ulong a = a1 | (a2 << 12) | (a3 >> 12);
            ulong b1 = a & 0xFF00FF0000FF00FF;
            ulong b2 = a & 0x00FF00FF00000000;
            ulong b3 = a & 0x00000000FF00FF00;
            return b1 | (b2 >> 24) | (b3 << 24);
        }

        /// <summary>
        /// Gets the empty count.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private byte GetEmptyCount(ulong board)
        {
            // Count the number of empty positions (= zero nibbles) in a board.
            board |= (board >> 2) & 0x3333333333333333;
            board |= (board >> 1);
            board = ~board & 0x1111111111111111;
            // At this point each nibble is:
            //  0 if the original nibble was non-zero
            //  1 if the original nibble was zero
            // Next sum them all
            board += board >> 32;
            board += board >> 16;
            board += board >> 8;
            board += board >> 4; // this can overflow to the next nibble if there were 16 empty positions
            return (byte)(board & 0xf);
        }


        /// <summary>
        /// Initializes the heuristics.
        /// </summary>
        public void InitHeuristics()
        {
            for (ulong longRow = 0; longRow < ushort.MaxValue + 1; ++longRow)
            {
                ushort row = (ushort)longRow;
                byte[] rowRanks = new byte[4] 
                {                
                    (byte)((row >>  0) & 0xf),                
                    (byte)((row >>  4) & 0xf),                
                    (byte)((row >>  8) & 0xf),                
                    (byte)((row >> 12) & 0xf)                    
                };

                // Heuristic score
                double sum = 0;
                byte empty = 0;
                byte merges = 0;

                byte prev = 0;
                byte counter = 0;
                for (byte i = 0; i < 4; ++i)
                {
                    byte rank = rowRanks[i];
                    sum += Math.Pow(rank, heuristicWeight.SumPower);
                    if (rank == 0)
                    {
                        empty++;
                    }
                    else
                    {
                        if (prev == rank)
                        {
                            counter++;
                        }
                        else if (counter > 0)
                        {
                            merges += (byte)(1 + counter);
                            counter = 0;
                        }
                        prev = rank;
                    }
                }
                if (counter > 0)
                {
                    merges += (byte)(1 + counter);
                }

                heuristics[row] = heuristicWeight.Penalty +
                    heuristicWeight.EmptyWeight * empty +
                    heuristicWeight.MergeWeight * merges -
                    heuristicWeight.MonoWeight * GetMonotonicity(rowRanks) -
                    heuristicWeight.SumWeight * sum;

                MoveRowLeft(ref rowRanks);
                
                ushort rowMovedLeft = (ushort)((rowRanks[0] << 0) |
                               (rowRanks[1] << 4) |
                               (rowRanks[2] << 8) |
                               (rowRanks[3] << 12));

                uint reversedRowMovedLeft = ReverseRow(rowMovedLeft);
                uint reversedRow = ReverseRow(row);

                rowsMovedLeft[row] = (ulong)(row ^ rowMovedLeft);                
                rowsMovedRight[reversedRow] = (reversedRow ^ reversedRowMovedLeft);
                columnsMovedUp[row] = UnpackColumn((ushort)row) ^ UnpackColumn((ushort)rowMovedLeft);
                columnsMovedDown[reversedRow] = UnpackColumn((ushort)reversedRow) ^ UnpackColumn((ushort)reversedRowMovedLeft);
            }
        }

        /// <summary>
        /// Gets the monotonicity.
        /// </summary>
        /// <param name="rowRanks">The row ranks.</param>
        /// <returns></returns>
        private double GetMonotonicity(byte[] rowRanks)
        {
            double monoLeft = 0;
            double monoRight = 0;

            for (byte i = 1; i < 4; ++i)
            {
                if (rowRanks[i - 1] > rowRanks[i])
                {
                    monoLeft += Math.Pow(rowRanks[i - 1], heuristicWeight.MonoPower) - Math.Pow(rowRanks[i], heuristicWeight.MonoPower);
                }
                else
                {
                    if (rowRanks[i] > rowRanks[i - 1])
                    {
                        monoRight += Math.Pow(rowRanks[i], heuristicWeight.MonoPower) - Math.Pow(rowRanks[i - 1], heuristicWeight.MonoPower);
                    }
                }
            }

            return Math.Min(monoLeft, monoRight);
        }

        /// <summary>
        /// Moves the row left.
        /// </summary>
        /// <param name="rowRanks">The row ranks.</param>
        private void MoveRowLeft(ref byte[] rowRanks)
        {
            for (int i = 0; i < 3; ++i)
            {
                int j;
                for (j = i + 1; j < 4; ++j)
                {
                    if (rowRanks[j] != 0) break;
                }
                if (j == 4) break;

                if (rowRanks[i] == 0)
                {
                    rowRanks[i] = rowRanks[j];
                    rowRanks[j] = 0;
                    i--;
                }
                else if (rowRanks[i] == rowRanks[j] && rowRanks[i] != 0xf)
                {
                    rowRanks[i]++;
                    rowRanks[j] = 0;
                }
            }
        }

        /// <summary>
        /// Unpacks the column.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private ulong UnpackColumn(ushort row)
        {
            ulong tmp = row;
            return (tmp | (tmp << 12) | (tmp << 24) | (tmp << 36)) & COL_MASK;
        }

        /// <summary>
        /// Reverses the row.
        /// </summary>
        /// <param name="row">The row.</param>
        /// <returns></returns>
        private ushort ReverseRow(ushort row)
        {
            return (ushort)((row >> 12) | ((row >> 4) & 0x00F0) | ((row << 4) & 0x0F00) | (row << 12));
        }


        /// <summary>
        /// Moves up.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private ulong MoveUp(ulong board)
        {
            ulong transposedBoard = Transpose(board);
            board ^= columnsMovedUp[(transposedBoard >> 0) & ROW_MASK] << 0;
            board ^= columnsMovedUp[(transposedBoard >> 16) & ROW_MASK] << 4;
            board ^= columnsMovedUp[(transposedBoard >> 32) & ROW_MASK] << 8;
            board ^= columnsMovedUp[(transposedBoard >> 48) & ROW_MASK] << 12;
            return board;
        }

        /// <summary>
        /// Moves down.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private ulong MoveDown(ulong board)
        {
            ulong transposedBoard = Transpose(board);
            board ^= columnsMovedDown[(transposedBoard >> 0) & ROW_MASK] << 0;
            board ^= columnsMovedDown[(transposedBoard >> 16) & ROW_MASK] << 4;
            board ^= columnsMovedDown[(transposedBoard >> 32) & ROW_MASK] << 8;
            board ^= columnsMovedDown[(transposedBoard >> 48) & ROW_MASK] << 12;
            return board;
        }

        /// <summary>
        /// Moves the left.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private ulong MoveLeft(ulong board)
        {
            board ^= (rowsMovedLeft[(board >> 0) & ROW_MASK]) << 0;
            board ^= (rowsMovedLeft[(board >> 16) & ROW_MASK]) << 16;
            board ^= (rowsMovedLeft[(board >> 32) & ROW_MASK]) << 32;
            board ^= (rowsMovedLeft[(board >> 48) & ROW_MASK]) << 48;
            return board;
        }

        /// <summary>
        /// Moves the right.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private ulong MoveRight(ulong board)
        {
            board ^= (rowsMovedRight[(board >> 0) & ROW_MASK]) << 0;
            board ^= (rowsMovedRight[(board >> 16) & ROW_MASK]) << 16;
            board ^= (rowsMovedRight[(board >> 32) & ROW_MASK]) << 32;
            board ^= (rowsMovedRight[(board >> 48) & ROW_MASK]) << 48;
            return board;
        }

        /// <summary>
        /// Moves the specified move.
        /// </summary>
        /// <param name="move">The move.</param>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private ulong Move(int move, ulong board)
        {
            switch (move)
            {
                case 0: 
                    return MoveLeft(board);
                case 1: 
                    return MoveUp(board);
                case 2: 
                    return MoveRight(board);
                case 3: 
                    return MoveDown(board);
                default:
                    return 4;
            }
        }

        /// <summary>
        /// Gets the score.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private double GetScore(ulong board)
        {
            return heuristics[(board >> 0) & ROW_MASK] +
                   heuristics[(board >> 16) & ROW_MASK] +
                   heuristics[(board >> 32) & ROW_MASK] +
                   heuristics[(board >> 48) & ROW_MASK];
        }

        /// <summary>
        /// Gets the heuristic score.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private double GetHeuristicScore(ulong board)
        {
            return GetScore(board) + GetScore(Transpose(board));
        }

        /// <summary>
        /// Gets the expectation maximum score.
        /// </summary>
        /// <param name="movesCache">The moves cache.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private double GetExpectationMaxScore(Dictionary<ulong, double> movesCache, int depth, ulong board)
        {
            if (depth >= depthLimit)
            {
                return GetHeuristicScore(board);
            }
            
            if (movesCache.ContainsKey(board))
            {
                return movesCache[board];
            }

            ushort emptyCount = GetEmptyCount(board);

            double result = 0;
            ulong currentBoard = board;
            ulong cell = 1;
            while (cell > 0)
            {
                if ((currentBoard & 0xf) == 0)
                {
                    result += GetMoveScore(movesCache, depth, board | cell) * 0.9f;
                    result += GetMoveScore(movesCache, depth, board | (cell << 1)) * 0.1f;
                }
                currentBoard >>= 4;
                cell <<= 4;
            }
            result = result / emptyCount;

            movesCache[board] = result;

            return result;
        }

        /// <summary>
        /// Gets the move score.
        /// </summary>
        /// <param name="movesCache">The moves cache.</param>
        /// <param name="depth">The depth.</param>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        private double GetMoveScore(Dictionary<ulong, double> movesCache, int depth, ulong board)
        {
            double best = 0.00000000000000001;            
            for (int move = 0; move < 4; move++)
            {
                ulong newboard = Move(move, board);
                if (board != newboard)
                {
                    best = Math.Max(best, GetExpectationMaxScore(movesCache, depth + 1, newboard));
                }
            }            

            return best;
        }

        /// <summary>
        /// Gets the top level score.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <param name="move">The move.</param>
        /// <returns></returns>
        private double GetTopLevelScore(ulong board, int move)
        {
            ulong newboard = Move(move, board);

            if (board == newboard)
            {
                return 0;
            }
            return GetExpectationMaxScore(new Dictionary<ulong, double>(), 0, newboard);
        }

        /// <summary>
        /// Gets the best move.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public MoveDirection GetBestMove(ulong board)
        {
            double bestScore = 0;
            MoveDirection bestMove = MoveDirection.None;

            Parallel.For(0, 4, (move) =>
            {
                double score = GetTopLevelScore(board, move);

                if (score > bestScore)
                {
                    bestScore = score;
                    bestMove = (MoveDirection)move;
                }
            });

            return bestMove;
        }

        /// <summary>
        /// Gets the maximum rank.
        /// </summary>
        /// <param name="board">The board.</param>
        /// <returns></returns>
        public byte GetMaxRank(ulong board)
        {
            byte maxrank = 0;
            while (board > 0)
            {
                maxrank = Math.Max(maxrank, (byte)(board & 0xf));
                board >>= 4;
            }
            return maxrank;
        }

        /// <summary>
        /// Sets the depth.
        /// </summary>
        /// <param name="depth">The depth.</param>
        public void SetDepth(int depth)
        {
            depthLimit = depth;
        }
    }
}
