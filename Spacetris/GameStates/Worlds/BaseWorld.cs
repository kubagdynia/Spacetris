﻿using SFML.Audio;
using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.DataStructures;
using Spacetris.Extensions;
using Spacetris.Managers;
using Spacetris.Settings;
using System;
using System.Threading;

namespace Spacetris.GameStates.Worlds
{
    public abstract class BaseWorld : BaseGameState, IWorld
    {
        private string _userName;
        private bool _isKeyDownEnabled = true;

        private Timer _timer;
        private int _tickTimer;

        public Sprite GameController;

        public Sound GameSoundMoveTetromino;
        public Sound GameSoundDropTetromino;
        public Sound GameSoundRemoveLine;
        public Sound GameSoundGameOver;
        public Sound GameSoundLevelUp;

        private WorldState _worldState;
        public WorldState WorldState
        {
            get => _worldState;
            set
            {
                if (_worldState != value && WorldStateChanged != null)
                {
                    _worldState = value;
                    WorldStateChanged(this, WorldState);
                }

                if (value == WorldState.NewGame)
                {
                    NewGame();
                    InitializeTimer();
                }

                if (value == WorldState.Continue)
                {
                    InitializeTimer();
                }
            }
        }

        public abstract int WorldColumns { get; }

        public abstract int WorldRows { get; }

        public abstract int SpriteBlockSize { get; }

        public Point2 DrawOffset { get; private set; } = Point2.Zero;

        public abstract Point2 WorldDrawOffset { get; }

        public abstract Point2 NextTetrominoPreviewPosition { get; }

        protected abstract string BackgroundTexturePath { get; }

        protected abstract string BlocksTexturePath { get; }

        protected abstract string FontName { get; }

        protected abstract string CounterFontName { get; }

        private bool _readyForRotate = true;

        private Point2 _offsetMove = Point2.Zero;

        private Sprite _blockSprite;
        public Sprite BlockSprite
        {
            get
            {
                if (_blockSprite == null)
                {
                    _blockSprite = LoadSprite(BlocksTexturePath, Point2.Zero);
                }

                return _blockSprite;
            }
        }

        private Sprite _backgroundSprite;
        public Sprite BackgroundSprite
        {
            get
            {
                if (_backgroundSprite == null)
                {
                    _backgroundSprite = LoadSprite(BackgroundTexturePath, DrawOffset);
                }

                return _backgroundSprite;
            }
        }

        public Font Font
        {
            get
            {
                return AssetManager.Instance.Font.Get(FontName);
            }
        }

        public Font CounterFont
        {
            get
            {
                return AssetManager.Instance.Font.Get(CounterFontName);
            }
        }

        private int[,] _world;

        public event EventHandler<WorldState> WorldStateChanged;

        public int[,] World
        {
            get
            {
                if (_world == null)
                {
                    _world = new int[WorldRows, WorldColumns];
                }

                return _world;
            }
        }

        public virtual void Initialize(RenderWindow target)
        {
            UpdateDrawOffset(target);
        }

        public virtual void Update(RenderWindow target, float deltaTime)
        {

        }

        public virtual void UpdateDrawOffset(RenderWindow target, int offsetX = 0, int offsetY = 0)
        {
            DrawOffset = new Point2(
                (target.Size.X - BackgroundSprite.Texture.Size.X) / 2 + offsetX,
                (target.Size.Y - BackgroundSprite.Texture.Size.Y) / 2 + offsetY);

            BackgroundSprite.Position = new Vector2f(DrawOffset.X, DrawOffset.Y);
        }

        public virtual void ChangeBlockSpritePosition(int x, int y)
        {
            BlockSprite.Position = new Vector2f(x, y);
        }

        public virtual void ChangeBlockSpriteTextureRect(int spriteBlockNumber)
        {
            if (spriteBlockNumber <= 0)
            {
                return;
            }

            spriteBlockNumber--;

            BlockSprite.TextureRect = new IntRect(spriteBlockNumber * SpriteBlockSize,
                0, SpriteBlockSize, SpriteBlockSize);
        }

