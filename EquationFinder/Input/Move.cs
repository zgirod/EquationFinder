﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Input;

namespace EquationFinder.Input
{

    /// <summary>
    /// Describes a sequences of buttons which must be pressed to active the move.
    /// A real game might add a virtual PerformMove() method to this class.
    /// </summary>
    public class Move
    {

        public string Name;

        // The sequence of button presses required to activate this move.
        public Buttons[] Sequence;

        // Set this to true if the input used to activate this move may
        // be reused as a component of longer moves.
        public bool IsSubMove;

        public Move(string name, params Buttons[] sequence)
        {
            Name = name;
            Sequence = sequence;
        }

    }
}
