﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework;
using EquationFinder.Helpers;
using EquationFinder.Input;
using EquationFinder.DomainLogic;
using Microsoft.Xna.Framework.Storage;
using EquationFinder.Objects;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Media;

namespace EquationFinder.Screens
{

    public enum ActionType
    {
        NUMBER,
        OPERATOR,
        LEFT_PARENTHESIS,
        RIGHT_PARENTHESIS
    }

    public class GameplayScreen : GameScreen
    {

        ContentManager _content;

        //board variables
        int _boardSize;
        int _currentX, _currentY;
        int _selectedX, _selectedY;
        int _target;
        int _score;
        int _totalCorrect;
        int _roundCorrect;
        FlashText _flashText = new FlashText();
        int[,] _gameBoardValues;
        BoardItem[,] _gameBoard;
        string _currentEquation;
        bool _isBoardInitialized;
        List<string> _undoEquations = new List<string>();
        List<string> _undoType = new List<string>();
        List<string> _selectedYXs = new List<string>();
        List<ActionType> _selectedActionTypes = new List<ActionType>();
        List<List<string>> _previouslySelectedYXs = new List<List<string>>();
        ClockTimer _clock = new ClockTimer();
        bool _loadedHighScores;
        List<HighScore> _highScores;
        Int64 _highScoreToBeat = 0;
        int _secondsPerRound = 60;
        int _secondForCorrectAnswer = 8;
        bool _isPaused;
        int _pasuedMenuCount;

        // The font used to display UI elements
        SpriteFont _gameFont;
        Texture2D _texture;

        //the sound effects
        double _lastPlayedSound = 0;
        SoundEffect _correctSound;
        SoundEffect _incorrectSound;
        Song _backgroundMusic;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        #region Initialization

        public GameplayScreen(int target)
        {

            //setup the transitions
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //set the global variables
            this._boardSize = GameplayOptions.BoardSize;
            this._currentX = 0;
            this._currentY = 0;
            _selectedX = -1;
            _selectedY = -1;
            _target = target;

            //get a board
            this._gameBoard = new BoardItem[GameplayOptions.BoardSize, GameplayOptions.BoardSize];
            this._gameBoardValues = BoardLogic.CreateBoard(GameplayOptions.BoardSize);

            this._currentEquation = "";
            this._selectedYXs.Clear();
            this._previouslySelectedYXs.Clear();
            this._selectedActionTypes.Clear();
            this._score = 0;
            this._isBoardInitialized = false;
            this._loadedHighScores = false;
            this._highScores = new List<HighScore>();
            this._highScoreToBeat = 0;
            this._isPaused = false;
            this._pasuedMenuCount = 1;

        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load the background texture we will be using
            _texture = _content.Load<Texture2D>("img/paper_fibers");

            // Load the score font
            _gameFont = _content.Load<SpriteFont>("gameFont");

            // Load the laser and explosion sound effect
            _correctSound = _content.Load<SoundEffect>("sound/correct");
            _incorrectSound = _content.Load<SoundEffect>("sound/incorrect");

            //if we want to play music, play it
            if (GameplayOptions.PlayMusic != "Off")
            {
                
                //get the song that we want to play
                _backgroundMusic = _content.Load<Song>(string.Format("sound/{0}Theme", GameplayOptions.PlayMusic));

                //play the song
                this.PlayMusic(_backgroundMusic);

            }

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
            _content.Unload();
        }

        #endregion

        #region Public Override Methods

        public override void Draw(GameTime gameTime)
        {

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            //if the board is initialized
            if (this._isBoardInitialized)
            {

                // Our player and enemy are both actually just text strings.
                SpriteBatch spriteBatch = ScreenManager.SpriteBatch;

                // Start drawing
                spriteBatch.Begin();

                //draw the background
                spriteBatch.Draw(_texture,
                    new Rectangle(
                        ScreenManager.GraphicsDevice.Viewport.X,
                        ScreenManager.GraphicsDevice.Viewport.Y,
                        ScreenManager.GraphicsDevice.Viewport.Width,
                        ScreenManager.GraphicsDevice.Viewport.Height), Color.White);

                //if we are not paused
                if (!_isPaused)
                {

                    this.DrawGame(spriteBatch, gameTime);

                }
                else //if we are paused
                {

                    this.DrawPausedMenu(spriteBatch, gameTime);

                }

                // Stop drawing
                spriteBatch.End();

            }

        }

