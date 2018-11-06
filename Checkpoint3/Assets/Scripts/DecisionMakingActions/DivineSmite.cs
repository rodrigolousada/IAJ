using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using System;

namespace Assets.Scripts.DecisionMakingActions
{
    public class DivineSmite : WalkToTargetAndExecuteAction
    {
        private int xpChange;

		public DivineSmite(AutonomousCharacter character, GameObject target) : base("DivineSmite",character,target)
		{
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