        public virtual Point2 GetTetrominoSpawnPosition()
        {
            return new Point2((WorldColumns - Tetrominos.TetrominoSize) / 2, -1);
        }

        public virtual void PinBlockToWorld(int x, int y, int tetrominoIndex)
        {
            World[x, y] = tetrominoIndex;
        }

        public virtual bool CheckWallCollider(Point2[] tetrominoBlocksPosition)
        {
            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                if (tetrominoBlocksPosition[i].X < 0 || tetrominoBlocksPosition[i].X >= WorldColumns)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual bool CheckWorldCollider(Point2[] tetrominoBlocksPosition)
        {
            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                if (tetrominoBlocksPosition[i].X < 0 || tetrominoBlocksPosition[i].X >= WorldColumns || tetrominoBlocksPosition[i].Y < 0)
                {
                    continue;
                }

                if (tetrominoBlocksPosition[i].Y >= WorldRows || World[tetrominoBlocksPosition[i].Y, tetrominoBlocksPosition[i].X] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        public virtual void DrawBackground(RenderWindow target, byte alpha = 255)
        {
            BackgroundSprite.Color = new Color(255, 255, 255, alpha);
            target.Draw(BackgroundSprite);
        }

        public virtual void DrawWorld(RenderWindow target, bool drawLandingShadow = true, byte alpha = 255)
        {
            BlockSprite.Color = new Color(255, 255, 255, alpha);

            for (int row = 0; row < WorldRows; row++)
            {
                for (int column = 0; column < WorldColumns; column++)
                {
                    int blockNumber = World[row, column];
                    if (blockNumber != 0)
                    {
                        ChangeBlockSpritePosition(
                            column * SpriteBlockSize + DrawOffset.X + WorldDrawOffset.X,
                            row * SpriteBlockSize + DrawOffset.Y + WorldDrawOffset.Y);

                        ChangeBlockSpriteTextureRect(blockNumber);

                        target.Draw(BlockSprite);
                    }
                }
            }
            if (drawLandingShadow)
            {
                DrawTetrominoLandingShadow(target);
            }
            DrawTetromino(target, GameState.CurrentTetrominoBlocksPosition);
        }

        public virtual void DrawGui(RenderWindow target, byte alpha = 255)
        {
            DrawNextTetrominoPreview(target);

            DrawScore(target, 272, 148, alpha);
            DrawLevel(target, 272, 208, alpha);
            DrawLines(target, 272, 266, alpha);
        }

        public virtual void DrawStartCounter(RenderWindow target)
        {
            DrawText(target, CounterFont, _tickTimer.ToString(), 440, 280, new Color(242, 51, 51, 189), 90, true, true);
        }

        public virtual void DrawGameOver(RenderWindow target)
        {
            DrawText(target, CounterFont, "GAME OVER", 470, 180, new Color(242, 51, 51, 189), 130, true, true);

            Color scoreColor = new Color(255, 255, 255, 189);

            // Top 5 scores
            if (GameSettings.IsEnoughPointsForTop5(GameState.Score))
            {
                DrawText(target, Font, "Congratulations!", 470, 280, scoreColor, 30, true, true);
                DrawText(target, Font, "Great Score", 470, 320, scoreColor, 30, true, true);

                DrawText(target, Font, "Level", 190, 390, scoreColor, 30, true, true);
                DrawText(target, Font, "Name", 470, 390, scoreColor, 30, true, true);
                DrawText(target, Font, "Score", 770, 390, scoreColor, 30, true, true);

                DrawText(target, Font, GameState.Level.ToString(), 190, 430, scoreColor, 30, true, true);
                DrawText(target, Font, GameState.Score.ToString(), 770, 430, scoreColor, 30, true, true);

                if (!string.IsNullOrEmpty(_userName))
                {
                    DrawText(target, Font, _userName, 470, 430, scoreColor, 30, true, true);
                }

                DrawText(target, Font, "Type your name and press enter", 470, 490, scoreColor, 30, true, true);
            }
            else
            {
                DrawText(target, Font, "It's all over for you!", 470, 280, scoreColor, 30, true, true);
                DrawText(target, Font, "Level", 190, 390, scoreColor, 30, true, true);
                DrawText(target, Font, "Score", 770, 390, scoreColor, 30, true, true);

                DrawText(target, Font, GameState.Level.ToString(), 190, 430, scoreColor, 30, true, true);
                DrawText(target, Font, GameState.Score.ToString(), 770, 430, scoreColor, 30, true, true);

                DrawText(target, Font, "Press Esc to back to menu", 470, 390, scoreColor, 30, true, true);
            }
        }

        public virtual void DrawTetromino(RenderWindow target, Point2[] tetrominoBlocksPosition)
        {
            ChangeBlockSpriteTextureRect(GameState.CurrentTetrominoNumber);
            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                ChangeBlockSpritePosition(
                    tetrominoBlocksPosition[i].X * SpriteBlockSize + DrawOffset.X + WorldDrawOffset.X,
                    tetrominoBlocksPosition[i].Y * SpriteBlockSize + DrawOffset.Y + WorldDrawOffset.Y);

                target.Draw(BlockSprite);
            }
        }

        public virtual void DrawNextTetrominoPreview(RenderWindow target)
        {
            int tetrominoNumber = GameState.NextTetrominoNumber;

            for (int row = 0; row < Tetrominos.TetrominoSize; row++)
            {
                for (int column = 0; column < Tetrominos.TetrominoSize; column++)
                {
                    int block = Tetrominos.GetTetrominoBlock(tetrominoNumber, 1, row, column);

                    if (block == 0)
                    {
                        continue;
                    }

                    ChangeBlockSpriteTextureRect(block);

                    ChangeBlockSpritePosition(
                        column * SpriteBlockSize + +DrawOffset.X + NextTetrominoPreviewPosition.X,
                        row * SpriteBlockSize + +DrawOffset.Y + NextTetrominoPreviewPosition.Y);

                    target.Draw(BlockSprite);
                }
            }
        }

        public virtual void DrawTetrominoLandingShadow(RenderWindow target)
        {
            FindTetrominoLandingPosition(out Point2[] shadowBlocksPosition);

            // Draw shadow
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                var rectangle =
                    new RectangleShape(new Vector2f(SpriteBlockSize, SpriteBlockSize))
                    {
                        FillColor = new Color(0, 0, 0, 150),
                        OutlineThickness = 1,
                        OutlineColor = new Color(255, 255, 255, 50),
                        Position = new Vector2f(
                            shadowBlocksPosition[i].X * SpriteBlockSize + DrawOffset.X + WorldDrawOffset.X,
                            shadowBlocksPosition[i].Y * SpriteBlockSize + DrawOffset.Y + WorldDrawOffset.Y)
                    };

                target.Draw(rectangle);
            }
        }

        public virtual void DrawAllLayers(RenderWindow target)
        {
            if (WorldState == WorldState.GameOver)
            {
                DrawBackground(target, 35);
                DrawWorld(target, false, 35);
                DrawGui(target, 35);
            }
            else
            {
                DrawBackground(target);
                DrawWorld(target);
                DrawGui(target);
            }

            if (WorldState == WorldState.GameOver)
            {
                DrawGameOver(target);
            }
            else if (WorldState == WorldState.NewGame || WorldState == WorldState.Continue)
            {
                DrawStartCounter(target);
            }

            if (Joystick.IsConnected(0))
            {
                DrawGameController(target);
            }
        }

        public virtual int CheckLines(RenderWindow target)
        {
            int removedLines = 0;

            for (int row = 0; row < WorldRows; row++)
            {
                if (LineIsFullOccupied(row))
                {
                    RemoveLine(row);
                    removedLines++;
                }
            }

            if (removedLines > 0)
            {
                RecalculatePositionsOnWorld();
                DrawWorld(target);
            }

            return removedLines;
        }

        public virtual int CalculateScore(int level, int lines, bool landed)
        {
            if (level < 0 || lines < 0 || lines > 4)
            {
                return 0;
            }

            int score = 0;

            if (landed)
            {
                score += 1;
            }

            if (lines == 1)
            {
                score += 40 * (level + 1);
            }
            else if (lines == 2)
            {
                score += 100 * (level + 1);
            }
            else if (lines == 3)
            {
                score += 300 * (level + 1);
            }
            else if (lines == 4)
            {
                score += 1200 * (level + 1);
            }

            return score;
        }

        public int CalculateLevel(int lines)
        {
            // Level up after every 10 lines
            int level = lines / 10;

            // Increase the speed of falling tetromino for each new level (800 milliseconds for 0 level)
            float speed = (48f - (2f * level)) / 60f;
            GameState.TotalFallTickDelay = speed;

            return level;
        }

        public virtual void DrawScore(RenderWindow target, int x, int y, byte alpha = 255)
        {

        }

        public virtual void DrawLevel(RenderWindow target, int x, int y, byte alpha = 255)
        {

        }

        public virtual void DrawLines(RenderWindow target, int x, int y, byte alpha = 255)
        {

        }

        public virtual bool SetTetrominoBlocksPosition(int tetrominoNumber, int tetrominoRotationStateNumber, out int spriteBlockNumber)
        {
            spriteBlockNumber = 0;

            int index = 0;
            for (int row = 0; row < Tetrominos.TetrominoSize; row++)
            {
                for (int column = 0; column < Tetrominos.TetrominoSize; column++)
                {
                    int tetrominoBlock = Tetrominos.GetTetrominoBlock(tetrominoNumber, tetrominoRotationStateNumber, row, column);
                    if (tetrominoBlock != 0)
                    {
                        var blockPosition = new Point2(column, row) + GameState.CurrentTetrominoPosition;

                        GameState.CurrentTetrominoBlocksPosition[index++] = blockPosition;

                        if (spriteBlockNumber == 0)
                        {
                            spriteBlockNumber = tetrominoBlock;
                        }
                    }
                }
            }

            return CheckWorldCollider(GameState.CurrentTetrominoBlocksPosition);
        }

        public virtual bool RotateTetromino()
        {
            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                GameState.PreviousTetrominoBlocksPosition[i] = GameState.CurrentTetrominoBlocksPosition[i];
            }

            // Rotate
            int oldTetrominoRotate = GameState.CurrentTetrominoRotationStateNumber;
            GameState.CurrentTetrominoRotationStateNumber++;
            if (GameState.CurrentTetrominoRotationStateNumber > Tetrominos.TetrominoRotationStates)
            {
                GameState.CurrentTetrominoRotationStateNumber = Tetrominos.TetrominoDefaultRotationState;
            }

            SetTetrominoBlocksPosition(GameState.CurrentTetrominoNumber, GameState.CurrentTetrominoRotationStateNumber, out int spriteBlockNumber);

            if (!CheckWallCollider(GameState.CurrentTetrominoBlocksPosition) || !CheckWorldCollider(GameState.CurrentTetrominoBlocksPosition))
            {
                for (int i = 0; i < Tetrominos.TetrominoSize; i++)
                {
                    GameState.CurrentTetrominoBlocksPosition[i] = GameState.PreviousTetrominoBlocksPosition[i];
                }
                GameState.CurrentTetrominoRotationStateNumber = oldTetrominoRotate;
                return false;
            }

            ChangeBlockSpriteTextureRect(spriteBlockNumber);

            return true;
        }

