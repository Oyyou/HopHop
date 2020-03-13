using Engine.Input;
using Engine.Models;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.Lib
{
  public abstract class BaseGame : Game
  {
    protected GraphicsDeviceManager _graphics;

    protected SpriteBatch _spriteBatch;

    public GameModel GameModel { get; protected set; }

    public static Random Random;

    public static GameMouse GameMouse;

    public static GameKeyboard GameKeyboard;

    public static int ScreenWidth;

    public static int ScreenHeight;
  }
}
