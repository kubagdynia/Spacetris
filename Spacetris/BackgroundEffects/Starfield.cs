using SFML.Graphics;
using SFML.System;
using Spacetris.DataStructures;

namespace Spacetris.BackgroundEffects;

public class Starfield : Transformable, Drawable
{
    private readonly Image _smallStarImage;
    private readonly Image _mediumStarImage;
    private readonly Image _largeStarImage;

    private readonly uint _maxSmallStars;
    private readonly uint _maxMediumStars;
    private readonly uint _maxLargeStars;
    
    private readonly List<Point2> _smallStars = new();
    private readonly List<Point2> _mediumStars = new();
    private readonly List<Point2> _largeStars = new();

    private readonly Random _randomX;

    private readonly Image _image;
    private Texture _texture;
    private Sprite _sprite;

    private readonly uint _width;
    private readonly uint _height;

    private readonly float _totalStarsMoveDelay = 0.02f;  // 20 ms
    private float _totalStarsMoveTimer;

    public Starfield(uint width, uint height)
    {
        _width = width;
        _height = height;

        _image = new Image(width, height, Color.Black);
        _texture = new Texture(_image);
        _sprite = new Sprite(_texture);

        uint smallSize = 1;
        uint mediumSize = 2;
        uint largeSize = 4;

        _smallStarImage = new Image(smallSize, smallSize, new Color(153, 153, 153));
        _mediumStarImage = new Image(mediumSize, mediumSize, new Color(204, 204, 204));
        _largeStarImage = new Image(largeSize, largeSize, Color.White);

        _randomX = new Random(new Time().AsMilliseconds());
        var randomY = new Random(new Time().AsMilliseconds() + 100);

        uint reduceStars = 8;
        uint classDifference = 3;

        _maxSmallStars = width / (reduceStars * 10) * (height / reduceStars);
        _maxMediumStars = width / (reduceStars * 10 * classDifference) * (height / (reduceStars * classDifference));
        _maxLargeStars = width / (reduceStars * 10 * classDifference * classDifference) * (height / (reduceStars * classDifference * classDifference));

        while (_smallStars.Count <= _maxSmallStars)
        {
            _smallStars.Add(new Point2(_randomX.Next() % width + 1, randomY.Next() % height + 1));
        }

        while (_mediumStars.Count <= _maxMediumStars)
        {
            _mediumStars.Add(new Point2(_randomX.Next() % width + 1, randomY.Next() % height + 1));
        }

        while (_largeStars.Count <= _maxLargeStars)
        {
            _largeStars.Add(new Point2(_randomX.Next() % width + 1, randomY.Next() % height + 1));
        }
    }
    
    public void UpdateStarfield(float deltaTime)
        {
            _totalStarsMoveTimer += deltaTime;

            // Tick
            if (_totalStarsMoveTimer <= _totalStarsMoveDelay)
            {
                return;
            }

            _totalStarsMoveTimer = 0;

            // Move the stars down and remove them if they cross the bottom line
            for (int i = 0; i < _smallStars.Count; i++)
            {
                if (_smallStars[i].Y > _height)
                {
                    _smallStars.RemoveAt(i);
                    continue;
                }

                _smallStars[i] += new Point2(0, 1);
            }

            for (int i = 0; i < _mediumStars.Count; i++)
            {
                if (_mediumStars[i].Y > _height)
                {
                    _mediumStars.RemoveAt(i);
                    continue;
                }

                _mediumStars[i] += new Point2(0, 2);
            }

            for (int i = 0; i < _largeStars.Count; i++)
            {
                if (_largeStars[i].Y > _height)
                {
                    _largeStars.RemoveAt(i);
                    continue;
                }

                _largeStars[i] += new Point2(0, 3);
            }

            // Adding more stars if their numbers falls bellow the maximum number
            while (_smallStars.Count <= _maxSmallStars)
            {
                _smallStars.Add(new Point2(_randomX.Next() % _width + 1, 0));
            }

            while (_mediumStars.Count <= _maxMediumStars)
            {
                _mediumStars.Add(new Point2(_randomX.Next() % _width + 1, 0));
            }

            while (_largeStars.Count <= _maxLargeStars)
            {
                _largeStars.Add(new Point2(_randomX.Next() % _width + 1, 0));
            }
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            _texture = new Texture(_image);

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

            _sprite = new Sprite(_texture);

            target.Draw(_sprite);
        }
}