﻿using Engine;
using Engine.Models;
using HopHop.GUI.Controls;
using HopHop.Lib;
using HopHop.Lib.Models;
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
    private List<UnitModel> _units;

    private Button _endTurnButton;

    private Sprite _controlPanel;

    private List<Button> _heroIcons;

    private List<Button> _abilityIcons;

    public BattleStates State;

    public BattleStates NextState;

    private Texture2D _abilityIconTexture;
    private Texture2D _abilityIconClickedTexture;

    public int SelectedHeroIndex = 0;
    private int _selectedAbilityIndex = -1;

    private UnitModel _previousUnit;
    private UnitModel _currentUnit;

    public int SelectedUnitId { get; set; }

    public BattleGUI(GameModel gameModel, List<UnitModel> units)
    {
      _units = units;
      SelectedUnitId = _units.First().Id;

      var content = gameModel.Content;

      var etTexture = content.Load<Texture2D>("GUI/Battle/EndTurn");
      var etPosition = new Vector2((BaseGame.ScreenWidth - etTexture.Width) - 10, (BaseGame.ScreenHeight - etTexture.Height) - 10);

      _endTurnButton = new Button(etTexture)
      {
        Position = etPosition,
      };

      var cpTexture = content.Load<Texture2D>("GUI/Battle/ControlPanel");
      _controlPanel = new Sprite(cpTexture)
      {
        Position = new Vector2(10, BaseGame.ScreenHeight - cpTexture.Height - 10),
      };

      var heroIconTexture = content.Load<Texture2D>("GUI/Battle/HeroIcon");
      var heroIconClickedTexture = content.Load<Texture2D>("GUI/Battle/HeroIcon_Clicked");
      _abilityIconTexture = content.Load<Texture2D>("GUI/Battle/AbilityIcon");
      _abilityIconClickedTexture = content.Load<Texture2D>("GUI/Battle/AbilityIcon_Clicked");

      _heroIcons = new List<Button>();
      _abilityIcons = new List<Button>();

      var x = _controlPanel.Position.X + 4;
      var y = _controlPanel.Position.Y + 20;
      foreach (var unit in _units)
      {
        _heroIcons.Add(new Button(heroIconTexture, heroIconClickedTexture) { Position = new Vector2(x, y), });
        x += heroIconTexture.Width + 2;
      }
    }

    private void UpdateUnit(UnitModel newUnit)
    {
      _previousUnit = _currentUnit;
      _currentUnit = newUnit;

      if (_previousUnit == _currentUnit)
        return;

      _selectedAbilityIndex = -1;

      _abilityIcons = new List<Button>();

      var x = _controlPanel.Position.X + 300;
      var y = _controlPanel.Position.Y + 36;
      foreach (var ability in _currentUnit.Abilities.Get())
      {
        _abilityIcons.Add(new Button(_abilityIconTexture, _abilityIconClickedTexture) { Position = new Vector2(x, y), });
        x += _abilityIconTexture.Width + 2;
      }
    }

    public void Update(GameTime gameTime)
    {
      UpdateUnit(_units[SelectedHeroIndex]);

      _endTurnButton.Update(gameTime);

      if (_endTurnButton.Clicked)
        NextState = BattleStates.EnemyTurn;

      UpdateHeroIcons(gameTime);

      UpdateAbilityIcons(gameTime);
    }

    private void UpdateHeroIcons(GameTime gameTime)
    {
      for (int i = 0; i < _heroIcons.Count; i++)
      {
        _heroIcons[i].Update(gameTime);
        _heroIcons[i].IsSelected = false;

        if (_heroIcons[i].IsClicked)
          SelectedHeroIndex = i;
      }

      _heroIcons[SelectedHeroIndex].IsSelected = true;
    }

    private void UpdateAbilityIcons(GameTime gameTime)
    {
      for (int i = 0; i < _abilityIcons.Count; i++)
      {
        _abilityIcons[i].Update(gameTime);
        _abilityIcons[i].IsSelected = false;

        if (_abilityIcons[i].IsClicked)
          _selectedAbilityIndex = i;
      }

      if (_selectedAbilityIndex > -1)
        _abilityIcons[_selectedAbilityIndex].IsSelected = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      _controlPanel.Draw(gameTime, spriteBatch);
      _endTurnButton.Draw(gameTime, spriteBatch);

      foreach (var icon in _heroIcons)
        icon.Draw(gameTime, spriteBatch);

      foreach (var icon in _abilityIcons)
        icon.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