        private void DrawPausedMenu(SpriteBatch spriteBatch, GameTime gameTime)
        {

            //calculate my x and y
            var x = (ScreenManager.GraphicsDevice.Viewport.Width / 2);
            var y = (ScreenManager.GraphicsDevice.Viewport.Height / 2) - 50;

            //draw the strings
            spriteBatch.DrawString(_gameFont, "Resume Game", new Vector2(x - Convert.ToInt32(_gameFont.MeasureString("Resume Game").Length()), y), _pasuedMenuCount == 1 ? Color.Blue : Color.Black);
            spriteBatch.DrawString(_gameFont, "Exit to menu", new Vector2(x - Convert.ToInt32(_gameFont.MeasureString("Exit to menu").Length()), y + 50), _pasuedMenuCount == 2 ? Color.Blue : Color.Black);


        }

        private void DrawGame(SpriteBatch spriteBatch, GameTime gameTime)
        {


            // Draw the score
            spriteBatch.DrawString(_gameFont, "Target: " + _target, new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Y + 10), Color.Black);
            spriteBatch.DrawString(_gameFont, "Equation: " + _currentEquation, new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Y + 40), Color.Black);
            spriteBatch.DrawString(_gameFont, "Score: " + string.Format("{0:n0}", this._score), new Vector2(ScreenManager.GraphicsDevice.Viewport.X + 30, ScreenManager.GraphicsDevice.Viewport.Y + 70), Color.Black);

            //get the time string
            var time = "Time:   " + _clock.displayClock;
            if (_clock.displayClock == "Game Over")
                time = "Game Over";

            //draw the space need for the time string
            Vector2 timeSize = _gameFont.MeasureString(time);
            spriteBatch.DrawString(_gameFont, time, new Vector2(ScreenManager.GraphicsDevice.Viewport.Width - timeSize.Length() - 10,
                ScreenManager.GraphicsDevice.Viewport.Y + 10), Color.Black);

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