        public virtual void MoveTetromino(RenderWindow target, int dx, int dy = 0)
        {
            if (WorldState != WorldState.Playing)
            {
                return;
            }

            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                GameState.PreviousTetrominoBlocksPosition[i] = GameState.CurrentTetrominoBlocksPosition[i];
            }

            // Move
            for (int i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                if (dx != 0)
                {
                    GameState.CurrentTetrominoBlocksPosition[i].X += dx;
                }

                if (dy != 0)
                {
                    GameState.CurrentTetrominoBlocksPosition[i].Y += dy;
                }
            }

            if (!CheckWallCollider(GameState.CurrentTetrominoBlocksPosition))
            {
                for (int i = 0; i < Tetrominos.TetrominoSize; i++)
                {
                    GameState.CurrentTetrominoBlocksPosition[i] = GameState.PreviousTetrominoBlocksPosition[i];
                }

                return;
            }

            if (!CheckWorldCollider(GameState.CurrentTetrominoBlocksPosition))
            {
                for (int i = 0; i < Tetrominos.TetrominoSize; i++)
                {
                    GameState.CurrentTetrominoBlocksPosition[i] = GameState.PreviousTetrominoBlocksPosition[i];
                }

                // We can pin it to the play field only when the tetromino moves down
                if (dy > 0)
                {
                    for (int i = 0; i < Tetrominos.TetrominoSize; i++)
                    {
                        PinBlockToWorld(GameState.CurrentTetrominoBlocksPosition[i].Y,
                            GameState.CurrentTetrominoBlocksPosition[i].X, GameState.CurrentTetrominoNumber);
                    }

                    int removedLines = CheckLines(target);

                    if (removedLines > 0)
                    {
                        PlaySound(GameSoundRemoveLine);
                    }
                    else
                    {
                        PlaySound(GameSoundDropTetromino);
                    }

                    _isKeyDownEnabled = false;

                    GameState.Lines += removedLines;

                    int newLevel = CalculateLevel(GameState.Lines);
                    if (newLevel != GameState.Level)
                    {
                        GameState.Level = newLevel;
                        PlaySound(GameSoundLevelUp);
                    } 
                    GameState.Score += CalculateScore(GameState.Level, removedLines, true);

                    CreateNewTetromino();
                }

                return;
            }

