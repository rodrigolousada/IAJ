using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using RAIN.Navigation.NavMesh;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using RAIN.Navigation.Graph;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    public class GoalBoundingPathfinding : NodeArrayAStarPathFinding
    {
        public GoalBoundingTable GoalBoundingTable { get; protected set;}
        public int DiscardedEdges { get; protected set; }
		public int TotalEdges { get; protected set; }

        public GoalBoundingPathfinding(NavMeshPathGraph graph, IHeuristic heuristic, GoalBoundingTable goalBoundsTable) : base(graph, heuristic)
        {
            this.GoalBoundingTable = goalBoundsTable;
        }

        public override void InitializePathfindingSearch(Vector3 startPosition, Vector3 goalPosition)
        {
            this.DiscardedEdges = 0;
			this.TotalEdges = 0;
            base.InitializePathfindingSearch(startPosition, goalPosition);
        }

        protected override void ProcessChildNode(NodeRecord parentNode, NavigationGraphEdge connectionEdge, int edgeIndex)
        {
			//TODO: Implement this method for the GoalBoundingPathfinding to Work. If you implemented the NodeArrayAStar properly, you wont need to change the search method.
            if (parentNode != null && GoalBoundingTable != null){
                var nodeGoalBounds = this.GoalBoundingTable.table[parentNode.node.NodeIndex];
                if(nodeGoalBounds != null && edgeIndex < nodeGoalBounds.connectionBounds.Length){
                    var bounds = nodeGoalBounds.connectionBounds[edgeIndex];
                    if(!bounds.PositionInsideBounds(this.GoalPosition))
                        return;
                }
            }
            base.ProcessChildNode(parentNode, connectionEdge, edgeIndex);
        }
    }
}
