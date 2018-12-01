using Assets.Scripts.GameManager;
using Assets.Scripts.IAJ.Unity.DecisionMaking.GOB;
using Assets.Scripts.IAJ.Unity.Utils;
using System;
using System.Collections.Generic;
using Action = Assets.Scripts.IAJ.Unity.DecisionMaking.GOB.Action;

namespace Assets.Scripts.IAJ.Unity.DecisionMaking.MCTS
{
    public class MCTSRAVE : MCTS
    {
        protected const float b = 1;
        protected List<Pair<int,Action>> ActionHistory { get; set; }
        public MCTSRAVE(CurrentStateWorldModel worldModel) :base(worldModel)
        {
        }

        protected override MCTSNode BestUCTChild(MCTSNode node)
        {
            float MCTSValue;
            float RAVEValue;
            float UCTValue;
            float bestUCT = float.MinValue;
            MCTSNode bestNode = null;
            //step 1, calculate beta and 1-beta. beta does not change from child to child. So calculate this only once
            //TODO: implement
            throw new NotImplementedException();

            //step 2, calculate the MCTS value, the RAVE value, and the UCT for each child and determine the best one
            
        }


        protected override Reward Playout(WorldModel initialPlayoutState)
        {
            //TODO: implement
            //actionHistory < - []
            //while s is nonterminal do
            //        chose a from Actions(s)uniformly at random
            //        actionHistory <- (Player(s), a)
            //        s <- Result(s, a)
            //return reward,actionHistory for state s
            throw new NotImplementedException();
        }

        protected override void Backpropagate(MCTSNode node, Reward reward)
        {
            //TODO: implement
            //while node is not null do
            //    N(node) <- N(node) + 1
            //    Q(node) <- Q(node) + r(node, Player(Parent(node)))
            //    actionHistory <- (Player(Parent(node), A(node))
            //    node <- Parent(node)

            //    if node is not null do
            //        p <- Player(node)
            //        foreach (childNode in Children(node))
            //            if ((p, A(childNode)) in actionHistory) do
            //                Nrave(childNode) <- Nrave(childNode) + 1
            //                Qrave(childNode) <- Qrave(childNode) + r(c, p)
            throw new NotImplementedException();
        }
    }
}
