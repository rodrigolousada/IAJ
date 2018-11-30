using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.DecisionMakingActions
{
    public class LayOnHands : IAJ.Unity.DecisionMaking.GOB.Action
    {
        private int manaChange;
        public int ManaCost { get; set; }
        public int LevelRequired { get; set; }
        public AutonomousCharacter Character { get; set; }

        public LayOnHands(AutonomousCharacter character) : base("LayOnHands")
        {
            this.Character = character;

            //TODO: implement
            this.manaChange = -7;
            this.ManaCost = 7;
            this.LevelRequired = 2;
        }

        public override float GetGoalChange(Goal goal)
        {
            //TODO: implement
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -(this.Character.GameManager.characterData.MaxHP - this.Character.GameManager.characterData.HP);
            }

            return change;
        }

        public override bool CanExecute()
        {
            //TODO: implement
            if (!base.CanExecute())
                return false;

            return (this.Character.GameManager.characterData.Mana >= this.ManaCost) 
                && (this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP)
                && (this.Character.GameManager.characterData.Level >= this.LevelRequired);
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //TODO: implement
            if (!base.CanExecute(worldModel))
                return false;

            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxhp = (int)worldModel.GetProperty(Properties.MAXHP);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var level = (int)worldModel.GetProperty(Properties.LEVEL);
            return (mana >= this.ManaCost) && (hp < maxhp) && (level >= this.LevelRequired);
        }

        public override void Execute()
        {
            //TODO: implement
            base.Execute();
            this.Character.GameManager.LayOnHands();
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            //TODO: implement
            base.ApplyActionEffects(worldModel);

            var maxhp = (int)worldModel.GetProperty(Properties.MAXHP);
            var hp = (int)worldModel.GetProperty(Properties.HP);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, goalValue + -(maxhp-hp));

            worldModel.SetProperty(Properties.HP, maxhp);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);
        }

    }
}