            GameState.CurrentTetrominoPosition += new Point2(dx, dy);
        }

        public virtual bool CreateNewTetromino(bool setDefaultStartPosition = true)
        {
            int tetrominoNumber;

            if (GameState.NextTetrominoNumber != 0)
            {
                tetrominoNumber = GameState.NextTetrominoNumber;
                GameState.NextTetrominoNumber = Tetrominos.GetRandomTetrominoNumber(tetrominoNumber);
            }
            else
            {
                tetrominoNumber = Tetrominos.GetRandomTetrominoNumber(GameState.CurrentTetrominoNumber);
                GameState.NextTetrominoNumber = Tetrominos.GetRandomTetrominoNumber(tetrominoNumber);
            }

            bool returnValue = CreateNewTetromino(tetrominoNumber, Tetrominos.TetrominoDefaultRotationState, setDefaultStartPosition);

            // If we can not create a new tetromino, finish the game and display game over
            if (!returnValue)
            {
                PlaySound(GameSoundGameOver);
                WorldState = WorldState.GameOver;
            }

            return returnValue;
        }

        /// <summary>
        /// 
        /// </summary>
        public virtual bool CreateNewTetromino(int tetrominoNumber, int tetrominoRotationStateNumber, bool setDefaultStartPosition = true)
        {
            GameState.CurrentTetrominoNumber = tetrominoNumber;
            GameState.CurrentTetrominoRotationStateNumber = tetrominoRotationStateNumber;

            if (setDefaultStartPosition)
            {
                GameState.CurrentTetrominoPosition = GetTetrominoSpawnPosition();
            }

            bool returnValue = SetTetrominoBlocksPosition(tetrominoNumber, tetrominoRotationStateNumber, out int spriteBlockNumber);

            ChangeBlockSpriteTextureRect(spriteBlockNumber);

            return returnValue;
        }

