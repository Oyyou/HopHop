using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopHop.GUI.Controls.Battle
{
  public class ButtonGroup
  {
    private List<Button> _buttons = new List<Button>();

    public int PreviousIndex { get; set; } = -1;

    public int CurrentIndex { get; set; } = -1;

    public int HoverIndex { get; set; } = -1;

    public Action OnButtonChange { get; set; } = null;

    public Rectangle Rectangle { get; private set; }

    public ButtonGroup()
    {

    }

    public void SetButtons(Func<List<Button>> getButtons)
    {
      _buttons = getButtons();

      Rectangle = new Rectangle();

      if (_buttons.Count == 0)
        return;

      var left = _buttons.OrderBy(c => c.Rectangle.Left).FirstOrDefault().Rectangle.Left;
      var top = _buttons.OrderBy(c => c.Rectangle.Top).FirstOrDefault().Rectangle.Top;
      var right = _buttons.OrderBy(c => c.Rectangle.Right).FirstOrDefault().Rectangle.Right;
      var bottom = _buttons.OrderBy(c => c.Rectangle.Bottom).FirstOrDefault().Rectangle.Bottom;

      Rectangle = new Rectangle(left, top, right - left, bottom - top);
    }

    public void Update(GameTime gameTime)
    {
      if (_buttons.Count == 0)
        return;

      HoverIndex = -1;

      for (int i = 0; i < _buttons.Count; i++)
      {
        _buttons[i].Update(gameTime);
        _buttons[i].IsSelected = false;

        if (_buttons[i].IsHovering)
          HoverIndex = i;

        if (_buttons[i].IsClicked)
        {
          CurrentIndex = i;
          OnButtonChange?.Invoke();
        }
      }

      if (CurrentIndex > -1)
        _buttons[CurrentIndex].IsSelected = true;

      PreviousIndex = CurrentIndex;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var icon in _buttons)
        icon.Draw(gameTime, spriteBatch);
    }

    public void Increment()
    {
      CurrentIndex++;

      if (CurrentIndex >= _buttons.Count)
        CurrentIndex = 0;

      for (int i = 0; i < _buttons.Count; i++)
      {
        if (_buttons[CurrentIndex].IsEnabled)
          break;

        CurrentIndex++;

        if (CurrentIndex >= _buttons.Count)
          CurrentIndex = 0;
      }
    }

    public void Decrement()
    {
      CurrentIndex--;

      if (CurrentIndex < 0)
        CurrentIndex = _buttons.Count - 1;

      for (int i = _buttons.Count - 1; i > -1; i--)
      {
        if (_buttons[CurrentIndex].IsEnabled)
          break;

        CurrentIndex--;

        if (CurrentIndex < 0)
          CurrentIndex = _buttons.Count - 1;
      }
    }
  }
}
