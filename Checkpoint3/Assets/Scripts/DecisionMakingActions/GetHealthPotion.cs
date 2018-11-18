using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using UnityEngine;

namespace Assets.Scripts.DecisionMakingActions
{
    public class GetHealthPotion : WalkToTargetAndExecuteAction
    {
        public GetHealthPotion(AutonomousCharacter character, GameObject target) : base("GetHealthPotion",character,target)
        {
        }

		public override bool CanExecute()
		{
            //TODO: implement
            if(!base.CanExecute())
                return false;

            return this.Character.GameManager.characterData.HP < this.Character.GameManager.characterData.MaxHP;
        }

		public override bool CanExecute(WorldModel worldModel)
		{
            //TODO: implement
            if(!base.CanExecute(worldModel))
                return false;
        
            var hp = (int)worldModel.GetProperty(Properties.HP);
            var maxhp = (int)worldModel.GetProperty(Properties.MAXHP);
            return hp < maxhp;
        }

		public override void Execute()
		{
            //TODO: implement
            base.Execute();
            this.Character.GameManager.GetHealthPotion(this.Target);
        }

		public override void ApplyActionEffects(WorldModel worldModel)
		{
            //TODO: implement
            base.ApplyActionEffects(worldModel);
            var maxhp = worldModel.GetProperty(Properties.MAXHP);
            worldModel.SetProperty(Properties.HP, maxhp);
            //disables the target object so that it can't be reused again
            worldModel.SetProperty(this.Target.name, false);
        }
    }
}
