using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using System.Numerics;
using NCalc;
using EquationFinder.Helpers;
using EquationFinder.Input;
using EquationFinder.DomainLogic;

namespace EquationFinder.Screens
{
    public class GameplayScreen : GameScreen
    {

        ContentManager content;

        //board variables
        int _boardSize;
        int _currentX, _currentY;
        int _selectedX, _selectedY;
        int _target;
        int[,] _gameBoardValues;
        BoardItem[,] _gameBoard;
        string _currentEquation;
        List<string> _undoEquations = new List<string>();
        List<string> _undoType = new List<string>();
        List<string> _selectedYXs = new List<string>();
        List<List<string>> _previouslySelectedYXs = new List<List<string>>();
        ClockTimer _clock = new ClockTimer(); 

        // The font used to display UI elements
        SpriteFont _font;
        Texture2D _texture;

        //the sound effects
        double _lastPlayedSound = 0;
        SoundEffect _correctSound;
        SoundEffect _incorrectSound;

        #region Initialization

        public GameplayScreen(int boardSize, BigInteger gameHash, int target)
        {

            //setup the transitions
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //set the global variables
            this._boardSize = boardSize;
            this._currentX = 0;
            this._currentY = 0;
            _selectedX = -1;
            _selectedY = -1;
            _target = target;

            //get a board
            this._gameBoard = new BoardItem[boardSize, boardSize];
            this._gameBoardValues = BoardLogic.CreateBoard(boardSize, gameHash);

            this._currentEquation = "";
            this._selectedYXs.Clear();
            this._previouslySelectedYXs.Clear();
            this.InitializeBoardItems();

        }

        public GameplayScreen(int boardSize, int target)
        {

            //setup the transitions
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //set the global variables
            this._boardSize = boardSize;
            this._currentX = 0;
            this._currentY = 0;
            _selectedX = -1;
            _selectedY = -1;
            _target = target;

            //get a board
            this._gameBoard = new BoardItem[boardSize, boardSize];
            this._gameBoardValues = BoardLogic.CreateBoard(boardSize);

            this._currentEquation = "";
            this._selectedYXs.Clear();
            this._previouslySelectedYXs.Clear();
            this.InitializeBoardItems();

        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (content == null)
                content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load the background texture we will be using
            _texture = content.Load<Texture2D>("img/paper_fibers");

            // Load the score font
            _font = content.Load<SpriteFont>("gameFont");

            // Load the laser and explosion sound effect
            _correctSound = content.Load<SoundEffect>("sound/correct");
            _incorrectSound = content.Load<SoundEffect>("sound/incorrect");

            // once the load has finished, we use ResetElapsedTime to tell the game's
            // timing mechanism that we have just finished a very long frame, and that
            // it should not try to catch up.
            ScreenManager.Game.ResetElapsedTime();
        }


        /// <summary>
        /// Unload graphics content used by the game.
        /// </summary>
        public override void UnloadContent()
        {
            content.Unload();
        }

        #endregion

        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            // Our player and enemy are both actually just text strings.
            SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

            // Start drawing
            spriteBatch.Begin();

            //draw the background
            spriteBatch.Draw(_texture, 
                new Rectangle(
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X,
                    ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y,
                    ScreenManager.GraphicsDevice.Viewport.Width,
                    ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

            //// Draw the score
            spriteBatch.DrawString(_font, "Target: " + _target, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), Color.Black);
            spriteBatch.DrawString(_font, "Equation: " + _currentEquation, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 30), Color.Black);
            spriteBatch.DrawString(_font, "Time: " + _clock.displayClock, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 60), Color.Black);

            int row = 0, col = 0;
            while (row < this._boardSize)
            {
                while (col < this._boardSize)
                {

                    //get whether the current item is selected or not
                    bool isSelected = IsActive && (row == this._currentY) && (col == this._currentX);

                    //get whether the current item is selected or not
                    bool isPreviouslySelected = _selectedYXs
                        .FirstOrDefault(x => x == string.Format("{0},{1}", row, col)) != null;

                    //show the item
                    this._gameBoard[row, col].Draw(this, isSelected, isPreviouslySelected, gameTime);

                    //go to the next column
                    col++;

                }

                //go to the next row
                row++;
                col = 0;

            }

