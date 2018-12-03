using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;
using System.Collections.Generic;

namespace Assets.Scripts.DecisionMakingActions
{
    public class DivineWrath : IAJ.Unity.DecisionMaking.GOB.Action
    {
        private int manaChange;
        private int xpChange;
        public int ManaCost { get; set; }
        public int LevelRequired { get; set; }
        public AutonomousCharacter Character { get; set; }
        protected GameObject[] Targets { get; set; }

        public DivineWrath(AutonomousCharacter character, GameObject[] targets) : base("DivineWrath")
        {
            this.Character = character;
            this.Targets = targets;

            //TODO: implement
            this.xpChange = 0;
            this.manaChange = -10;
            this.ManaCost = 10;
            this.LevelRequired = 3;

            foreach (var target in this.Targets) {
                if (target.tag.Equals("Skeleton"))
                {
                    this.xpChange += 3;
                }
                else if (target.tag.Equals("Orc"))
                {
                    this.xpChange += 10;
                }
                else if (target.tag.Equals("Dragon"))
                {
                    this.xpChange += 20;
                }
            }
        }

        public override float GetGoalChange(Goal goal)
        {
            //TODO: implement
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
            {
                change += -this.xpChange;
            }

            return change;
        }

        public override bool CanExecute()
        {
            //TODO: implement
            if (!base.CanExecute())
                return false;

            return (this.Character.GameManager.characterData.Mana >= this.ManaCost)
                && (this.Character.GameManager.characterData.Level >= this.LevelRequired)
                && (this.Targets != null);
        }

        public override bool CanExecute(WorldModel worldModel)
        {
            //TODO: implement
            if (!base.CanExecute(worldModel))
                return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            var level = (int)worldModel.GetProperty(Properties.LEVEL);
            return (mana >= this.ManaCost) && (level >= this.LevelRequired) && (this.Targets != null);
        }

        public override void Execute()
        {
            //TODO: implement
            base.Execute();
            this.Character.GameManager.DivineWrath();
        }


        public override void ApplyActionEffects(WorldModel worldModel)
        {
            //TODO: implement
            base.ApplyActionEffects(worldModel);

            var goalValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, goalValue + this.xpChange);

            var maxhp = worldModel.GetProperty(Properties.MAXHP);
            worldModel.SetProperty(Properties.HP, maxhp);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);
            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);

            //disables the target object so that it can't be reused again
            foreach (var target in this.Targets) {
                if(target!=null)
                    worldModel.SetProperty(target.name, false);
            }
        }

        public override float GetHValue(WorldModel worldModel)
        {
            //you would be dumb not to use if possible
            return 100.0f;
        }

    }
}
