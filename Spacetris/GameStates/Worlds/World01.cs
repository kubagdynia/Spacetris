using SFML.Graphics;
using Spacetris.BackgroundEffects;
using Spacetris.DataStructures;
using Spacetris.Managers;
using Spacetris.Settings;

namespace Spacetris.GameStates.Worlds;

public class World01 : BaseWorld
{
    private Starfield _starfield;

    public override int WorldColumns => 10;
    public override int WorldRows => 20;

    public override int SpriteBlockSize => 20;   // Size in pixels

    public override Point2 WorldDrawOffset => new(20, 20);

    public override Point2 NextTetrominoPreviewPosition => new(229, 20);

    protected override string BackgroundTexturePath => Path.Combine(GameSettings.BackgroundPath, "background.png");

    protected override string BlocksTexturePath => Path.Combine(GameSettings.TilesetsPath, "tilesets.png");

    protected override AssetManagerItemName FontName => AssetManagerItemName.ArialFont;

    protected override AssetManagerItemName CounterFontName => AssetManagerItemName.TetrisFont;

    public override void DrawScore(RenderWindow target, int x, int y, byte alpha = 255)
    {
        DrawInfo(target, GameState.Score.ToString(), x + DrawOffset.X, y + DrawOffset.Y, alpha);
    }

    public override void DrawLevel(RenderWindow target, int x, int y, byte alpha = 255)
    {
        DrawInfo(target, GameState.Level.ToString(), x + DrawOffset.X, y + DrawOffset.Y, alpha);
    }

    public override void DrawLines(RenderWindow target, int x, int y, byte alpha = 255)
    {
        DrawInfo(target, GameState.Lines.ToString(), x + DrawOffset.X, y + DrawOffset.Y, alpha);
    }

    public override void Initialize(RenderWindow target)
    {
        base.Initialize(target);

        _starfield = new Starfield(target.Size.X, target.Size.Y);
    }

    public override void Update(RenderWindow target, float deltaTime)
    {
        base.Update(target, deltaTime);

        _starfield.UpdateStarfield(deltaTime);
    }

    public override void DrawBackground(RenderWindow target, byte alpha = 255)
    {
        target.Draw(_starfield);

        base.DrawBackground(target, alpha);
    }

    private void DrawInfo(RenderWindow target, string value, int x, int y, byte alpha = 189)
    {
        DrawText(target, Font, value, x, y, new Color(242, 51, 51, alpha), 20, true, true);
    }

    protected override void LoadContent()
    {
        GameSoundMoveTetromino = LoadSound("move.ogg");
        GameSoundDropTetromino = LoadSound("drop.ogg");
        GameSoundRemoveLine = LoadSound("removeline.ogg");
        GameSoundGameOver = LoadSound("gameover.ogg");
        GameSoundLevelUp = LoadSound("levelup.ogg");

        // Load sprites
        GameControllerSprite = LoadGameControllerSprite();
    }
}