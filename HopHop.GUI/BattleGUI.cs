using Engine;
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

    private List<Button> _unitIcons;

    private List<Button> _abilityIcons;

    private List<Button> _targetIcons;

    public BattleStates State;

    public BattleStates NextState;

    private Dictionary<int, List<Texture2D>> _abilityIconTextures = new Dictionary<int, List<Texture2D>>();
    private Dictionary<string, Texture2D> _targetIconTextures = new Dictionary<string, Texture2D>();
    private Texture2D _abilityIconClickedTexture;

    public int SelectedUnitIndex = 0;
    public int SelectedAbilityIndex = -1;
    public int SelectedTargetIndex = -1;

    private UnitModel _previousUnit;
    private UnitModel _currentUnit;

    public int SelectedUnitId { get; set; }

    public Action OnUnitChanged { get; set; }

    public Action OnAbilityChanged { get; set; }

    public Action OnTargetChanged { get; set; }

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

      var unitIconTexture = content.Load<Texture2D>("GUI/Battle/HeroIcon");
      var unitIconClickedTexture = content.Load<Texture2D>("GUI/Battle/HeroIcon_Clicked");

      foreach (var unit in units)
      {
        foreach (var ability in unit.Abilities.Get())
        {
          if (!_abilityIconTextures.ContainsKey(unit.Id))
            _abilityIconTextures.Add(unit.Id, new List<Texture2D>());

          _abilityIconTextures[unit.Id].Add(content.Load<Texture2D>("GUI/Battle/AbilityIcons/" + ability.IconName));
        }
      }

      _targetIconTextures.Add("Enemy", content.Load<Texture2D>("GUI/Battle/TargetIcons/Enemy"));

      _abilityIconClickedTexture = content.Load<Texture2D>("GUI/Battle/AbilityIcon_Clicked");

      _unitIcons = new List<Button>();
      _abilityIcons = new List<Button>();

      var x = _controlPanel.Position.X + 4;
      var y = _controlPanel.Position.Y + 20;
      foreach (var unit in _units)
      {
        _unitIcons.Add(new Button(unitIconTexture, unitIconClickedTexture) { Position = new Vector2(x, y), });
        x += unitIconTexture.Width + 2;
      }
    }

    private void UpdateUnit(UnitModel newUnit)
    {
      _previousUnit = _currentUnit;
      _currentUnit = newUnit;

      if (_previousUnit == _currentUnit)
        return;

      SelectedAbilityIndex = -1;

      _abilityIcons = new List<Button>();

      var x = _controlPanel.Position.X + 300;
      var y = _controlPanel.Position.Y + 36;
      var abilities = _currentUnit.Abilities.Get().ToList();
      for (int i = 0; i < 4; i++)
      {
        var ability = abilities[i];

        var texture = _abilityIconTextures[newUnit.Id][i];

        if (ability != null)
          _abilityIcons.Add(new Button(texture, _abilityIconClickedTexture) { Position = new Vector2(x, y), });

        x += texture.Width + 2;
      }

      _targetIcons = new List<Button>();
    }

    private List<UnitModel> _previousTargets = new List<UnitModel>();

    public void Update(GameTime gameTime, List<UnitModel> targets)
    {
      if (_previousTargets.Count != targets.Count || !_previousTargets.All(c => targets.Contains(c)))
      {
        _previousTargets = targets;
        _targetIcons = new List<Button>();

        var x = _abilityIcons.First().Position.X;
        var y = _controlPanel.Position.Y - 55;

        for (int i = 0; i < targets.Count; i++)
        {
          var target = targets[i];

          var texture = _targetIconTextures["Enemy"];

          _targetIcons.Add(new Button(texture, _abilityIconClickedTexture) { Position = new Vector2(x, y), HoverColour = Color.Red });

          x += texture.Width + 2;
        }
      }

      UpdateUnit(_units[SelectedUnitIndex]);

      _endTurnButton.Update(gameTime);

      if (_endTurnButton.Clicked)
        NextState = BattleStates.EnemyTurn;

      UpdateUnitIcons(gameTime);

      UpdateAbilityIcons(gameTime);

      UpdateTargetIcons(gameTime);
    }

    private void UpdateUnitIcons(GameTime gameTime)
    {
      for (int i = 0; i < _unitIcons.Count; i++)
      {
        _unitIcons[i].Update(gameTime);
        _unitIcons[i].IsSelected = false;

        if (_unitIcons[i].IsClicked)
        {
          SelectedUnitIndex = i;
          OnUnitChanged?.Invoke();
        }
      }

      _unitIcons[SelectedUnitIndex].IsSelected = true;
    }

    private void UpdateAbilityIcons(GameTime gameTime)
    {
      for (int i = 0; i < _abilityIcons.Count; i++)
      {
        _abilityIcons[i].Update(gameTime);
        _abilityIcons[i].IsSelected = false;

        if (_abilityIcons[i].IsClicked)
        {
          SelectedAbilityIndex = i;
          OnAbilityChanged?.Invoke();
        }
      }

      if (SelectedAbilityIndex > -1)
        _abilityIcons[SelectedAbilityIndex].IsSelected = true;
    }

    private void UpdateTargetIcons(GameTime gameTime)
    {
      for (int i = 0; i < _targetIcons.Count; i++)
      {
        _targetIcons[i].Update(gameTime);
        _targetIcons[i].IsSelected = false;

        if (_targetIcons[i].IsClicked)
        {
          SelectedTargetIndex = i;
          OnTargetChanged?.Invoke();
        }
      }

      if (SelectedTargetIndex > -1)
        _targetIcons[SelectedTargetIndex].IsSelected = true;
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      _controlPanel.Draw(gameTime, spriteBatch);
      _endTurnButton.Draw(gameTime, spriteBatch);

      foreach (var icon in _unitIcons)
        icon.Draw(gameTime, spriteBatch);

      foreach (var icon in _abilityIcons)
        icon.Draw(gameTime, spriteBatch);

      foreach (var icon in _targetIcons)
        icon.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
