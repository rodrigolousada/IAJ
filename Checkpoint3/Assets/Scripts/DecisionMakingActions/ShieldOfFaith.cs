using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;

namespace Assets.Scripts.DecisionMakingActions
{
    public class ShieldOfFaith : IAJ.Unity.DecisionMaking.GOB.Action
    {
        public AutonomousCharacter Character { get; set; }

		public ShieldOfFaith(AutonomousCharacter character) : base("ShieldOfFaith")
		{
            this.Character = character;

			//TODO: implement
			throw new NotImplementedException();
		}

		public override float GetGoalChange(Goal goal)
		{
			//TODO: implement
			throw new NotImplementedException();
		}

		public override bool CanExecute()
		{
			//TODO: implement
			throw new NotImplementedException();
		}

		public override bool CanExecute(WorldModel worldModel)
		{
			//TODO: implement
			throw new NotImplementedException();
		}

		public override void Execute()
		{
			//TODO: implement
			throw new NotImplementedException();
		}


		public override void ApplyActionEffects(WorldModel worldModel)
		{
			//TODO: implement
			throw new NotImplementedException();
		}

    }
}