            // Stop drawing
            spriteBatch.End();

        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // clock start and update  
            if (_clock.isRunning == false)
            {
                //count 60 seconds down 
                _clock.Start(60);
            }
            else
            {
                _clock.CheckTime(gameTime);
            } 

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public override void HandleMove(GameTime gameTime, Move move)
        {

            if (move.Name == "Down")
            {

                this._currentY++;
                if (this._currentY >= this._boardSize)
                    this._currentY = 0;

            }
            else if (move.Name == "Left")
            {

                this._currentX--;
                if (this._currentX < 0)
                    this._currentX = this._boardSize - 1;

            }
            else if (move.Name == "Up")
            {

                this._currentY--;
                if (this._currentY < 0)
                    this._currentY = this._boardSize - 1;

            }
            else if (move.Name == "Right")
            {

                this._currentX++;
                if (this._currentX >= this._boardSize)
                    this._currentX = 0;

            }
            else if (move.Name == "Undo")
            {

                //if we have previous equations to undo
                if (_undoEquations.Count() > 0)
                {

                    //get if we have an undo number
                    var undoType = _undoType[_undoType.Count() - 1];

                    //if we have a number to undo
                    if (undoType == "Number")
                    {

                        ////remove the most recent selected
                        _selectedYXs.RemoveAt(_selectedYXs.Count() - 1);

                        if (_selectedYXs.Count() > 0)
                        {

                            var crumbs = _selectedYXs[_selectedYXs.Count() - 1].Split(new char[] { ',' });
                            _selectedY = Convert.ToInt32(crumbs[0]);
                            _selectedX = Convert.ToInt32(crumbs[1]);


                        }
                        else
                        {

                            //update the selected values
                            _selectedX = -1;
                            _selectedY = -1;

                        }

                    }
                    
                    //get the previous equation
                    var previousEquation = _undoEquations[_undoEquations.Count() - 1];

                    //we used the previous equation so remove it
                    _undoEquations.RemoveAt(_undoEquations.Count() - 1);
                    _undoType.RemoveAt(_undoType.Count() - 1);

                    //set the previous equation
                    _currentEquation = previousEquation;




                }

            }
            else if (move.Name == "Left Parenthesis")
            {

                if (this.IsValidLeftParenthesisMove(gameTime))
                {

                    //add the current equation to the list
                    _undoEquations.Add(_currentEquation);
                    _undoType.Add("Left Parenthesis");

                    //update the equation
                    _currentEquation += " (";
                    _currentEquation = _currentEquation.Trim();

                }

            }
            else if (move.Name == "Right Parenthesis")
            {

                if (this.IsValidRightParenthesisMove(gameTime))
                {

                    //add the current equation to the list
                    _undoEquations.Add(_currentEquation);
                    _undoType.Add("Right Parenthesis");

                    //update the equation
                    _currentEquation += ")";

                }

            }

            else if (move.Name == "A"
                || move.Name == "B"
                || move.Name == "X"
                || move.Name == "Y")
            {

                this.AddOperation(gameTime, move);

            }
            else if (move.Name == "Select")
            {
                this.SelectNumber(gameTime, move);
            }
            else if (move.Name == "Evaluate")
            {

                //if we don't have a current equation, select the current number
                if (String.IsNullOrWhiteSpace(_currentEquation.Trim(new char[] { '(', ')' })))
                    return;

                //create an expression for the current equation
                Expression e = new Expression(this._currentEquation);

                //if the equation has errors
                if (e.HasErrors())
                {

                    //play the bad sound
                    this.PlaySound(_incorrectSound, gameTime);

                }
                else //if we don't have any errors
                {

                    //get the value of the actual expression entered
                    var actual = Convert.ToInt32(e.Evaluate());

                    //if they got it right
                    if (actual == _target)
                    {

                        //if they have used the equation previously
                        if (UseEquationPreviously())
                        {
                            //play the bad sound
                            this.PlaySound(_incorrectSound, gameTime);
                        }
                        else
                        {

                            //add the correct path to the previously used paths
                            this._previouslySelectedYXs.Add(new List<string>());

                            //add the YXs used in this equation
                            foreach (var YX in _selectedYXs)
                                this._previouslySelectedYXs[this._previouslySelectedYXs.Count - 1].Add(YX);

                            //add time back to the clock
                            _clock.AddTime(4);

                            //play the bad sound
                            this.PlaySound(_correctSound, gameTime);

                        }

                    }
                    else //if they got it wrong 
                    {

                        //play the bad sound
                        this.PlaySound(_incorrectSound, gameTime);

                    }

                }

                //reset the equation
                this._currentEquation = "";
                this._undoEquations.Clear();
                this._undoType.Clear();
                this._selectedYXs.Clear();
                this._selectedX = -1;
                this._selectedY = -1;

            }

            base.HandleMove(gameTime, move);

        }

        #region Private Board Methods

        private void PlaySound(SoundEffect soundEffect, GameTime gameTime)
        {

            //we don't want to play sounds to close together
            if (gameTime.TotalGameTime.TotalMilliseconds - _lastPlayedSound >= 500)
            {
            soundEffect.Play();
                _lastPlayedSound = gameTime.TotalGameTime.TotalMilliseconds;
            }

        }

