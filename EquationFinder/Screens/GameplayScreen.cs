using System;
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
using System.Reflection;

namespace EquationFinder.Screens
{

    public enum ActionType
    {
        NUMBER,
        OPERATOR,
        LEFT_PARENTHESIS,
        RIGHT_PARENTHESIS
    }

    public enum SelectActionResult
    {
        NOT_CONNECTED,
        OPERATOR_FIRST,
        ALREADY_SELECTED,
        SUCCESS,
        TOO_LONG
    }

    public enum MoveTypeCheck
    {
        REAL,
        HUD
    }

    public class GameplayScreen : GameScreen
    {

        ContentManager _content;

        //board variables
        GameTypeEnum _gameTypeEnum;
        int _boardSize;
        int _currentX, _currentY;
        int _selectedX, _selectedY;
        int _target;
        int _startingNumber;
        int _score;
        int _totalCorrect;
        int _roundCorrect;
        int _roundTarget;
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
        int _secondsPerRound = 90;
        int _secondForCorrectAnswer = 5;
        bool _isPaused;
        int _pasuedMenuCount;

        // The font used to display UI elements
        SpriteFont _gameFont;
        SpriteFont _boardFont;
        ColorPalette _colorPalatte;

        //the sound effects
        SoundEffect _correctSound;
        SoundEffect _incorrectSound;
        Song _backgroundMusic;
        Texture2D _backgroundImage;

        //the display hud for moves
        Dictionary<string, string> _hudMoves;
        string _hudMovesDisplay;

        private int _moveCount = 0;
        private Texture2D _emptyTexture;
        private bool _drawPreviousLines;

        public GamePadState GamePadState { get; private set; }
        public KeyboardState KeyboardState { get; private set; }

        private List<Color> _predefinedColors = new List<Color>();

        /// <summary>
        /// The last "real time" that new input was received. Slightly late button
        /// presses will not update this time; they are merged with previous input.
        /// </summary>
        public TimeSpan LastDirectionalInputTime { get; private set; }

        #region Initialization

        public GameplayScreen(int target)
        {

            //if the target is less than 0, create a random target
            if (target < 0)
            {
                var random = new Random();
                target = random.Next(11, 50);
            }

            //setup the transitions
            TransitionOnTime = TimeSpan.FromSeconds(1.5);
            TransitionOffTime = TimeSpan.FromSeconds(0.5);

            //set the global variables
            this._gameTypeEnum = GameplayOptions.GameType;
            this._boardSize = GameplayOptions.BoardSize;
            this._currentX = 0;
            this._currentY = 0;
            _selectedX = -1;
            _selectedY = -1;
            _target = target;
            _startingNumber = target;

            //get a board
            this._gameBoard = new BoardItem[GameplayOptions.BoardSize, GameplayOptions.BoardSize];
            this._gameBoardValues = BoardLogic.CreateBoard(GameplayOptions.BoardSize, GameplayOptions.StartNumber);

            this._drawPreviousLines = false;
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
            this._roundTarget = Convert.ToInt32(Math.Floor(this._target / 3f));
            if (_roundTarget < 1)
                _roundTarget = 1;

            //setup the HUD for the moves
            this._hudMoves = new Dictionary<string, string>();

            //get a list of all the colors we want to use to draw lines
            _predefinedColors.Add(new Color(82, 0, 229));
            _predefinedColors.Add(new Color(0, 229, 15));
            _predefinedColors.Add(new Color(229, 0, 51));
            _predefinedColors.Add(new Color(0, 118, 229));
            _predefinedColors.Add(new Color(185, 229, 0));
            _predefinedColors.Add(new Color(206, 0, 229));
            _predefinedColors.Add(new Color(0, 229, 139));
            _predefinedColors.Add(new Color(229, 72, 0));
            _predefinedColors.Add(new Color(5, 0, 229));
            _predefinedColors.Add(new Color(61, 229, 0));
            _predefinedColors.Add(new Color(229, 0, 128));
            _predefinedColors.Add(new Color(0, 195, 229));
            _predefinedColors.Add(new Color(229, 196, 0));
            _predefinedColors.Add(new Color(129, 0, 229));
            _predefinedColors.Add(new Color(0, 229, 62));
            _predefinedColors.Add(new Color(229, 0, 4));
            _predefinedColors.Add(new Color(0, 71, 229));
            _predefinedColors.Add(new Color(138, 229, 0));
            _predefinedColors.Add(new Color(229, 0, 205));
            _predefinedColors.Add(new Color(0, 229, 186));
            _predefinedColors.Add(new Color(229, 119, 0));
            _predefinedColors.Add(new Color(52, 0, 229));
            _predefinedColors.Add(new Color(14, 229, 0));
            _predefinedColors.Add(new Color(229, 0, 81));
            _predefinedColors.Add(new Color(0, 148, 229));
            _predefinedColors.Add(new Color(215, 229, 0));
            _predefinedColors.Add(new Color(177, 0, 229));
            _predefinedColors.Add(new Color(0, 229, 110));
            _predefinedColors.Add(new Color(229, 43, 0));
            _predefinedColors.Add(new Color(0, 23, 229));
            _predefinedColors.Add(new Color(90, 229, 0));
            _predefinedColors.Add(new Color(229, 0, 157));
            _predefinedColors.Add(new Color(0, 224, 229));
            _predefinedColors.Add(new Color(229, 167, 0));
            _predefinedColors.Add(new Color(100, 0, 229));
            _predefinedColors.Add(new Color(0, 229, 33));
            _predefinedColors.Add(new Color(229, 0, 33));
            _predefinedColors.Add(new Color(0, 100, 229));
            _predefinedColors.Add(new Color(167, 229, 0));
            _predefinedColors.Add(new Color(224, 0, 229));

        }

