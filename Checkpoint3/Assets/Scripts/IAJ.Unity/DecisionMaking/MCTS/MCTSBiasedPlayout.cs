using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using UnityEngine;
using Assets.Scripts.DecisionMakingActions;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSBiasedPlayout : MCTS
    {
        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }

        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            //TODO: implement

            //while s is nonterminal do
            //        chose a from Actions(s)uniformly at random
            //s <- Result(s, a)
            //return reward for state s

            WorldModel state = initialPlayoutState.GenerateChildWorldModel();
            var actions = state.GetExecutableActions();
            Reward reward = new Reward();

            if (actions.Length == 0) {
                reward.Value = 0;
                reward.PlayerID = state.GetNextPlayer();
                return reward;
            }

            while (!state.IsTerminal()) {
                List<float> heuristicValues = new List<float>();
                float heuristic_total = 0;

                foreach (var action in actions) {
                    var h = CalcHeuristic(state, action);
                    heuristic_total += h;
                    heuristicValues.Add(heuristic_total);
                }
                var random = UnityEngine.Random.Range(0, heuristic_total);
                for (int i = 0; i < heuristicValues.Count; i++) {
                    if (random <= heuristicValues[i]) {
                        var action = actions[i];
                        action.ApplyActionEffects(state);
                        state.CalculateNextPlayer();
                        state = state.GenerateChildWorldModel();
                        break;
                    }
                }
            }
            reward.Value = state.GetScore();
            reward.PlayerID = state.GetNextPlayer();
            return reward;
        }

        protected float CalcHeuristic(WorldModel parentState, GOB.Action action) //MCTSNode
        {
            //TODO: implement
            WalkToTargetAndExecuteAction walk_action = action as WalkToTargetAndExecuteAction;
            float h = 10;

            if (action.Name.StartsWith("LevelUp") && action.CanExecute(parentState)) {
                h += 10000;
            }
            if (action.Name.StartsWith("DivineWrath") && action.CanExecute(parentState)) {
                h += 10000;
            }

            float distance;
            if (walk_action != null) {
                distance = action.GetDuration(parentState);
                h += Mathf.Abs(100/(distance+1));

                //if (action.Name.Contains("Chest") && action.CanExecute()) {
                //    h += 20;
                //}
                if (action.Name.StartsWith("GetHealthPotion") && (int)parentState.GetProperty(Properties.SHIELDHP)==0 && (int)parentState.GetProperty(Properties.HP) < (int)parentState.GetProperty(Properties.MAXHP)*0.3) {
                    h += 10;
                }
                else if (action.Name.StartsWith("GetHealthPotion") && (int)parentState.GetProperty(Properties.HP)+(int)parentState.GetProperty(Properties.SHIELDHP) >= (int)parentState.GetProperty(Properties.MAXHP) * 0.4) {
                    h -= 8;
                }

                if (action.Name.Contains("Skeleton") && (int)parentState.GetProperty(Properties.HP) <= 5) {
                    h = 0;
                }
                else { h += 5; }
                if (action.Name.Contains("Orc") && (int)parentState.GetProperty(Properties.HP) <= 10) {
                    h = 0;
                }
                else if (action.Name.Contains("Dragon") && (int)parentState.GetProperty(Properties.HP) <= 20) {
                    h = 0;
                }
            }
            else {
                h += 10;
            }

            if(!action.CanExecute()){
                h = 0;
            }
            return h;
        }
    }
}