            //if we have flash text, draw the string
            if (_flashText.Active)
            {

                //calculate where the flash text goes
                Vector2 textSize = _gameFont.MeasureString(_flashText.Text);

                //draw the flash text
                if (_flashText.IsErrorText)
                {

                    spriteBatch.DrawString(
                        _gameFont,
                        _flashText.Text,
                        new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, ScreenManager.GraphicsDevice.Viewport.Height - 50f) - (textSize / 2),
                        Color.Red);

                }
                else
                {

                    spriteBatch.DrawString(
                        _gameFont,
                        _flashText.Text,
                        new Vector2(ScreenManager.GraphicsDevice.Viewport.Width / 2, 50f) - (textSize / 2),
                        Color.DarkBlue);

                }


            }


        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // clock start and update  
            if (_clock.isRunning == false)
            {

                //update the board items
                if (!this._isBoardInitialized)
                {
                    UpdateBoardItems();
                    this._isBoardInitialized = true;
                }

                //count 60 seconds down 
                _clock.Start(_secondsPerRound);
            }
            else if (_clock.isFinished == false && _isPaused == false)
            {

                int row = 0, col = 0;
                while (row < this._boardSize)
                {
                    col = 0;
                    while (col < this._boardSize)
                    {
                        _gameBoard[row, col].Update(this, (_currentY == row && _currentX == col), gameTime);
                        col++;
                    }
                    row++;
                }


                if (_flashText.Active == false || _flashText.RunClock)
                {

                    //check the game clock
                    _clock.CheckTime(gameTime);

                    //check to see if we qualified for the next round
                    if (_clock.displayClock == "Game Over" && this._roundCorrect >= 4 && EquationFinderGame.IsTrailMode == false)
                    {

                        //go to the next round
                        _clock.NextRound(_secondsPerRound);

                        //reset the round count
                        this._roundCorrect = 0;

                        //go to the next number
                        this._target++;

                        //set the flash text
                        _flashText.Text = string.Format("Next Level {0}", _target);
                        _flashText.StartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        _flashText.Active = true;
                        _flashText.RunClock = true;
                        
                        //reset the board for the next level
                        this.ResetBoardForNextLevel();

                    }

                }

                //reset the flash text
                if( _flashText.Active == true
                    && (gameTime.TotalGameTime.TotalMilliseconds - _flashText.StartTime) > 1000)
                {
                    _flashText.Active = false;
                    _flashText.Text = "";
                    _flashText.RunClock = true;
                }   

                //if we want to save the high score file
                if (_clock.isFinished == true)
                {

                    // the game is over, show the high score screen
                    MediaPlayer.Stop();
                    LoadingScreen.Load(ScreenManager, true, null, new SaveHighScoreScreen(_boardSize, _score, _score > _highScoreToBeat));

                }

                if (_loadedHighScores == false)
                {

                    //load the high scores
                    _loadedHighScores = true;
                    _highScores = this.LoadHighScores();

                    //set the high score we need to beat to store our score
                    if (_highScores.Count >= 5)
                        _highScoreToBeat = _highScores.Min(x => x.Score);
                    else
                        _highScoreToBeat = 0;


                }
                
            }

            GamePadState lastGamePadState = GamePadState;
            KeyboardState lastKeyboardState = KeyboardState;
            GamePadState = GamePad.GetState(PlayerIndex.One);
            KeyboardState = Keyboard.GetState(PlayerIndex.One);

            //get the direction
            var direction = Direction.FromInput(GamePadState, KeyboardState);
            if (Direction.FromInput(lastGamePadState, lastKeyboardState) != direction && direction != 0.0)
                this.HandleDirection(direction);

            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public override void HandleMove(GameTime gameTime, Move move)
        {

            //don't do anything if the clock isn't running
            if (_clock.isRunning == false || _clock.isFinished == true)
                return;

            //determine which input mode we should be in
            if (_isPaused)
                this.HandlePausedMove(gameTime, move);
            else
                this.HandleGameplayMove(gameTime, move);

            base.HandleMove(gameTime, move);

        }

        #endregion

        #region Private Board Methods


        private void PlayMusic(Song song)
        {
            // Due to the way the MediaPlayer plays music,
            // we have to catch the exception. Music will play when the game is not tethered
            try
            {
                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;

            }
            catch { }
        }

        private void HandleDirection(Buttons direction)
        {

            if (_isPaused)
                this.HandlePausedDirection(direction);
            else
                this.HandleGameplayDirection(direction);

        }

        private void HandleGameplayDirection(Buttons direction)
        {

            if (direction.Equals(Buttons.DPadDown)
                || direction.Equals(Buttons.LeftThumbstickDown))
            {

                this._currentY++;
                if (this._currentY >= this._boardSize)
                    this._currentY = 0;

            }
            else if (direction.Equals(Buttons.DPadLeft) || direction.Equals(Buttons.LeftThumbstickLeft))
            {

                this._currentX--;
                if (this._currentX < 0)
                    this._currentX = this._boardSize - 1;

            }
            else if (direction.Equals(Buttons.DPadUp) || direction.Equals(Buttons.LeftThumbstickUp))
            {

                this._currentY--;
                if (this._currentY < 0)
                    this._currentY = this._boardSize - 1;

            }
            else if (direction.Equals(Buttons.DPadRight) || direction.Equals(Buttons.LeftThumbstickRight))
            {

                this._currentX++;
                if (this._currentX >= this._boardSize)
                    this._currentX = 0;

            }

        }

        private void HandlePausedDirection(Buttons direction)
        {

            if (_pasuedMenuCount == 1)
                _pasuedMenuCount = 2;
            else
                _pasuedMenuCount = 1;

        }

        private void CalculateScore()
        {

            //get the number of moves
            var numberOfMoves = this._selectedActionTypes.Where(x =>
                x == ActionType.NUMBER
                || x == ActionType.OPERATOR).Count();

            //calculate the new score
            this._score += this._target * numberOfMoves;

        }

        private void PlaySound(SoundEffect soundEffect, GameTime gameTime)
        {

            //if we want to play sound effects
            if (GameplayOptions.PlaySoundEffects)
            {

                //we don't want to play sounds to close together
                if (gameTime.TotalGameTime.TotalMilliseconds - _lastPlayedSound >= 500)
                {
                    soundEffect.Play();
                    _lastPlayedSound = gameTime.TotalGameTime.TotalMilliseconds;
                }

            }

        }

        private void AddOperation(GameTime gameTime, Move move)
        {

            //if we don't have any equation select the number
            //if (string.IsNullOrWhiteSpace(_currentEquation))
            //{
                
            //}


            //if we are on the letter A
            if (move.Name == "A")
            {

                //if we dont have a current equation OR if we are not on the previously selected
                if (string.IsNullOrEmpty(_currentEquation.Trim())
                    || _currentX != _selectedX 
                    || _currentY != _selectedY)
                {

                    //select the current number
                    this.SelectNumber(gameTime);

                }

                    //if the previous equation was an operator, cycle through the operator
                else if (_selectedActionTypes.Count > 0 && _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.OPERATOR
                    && _currentX == _selectedX
                    && _currentY == _selectedY)
                {

                    //cycle through the operators
                    this.CycleThroughOperators(gameTime);

                }
                else if (_selectedActionTypes.Count > 0 && _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.NUMBER
                    && _currentX == _selectedX
                    && _currentY == _selectedY)
                {

                    this.SelectNewOperator(move, gameTime);

                }

            }

            //if hit B
            else if (move.Name == "B")
            {

                //if we dont have a current equation OR if we are not on the previously selected
                if (string.IsNullOrEmpty(_currentEquation.Trim()))
                {

                    //play error sound
                    this.PlaySound(_incorrectSound, gameTime);

                    //set the flash text
                    this._flashText.SetFlashText("You must select a number first", gameTime.TotalGameTime.TotalSeconds, true, true);

                }

                    //if the previous equation was an operator, cycle through the operator
                else if (_selectedActionTypes.Count > 0 && _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.OPERATOR)
                {

                    //cycle through the operators
                    this.CycleThroughOperators(gameTime);

                }
                else if (_selectedActionTypes.Count > 0 && _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.NUMBER)
                {

                    this.SelectNewOperator(move, gameTime);

                }

            }

            //if we selected X or Y
            if (move.Name == "X" || move.Name == "Y")
            {

                //evaluate the equation
                this.EvaulateEquation(gameTime);

            }

        }

        private void SelectNewOperator(Move move, GameTime gameTime)
        {

            //add the current equation to the list
            _undoEquations.Add(_currentEquation);
            _undoType.Add("Operation");
            _selectedActionTypes.Add(ActionType.OPERATOR);

            //add the symbol
            _currentEquation += string.Format(" +");

        }

        private void CycleThroughOperators(GameTime gameTime)
        {

            //get the current operator
            var currentOperator = _currentEquation[_currentEquation.Length - 1].ToString();

            //get the new operator
            var newOperator = "-";
            if (currentOperator == "-")
            {
                newOperator = "*";
            }
            else if (currentOperator == "*")
            {
                newOperator = "/";
            }
            else if (currentOperator == "/")
            {
                newOperator = "+";
            }

            //set the new equation
            _currentEquation = _currentEquation.Substring(0, _currentEquation.Length - 1) + newOperator;

            //play success sound
            this.PlaySound(_correctSound, gameTime);

        }

        private void SelectNumber(GameTime gameTime)
        {

            //if we have a valid move
            if (this.IsValidNumberMove(gameTime))
            {

                //add the current equation to the list
                _undoEquations.Add(_currentEquation);
                _undoType.Add("Number");
                _selectedActionTypes.Add(ActionType.NUMBER);

                //get the current value
                var currentValue = this._gameBoardValues[_currentY, _currentX].ToString();

                //add the currentValue to our equation
                if (String.IsNullOrEmpty(_currentEquation))
                    this._currentEquation = currentValue;
                else
                    this._currentEquation += string.Format(" {0}", currentValue);

                //update the selected values
                _selectedX = _currentX;
                _selectedY = _currentY;

                //add the values to the previously selected list
                _selectedYXs.Add(string.Format("{0},{1}", _currentY, _currentX));

            }
            else
            {

                //TODO: Beef up error message, already selected number, numbers must connect

                //set the flash text
                this._flashText.SetFlashText("You must select an operator first", gameTime.TotalGameTime.TotalMilliseconds, true, true);

            }

        }

        private void UpdateBoardItems()
        {

            //we need to calculate the start point for the x and y axis
            int width, height, x, y;
            width = _boardSize * 50;
            height = _boardSize * 35;

            //get our starting X position
            x = (ScreenManager.GraphicsDevice.Viewport.Width / 2) - (width / 2);
            y = (ScreenManager.GraphicsDevice.Viewport.Height / 9 * 5) - (height / 2);

            // start at 175, 175
            Vector2 position = new Vector2(x, y);

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
                position.X = x;
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
                        if (_selectedYXs.FirstOrDefault(x => x == YX) != null)
                            countFound++;

                    }

                    //if the total count matches and all the items match, we already used this chain
                    if (countFound == currentSelectedTotal)
                        return true;

                }

            }

            return false;

        }

        private void ResetBoardForNextLevel()
        {
            this._currentEquation = "";
            this._selectedYXs.Clear();
            this._previouslySelectedYXs.Clear();
            this._selectedActionTypes.Clear();
        }

        private void HandleGameplayMove(GameTime gameTime, Move move)
        {


            if (move.Name == "Start")
            {

                if (!_isPaused)
                {
                    _isPaused = true;
                    MediaPlayer.Pause();
                }

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
                    _selectedActionTypes.RemoveAt(_selectedActionTypes.Count() - 1);

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
                    _selectedActionTypes.Add(ActionType.LEFT_PARENTHESIS);

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
                    _selectedActionTypes.Add(ActionType.RIGHT_PARENTHESIS);

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
                this.SelectNumber(gameTime);
            }
            else if (move.Name == "Evaluate")
            {

                //evaluate the equation
                this.EvaulateEquation(gameTime);

            }

        }

        private void EvaulateEquation(GameTime gameTime)
        {

            //if we don't have a current equation, select the current number
            if (String.IsNullOrEmpty(_currentEquation.Trim(new char[] { '(', ')' })))
                return;

            //get the result for the current equation
            decimal result = 0.0m;
            bool invalidEquation = false;
            try
            {
                result = Parser.ParseEquation(_currentEquation);
            }
            catch
            {
                invalidEquation = true;
            }


            //if the equation has errors
            if (invalidEquation)
            {

                //set the flash text
                _flashText.SetFlashText("Invalid equation", gameTime.TotalGameTime.TotalMilliseconds, true, false);

                //play the bad sound
                this.PlaySound(_incorrectSound, gameTime);

            }
            else //if we don't have any errors
            {

                //get the value of the actual expression entered
                var actual = Convert.ToInt32(result);

                //if they got it right
                if (actual == _target)
                {

                    //if they have used the equation previously
                    if (UseEquationPreviously())
                    {

                        //set the flash text
                        _flashText.SetFlashText("Already used that equation", gameTime.TotalGameTime.TotalMilliseconds, true, false);

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
                        _clock.AddTime(_secondForCorrectAnswer);

                        //we got a correct answer, so we need to calculate the correct answer
                        this.CalculateScore();
                        this._totalCorrect++;
                        this._roundCorrect++;

                        //set the flash text
                        _flashText.SetFlashText("Correct!", gameTime.TotalGameTime.TotalMilliseconds, true, false);

                        //play the bad sound
                        this.PlaySound(_correctSound, gameTime);

                    }

                }
                else //if they got it wrong 
                {

                    //set the flash text
                    _flashText.SetFlashText(string.Format("Incorrect, you number was {0}", actual), gameTime.TotalGameTime.TotalMilliseconds, true, false);

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

        private void HandlePausedMove(GameTime gameTime, Move move)
        {
            
            //TODO: Handle paused game logic here
            if (move.Name == "Start"
                || move.Name == "B"
                || (move.Name == "A" && _pasuedMenuCount == 1))
            {

                if (_isPaused)
                {
                    _isPaused = false;
                    MediaPlayer.Play(_backgroundMusic);
                }

            }
            else if (move.Name == "A" && _pasuedMenuCount == 2)
            {

                MediaPlayer.Stop();
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

        }

        #endregion

        #region High Score Methods

        public void SaveHighScores(string initials)
        {

            //add the new high score object
            _highScores.Add(new HighScore() 
            {
                Initials = initials,
                Score = _score
            });

            //save the file
            StorageHelper.SaveHighScores(_highScores, _boardSize);

        }

        public List<HighScore> LoadHighScores()
        {

            //get the high scores
            return StorageHelper.LoadHighScores(_boardSize);

        }

        #endregion

    }

}
