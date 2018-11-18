using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;
using Assets.Scripts.GameManager;

namespace Assets.Scripts.DecisionMakingActions
{
    public class ShieldOfFaith : IAJ.Unity.DecisionMaking.GOB.Action
    {
        private int manaChange;
        public int ShieldOfFaithHP { get; set; }
        public int ManaCost { get; set; }
        public AutonomousCharacter Character { get; set; }

        public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
		{
            this.Character = character;

            //TODO: implement
            this.manaChange = -5;
            this.ManaCost = 5;
            this.ShieldOfFaithHP = 5;
        }

		public override float GetGoalChange(Goal goal)
		{
            //TODO: implement
            var change = base.GetGoalChange(goal);

            if (goal.Name == AutonomousCharacter.SURVIVE_GOAL)
            {
                change += this.ShieldOfFaithHP - this.Character.GameManager.characterData.ShieldHP;
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
            this.Character.GameManager.ShieldOfFaith();
        }


		public override void ApplyActionEffects(WorldModel worldModel)
		{
            //TODO: implement
            base.ApplyActionEffects(worldModel);
            worldModel.SetProperty(Properties.SHIELDHP, this.ShieldOfFaithHP);
            var mana = (int) worldModel.GetProperty(Properties.MANA);
            worldModel.SetProperty(Properties.MANA, mana + this.manaChange);
        }

    }
}
