using Assets.Scripts.GameManager;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.GOB
{
    public class DepthLimitedGOAPDecisionMaking
    {
        public const int MAX_DEPTH = 3;
        public int ActionCombinationsProcessedPerFrame { get; set; }
        public float TotalProcessingTime { get; set; }
        public int TotalActionCombinationsProcessed { get; set; }
        public bool InProgress { get; set; }

        public CurrentStateWorldModel InitialWorldModel { get; set; }
        private List<Goal> Goals { get; set; }
        private WorldModel[] Models { get; set; }
        private Action[] ActionPerLevel { get; set; }
        public Action[] BestActionSequence { get; private set; }
        public Action BestAction { get; private set; }
        public float BestDiscontentmentValue { get; private set; }
        private int CurrentDepth {  get; set; }

        public DepthLimitedGOAPDecisionMaking(CurrentStateWorldModel currentStateWorldModel, List<Action> actions, List<Goal> goals)
        {
            this.ActionCombinationsProcessedPerFrame = 200;
            this.Goals = goals;
            this.InitialWorldModel = currentStateWorldModel;
        }

        public void InitializeDecisionMakingProcess()
        {
            this.InProgress = true;
            this.TotalProcessingTime = 0.0f;
            this.TotalActionCombinationsProcessed = 0;
            this.CurrentDepth = 0;
            this.Models = new WorldModel[MAX_DEPTH + 1];
            this.Models[0] = this.InitialWorldModel;
            this.ActionPerLevel = new Action[MAX_DEPTH];
            this.BestActionSequence = new Action[MAX_DEPTH];
            this.BestAction = null;
            this.BestDiscontentmentValue = float.MaxValue;
            this.InitialWorldModel.Initialize();
        }

        public Action ChooseAction()
        {
			
			var processedActions = 0;

			var startTime = Time.realtimeSinceStartup;

            //TODO: Implement
            while (processedActions < this.ActionCombinationsProcessedPerFrame)
            {
                this.TotalActionCombinationsProcessed += 1;
                if (this.CurrentDepth < 0) {
                    this.CurrentDepth = 0;
                    this.InProgress = false;
                    this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
                    return this.BestAction;
                }

                else if (this.CurrentDepth >= MAX_DEPTH) {
                    var currentValue = this.Models[this.CurrentDepth].CalculateDiscontentment(this.Goals);

                    if (currentValue < this.BestDiscontentmentValue) {
                        this.BestDiscontentmentValue = currentValue;
                        this.BestAction = this.BestActionSequence[0];
                    }

                    this.TotalActionCombinationsProcessed += 1;
                    this.CurrentDepth -= 1;
                    continue;
                }

                var nextAction = this.Models[this.CurrentDepth].GetNextAction();
                if (nextAction != null) {
                    this.Models[this.CurrentDepth + 1] = this.Models[CurrentDepth].GenerateChildWorldModel();
                    nextAction.ApplyActionEffects(this.Models[this.CurrentDepth + 1]);
                    this.Models[this.CurrentDepth + 1].CalculateNextPlayer(); //delete?
                    this.BestActionSequence[this.CurrentDepth] = nextAction;
                    this.CurrentDepth += 1;
                    processedActions += 1;
                }
                else {
                    CurrentDepth -= 1;
                }
            }


            this.TotalProcessingTime += Time.realtimeSinceStartup - startTime;
			this.InProgress = false;
			return this.BestAction;
        }
    }
}