        public virtual void ClearWorld()
        {
            for (int row = 0; row < WorldRows; row++)
            {
                for (int column = 0; column < WorldColumns; column++)
                {
                    World[row, column] = 0;
                }
            }
        }

        public virtual void KeyPressed(RenderWindow target, object sender, KeyEventArgs e)
        {
            if (e.Code != Keyboard.Key.Down)
            {
                _isKeyDownEnabled = true;
            }

            if (WorldState == WorldState.GameOver)
            {
                if (e.Code == Keyboard.Key.Escape)
                {
                    WorldState = WorldState.Quit;
                }
                else if (GameSettings.IsEnoughPointsForTop5(GameState.Score))
                {
                    if (AllowedKeyboardChars.ContainsKey(e.Code) && _userName.Length < 20)
                    {
                        _userName += AllowedKeyboardChars[e.Code];
                    }
                    else if (e.Code == Keyboard.Key.BackSpace && _userName.Length > 0)
                    {
                        _userName = _userName.Remove(_userName.Length - 1);
                    }
                    else if (e.Code == Keyboard.Key.Return)
                    {
                        GameSettings.AddScore(new ScoreLine(_userName, GameState.Lines, GameState.Level, GameState.Score));
                        WorldState = WorldState.Quit;
                    }
                }
            }
            else if (e.Code == Keyboard.Key.Escape || (e.Code == Keyboard.Key.P && WorldState != WorldState.GameOver))
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }
                WorldState = WorldState.Pause;
            }
            else if ((_readyForRotate && (e.Code == Keyboard.Key.Up || e.Code == Keyboard.Key.W)) ||
                e.Code == Keyboard.Key.Left || e.Code == Keyboard.Key.A || e.Code == Keyboard.Key.Right || e.Code == Keyboard.Key.D ||
                e.Code == Keyboard.Key.Space || ((e.Code == Keyboard.Key.Down || e.Code == Keyboard.Key.S) && _isKeyDownEnabled))
            {
                if (WorldState == WorldState.NewGame || WorldState == WorldState.Continue)
                {
                    WorldState = WorldState.Playing;
                }

                if (_readyForRotate && (e.Code == Keyboard.Key.Up || e.Code == Keyboard.Key.W))
                {
                    _readyForRotate = false;
                    if (RotateTetromino())
                    {
                        PlaySound(GameSoundMoveTetromino);
                    }
                }
                else if (e.Code == Keyboard.Key.Left || e.Code == Keyboard.Key.A)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, -1);
                }
                else if (e.Code == Keyboard.Key.Right || e.Code == Keyboard.Key.D)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 1);
                }
                else if ((e.Code == Keyboard.Key.Down || e.Code == Keyboard.Key.S) && _isKeyDownEnabled)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 0, 1);
                }
                else if (e.Code == Keyboard.Key.Space)
                {
                    FindTetrominoLandingPosition(out Point2[] landingBlocksPosition);
                    MoveTetromino(target, 0, landingBlocksPosition[0].Y - GameState.CurrentTetrominoBlocksPosition[0].Y);
                    MoveTetromino(target, 0, 1);
                }
            }
        }

        public virtual void KeyReleased(RenderWindow target, object sender, KeyEventArgs e)
        {
            if (WorldState != WorldState.Playing)
            {
                return;
            }

            if (e.Code == Keyboard.Key.Up || e.Code == Keyboard.Key.W)
            {
                _readyForRotate = true;
            }

            if ((e.Code == Keyboard.Key.Down || e.Code == Keyboard.Key.S) && !_isKeyDownEnabled)
            {
                _isKeyDownEnabled = true;
            }
        }

        public virtual void JoystickConnected(object sender, JoystickConnectEventArgs arg)
        {
            #if DEBUG
            $"Controller connected: {arg.JoystickId}".Log();
            #endif
        }

        public virtual void JoystickDisconnected(object sender, JoystickConnectEventArgs arg)
        {
            #if DEBUG
            $"Controller disconnected: {arg.JoystickId}".Log();
            #endif
        }

        public virtual void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg)
        {
            #if DEBUG
            $"Controller ({arg.JoystickId}) Button Pressed: {arg.Button})".Log();
            #endif

            if (WorldState == WorldState.NewGame || WorldState == WorldState.Continue)
            {
                WorldState = WorldState.Playing;
            }

            // Press Bumper button
            if (arg.Button == 4 || arg.Button == 5)
            {
                FindTetrominoLandingPosition(out Point2[] landingBlocksPosition);
                MoveTetromino(target, 0, landingBlocksPosition[0].Y - GameState.CurrentTetrominoBlocksPosition[0].Y);
                MoveTetromino(target, 0, 1);
            }
            // Press Menu or B button
            else if (arg.Button == 7 || arg.Button == 1)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                }
                WorldState = WorldState.Pause;
            }
        }

        public virtual void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg)
        {
            #if DEBUG
            $"Controller ({arg.JoystickId}) Button Released: {arg.Button})".Log();
            #endif
        }

        public virtual void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg)
        {
            #if DEBUG
            $"Controller ({arg.JoystickId}) Moved: Axis({arg.Axis}), Position({arg.Position})".Log();
            #endif

            if (arg.Axis == Joystick.Axis.PovX || arg.Axis == Joystick.Axis.PovY)
            {
                if (WorldState == WorldState.NewGame || WorldState == WorldState.Continue)
                {
                    WorldState = WorldState.Playing;
                }

                if (arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position + 100) < GamepadMinimumInputThreshold)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, -1);
                }
                else if (arg.Axis == Joystick.Axis.PovX && Math.Abs(arg.Position - 100) < GamepadMinimumInputThreshold)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 1);
                }
                else if (arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position + 100) < GamepadMinimumInputThreshold)
                {
                    PlaySound(GameSoundMoveTetromino);
                    MoveTetromino(target, 0, 1);
                }
                else if (_readyForRotate && arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position - 100) < GamepadMinimumInputThreshold)
                {
                    _readyForRotate = false;
                    if (RotateTetromino())
                    {
                        PlaySound(GameSoundMoveTetromino);
                    }
                }
                else if (arg.Axis == Joystick.Axis.PovY && Math.Abs(arg.Position) < GamepadMinimumInputThreshold)
                {
                    _readyForRotate = true;
                }
            }
        }

        public virtual void DrawGameController(RenderWindow target)
        {
            target.Draw(GameController);
        }

        /// <summary>
        /// Find the position of tetromino landing
        /// </summary>
        private void FindTetrominoLandingPosition(out Point2[] landingBlocksPosition)
        {
            landingBlocksPosition = new Point2[GameState.CurrentTetrominoBlocksPosition.Length];

            GameState.CurrentTetrominoBlocksPosition.CopyTo(landingBlocksPosition, 0);

            while (CheckWorldCollider(landingBlocksPosition))
            {
                for (var i = 0; i < Tetrominos.TetrominoSize; i++)
                {
                    landingBlocksPosition[i].Y++;
                }
            }

            // Put tetromino over the landing position
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                landingBlocksPosition[i].Y--;
            }
        }

        private void NewGame()
        {
            _userName = string.Empty;
            ClearWorld();
            CreateNewTetromino();
            GameState.Score = 0;
            GameState.Lines = 0;
            GameState.Level = 0;
        }

        private bool LineIsFullOccupied(int lineNumber)
        {
            // All columns must be occupied by blocks
            for (int column = 0; column < WorldColumns; column++)
            {
                if (World[lineNumber, column] == 0)
                {
                    return false;
                }
            }

            return true;
        }

        private bool LineInEmpty(int lineNumber)
        {
            // All columns must be empty from blocks
            for (int column = 0; column < WorldColumns; column++)
            {
                if (World[lineNumber, column] != 0)
                {
                    return false;
                }
            }

            return true;
        }

        private void RecalculatePositionsOnWorld()
        {
            int[,] newPlayField = new int[WorldRows, WorldColumns];

            int newPlayFieldRow = WorldRows;
            for (int row = WorldRows - 1; row >= 0; row--)
            {
                if (!LineInEmpty(row))
                {
                    newPlayFieldRow--;

                    for (int column = 0; column < WorldColumns; column++)
                    {
                        newPlayField[newPlayFieldRow, column] = World[row, column];
                    }
                }
            }

            // Copy newPlayField to PlayField
            for (int row = 0; row < WorldRows; row++)
            {
                for (int column = 0; column < WorldColumns; column++)
                {
                    World[row, column] = newPlayField[row, column];
                }
            }
        }

        private void RemoveLine(int lineNumber)
        {
            for (int column = 0; column < WorldColumns; column++)
            {
                World[lineNumber, column] = 0;
            }
        }

        private void InitializeTimer()
        {
            _tickTimer = 4;
            _timer = new Timer(Tick, DateTime.Now, 0, 500);
        }

        private void Tick(object state)
        {
            "Counter Tick".Log();

            if (--_tickTimer == 0)
            {
                WorldState = WorldState.Playing;
                _timer.Dispose();
            }
        }
    }
}
