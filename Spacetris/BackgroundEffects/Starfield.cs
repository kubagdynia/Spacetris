using SFML.Graphics;
using SFML.System;
using Spacetris.DataStructures;

namespace Spacetris.BackgroundEffects;

public class Starfield : Transformable, IBackgroundEffects
{
    private readonly Image _smallStarImage;
    private readonly Image _mediumStarImage;
    private readonly Image _largeStarImage;

    private readonly int _maxSmallStars;
    private readonly int _maxMediumStars;
    private readonly int _maxLargeStars;
    
    private readonly List<Point2> _smallStars = new();
    private readonly List<Point2> _mediumStars = new();
    private readonly List<Point2> _largeStars = new();

    private readonly Random _randomX;
    private readonly Random _randomY;

    private readonly Texture _texture;
    private readonly Sprite _sprite;

    private readonly uint _width;
    private readonly uint _height;

    private const float TotalStarsMoveDelay = 0.02f; // 20 ms
    private float _totalStarsMoveTimer;

    public Starfield(uint width, uint height)
    {
        _width = width;
        _height = height;
        
        _texture = new Texture(width, height);
        _sprite = new Sprite(_texture);

        const uint smallSize = 1;
        const uint mediumSize = 2;
        const uint largeSize = 4;

        _smallStarImage = new Image(smallSize, smallSize, new Color(153, 153, 153));
        _mediumStarImage = new Image(mediumSize, mediumSize, new Color(204, 204, 204));
        _largeStarImage = new Image(largeSize, largeSize, Color.White);

        _randomX = new Random(new Time().AsMilliseconds());
        _randomY = new Random(new Time().AsMilliseconds() + 100);

        const uint reduceStars = 8;
        const uint classDifference = 3;

        _maxSmallStars = (int)(width / (reduceStars * 10) * (height / reduceStars));
        _maxMediumStars = (int)(width / (reduceStars * 10 * classDifference) * (height / (reduceStars * classDifference)));
        _maxLargeStars = (int)(width / (reduceStars * 10 * classDifference * classDifference) * (height / (reduceStars * classDifference * classDifference)));

        InitializeStars(_smallStars, _maxSmallStars);
        InitializeStars(_mediumStars, _maxMediumStars);
        InitializeStars(_largeStars, _maxLargeStars);
    }

    private void InitializeStars(List<Point2> stars, int maxStars)
    {
        while (stars.Count < maxStars)
        {
            stars.Add(new Point2(_randomX.Next((int)_width), _randomY.Next((int)_height)));
        }
    }
    
    public void Update(float deltaTime)
    {
        _totalStarsMoveTimer += deltaTime;

        // Tick
        if (_totalStarsMoveTimer <= TotalStarsMoveDelay)
        {
            return;
        }

        _totalStarsMoveTimer = 0;

        // Move the stars down and remove them if they cross the bottom line
        UpdateStars(_smallStars, 1);
        UpdateStars(_mediumStars, 2);
        UpdateStars(_largeStars, 3);

        // Adding more stars if their numbers falls bellow the maximum number
        ReplenishStars(_smallStars, _maxSmallStars);
        ReplenishStars(_mediumStars, _maxMediumStars);
        ReplenishStars(_largeStars, _maxLargeStars);
    }
    
    private void UpdateStars(List<Point2> stars, int speed)
    {
        for (int i = stars.Count - 1; i >= 0; i--)
        {
            stars[i] += new Point2(0, speed);
            if (stars[i].Y > _height)
            {
                stars.RemoveAt(i);
            }
        }
    }
    
    private void ReplenishStars(List<Point2> stars, int maxStars)
    {
        while (stars.Count < maxStars)
        {
            stars.Add(new Point2(_randomX.Next((int)_width), 0));
        }
    }

    public void Draw(RenderTarget target, RenderStates states)
    {
        _texture.Update(new Image(_width, _height, Color.Black));

        foreach (var star in _smallStars)
        {
            _texture.Update(_smallStarImage, (uint)star.X, (uint)star.Y);
        }

        foreach (var star in _mediumStars)
        {
            _texture.Update(_mediumStarImage, (uint)star.X, (uint)star.Y);
        }

        foreach (var star in _largeStars)
        {
            _texture.Update(_largeStarImage, (uint)star.X, (uint)star.Y);
        }

        target.Draw(_sprite);
    }
}