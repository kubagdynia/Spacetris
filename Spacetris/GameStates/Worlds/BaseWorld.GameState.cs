using SFML.Graphics;
using SFML.System;
using Spacetris.DataStructures;
using Spacetris.Extensions;

namespace Spacetris.GameStates.Worlds;

public abstract partial class BaseWorld
{
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
            // ReSharper disable once PossibleLossOfFraction
            (target.Size.X - BackgroundSprite.Texture.Size.X) / 2 + offsetX,
            // ReSharper disable once PossibleLossOfFraction
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
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
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
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
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

    public virtual int CheckLines(RenderWindow target)
    {
        var removedLines = 0;

        for (var row = 0; row < WorldRows; row++)
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

        var score = 0;

        if (landed)
        {
            score += 1;
        }

        switch (lines)
        {
            case 1:
                score += 40 * (level + 1);
                break;
            case 2:
                score += 100 * (level + 1);
                break;
            case 3:
                score += 300 * (level + 1);
                break;
            case 4:
                score += 1200 * (level + 1);
                break;
        }

        return score;
    }

    public int CalculateLevel(int lines)
    {
        // Level up after every 10 lines
        var level = lines / 10;

        // Increase the speed of falling tetromino for each new level (800 milliseconds for 0 level)
        var speed = (48f - 2f * level) / 60f;
        GameState.TotalFallTickDelay = speed;

        return level;
    }

    public virtual bool SetTetrominoBlocksPosition(int tetrominoNumber, int tetrominoRotationStateNumber, out int spriteBlockNumber)
    {
        spriteBlockNumber = 0;

        var index = 0;
        for (var row = 0; row < Tetrominos.TetrominoSize; row++)
        {
            for (var column = 0; column < Tetrominos.TetrominoSize; column++)
            {
                var tetrominoBlock = Tetrominos.GetTetrominoBlock(tetrominoNumber, tetrominoRotationStateNumber, row, column);
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
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
        {
            GameState.PreviousTetrominoBlocksPosition[i] = GameState.CurrentTetrominoBlocksPosition[i];
        }

        // Rotate
        var oldTetrominoRotate = GameState.CurrentTetrominoRotationStateNumber;
        GameState.CurrentTetrominoRotationStateNumber++;
        if (GameState.CurrentTetrominoRotationStateNumber > Tetrominos.TetrominoRotationStates)
        {
            GameState.CurrentTetrominoRotationStateNumber = Tetrominos.TetrominoDefaultRotationState;
        }

        SetTetrominoBlocksPosition(GameState.CurrentTetrominoNumber, GameState.CurrentTetrominoRotationStateNumber, out var spriteBlockNumber);

        if (!CheckWallCollider(GameState.CurrentTetrominoBlocksPosition) || !CheckWorldCollider(GameState.CurrentTetrominoBlocksPosition))
        {
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
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

        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
        {
            GameState.PreviousTetrominoBlocksPosition[i] = GameState.CurrentTetrominoBlocksPosition[i];
        }

        // Move
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
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
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                GameState.CurrentTetrominoBlocksPosition[i] = GameState.PreviousTetrominoBlocksPosition[i];
            }

            return;
        }

        if (!CheckWorldCollider(GameState.CurrentTetrominoBlocksPosition))
        {
            for (var i = 0; i < Tetrominos.TetrominoSize; i++)
            {
                GameState.CurrentTetrominoBlocksPosition[i] = GameState.PreviousTetrominoBlocksPosition[i];
            }

            // We can pin it to the play field only when the tetromino moves down
            if (dy > 0)
            {
                for (var i = 0; i < Tetrominos.TetrominoSize; i++)
                {
                    PinBlockToWorld(GameState.CurrentTetrominoBlocksPosition[i].Y,
                        GameState.CurrentTetrominoBlocksPosition[i].X, GameState.CurrentTetrominoNumber);
                }

                var removedLines = CheckLines(target);

                PlaySound(removedLines > 0 ? GameSoundRemoveLine : GameSoundDropTetromino);

                _isKeyDownEnabled = false;

                GameState.Lines += removedLines;

                var newLevel = CalculateLevel(GameState.Lines);
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

        var returnValue =
            CreateNewTetromino(tetrominoNumber, Tetrominos.TetrominoDefaultRotationState, setDefaultStartPosition);

        // If we can not create a new tetromino, finish the game and display game over
        if (!returnValue)
        {
            PlaySound(GameSoundGameOver);
            WorldState = WorldState.GameOver;
        }

        return returnValue;
    }

    public virtual bool CreateNewTetromino(int tetrominoNumber, int tetrominoRotationStateNumber, bool setDefaultStartPosition = true)
    {
        GameState.CurrentTetrominoNumber = tetrominoNumber;
        GameState.CurrentTetrominoRotationStateNumber = tetrominoRotationStateNumber;

        if (setDefaultStartPosition)
        {
            GameState.CurrentTetrominoPosition = GetTetrominoSpawnPosition();
        }

        var returnValue =
            SetTetrominoBlocksPosition(tetrominoNumber, tetrominoRotationStateNumber, out var spriteBlockNumber);

        ChangeBlockSpriteTextureRect(spriteBlockNumber);

        return returnValue;
    }

    public virtual void ClearWorld()
    {
        for (var row = 0; row < WorldRows; row++)
        {
            for (var column = 0; column < WorldColumns; column++)
            {
                World[row, column] = 0;
            }
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
        for (var column = 0; column < WorldColumns; column++)
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
        for (var column = 0; column < WorldColumns; column++)
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
        var newPlayField = new int[WorldRows, WorldColumns];

        var newPlayFieldRow = WorldRows;
        for (var row = WorldRows - 1; row >= 0; row--)
        {
            if (!LineInEmpty(row))
            {
                newPlayFieldRow--;

                for (var column = 0; column < WorldColumns; column++)
                {
                    newPlayField[newPlayFieldRow, column] = World[row, column];
                }
            }
        }

        // Copy newPlayField to PlayField
        for (var row = 0; row < WorldRows; row++)
        {
            for (var column = 0; column < WorldColumns; column++)
            {
                World[row, column] = newPlayField[row, column];
            }
        }
    }

    private void RemoveLine(int lineNumber)
    {
        for (var column = 0; column < WorldColumns; column++)
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
#if DEBUG
        "Counter Tick".Log();
#endif
        if (--_tickTimer == 0)
        {
            WorldState = WorldState.Playing;
            _timer.Dispose();
        }
    }
}