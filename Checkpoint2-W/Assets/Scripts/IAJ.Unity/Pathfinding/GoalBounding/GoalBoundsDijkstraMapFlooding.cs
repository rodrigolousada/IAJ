using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures.GoalBounding;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.GoalBounding
{
    //The Dijkstra algorithm is similar to the A* but with a couple of differences
    //1) no heuristic function
    //2) it will not stop until the open list is empty
    //3) we dont need to execute the algorithm in multiple steps (because it will be executed offline)
    //4) we don't need to return any path (partial or complete)
    //5) we don't need to do anything when a node is already in closed
    public class GoalBoundsDijkstraMapFlooding
    {
        public NavMeshPathGraph NavMeshGraph { get; protected set; }
        public NavigationGraphNode StartNode { get; protected set; }
        public NodeGoalBounds NodeGoalBounds { get; protected set; }
        protected NodeRecordArray NodeRecordArray { get; set; }
        protected List<NavigationGraphNode> Nodes { get; set; }

        public IOpenSet Open { get; protected set; }
        public IClosedSet Closed { get; protected set; }
        
        public GoalBoundsDijkstraMapFlooding(NavMeshPathGraph graph)
        {
            this.NavMeshGraph = graph;
            //do not change this
            var nodes = this.GetNodesHack(graph);
            this.NodeRecordArray = new NodeRecordArray(nodes);
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;
            this.Nodes = nodes;
        }

        public void Search(NavigationGraphNode startNode, NodeGoalBounds nodeGoalBounds)
        {
            //TODO: Implement the algorithm that calculates the goal bounds using a dijkstra
            //Given that the nodes in the graph correspond to the edges of a polygon, we won't be able to use the vertices of the polygon to update the bounding boxes
            
            this.NodeRecordArray = new NodeRecordArray(this.Nodes);
            NodeRecord lowestCost = null;
            this.Open = this.NodeRecordArray;
            this.Closed = this.NodeRecordArray;

            for (int i=0; i < startNode.OutEdgeCount; i++)
            {
                nodeGoalBounds.connectionBounds[i] = ScriptableObject.CreateInstance<DataStructures.GoalBounding.Bounds>();
                this.ProcessChildNode(this.NodeRecordArray.GetNodeRecord(startNode), startNode.EdgeOut(i), i);
            }

            while (this.Open.CountOpen()!=0){ 
                
                lowestCost = this.Open.GetBestAndRemove();
                this.Closed.AddToClosed(lowestCost);
                nodeGoalBounds.connectionBounds[lowestCost.connectionIndex].UpdateBounds(lowestCost.node.Position);

                var connectLowestNode = lowestCost.node.OutEdgeCount;
                for (int i=0; i < connectLowestNode; i++){
                    this.ProcessChildNode(lowestCost, lowestCost.node.EdgeOut(i),lowestCost.connectionIndex);
                }
            }
            this.NodeGoalBounds = nodeGoalBounds;
        }


        protected void ProcessChildNode(NodeRecord parent, NavigationGraphEdge connectionEdge, int connectionIndex)
        {
            //TODO: Implement this method that processes a child node. Then you can use it in the Search method above.
            //------------------------------------------------------------------
            float f; //function
            float g; //custo

            var childNode = connectionEdge.ToNode;
            var childNodeRecord = this.NodeRecordArray.GetNodeRecord(childNode);

            if (childNodeRecord == null)
            {
                //this piece of code is used just because of the special start nodes and goal nodes added to the RAIN Navigation graph when a new search is performed.
                //Since these special goals were not in the original navigation graph, they will not be stored in the NodeRecordArray and we will have to add them
                //to a special structure
                //it's ok if you don't understand this, this is a hack and not part of the NodeArrayA* algorithm, just do NOT CHANGE THIS, or your algorithm will not work
                childNodeRecord = new NodeRecord
                {
                    node = childNode,
                    parent = parent,
                    connectionIndex = connectionIndex,
                    status = NodeStatus.Unvisited
                };
                this.NodeRecordArray.AddSpecialCaseNode(childNodeRecord);
            }

            f = g = parent.gValue + (childNode.LocalPosition - parent.node.LocalPosition).magnitude;

            //se nao tivermos melhorias, utilizar isto
            var statChildNode = childNodeRecord.status;
            var valueChildNode = childNodeRecord.fValue;

            if (statChildNode == NodeStatus.Unvisited)
            {
                childNodeRecord.gValue = g;
                childNodeRecord.fValue = f;
                childNodeRecord.parent = parent;
                childNodeRecord.connectionIndex = connectionIndex;
                this.Open.AddToOpen(childNodeRecord);
            }
            else if (statChildNode == NodeStatus.Open && valueChildNode > f)
            {
                childNodeRecord.gValue = g;
                childNodeRecord.fValue = f;
                childNodeRecord.parent = parent;
                childNodeRecord.connectionIndex = connectionIndex;

            }

            //------------------------------------------------------------------
        }

        private List<NavigationGraphNode> GetNodesHack(NavMeshPathGraph graph)
        {
            //this hack is needed because in order to implement NodeArrayA* you need to have full acess to all the nodes in the navigation graph in the beginning of the search
            //unfortunately in RAINNavigationGraph class the field which contains the full List of Nodes is private
            //I cannot change the field to public, however there is a trick in C#. If you know the name of the field, you can access it using reflection (even if it is private)
            //using reflection is not very efficient, but it is ok because this is only called once in the creation of the class
            //by the way, NavMeshPathGraph is a derived class from RAINNavigationGraph class and the _pathNodes field is defined in the base class,
            //that's why we're using the type of the base class in the reflection call
            return (List<NavigationGraphNode>)Utils.Reflection.GetInstanceField(typeof(RAINNavigationGraph), graph, "_pathNodes");
        }

    }
}
