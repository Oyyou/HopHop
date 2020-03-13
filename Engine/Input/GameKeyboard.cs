using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Input
{
  public class GameKeyboard
  {
    private KeyboardState _previousState;
    private KeyboardState _currentState;

    public void Update()
    {
      _previousState = _currentState;
      _currentState = Keyboard.GetState();
    }

    public bool IsKeyDown(Keys key)
    {
      return _currentState.IsKeyDown(key);
    }

    public bool IsKeyPressed(Keys key)
    {
      return _previousState.IsKeyDown(key) &&
        _currentState.IsKeyUp(key);
    }
  }
}
