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


        private int CurrentIterations { get; set; }
        private int CurrentIterationsInFrame { get; set; }
        private int CurrentDepth { get; set; }

        private CurrentStateWorldModel CurrentStateWorldModel { get; set; }
        private MCTSNode InitialNode { get; set; }
        private System.Random RandomGenerator { get; set; }
        
        

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
            //        node1 < -Selection(node0)
            //        reward < -Playout(S(node1))
            //        Backpropagation(node1, reward)
            //return A(BestInitialChild(node0))

            float elapsedTime = 0f;

            while (elapsedTime < this.MaxProcessingTimePerFrame) {
                var n1 = this.Selection(selectedNode);
                reward = this.Playout(n1.State);
                this.Backpropagate(n1, reward);

                elapsedTime += Time.realtimeSinceStartup - startTime;
                this.CurrentIterations += 1;
                this.TotalIterations += 1;
            }

            this.TotalProcessingTime += elapsedTime;

            if (this.CurrentIterations >= this.MaxIterations) {
                var n0 = this.BestChild(selectedNode);
                if (n0 != null) {
                    if (n0.Q == 0)
                        return null;

                    this.BestFirstChild = n0;

                    if (!this.IsInfinite)
                        this.InProgress = false;
                    return n0.Action;
                }
            }
            return null;

        }

        private MCTSNode Selection(MCTSNode initialNode)
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
                        currentNode = BestUCTChild(currentNode);
                        this.CurrentDepth++;
                        if (this.CurrentDepth > this.MaxSelectionDepthReached)
                            this.MaxSelectionDepthReached = this.CurrentDepth;
                    }
                    //No children to expand because all were cut
                    else {
                        return currentNode;
                    }
                }
            }
            return currentNode;
        }

        private Reward Playout(WorldModel initialPlayoutState)
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
                //s.CalculateNextPlayer();
                s = s.GenerateChildWorldModel();
            }

            var reward = new Reward {
                Value = s.GetScore(),
                PlayerID = 0
            };

            return reward;
        }

        private void Backpropagate(MCTSNode node, Reward reward)
        {
            //TODO: implement

            //while node is not null do
            //    N(node) <- N(node) + 1
            //    Q(node) <- Q(node) + r(n, Player(Parent(node)))
            //    node <- Parent(node)
            //; ; The reward is determined by the parent’s player
            //; ; because the parent will decide the best child

            while (node!=null) {
                //if (node.PlayerID == reward.PlayerID) {
                    node.N++;
                    node.Q += reward.Value;
                //}
                node = node.Parent;
            }
        }

        private MCTSNode Expand(MCTSNode parent, GOB.Action action)
        {
            //TODO: implement

            //choose a from untried actions from Actions(S(node))
            //add a new child node’ to node
            //    with S(node’) = Result(S(node), a)
            //    with A(node’) = a
            //return node’

            WorldModel state = parent.State.GenerateChildWorldModel();
            action.ApplyActionEffects(state);
            //state.CalculateNextPlayer();

            MCTSNode child = new MCTSNode(state)
            {
                Action = action,
                Parent = parent,
                //PlayerID = state.GetNextPlayer()
            };
            parent.ChildNodes.Add(child);

            return child;
        }

        //gets the best child of a node, using the UCT formula
        private MCTSNode BestUCTChild(MCTSNode node)
        {
            //TODO: implement
            throw new NotImplementedException();
        }

        //this method is very similar to the bestUCTChild, but it is used to return the final action of the MCTS search, and so we do not care about
        //the exploration factor
        private MCTSNode BestChild(MCTSNode node)
        {
            //TODO: implement
            throw new NotImplementedException();
        }
    }
}
