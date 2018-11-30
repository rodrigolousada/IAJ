using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTS
    {
        public const float C = 1.4f;
        public bool InProgress { get; private set; }
        public int MaxIterations { get; set; }
        public int MaxIterationsProcessedPerFrame { get; set; }
        public int MaxProcessingTimePerFrame { get; set; } //comment
        public int TotalIterations { get; set; } //comment
        public bool IsInfinite { get; set; } //comment
        public int MaxPlayoutDepthReached { get; private set; }
        public int MaxSelectionDepthReached { get; private set; }
        public float TotalProcessingTime { get; private set; }
        public MCTSNode BestFirstChild { get; set; }
        public List<GOB.Action> BestActionSequence { get; private set; }


        protected int CurrentIterations { get; set; }
        protected int CurrentIterationsInFrame { get; set; }
        protected int CurrentDepth { get; set; }

        protected CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        protected MCTSNode InitialNode { get; set; }
        protected System.Random RandomGenerator { get; set; }

        public bool RobustMCTS { get; set; } //comment

        public MCTS(CurrentStateWorldModel currentStateWorldModel)
        {
            this.InProgress = false;
            this.IsInfinite = false; //comment
            this.CurrentStateWorldModel = currentStateWorldModel;
            this.MaxIterations = 100;
            this.MaxIterationsProcessedPerFrame = 10;
            this.MaxProcessingTimePerFrame = 5; //comment
            this.TotalIterations = 0; //comment
            this.RandomGenerator = new System.Random();
            this.RobustMCTS = true; //comment
        }


        public void InitializeMCTSearch()
        {
            this.MaxPlayoutDepthReached = 0;
            this.MaxSelectionDepthReached = 0;
            this.CurrentIterations = 0;
            this.CurrentIterationsInFrame = 0;
            this.TotalProcessingTime = 0.0f;
            this.CurrentStateWorldModel.Initialize();
            this.InitialNode = new MCTSNode(this.CurrentStateWorldModel)
            {
                Action = null,
                Parent = null,
                PlayerID = 0
            };
            this.InProgress = true;
            this.BestFirstChild = null;
            this.BestActionSequence = new List<GOB.Action>();
        }

        public GOB.Action Run()
        {
            MCTSNode selectedNode = this.InitialNode;
            Reward reward;

            var startTime = Time.realtimeSinceStartup;
            this.CurrentIterationsInFrame = 0;

            //TODO: implement

            //create root node node0 for state s0
            //while within computational budget do
            //        node1 <- Selection(node0)
            //        reward <- Playout(S(node1))
            //        Backpropagation(node1, reward)
            //return A(BestInitialChild(node0))

            float timeInFrame = 0f;

            while ((timeInFrame < this.MaxProcessingTimePerFrame) && (this.CurrentIterations < this.MaxIterations)) {
                var node1 = this.Selection(selectedNode);
                reward = this.Playout(node1.State);
                this.Backpropagate(node1, reward);

                timeInFrame = Time.realtimeSinceStartup - startTime;
                this.CurrentIterations += 1;
                this.TotalIterations += 1;
            }
            
            this.TotalProcessingTime += timeInFrame;

            if (this.CurrentIterations >= this.MaxIterations) {
                var node0 = this.BestChild(selectedNode);
                if (node0 != null) {
                    if (node0.Q == 0)
                        return null;

                    this.BestFirstChild = node0;

                    if (!this.IsInfinite)
                        this.InProgress = false;
                    return node0.Action;
                }
            }
            return null;

        }

        protected MCTSNode Selection(MCTSNode initialNode)
        {
            GOB.Action nextAction;
            MCTSNode currentNode = initialNode;
            MCTSNode bestChild;


            //TODO: implement

            //while node is nonterminal do
            //        if node not fully expanded then
            //            return Expand(node)
            //        else
            //            node < -BestChild(node)
            //return node

            this.CurrentDepth = 0;
            while (!currentNode.State.IsTerminal()) {
                nextAction = currentNode.State.GetNextAction();
                if (nextAction != null) {
                    //if (this.ApplyCuts && this.ToCut(currentNode.State, nextAction)) {
                    //    continue;
                    //}
                    return Expand(currentNode, nextAction);
                }
                else {
                    if (currentNode.ChildNodes.Count > 0) {
                        bestChild = currentNode;
                        currentNode = this.BestUCTChild(currentNode);
                        this.CurrentDepth++;
                        if (this.CurrentDepth > this.MaxSelectionDepthReached)
                            this.MaxSelectionDepthReached = this.CurrentDepth;
                    }
                    else {
                        return currentNode;
                    }
                }
            }
            return currentNode;
        }

        protected virtual Reward Playout(WorldModel initialPlayoutState)
        {
            //TODO: implement

            //while s is nonterminal do
            //        chose a from Actions(s)uniformly at random
            //s <- Result(s, a)
            //return reward for state s

            WorldModel s = initialPlayoutState.GenerateChildWorldModel();
            while (!s.IsTerminal()) {
                var executable_actions = s.GetExecutableActions();
                var action = executable_actions[UnityEngine.Random.Range(0, executable_actions.Length - 1)];
                action.ApplyActionEffects(s);
                s.CalculateNextPlayer();
                s = s.GenerateChildWorldModel();
            }

            var reward = new Reward {
                Value = s.GetScore(),
                PlayerID = 0
            };

            return reward;
        }

        protected virtual void Backpropagate(MCTSNode node, Reward reward)
        {
            //TODO: implement

            //while node is not null do
            //    N(node) <- N(node) + 1
            //    Q(node) <- Q(node) + r(n, Player(Parent(node)))
            //    node <- Parent(node)
            //; ; The reward is determined by the parent’s player
            //; ; because the parent will decide the best child

            while (node!=null) {
                if (node.PlayerID == reward.PlayerID) {
                    node.N++;
                    node.Q += reward.Value;
                }
                node = node.Parent;
            }
        }

        protected MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            //TODO: implement

            //choose a from untried actions from Actions(S(node))
            //add a new child node’ to node
            //    with S(node’) = Result(S(node), a)
            //    with A(node’) = a
            //return node’

            WorldModel state = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(state);
            state.CalculateNextPlayer();

            MCTSNode child_node = new MCTSNode(state)
            {
                Action = action,
                Parent = parent,
                PlayerID = state.GetNextPlayer()
            };
            parent.ChildNodes.Add(child_node);

            return child_node;
        }

        //gets the best child of a node, using the UCT formula
        protected virtual MCTSNode BestUCTChild(MCTSNode node)
        {
            //TODO: implement
            float parentLog = Mathf.Log(node.N);
            float currentUCT;
            float bestUCT = -1;

            MCTSNode bestChild = null;
            foreach(var childNode in node.ChildNodes) {
                var exploitation = (childNode.Q / childNode.N);
                var exploration = C * Mathf.Sqrt(parentLog / childNode.N);
                currentUCT = exploitation + exploration;

                if(currentUCT > bestUCT) {
                    bestUCT = currentUCT;
                    bestChild = childNode;
                }
            }
            return bestChild;
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        protected MCTSNode BestChild(MCTSNode node)
        {
            //TODO: implement
            List<MCTSNode> childNodes = node.ChildNodes;
            float currentUCT;
            float bestUCT = -1;

            MCTSNode bestChild = null;

            foreach(var childNode in childNodes) {
                //if (this.ApplyCuts && this.ToCut(node.State, childNode.Action))
                //    continue;
                if (this.RobustMCTS)
                    currentUCT = childNode.N;
                else
                    currentUCT = (childNode.Q / childNode.N);

                if(currentUCT > bestUCT) {
                    bestUCT = currentUCT;
                    bestChild = childNode;
                }
            }
            return bestChild;
        }

        //protected GOB.Action BestFinalAction(MCTSNode node)
        //{
        //    //TODO: implement
        //    throw new NotImplementedException();
        //}
    }
}
