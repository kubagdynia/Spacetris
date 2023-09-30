namespace Spacetris;

public static class Tetrominos
{
    public const int TetrominoSize = 4;

    public const int TetrominoRotationStates = 4;

    public const int TetrominoDefaultRotationState = 1;

    private const int NumberOfTetrominos = 7;

    public static int GetTetrominoBlock(int tetrominoNumber, int tetrominoRotationStateNumber, int row, int column)
    {
        int tetrominoBlock = TetrominosList[tetrominoNumber - 1, tetrominoRotationStateNumber - 1, row, column];
        return tetrominoBlock;
    }

    public static int GetRandomTetrominoNumber(int numberToExclude)
    {
        Random random = new Random(DateTime.Now.Millisecond);

        int resultIndex = random.Next() % NumberOfTetrominos + 1;

        if (resultIndex == numberToExclude)
        {
            resultIndex++;

            if (resultIndex > NumberOfTetrominos)
            {
                resultIndex = 1;
            }
        }

        return resultIndex;
    }

    private static readonly int[,,,] TetrominosList =
        {
        // I-Block, light blue
        {
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 },
                { 0, 0, 1, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 1, 1, 1, 1 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 },
                { 0, 1, 0, 0 }
            }
        },
        // J-Block, blue
        {
            {
                { 0, 0, 0, 0 },
                { 2, 0, 0, 0 },
                { 2, 2, 2, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 2, 2, 0 },
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 2, 2, 2, 0 },
                { 0, 0, 2, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 2, 0, 0 },
                { 0, 2, 0, 0 },
                { 2, 2, 0, 0 }
            }
        },
        // L-Block, orange
        {
            {
                { 0, 0, 0, 0 },
                { 0, 0, 3, 0 },
                { 3, 3, 3, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 3, 0, 0 },
                { 0, 3, 0, 0 },
                { 0, 3, 3, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 3, 3, 3, 0 },
                { 3, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 3, 3, 0, 0 },
                { 0, 3, 0, 0 },
                { 0, 3, 0, 0 }
            }
        },
        // O-Block, yellow
        {
            {
                { 0, 0, 0, 0 },
                { 0, 4, 4, 0 },
                { 0, 4, 4, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 4, 4, 0 },
                { 0, 4, 4, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 4, 4, 0 },
                { 0, 4, 4, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 4, 4, 0 },
                { 0, 4, 4, 0 },
                { 0, 0, 0, 0 }
            }
        },
        // S-Block, green
        {
            {
                { 0, 0, 0, 0 },
                { 0, 5, 5, 0 },
                { 5, 5, 0, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 5, 0, 0 },
                { 0, 5, 5, 0 },
                { 0, 0, 5, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 0, 5, 5, 0 },
                { 5, 5, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 5, 0, 0, 0 },
                { 5, 5, 0, 0 },
                { 0, 5, 0, 0 }
            }
        },
        // Z-Block, red
        {
            {
                { 0, 0, 0, 0 },
                { 6, 6, 0, 0 },
                { 0, 6, 6, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 6, 0 },
                { 0, 6, 6, 0 },
                { 0, 6, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 6, 6, 0, 0 },
                { 0, 6, 6, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 6, 0, 0 },
                { 6, 6, 0, 0 },
                { 6, 0, 0, 0 }
            }
        },
        // T-Block, purple
        {
            {
                { 0, 0, 0, 0 },
                { 0, 7, 0, 0 },
                { 7, 7, 7, 0 },
                { 0, 0, 0, 0 }
            },
            {
                { 0, 0, 0, 0 },
                { 0, 7, 0, 0 },
                { 0, 7, 7, 0 },
                { 0, 7, 0, 0 }
            },{
                { 0, 0, 0, 0 },
                { 0, 0, 0, 0 },
                { 7, 7, 7, 0 },
                { 0, 7, 0, 0 }
            },{
                { 0, 0, 0, 0 },
                { 0, 7, 0, 0 },
                { 7, 7, 0, 0 },
                { 0, 7, 0, 0 }
            }
        }
    };
}