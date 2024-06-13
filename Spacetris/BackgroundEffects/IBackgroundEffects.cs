using SFML.Graphics;

namespace Spacetris.BackgroundEffects;

public interface IBackgroundEffects : Drawable
{
    public void Update(float deltaTime);
}