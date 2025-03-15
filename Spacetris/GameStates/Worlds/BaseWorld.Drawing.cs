using SFML.Graphics;
using SFML.System;
using SFML.Window;
using Spacetris.DataStructures;
using Spacetris.Settings;

namespace Spacetris.GameStates.Worlds;

public abstract partial class BaseWorld : BaseGameState, IWorld
{
    public virtual void DrawBackground(RenderWindow target, byte alpha = 255)
    {
        BackgroundSprite.Color = new Color(255, 255, 255, alpha);
        target.Draw(BackgroundSprite);
    }

    public virtual void DrawWorld(RenderWindow target, bool drawLandingShadow = true, byte alpha = 255)
    {
        BlockSprite.Color = new Color(255, 255, 255, alpha);

        for (var row = 0; row < WorldRows; row++)
        {
            for (var column = 0; column < WorldColumns; column++)
            {
                var blockNumber = World[row, column];
                if (blockNumber == 0) continue;
                
                ChangeBlockSpritePosition(
                    column * SpriteBlockSize + DrawOffset.X + WorldDrawOffset.X,
                    row * SpriteBlockSize + DrawOffset.Y + WorldDrawOffset.Y);

                ChangeBlockSpriteTextureRect(blockNumber);

                target.Draw(BlockSprite);
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

        var scoreColor = new Color(255, 255, 255, 189);

        // Top 5 scores
        if (GameSettings.IsEnoughPointsForTopScores(GameState.Score))
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
        for (var i = 0; i < Tetrominos.TetrominoSize; i++)
        {
            ChangeBlockSpritePosition(
                tetrominoBlocksPosition[i].X * SpriteBlockSize + DrawOffset.X + WorldDrawOffset.X,
                tetrominoBlocksPosition[i].Y * SpriteBlockSize + DrawOffset.Y + WorldDrawOffset.Y);

            target.Draw(BlockSprite);
        }
    }

    public virtual void DrawNextTetrominoPreview(RenderWindow target)
    {
        var tetrominoNumber = GameState.NextTetrominoNumber;

        for (var row = 0; row < Tetrominos.TetrominoSize; row++)
        {
            for (var column = 0; column < Tetrominos.TetrominoSize; column++)
            {
                var block = Tetrominos.GetTetrominoBlock(tetrominoNumber, 1, row, column);

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

        switch (WorldState)
        {
            case WorldState.GameOver:
                DrawGameOver(target);
                break;
            case WorldState.NewGame or WorldState.Continue:
                DrawStartCounter(target);
                break;
        }

        if (Joystick.IsConnected(0))
        {
            DrawGameController(target);
        }
    }

    public virtual void DrawGameController(RenderWindow target)
    {
        target.Draw(GameControllerSprite);
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
}
