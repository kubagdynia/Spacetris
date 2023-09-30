using SFML.Audio;
using SFML.Graphics;

namespace Spacetris.Managers;

public class AssetManager
{
    private static AssetManager _instance;
    private static readonly object Sync = new();

    #region SINGLETON

    private AssetManager() { }

    public static AssetManager Instance
    {
        get
        {
            if (_instance == null)
            {
                lock (Sync)
                {
                    if (_instance == null)
                    {
                        var instance = new AssetManager();
                        Thread.MemoryBarrier();
                        _instance = instance;
                    }
                }
            }
            return _instance;
        }
    }

    #endregion

    private Manager<Texture> _texture;

    public Manager<Texture> Texture => _texture ??= new Manager<Texture>();

    private Manager<Font> _font;

    public Manager<Font> Font => _font ??= new Manager<Font>();

    private Manager<Music> _music;

    public Manager<Music> Music => _music ??= new Manager<Music>();

    private Manager<SoundBuffer> _sound;

    public Manager<SoundBuffer> Sound => _sound ??= new Manager<SoundBuffer>();
}