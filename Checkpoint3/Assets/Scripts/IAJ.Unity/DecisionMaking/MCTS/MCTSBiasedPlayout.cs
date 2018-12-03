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
        public const int MCTS_MAX_DEPTH = 5;
        public bool depthLimited = true;

        public MCTSBiasedPlayout(CurrentStateWorldModel currentStateWorldModel) : base(currentStateWorldModel)
        {
        }
        private static readonly System.Random random = new System.Random();


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
            var CurrentDepth = 0;
            //var heuristic_value = 0f;

            if (actions.Length == 0) {
                reward.Value = 0;
                reward.PlayerID = state.GetNextPlayer();
                return reward;
            }

            while (!state.IsTerminal() && (!depthLimited||CurrentDepth < MCTS_MAX_DEPTH)) {
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
                        //if (i != 0)
                        //    heuristic_value = heuristicValues[i] - heuristicValues[i - 1];
                        //else
                        //    heuristic_value = heuristicValues[i];
                        break;
                    }
                }
                CurrentDepth += 1;
            }

            //if(depthLimited && CurrentDepth >= MCTS_MAX_DEPTH) {
            //    reward.Value = heuristic_value;
            //}
            //else {
                reward.Value = state.GetScore();
            //}
            reward.PlayerID = state.GetNextPlayer();
            return reward;
        }

        protected float CalcHeuristic(WorldModel parentState, GOB.Action action) //MCTSNode
        {
            //TODO: implement
            WalkToTargetAndExecuteAction walk_action = action as WalkToTargetAndExecuteAction;
            float h = 2;

            if (action.Name.StartsWith("LevelUp") && action.CanExecute(parentState)) {
                h += 100;
            }
            if (action.Name.StartsWith("DivineWrath") && action.CanExecute(parentState)) {
                h += 100;
            }

            float euclidian_distance;
            if (walk_action != null) {
                euclidian_distance = action.GetDuration(parentState);
                h += Mathf.Abs(12 - (3/2)*euclidian_distance);

                if (action.Name.StartsWith("GetHealthPotion") && (int)parentState.GetProperty(Properties.HP) < (int)parentState.GetProperty(Properties.MAXHP)*0.3) {
                    h += 20;
                }
                else if (action.Name.Contains("Skeleton") && (int)parentState.GetProperty(Properties.HP) <= 6) {
                    h = 0;
                }
                else if (action.Name.Contains("Orc") && (int)parentState.GetProperty(Properties.HP) <= 20) {
                    h = 0;
                }
                else if (action.Name.Contains("Dragon") && (int)parentState.GetProperty(Properties.HP) <= 36) {
                    h = 0;
                }
            }
            else {
                h += 12;
            }
            return h;
        }
    }
}
