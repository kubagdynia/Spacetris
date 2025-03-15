using SFML.Audio;
using SFML.Graphics;
using Spacetris.DataStructures;
using Spacetris.Managers;

namespace Spacetris.GameStates.Worlds;

public abstract partial class BaseWorld
{
    private string _userName;
    private bool _isKeyDownEnabled = true;

    private Timer _timer;
    private int _tickTimer;

    protected Sprite GameControllerSprite;

    protected Sound GameSoundMoveTetromino;
    protected Sound GameSoundDropTetromino;
    protected Sound GameSoundRemoveLine;
    protected Sound GameSoundGameOver;
    protected Sound GameSoundLevelUp;

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

            switch (value)
            {
                case WorldState.NewGame:
                    NewGame();
                    InitializeTimer();
                    break;
                case WorldState.Continue:
                    InitializeTimer();
                    break;
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

    protected abstract AssetManagerItemName FontName { get; }

    protected abstract AssetManagerItemName CounterFontName { get; }

    private bool _readyForRotate = true;

    private Sprite _blockSprite;
    public Sprite BlockSprite => _blockSprite ??= LoadSprite(BlocksTexturePath, Point2.Zero);

    private Sprite _backgroundSprite;
    public Sprite BackgroundSprite => _backgroundSprite ??= LoadSprite(BackgroundTexturePath, DrawOffset);

    public Font Font => AssetManager.Instance.Font.Get(FontName);

    public Font CounterFont => AssetManager.Instance.Font.Get(CounterFontName);

    private int[,] _world;

    public event EventHandler<WorldState> WorldStateChanged;

    public int[,] World => _world ??= new int[WorldRows, WorldColumns];
}
