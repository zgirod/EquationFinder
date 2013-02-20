﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquationFinder.Input
{
    /// <summary>
    /// Represents a set of available moves for matching. This internal storage of this
    /// class is optimized for efficient match searches.
    /// </summary>
    public class MoveList
    {
        private Move[] moves;

        public MoveList(IEnumerable<Move> moves)
        {
            // Store the list of moves in order of decreasing sequence length.
            // This greatly simplifies the logic of the DetectMove method.
            this.moves = moves.OrderByDescending(m => m.Sequence.Length).ToArray();
        }

        /// <summary>
        /// Finds the longest Move which matches the given input, if any.
        /// </summary>
        public Move DetectMove(InputManager input)
        {
            // Perform a linear search for a move which matches the input. This relies
            // on the moves array being in order of decreasing sequence length.
            foreach (Move move in moves)
            {
                if (input.Matches(move))
                {
                    return move;
                }
            }
            return null;
        }

        public int LongestMoveLength
        {
            get
            {
                // Since they are in decreasing order,
                // the first move is the longest.
                return moves[0].Sequence.Length;
            }
        }
    }
}