        private void AddOperation(GameTime gameTime, Move move)
        {

            //if we have a valid move
            if (IsValidOperationMove(gameTime))
            {

                //add the current equation to the list
                _undoEquations.Add(_currentEquation);
                _undoType.Add("Operation");

                //get the arithmetic symbol
                var symbol = "+";
                if (move.Name == "X")
                    symbol = "*";
                else if (move.Name == "B")
                    symbol = "-";
                else if (move.Name == "Y")
                    symbol = "/";

                //add the symbol
                _currentEquation += string.Format(" {0}", symbol);

            }

        }

        private void SelectNumber(GameTime gameTime, Move move)
        {

            //if we have a valid move
            if (this.IsValidNumberMove(gameTime))
            {

                //add the current equation to the list
                _undoEquations.Add(_currentEquation);
                _undoType.Add("Number");

                //get the current value
                var currentValue = this._gameBoardValues[_currentY, _currentX].ToString();

                //add the currentValue to our equation
                if (String.IsNullOrWhiteSpace(_currentEquation))
                    this._currentEquation = currentValue;
                else
                    this._currentEquation += string.Format(" {0}", currentValue);

                //update the selected values
                _selectedX = _currentX;
                _selectedY = _currentY;

                //add the values to the previously selected list
                _selectedYXs.Add(string.Format("{0},{1}", _currentY, _currentX));

            }

        }

        private void InitializeBoardItems()
        {

            // start at 175, 175
            Vector2 position = new Vector2(175f, 175f);

            int row = 0, col = 0;
            while (row < this._boardSize)
            {
                while (col < this._boardSize)
                {

                    //set the game board item's default properties
                    this._gameBoard[row, col] = new BoardItem(this._gameBoardValues[row, col].ToString(),
                        this._gameBoardValues[row, col].ToString());
                    this._gameBoard[row, col].Position = position;
                    this._gameBoard[row, col].X = col;
                    this._gameBoard[row, col].Y = row;

                    //increase the x position
                    position.X += 50;

                    //go to the next column
                    col++;

                }

                //go to the next row
                row++;
                col = 0;
                position.X = 175;
                position.Y += 35;

            }

        }

        private bool IsValidOperationMove(GameTime gameTime)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number" || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_incorrectSound, gameTime);
                return false;

            }

        }

        private bool IsValidRightParenthesisMove(GameTime gameTime)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number"
                    || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_incorrectSound, gameTime);
                return false;

            }

        }

        private bool IsValidLeftParenthesisMove(GameTime gameTime)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number" 
                    || undoType == "Operation"
                    || undoType == "Left Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_correctSound, gameTime);
                return true;

            }

        }

        private bool IsValidNumberMove(GameTime gameTime)
        {


            //Check to make sure this is a valid move for a number
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number" || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_incorrectSound, gameTime);
                    return false;
                }

            }

            if (_selectedX < 0 && _selectedY < 0)
            {
                this.PlaySound(_correctSound, gameTime);
                return true;
            }

                //if the current is within range of the selected
            else if ((Math.Abs(_selectedX - _currentX) <= 1) && (Math.Abs(_selectedY - _currentY) <= 1))
            {
                //if the current is the selected, return false
                if (_selectedY == _currentY && _selectedX == _currentX)
                {
                    //play the bad sound
                    this.PlaySound(_incorrectSound, gameTime);
                    return false;
                }
                else
                {

                    //for each previously selected YX
                    foreach (var yx in _selectedYXs)
                    {

                        //get the x and y
                        var crumbs = yx.Split(new char[] { ',' });

                        //if we have already selected the YX
                        if (_currentY == int.Parse(crumbs[0]) && _currentX == int.Parse(crumbs[1]))
                        {
                            //play the bad sound
                            this.PlaySound(_incorrectSound, gameTime);
                            return false;
                        }

                    }

                    //play the good sound
                    this.PlaySound(_correctSound, gameTime);
                    return true;
                }

            }
            else
            {
                //play the bad sound
                this.PlaySound(_incorrectSound, gameTime);
                return false;
            }


        }

        private bool UseEquationPreviously()
        {

            var currentSelectedTotal = _selectedYXs.Count();
            var countFound = 0;

            //for each YX already selected
            foreach (var previoulySelectedYX in _previouslySelectedYXs)
            {

                //if only need to continue if the currently selected total was previously selected
                if (previoulySelectedYX.Count() == currentSelectedTotal)
                {

                    //for each previously selected YX, check to see if they exist in the currently selected YX
                    countFound = 0;
                    foreach (var YX in previoulySelectedYX)
                    {

                        //count each one found
                        if (_selectedYXs.Exists(x => x == YX))
                            countFound++;

                    }

                    //if the total count matches and all the items match, we already used this chain
                    if (countFound == currentSelectedTotal)
                        return true;

                }

            }

            return false;

        }

        #endregion


    }
}
