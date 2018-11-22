using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.DecisionMakingActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private int hpChange;
        private int xpChange;
        private int manaChange;

        public int ManaCost { get; set; }

        public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
		{
            //TODO: implement
            this.ManaCost = 2;
            this.Target = target;
            if (target.tag.Equals("Skeleton"))
            {
                this.hpChange = -2;
                this.xpChange = 3;
                this.manaChange = -2;
            }

        }

		public override float GetGoalChange(Goal goal)
		{
            //TODO: implement
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += -this.hpChange;
            }
            else if (goal.Name == AutonomousCharacter.GAIN_XP_GOAL)
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

            return this.Character.GameManager.characterData.Mana >= this.ManaCost;
        }

		public override bool CanExecute(WorldModel worldModel)
		{
            //TODO: implement
            if (!base.CanExecute(worldModel))
                return false;

            var mana = (int)worldModel.GetProperty(Properties.MANA);
            return mana >= this.ManaCost;
        }

		public override void Execute()
		{
            //TODO: implement
            base.Execute();
            this.Character.GameManager.DivineSmite(this.Target);
        }


		public override void ApplyActionEffects(WorldModel worldModel)
		{
            //TODO: implement
            base.ApplyActionEffects(worldModel);

            var xpValue = worldModel.GetGoalValue(AutonomousCharacter.GAIN_XP_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.GAIN_XP_GOAL, xpValue - this.xpChange);

            var surviveValue = worldModel.GetGoalValue(AutonomousCharacter.SURVIVE_GOAL);
            worldModel.SetGoalValue(AutonomousCharacter.SURVIVE_GOAL, surviveValue - this.hpChange);

            var hp = (int)worldModel.GetProperty(Properties.HP);
            var shieldhp = (int)worldModel.GetProperty(Properties.SHIELDHP);
            var shield_value = shieldhp + this.hpChange;
            if (shield_value < 0)
            {
                worldModel.SetProperty(Properties.HP, hp + shield_value);
                worldModel.SetProperty(Properties.SHIELDHP, 0);
            }
            else
            {
                worldModel.SetProperty(Properties.HP, shield_value);
            }
            var xp = (int)worldModel.GetProperty(Properties.XP);
            worldModel.SetProperty(Properties.XP, xp + this.xpChange);
            var mana = (int)worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);


            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }

    }
}