        /// <summary>
        /// Load graphics content for the game.
        /// </summary>
        public override void LoadContent()
        {
            if (_content == null)
                _content = new ContentManager(ScreenManager.Game.Services, "Content");

            // Load the background texture we will be using
            _backgroundImage = _content.Load<Texture2D>("img/bg/" + GameplayOptions.BackgroundImage);

            //Load the fonts
            _gameFont = _content.Load<SpriteFont>("gameFont");
            _boardFont = _content.Load<SpriteFont>("boardFont");

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
                spriteBatch.Draw(_backgroundImage,
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
            var x = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X;
            var y = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height / 2) - 50;

            //draw the strings
            spriteBatch.DrawString(_gameFont, "Resume Game", new Vector2(x, y), _pasuedMenuCount == 1 ? _colorPalatte.PausedMenuTextSelected : _colorPalatte.PausedMenuTextUnselected);
            spriteBatch.DrawString(_gameFont, "Exit to menu", new Vector2(x, y + 50), _pasuedMenuCount == 2 ? _colorPalatte.PausedMenuTextSelected : _colorPalatte.PausedMenuTextUnselected);

        }

        private void DrawGame(SpriteBatch spriteBatch, GameTime gameTime)
        {

            // Draw the score
            spriteBatch.DrawString(_gameFont, "Target: " + _target, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), this._colorPalatte.DisplayText);
            spriteBatch.DrawString(_gameFont, "Score: " + string.Format("{0:n0}", this._score), new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 30), this._colorPalatte.DisplayText);
            spriteBatch.DrawString(_gameFont, "Equation: " + _currentEquation, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X, ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 60), this._colorPalatte.DisplayText);

            //get the time string
            var time = "Time:   " + _clock.displayClock;
            if (_clock.displayClock == "Game Over")
                time = "Game Over";

            //if we are doing free play, set the time
            if (_gameTypeEnum == GameTypeEnum.FREE_PLAY)
                time = "Time: Free Play";

            //draw the space need for the time string
            Vector2 stringSize = _gameFont.MeasureString(time);
            spriteBatch.DrawString(_gameFont, time, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - stringSize.Length(),
                ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y), this._colorPalatte.DisplayText);

            //if they are in trail mode, show N/A
            var remaining = "Next Level: ";
            if (Guide.IsTrialMode || _gameTypeEnum == GameTypeEnum.FREE_PLAY)
                remaining += "N/A";
            else if (Math.Max(this._roundTarget - this._roundCorrect, 0) > 0)
                remaining += string.Format("{0:n0}", Math.Max(this._roundTarget - this._roundCorrect, 0));
            else
                remaining += "unlocked";

            stringSize = _gameFont.MeasureString(remaining);
            spriteBatch.DrawString(_gameFont, remaining, new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width - stringSize.Length(), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 30), this._colorPalatte.DisplayText);

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
                    this._gameBoard[row, col].Draw(this, 
                        isSelected, 
                        isPreviouslySelected, 
                        gameTime, 
                        this._colorPalatte,
                        _boardFont);

                    //go to the next column
                    col++;

                }

                //go to the next row
                row++;
                col = 0;

            }

            //draw the hud at the bottom
            stringSize = _gameFont.MeasureString(this.HudMovesDisplay);
            spriteBatch.DrawString(
                    _gameFont,
                    this.HudMovesDisplay,
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - (stringSize.Length() / 2), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height - 30),
                    this._colorPalatte.DisplayText);

            //if we have flash text, draw the string
            if (_flashText.Active)
            {

                //calculate where the flash text goes
                Vector2 textSize = _gameFont.MeasureString(_flashText.Text);

                //display the text
                spriteBatch.DrawString(
                    _gameFont,
                    _flashText.Text,
                    new Vector2(ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - (textSize.Length() / 2), ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 30),
                    (_flashText.IsErrorText) ? this._colorPalatte.FlashTextIncorrect : this._colorPalatte.FlashTextCorrect);

            }

            //if we want to draw the lines
            if (_drawPreviousLines)
                DrawLines(spriteBatch);

        }

        private void DrawLines(SpriteBatch spriteBatch)
        {

            var colorLoop = 0;
            foreach (var previousSelectedYX in _previouslySelectedYXs.OrderByDescending(x => x.Count()))
            {

                var count = previousSelectedYX.Count();
                if (count > 1) 
                {
                    var i = 0;
                    while (i <= count - 2) 
                    {

                        var crumbs = previousSelectedYX[i].Split(new char[] { ',' });
                        var point1 = this._gameBoard[Convert.ToInt32(crumbs[0]), Convert.ToInt32(crumbs[1])].GetCenter(this);

                        crumbs = previousSelectedYX[i + 1].Split(new char[] { ',' });
                        var point2 = this._gameBoard[Convert.ToInt32(crumbs[0]), Convert.ToInt32(crumbs[1])].GetCenter(this);

                        this.DrawLine(spriteBatch, _predefinedColors[colorLoop % _predefinedColors.Count()], point1, point2, 3f, 0.0f);

                        //go to the next point
                        i++;
                    }
                }

                colorLoop++;

            }

        }

        /// <summary>
        /// Draw a line into a SpriteBatch
        /// https://gist.github.com/pctroll/2731936
        /// </summary>
        /// <param name="batch">SpriteBatch to draw line</param>
        /// <param name="color">The line color</param>
        /// <param name="point1">Start Point</param>
        /// <param name="point2">End Point</param>
        /// <param name="Layer">Layer or Z position</param>
        public void DrawLine(SpriteBatch batch, Color color, Vector2 point1,
                                    Vector2 point2, float thinkness, float Layer)
        {

            float angle = (float)Math.Atan2(point2.Y - point1.Y, point2.X - point1.X);
            float length = (point2 - point1).Length();

            batch.Draw(_emptyTexture, point1, null, color,
                       angle, Vector2.Zero, new Vector2(length, thinkness),
                       SpriteEffects.None, Layer);
        }

        public override void Update(GameTime gameTime, bool otherScreenHasFocus, bool coveredByOtherScreen)
        {

            // clock start and update  
            if (_clock.isRunning == false)
            {

                //update the board items
                if (!this._isBoardInitialized)
                {

                    //get the color palette
                    this._colorPalatte = ColorPaletteHelper.GetColorPalette();

                    //update the board items
                    UpdateBoardItems();

                    //run the hud update
                    this.PopulateHudMoves(gameTime);

                    //setup the empty texture for drawing lines
                    _emptyTexture = new Texture2D(ScreenManager.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                    _emptyTexture.SetData(new[] { Color.White });

                    //set the init flag
                    this._isBoardInitialized = true;

                }

                //count 90 seconds down 
                _clock.Start(_secondsPerRound);
            }
            else if ((_clock.isFinished == false || _gameTypeEnum == GameTypeEnum.FREE_PLAY) && _isPaused == false)
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
                    if (_clock.displayClock == "Game Over" 
                        && this._roundCorrect >= this._roundTarget
                        && Guide.IsTrialMode == false
                        && _gameTypeEnum == GameTypeEnum.TIMED)
                    {

                        //go to the next round
                        _clock.NextRound(_secondsPerRound);

                        //go to the next number
                        this._target++;

                        //reset the round count
                        this._roundCorrect = 0;
                        this._roundTarget = Convert.ToInt32(Math.Floor(this._target / 3f));
                        if (_roundTarget < 1)
                            _roundTarget = 1;

                        //set the flash text
                        _flashText.Text = string.Format("Next Level {0}", _target);
                        _flashText.StartTime = gameTime.TotalGameTime.TotalMilliseconds;
                        _flashText.Active = true;
                        _flashText.RunClock = true;
                        
                        //reset the board for the next level
                        this.ResetBoardForNextLevel(gameTime);

                    }

                }

                //reset the flash text
                if( _flashText.Active == true
                    && (gameTime.TotalGameTime.TotalMilliseconds - _flashText.StartTime) > 1250)
                {
                    _flashText.Active = false;
                    _flashText.Text = "";
                    _flashText.RunClock = true;
                }   

                //if we want to save the high score file
                if (_clock.isFinished == true
                    && _gameTypeEnum == GameTypeEnum.TIMED)
                {

                    // the game is over, show the high score screen
                    MediaPlayer.Stop();
                    LoadingScreen.Load(ScreenManager, true, null, new SaveHighScoreScreen(_boardSize, _score, _score > _highScoreToBeat, _startingNumber));

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
            GamePadState = InputHelpers.GetGamePadStateForAllPLayers();
            KeyboardState = InputHelpers.GetKeyboardStateForAllPLayers();

            TimeSpan time = gameTime.TotalGameTime;
            TimeSpan timeSinceLast = time - LastDirectionalInputTime;

            //get the direction
            var direction = Direction.FromInput(GamePadState, KeyboardState, lastGamePadState);
            if (Direction.FromInput(lastGamePadState, lastKeyboardState, lastGamePadState) != direction
                && direction != 0.0
                && timeSinceLast >= TimeSpan.FromMilliseconds(165))
            {
                LastDirectionalInputTime = time;
                this.HandleDirection(direction, gameTime);
            }
                
            base.Update(gameTime, otherScreenHasFocus, coveredByOtherScreen);

        }

        public override void HandleMove(GameTime gameTime, Move move)
        {

            _moveCount++;

            //don't do anything if the clock isn't running
            if ((_clock.isRunning == false || _clock.isFinished == true) && _gameTypeEnum != GameTypeEnum.FREE_PLAY)
                return;

            //determine which input mode we should be in
            if (_isPaused)
                this.HandlePausedMove(gameTime, move);
            else
            {
                this.HandleGameplayMove(gameTime, move, MoveTypeCheck.REAL);
                this.PopulateHudMoves(gameTime);
            }

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

                MediaPlayer.Volume = 0.7f;

                // Play the music
                MediaPlayer.Play(song);

                // Loop the currently playing song
                MediaPlayer.IsRepeating = true;

            }
            catch { }
        }

        private void HandleDirection(Buttons direction, GameTime gameTime)
        {

            if (_isPaused)
                this.HandlePausedDirection(direction);
            else
            {
                this.HandleGameplayDirection(direction);
                this.PopulateHudMoves(gameTime);
            }

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

        private void PlaySound(SoundEffect soundEffect, GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {

            //if we want to play sound effects
            if (GameplayOptions.PlaySoundEffects && moveTypeCheck == MoveTypeCheck.REAL)
            {

                float volume = 0.7f;
                float soundStackingLevel = 1;
 
                //Each time you play a sound 
                soundEffect.Play(volume * soundStackingLevel, 0, 0); 
                soundStackingLevel *= 0.5f; 
             
                //Each update 
                soundStackingLevel = Math.Min(soundStackingLevel + 0.1f, 1f); 

            }

        }

        private string AddOperation(GameTime gameTime, Move move, MoveTypeCheck moveTypeCheck)
        {

            //if we are on the letter A
            if (move.Name == "A"
                || move.Name == "B")
            {

                //if we dont have a current equation OR if we are not on the previously selected
                if (string.IsNullOrEmpty(_currentEquation.Trim())
                    || _currentX != _selectedX 
                    || _currentY != _selectedY)
                {

                    //select the current number
                    return this.SelectNumber(gameTime, moveTypeCheck);

                }

                    //if the previous equation was an operator, cycle through the operator
                else if (_selectedActionTypes.Count > 0 && _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.OPERATOR
                    && _currentX == _selectedX
                    && _currentY == _selectedY)
                {

                    //cycle through the operators
                    return this.CycleThroughOperators(gameTime, moveTypeCheck);

                }
                else if (_selectedActionTypes.Count > 0 
                    && (_selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.NUMBER
                        || _selectedActionTypes[_selectedActionTypes.Count() - 1] == ActionType.RIGHT_PARENTHESIS)
                    && _currentX == _selectedX
                    && _currentY == _selectedY)
                {

                    if (moveTypeCheck == MoveTypeCheck.REAL)
                        this.SelectNewOperator();
                    else if (moveTypeCheck == MoveTypeCheck.HUD)
                        return "+";

                }

            }

            //if we selected X or Y
            else if (move.Name == "X" || move.Name == "Y")
            {

                //evaluate the equation
                this.EvaulateEquation(gameTime, MoveTypeCheck.REAL);

            }

            return null;

        }

        private void SelectNewOperator()
        {

            //add the current equation to the list
            _undoEquations.Add(_currentEquation);
            _undoType.Add("Operation");
            _selectedActionTypes.Add(ActionType.OPERATOR);

            //add the symbol
            _currentEquation += string.Format(" +");

        }

        private string CycleThroughOperators(GameTime gameTime, MoveTypeCheck moveTypeCheck)
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

            //if we have a real move
            if (moveTypeCheck == MoveTypeCheck.REAL)
            {

                //set the new equation
                _currentEquation = _currentEquation.Substring(0, _currentEquation.Length - 1) + newOperator;

                //play success sound
                this.PlaySound(_correctSound, gameTime, moveTypeCheck);

            }
            else if (moveTypeCheck == MoveTypeCheck.HUD)
            {

                return newOperator;

            }

            return null;

        }

        private string SelectNumber(GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {

            var result = this.IsValidNumberMove(gameTime, moveTypeCheck);

            //if we have a valid move
            if (result == SelectActionResult.SUCCESS
                || result == SelectActionResult.OPERATOR_FIRST)
            {

                //if we have a real move
                if (moveTypeCheck == MoveTypeCheck.REAL)
                {

                    //if the user forgot to set an operator, assume they meant to put a '+' and then select the letter
                    if (result == SelectActionResult.OPERATOR_FIRST)
                        this.SelectNewOperator();

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
                else if (moveTypeCheck == MoveTypeCheck.HUD)
                {

                    //get the return string and return it
                    var returnValue = String.Empty;
                    if (result == SelectActionResult.OPERATOR_FIRST)
                        returnValue = "+ ";
                    return returnValue + this._gameBoardValues[_currentY, _currentX].ToString();

                }


            }
            else
            {

                //if we have a real move
                if (moveTypeCheck == MoveTypeCheck.REAL)
                {

                    //set the flash text
                    if (result == SelectActionResult.TOO_LONG)
                        this._flashText.SetFlashText("The equation maximum is 15 numbers.", gameTime.TotalGameTime.TotalMilliseconds, true, true);
                    else if (result == SelectActionResult.ALREADY_SELECTED)
                        this._flashText.SetFlashText("Number already selected.", gameTime.TotalGameTime.TotalMilliseconds, true, true);
                    else if (result == SelectActionResult.NOT_CONNECTED)
                        this._flashText.SetFlashText("Numbers must be connected.", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                }

            }

            return null;

        }

        private void UpdateBoardItems()
        {

            //we need to calculate the start point for the x and y axis
            int width, x, y;
            width = (_boardSize * 60) - 30;

            //get our starting X,Y position
            x = (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X + (ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Width / 2) - (width / 2));
            //y = (int)((decimal)ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Height * 0.56m) - (height / 2);
            //x = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.X;
            y = ScreenManager.GraphicsDevice.Viewport.TitleSafeArea.Y + 120;

            //set the starting position
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
                    position.X += 60;

                    //go to the next column
                    col++;

                }

                //go to the next row
                row++;
                col = 0;
                position.X = x;
                position.Y += 31;

            }

        }

        private bool IsValidOperationMove(GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number" || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                return false;

            }

        }

        private bool IsValidRightParenthesisMove(GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number"
                    || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                return false;

            }

        }

        private bool IsValidLeftParenthesisMove(GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {

            //Check to make sure this is a valid move
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Operation"
                    || undoType == "Left Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                    return true;
                }
                else
                {
                    this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                    return false;
                }

            }
            else
            {

                this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                return true;

            }

        }

        private SelectActionResult IsValidNumberMove(GameTime gameTime, MoveTypeCheck moveTypeCheck)
        {


            if (_selectedX < 0 && _selectedY < 0)
            {
                this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                return SelectActionResult.SUCCESS;
            }

            if (!((Math.Abs(_selectedX - _currentX) <= 1) && (Math.Abs(_selectedY - _currentY) <= 1)))
            {
                //play the bad sound
                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                return SelectActionResult.NOT_CONNECTED;
            }


            if (_undoType.Where(x => x == "Number").Count() >= 15)
            {
                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                return SelectActionResult.TOO_LONG;
            }

            //Check to make sure this is a valid move for a number
            if (_undoType.Count() > 0)
            {

                var undoType = _undoType[_undoType.Count() - 1];
                if (undoType == "Number" || undoType == "Right Parenthesis")
                {
                    this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                    return SelectActionResult.OPERATOR_FIRST;
                }

            }

            
            //if the current is the selected, return false
            if (_selectedY == _currentY && _selectedX == _currentX)
            {
                //play the bad sound
                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                return SelectActionResult.ALREADY_SELECTED;
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
                        this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
                        return SelectActionResult.ALREADY_SELECTED;
                    }

                }

                //play the good sound
                this.PlaySound(_correctSound, gameTime, moveTypeCheck);
                return SelectActionResult.SUCCESS;
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

        private void ResetBoardForNextLevel(GameTime gameTime)
        {
            this._currentEquation = "";
            this._selectedYXs.Clear();
            this._previouslySelectedYXs.Clear();
            this._selectedActionTypes.Clear();

            //run the hud update
            this.PopulateHudMoves(gameTime);

        }

        private string HandleGameplayMove(GameTime gameTime, Move move, MoveTypeCheck moveTypeCheck)
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

                    //if we have a real move
                    if (moveTypeCheck == MoveTypeCheck.REAL)
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
                    else if (moveTypeCheck == MoveTypeCheck.HUD)
                    {
                        return "Undo";
                    }

                }

            }
            else if (move.Name == "Left Parenthesis")
            {

                //if we have a valid move
                if (this.IsValidLeftParenthesisMove(gameTime, moveTypeCheck))
                {

                    //if we are doing the move for real
                    if (moveTypeCheck == MoveTypeCheck.REAL)
                    {

                        //add the current equation to the list
                        _undoEquations.Add(_currentEquation);
                        _undoType.Add("Left Parenthesis");
                        _selectedActionTypes.Add(ActionType.LEFT_PARENTHESIS);

                        //update the equation
                        _currentEquation += " (";
                        _currentEquation = _currentEquation.Trim();

                    }
                    else if (moveTypeCheck == MoveTypeCheck.HUD)
                    {

                        return "(";

                    }

                }
                else
                {
                    //if we are doing the move for real, set the text
                    if (moveTypeCheck == MoveTypeCheck.REAL)
                        this._flashText.SetFlashText("Can't use left parenthesis now.", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                }

            }
            else if (move.Name == "Right Parenthesis")
            {

                //if we have a valid move
                if (this.IsValidRightParenthesisMove(gameTime, moveTypeCheck))
                {

                    //if we are doing the move for real
                    if (moveTypeCheck == MoveTypeCheck.REAL)
                    {

                        //add the current equation to the list
                        _undoEquations.Add(_currentEquation);
                        _undoType.Add("Right Parenthesis");
                        _selectedActionTypes.Add(ActionType.RIGHT_PARENTHESIS);

                        //update the equation
                        _currentEquation += ")";

                    }
                    else if (moveTypeCheck == MoveTypeCheck.HUD)
                    {

                        return ")";

                    }

                }
                else
                {

                    //if we are doing the move for real, set the text
                    if (moveTypeCheck == MoveTypeCheck.REAL)
                        this._flashText.SetFlashText("Can't use right parenthesis now.", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                }

            }
            else if (move.Name == "Y")
            {
                _drawPreviousLines = !_drawPreviousLines;
            }
            else if (move.Name == "A"
                || move.Name == "B"
                || move.Name == "X")
            {

                return this.AddOperation(gameTime, move, moveTypeCheck);

            }
            else if (move.Name == "Evaluate")
            {

                //evaluate the equation
                this.EvaulateEquation(gameTime, MoveTypeCheck.REAL);

            }

            return null;

        }

        private void EvaulateEquation(GameTime gameTime, MoveTypeCheck moveTypeCheck)
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
                _flashText.SetFlashText("Invalid equation", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                //play the bad sound
                this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);

            }
            else //if we don't have any errors
            {

                //get the value of the actual expression entered
                var actual = 0;
                if (int.TryParse(result.ToString(), out actual))
                {

                    //if it is an invalid actual
                    if (actual == int.MinValue)
                    {

                        //set the flash text
                        _flashText.SetFlashText("Invalid equation", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                        //play the bad sound
                        this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);

                    }
                    else if (actual == _target)  //if they got it right
                    {

                        //if they have used the equation previously
                        if (UseEquationPreviously())
                        {

                            //set the flash text
                            _flashText.SetFlashText("Already used that equation", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                            //play the bad sound
                            this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);
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
                            this.PlaySound(_correctSound, gameTime, moveTypeCheck);

                        }

                    }
                    else //if they got it wrong 
                    {

                        //set the flash text
                        _flashText.SetFlashText(string.Format("Incorrect, result was {0}", actual), gameTime.TotalGameTime.TotalMilliseconds, true, true);

                        //play the bad sound
                        this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);

                    }

                }
                else
                {

                    //set the flash text
                    _flashText.SetFlashText("Invalid equation", gameTime.TotalGameTime.TotalMilliseconds, true, true);

                    //play the bad sound
                    this.PlaySound(_incorrectSound, gameTime, moveTypeCheck);

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
            
            if (move.Name == "Start"
                || move.Name == "B"
                || (move.Name == "A" && _pasuedMenuCount == 1))
            {

                //if the game is pasued, let's unpause it
                if (_isPaused)
                {

                    //if we need to, start the music again
                    if (_backgroundMusic != null)
                        MediaPlayer.Play(_backgroundMusic);

                    //resume the game
                    _isPaused = false;

                }

            }
            else if (move.Name == "A" && _pasuedMenuCount == 2)
            {

                MediaPlayer.Stop();
                LoadingScreen.Load(ScreenManager, true, null, new MainMenuScreen());

            }

        }

        private void PopulateHudMoves(GameTime gameTime)
        {
            //clear out the hud moves
            _hudMoves.Clear();
            _hudMovesDisplay = null;

            _hudMoves.Add("A: ", HandleGameplayMove(gameTime, new Move("A", null), MoveTypeCheck.HUD) ?? "N/A");
            _hudMoves.Add("LB: ", HandleGameplayMove(gameTime, new Move("Left Parenthesis", null), MoveTypeCheck.HUD) ?? "N/A");
            _hudMoves.Add("RB: ", HandleGameplayMove(gameTime, new Move("Right Parenthesis", null), MoveTypeCheck.HUD) ?? "N/A");
            _hudMoves.Add("LT: ", HandleGameplayMove(gameTime, new Move("Undo", null), MoveTypeCheck.HUD) ?? "N/A");
            _hudMoves.Add("RT: ", "Eval");
            _hudMoves.Add("Y: ", _drawPreviousLines == false ? "Show Lines" : "Hide Lines");

        }

        public string HudMovesDisplay 
        {
            get 
            {
                //if we don't have hub moves display
                if (_hudMovesDisplay == null) 
                {
                    _hudMovesDisplay = string.Join(", ", _hudMoves.Select(x => x.Key + x.Value).ToArray());
                }
             
                return _hudMovesDisplay;
            }

        }

        #endregion

        #region High Score Methods

        public List<HighScore> LoadHighScores()
        {

            //get the high scores
            return StorageHelper.LoadHighScores(_boardSize);

        }

        #endregion

    }

}
