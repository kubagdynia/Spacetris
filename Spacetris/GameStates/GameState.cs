using Spacetris.DataStructures;

namespace Spacetris.GameStates;

public static class GameState
{
    public static int CurrentTetrominoNumber { get; set; }

    public static int NextTetrominoNumber { get; set; }

    public static int CurrentTetrominoRotationStateNumber { get; set; }

    public static Point2 CurrentTetrominoPosition { get; set; } = Point2.Zero;

    public static float TotalFallTickDelay = 0.8f; // https://en.wikipedia.org/wiki/Tetris

    public static int Score = 0;

    public static int Lines = 0;

    public static int Level = 0;

    public static readonly Point2[] CurrentTetrominoBlocksPosition = new Point2[Tetrominos.TetrominoSize];

    public static readonly Point2[] PreviousTetrominoBlocksPosition = new Point2[Tetrominos.TetrominoSize];
}