using SFML.Graphics;
using SFML.Window;
using Spacetris.DataStructures;
using System;

namespace Spacetris.GameStates.Worlds
{
    public interface IWorld
    {
        event EventHandler<WorldState> WorldStateChanged;

        WorldState WorldState { get; set; }

        int WorldColumns { get; }
        int WorldRows { get; }
        int SpriteBlockSize { get; }

        Point2 DrawOffset { get; }

        Point2 WorldDrawOffset { get; }

        Point2 NextTetrominoPreviewPosition { get; }

        Sprite BlockSprite { get; }

        Sprite BackgroundSprite { get; }

        Font Font { get; }

        Font CounterFont { get; }

        void Initialize(RenderWindow target);

        void Update(RenderWindow target, float deltaTime);

        int[,] World { get; }

        void ChangeBlockSpriteTextureRect(int spriteBlockNumber);

        void ChangeBlockSpritePosition(int x, int y);

        Point2 GetTetrominoSpawnPosition();

        void PinBlockToWorld(int x, int y, int tetrominoIndex);

        bool CheckWallCollider(Point2[] tetrominoBlocksPosition);

        bool CheckWorldCollider(Point2[] tetrominoBlocksPosition);

        void DrawBackground(RenderWindow target, byte alpha = 255);

        void DrawWorld(RenderWindow target, bool drawLandingShadow = true, byte alpha = 255);

        void DrawTetromino(RenderWindow target, Point2[] tetrominoBlocksPosition);

        void DrawNextTetrominoPreview(RenderWindow target);

        void DrawTetrominoLandingShadow(RenderWindow target, Point2[] tetrominoBlocksPosition);

        void DrawAllLayers(RenderWindow target);

        void DrawGui(RenderWindow target, byte alpha = 255);

        void DrawStartCounter(RenderWindow target);

        void DrawGameOver(RenderWindow target);

        int CheckLines(RenderWindow target);

        int CalculateScore(int level, int lines, bool landed);

        int CalculateLevel(int lines);

        void DrawScore(RenderWindow target, int x, int y, byte alpha = 255);

        void DrawLevel(RenderWindow target, int x, int y, byte alpha = 255);

        void DrawLines(RenderWindow target, int x, int y, byte alpha = 255);

        void UpdateDrawOffset(RenderWindow target, int offsetX = 0, int offsetY = 0);

        bool SetTetrominoBlocksPosition(int tetrominoNumber, int tetrominoRotationStateNumber, out int spriteBlockNumber);

        void RotateTetromino();

        void MoveTetromino(RenderWindow target, int dx, int dy = 0);

        bool CreateNewTetromino(bool setDefaultStartPosition = true);

        bool CreateNewTetromino(int tetrominoNumber, int tetrominoRotationStateNumber, bool setDefaultStartPosition = true);

        void ClearWorld();

        void KeyPressed(RenderWindow target, object sender, KeyEventArgs e);

        void KeyReleased(RenderWindow target, object sender, KeyEventArgs e);

        void JoystickConnected(object sender, JoystickConnectEventArgs arg);

        void JoystickDisconnected(object sender, JoystickConnectEventArgs arg);

        void JoystickButtonPressed(RenderWindow target, object sender, JoystickButtonEventArgs arg);

        void JoystickButtonReleased(RenderWindow target, object sender, JoystickButtonEventArgs arg);

        void JoystickMoved(RenderWindow target, object sender, JoystickMoveEventArgs arg);
    }
}
