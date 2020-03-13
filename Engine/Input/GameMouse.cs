using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input
{
  public class GameMouse
  {
    private MouseState _previousState;
    private MouseState _currentState;
    private Matrix _matrix;

    /// <summary>
    /// These are objects the mouse is currently hovering over
    /// </summary>
    public List<IClickable> ClickableObjects;


    /// <summary>
    /// The single object we're able to click
    /// </summary>
    public IClickable ValidObject
    {
      get
      {
        return ClickableObjects.OrderBy(c => c.Layer).LastOrDefault();
      }
    }

    public Vector2 Position
    {
      get
      {
        return _currentState.Position.ToVector2();
      }
    }

    public Vector2 Position_WithCamera
    {
      get
      {
        if (_matrix == null)
          return Position;

        return Position - (new Vector2(_matrix.Translation.X, _matrix.Translation.Y));
      }
    }

    public Rectangle Rectangle
    {
      get
      {
        return new Rectangle((int)Position.X, (int)Position.Y, 1, 1);
      }
    }

    public Rectangle Rectangle_WithCamera
    {
      get
      {
        return new Rectangle((int)Position_WithCamera.X, (int)Position_WithCamera.Y, 1, 1);
      }
    }

    public bool HasLeftClicked
    {
      get
      {
        return _previousState.LeftButton == ButtonState.Pressed &&
          _currentState.LeftButton == ButtonState.Released;
      }
    }

    public bool HasRightClicked
    {
      get
      {
        return _previousState.RightButton == ButtonState.Pressed &&
          _currentState.RightButton == ButtonState.Released;
      }
    }

    public GameMouse()
    {
      ClickableObjects = new List<IClickable>();
    }

    public void AddCamera(Matrix matrix)
    {
      _matrix = matrix;
    }

    public void AddObject(IClickable clickableObject)
    {
      if (!ClickableObjects.Contains(clickableObject))
        ClickableObjects.Add(clickableObject);
    }

    public void Update()
    {
      _previousState = _currentState;
      _currentState = Mouse.GetState();
    }

    public bool Intersects(Rectangle rectangle)
    {
      return this.Rectangle.Intersects(rectangle);
    }

    public bool Intersects_withCamera(Rectangle rectangle)
    {
      return this.Rectangle_WithCamera.Intersects(rectangle);
    }
  }
}
