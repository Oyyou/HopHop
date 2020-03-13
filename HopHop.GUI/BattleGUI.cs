using Engine.Models;
using HopHop.GUI.Controls;
using HopHop.Lib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HopHop.Lib.Enums;

namespace HopHop.GUI
{
  public class BattleGUI
  {
    private Button _endTurnButton;

    public BattleStates State;

    public BattleStates NextState;

    public BattleGUI(GameModel gameModel)
    {
      var content = gameModel.Content;

      var etTexture = content.Load<Texture2D>("GUI/Battle/EndTurn");
      var etPosition = new Vector2((BaseGame.ScreenWidth - etTexture.Width) - 20, (BaseGame.ScreenHeight - etTexture.Height) - 20);

      _endTurnButton = new Button(etTexture)
      {
        Position = etPosition,
      };
    }

    public void Update(GameTime gameTime)
    {
      _endTurnButton.Update(gameTime);

      if (_endTurnButton.Clicked)
      {
        NextState = BattleStates.EnemyTurn;
      }
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      _endTurnButton.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
