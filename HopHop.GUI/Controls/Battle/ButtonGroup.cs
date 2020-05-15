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
    public List<Button> Buttons = new List<Button>();

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
      Buttons = getButtons();

      Rectangle = new Rectangle();

      if (Buttons.Count == 0)
        return;

      var left = Buttons.OrderBy(c => c.Rectangle.Left).FirstOrDefault().Rectangle.Left;
      var top = Buttons.OrderBy(c => c.Rectangle.Top).FirstOrDefault().Rectangle.Top;
      var right = Buttons.OrderBy(c => c.Rectangle.Right).FirstOrDefault().Rectangle.Right;
      var bottom = Buttons.OrderBy(c => c.Rectangle.Bottom).FirstOrDefault().Rectangle.Bottom;

      Rectangle = new Rectangle(left, top, right - left, bottom - top);
    }

    public void Update(GameTime gameTime)
    {
      if (Buttons.Count == 0)
        return;

      HoverIndex = -1;

      for (int i = 0; i < Buttons.Count; i++)
      {
        Buttons[i].Update(gameTime);
        Buttons[i].IsSelected = false;

        if (Buttons[i].IsHovering)
          HoverIndex = i;

        if (Buttons[i].IsClicked && Buttons[i].IsEnabled)
        {
          CurrentIndex = i;
          OnButtonChange?.Invoke();
        }
      }

      if (CurrentIndex > -1)
        Buttons[CurrentIndex].IsSelected = true;

      PreviousIndex = CurrentIndex;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      foreach (var icon in Buttons)
        icon.Draw(gameTime, spriteBatch);
    }

    public void Increment()
    {
      CurrentIndex++;

      if (CurrentIndex >= Buttons.Count)
        CurrentIndex = 0;

      for (int i = 0; i < Buttons.Count; i++)
      {
        if (Buttons[CurrentIndex].IsEnabled)
        {
          OnButtonChange();
          break;
        }

        CurrentIndex++;

        if (CurrentIndex >= Buttons.Count)
          CurrentIndex = 0;
      }
    }

    public void Decrement()
    {
      CurrentIndex--;

      if (CurrentIndex < 0)
        CurrentIndex = Buttons.Count - 1;

      for (int i = Buttons.Count - 1; i > -1; i--)
      {
        if (Buttons[CurrentIndex].IsEnabled)
        {
          OnButtonChange();
          break;
        }

        CurrentIndex--;

        if (CurrentIndex < 0)
          CurrentIndex = Buttons.Count - 1;
      }
    }
  }
}
