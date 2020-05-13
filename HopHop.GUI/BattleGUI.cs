using Engine;
using Engine.Models;
using HopHop.GUI.Controls;
using HopHop.GUI.Controls.Battle;
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
    private GameModel _gameModel;

    private List<UnitModel> _units;

    private Button _endTurnButton;

    private Sprite _controlPanel;

    public ButtonGroup UnitsGroup { get; private set; } = new ButtonGroup();

    public ButtonGroup AbilitiesGroup { get; private set; } = new ButtonGroup();

    public ButtonGroup TargetsGroup { get; private set; } = new ButtonGroup();

    public BattleStates State;

    public BattleStates NextState;

    private Dictionary<int, List<Texture2D>> _abilityIconTextures = new Dictionary<int, List<Texture2D>>();
    private Dictionary<string, Texture2D> _targetIconTextures = new Dictionary<string, Texture2D>();
    private Texture2D _abilityIconClickedTexture;

    private List<UnitModel> _previousTargets = new List<UnitModel>();
    private List<UnitModel> _targets = new List<UnitModel>();

    private AbilityModel _previousAbility = null;
    private AbilityModel _currentAbility = null;

    public int SelectedUnitIndex
    {
      get
      {
        return UnitsGroup.CurrentIndex;
      }
      set
      {
        UnitsGroup.CurrentIndex = value;
      }
    }

    public int SelectedAbilityIndex
    {
      get
      {
        return AbilitiesGroup.CurrentIndex;
      }
      set
      {
        AbilitiesGroup.CurrentIndex = value;
      }
    }

    public int SelectedTargetIndex
    {
      get
      {
        return TargetsGroup.CurrentIndex;
      }
      set
      {
        TargetsGroup.CurrentIndex = value;
      }
    }

    public int HoveringTargetIndex
    {
      get
      {
        return TargetsGroup.HoverIndex;
      }
      set
      {
        TargetsGroup.HoverIndex = value;
      }
    }

    private UnitModel _previousUnit;
    private UnitModel _currentUnit;

    public int SelectedUnitId { get; set; }

    public BattleGUI(GameModel gameModel, List<UnitModel> units)
    {
      _gameModel = gameModel;
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
      _targetIconTextures.Add("Friendly", content.Load<Texture2D>("GUI/Battle/TargetIcons/Friendly"));

      _abilityIconClickedTexture = content.Load<Texture2D>("GUI/Battle/AbilityIcon_Clicked");


      UnitsGroup = new ButtonGroup()
      {
        CurrentIndex = 0,
      };

      UnitsGroup.SetButtons(GetUnitButtons);
    }

    private List<Button> GetUnitButtons()
    {
      var unitIconTexture = _gameModel.Content.Load<Texture2D>("GUI/Battle/HeroIcon");
      var unitIconClickedTexture = _gameModel.Content.Load<Texture2D>("GUI/Battle/HeroIcon_Clicked");

      var buttons = new List<Button>();

      var x = _controlPanel.Position.X + 4;
      var y = _controlPanel.Position.Y + 20;
      foreach (var unit in _units)
      {
        buttons.Add(new Button(unitIconTexture, unitIconClickedTexture) { Position = new Vector2(x, y), });
        x += unitIconTexture.Width + 2;
      }

      return buttons;
    }

    private List<Button> GetAbilityButtons()
    {
      var buttons = new List<Button>();

      var newUnit = _units[UnitsGroup.CurrentIndex];

      var x = _controlPanel.Position.X + 300;
      var y = _controlPanel.Position.Y + 36;
      var abilities = _currentUnit.Abilities.Get().ToList();
      for (int i = 0; i < 4; i++)
      {
        var ability = abilities[i];

        var texture = _abilityIconTextures[newUnit.Id][i];

        if (ability != null)
          buttons.Add(new Button(texture, _abilityIconClickedTexture) { Position = new Vector2(x, y), });

        x += texture.Width + 2;
      }

      return buttons;
    }

    private List<Button> GetTargetButtons()
    {
      var buttons = new List<Button>();

      var x = AbilitiesGroup.Rectangle.X;
      var y = _controlPanel.Position.Y - 55;

      for (int i = 0; i < _targets.Count; i++)
      {
        var target = _targets[i];

        Texture2D texture = null;

        switch (target.UnitType)
        {
          case UnitModel.UnitTypes.Friendly:
            texture = _targetIconTextures["Friendly"];
            break;

          case UnitModel.UnitTypes.Enemy:
            texture = _targetIconTextures["Enemy"];
            break;

          default:
            throw new Exception("Unknow unit Type: " + target.UnitType);
        }

        buttons.Add(new Button(texture, _abilityIconClickedTexture) { Position = new Vector2(x, y), HoverColour = Color.Red });

        x += texture.Width + 2;
      }

      return buttons;
    }

    /// <summary>
    /// Check if the selected unit has changed
    /// </summary>
    private void CheckIfUnitChanged()
    {
      if (_previousUnit == _currentUnit)
        return;

      SelectedAbilityIndex = -1;
      SelectedTargetIndex = -1;

      _targets = new List<UnitModel>();

      AbilitiesGroup.SetButtons(GetAbilityButtons);
      TargetsGroup.SetButtons(GetTargetButtons);
    }

    private void CheckIfTargetChanged()
    {
      if (!(_previousTargets.Count != _targets.Count || !_previousTargets.All(c => _targets.Contains(c))))
        return;

      //if (_targets.Count == 0)
      //  return;

      TargetsGroup.SetButtons(GetTargetButtons);
    }

    private void CheckIfAbilityChanged()
    {
      if (_previousAbility == _currentAbility)
        return;

      TargetsGroup.SetButtons(GetTargetButtons);
    }

    public void Update(GameTime gameTime, List<UnitModel> targets)
    {
      _previousTargets = _targets;
      _targets = targets;

      _previousUnit = _currentUnit;
      _currentUnit = _units[UnitsGroup.CurrentIndex];

      _previousAbility = _currentAbility;
      _currentAbility = _currentUnit.Abilities.Get(AbilitiesGroup.CurrentIndex);

      CheckIfUnitChanged();

      CheckIfTargetChanged();

      //CheckIfAbilityChanged();

      _endTurnButton.Update(gameTime);

      if (_endTurnButton.Clicked)
        NextState = BattleStates.EnemyTurn;

      UnitsGroup.Update(gameTime);

      AbilitiesGroup.Update(gameTime);

      TargetsGroup.Update(gameTime);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
      spriteBatch.Begin();

      _controlPanel.Draw(gameTime, spriteBatch);
      _endTurnButton.Draw(gameTime, spriteBatch);

      UnitsGroup.Draw(gameTime, spriteBatch);

      AbilitiesGroup.Draw(gameTime, spriteBatch);

      TargetsGroup.Draw(gameTime, spriteBatch);

      spriteBatch.End();
    }
  }
}
