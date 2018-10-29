using RAIN.Navigation.Graph;
//using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics
{
    public class EuclidianHeuristic : IHeuristic
    {
        public float H(NavigationGraphNode node, NavigationGraphNode goalNode)
        {
            //var nodePosition = node.Position;
            //var goalNodePosition = goalNode.Position;
            //return (new Vector3(nodePosition.x - goalNodePosition.x, nodePosition.y - goalNodePosition.y, nodePosition.z - goalNodePosition.z).magnitude);
            return (node.Position - goalNode.Position).magnitude;
        }
    }
}