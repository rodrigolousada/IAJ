﻿using RAIN.Navigation.Graph;
using System.Collections.Generic;
using System.Linq;

namespace Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures
{
    public class SimpleHashMap : IOpenSet, IClosedSet
    {
        private Dictionary<NavigationGraphNode, NodeRecord> NodeRecords { get; set; }

        public SimpleHashMap()
        {
            this.NodeRecords = new Dictionary<NavigationGraphNode, NodeRecord>();
        }

        public void Initialize()
        {
            this.NodeRecords.Clear(); 
        }

        public int CountOpen()
        {
            return this.NodeRecords.Count;
        }

        public void AddToClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord.node,nodeRecord);
        }

        public void RemoveFromClosed(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord.node);
        }

        public NodeRecord SearchInClosed(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            NodeRecord node = null;
            this.NodeRecords.TryGetValue(nodeRecord.node, out node);
            return node;
        }

        public void AddToOpen(NodeRecord nodeRecord)
        {
            this.NodeRecords.Add(nodeRecord.node,nodeRecord);
        }

        public void RemoveFromOpen(NodeRecord nodeRecord)
        {
            this.NodeRecords.Remove(nodeRecord.node);
        }

        public NodeRecord SearchInOpen(NodeRecord nodeRecord)
        {
            //here I cannot use the == comparer because the nodeRecord will likely be a different computational object
            //and therefore pointer comparison will not work, we need to use Equals
            //LINQ with a lambda expression
            NodeRecord node = null;
            this.NodeRecords.TryGetValue(nodeRecord.node, out node);
            return node;
        }

        public ICollection<NodeRecord> All()
        {
            return this.NodeRecords.Values.ToList();
        }

        public void Replace(NodeRecord nodeToBeReplaced, NodeRecord nodeToReplace)
        {
            //since the list is not ordered we do not need to remove the node and add the new one, just copy the different values
            //remember that if NodeRecord is a struct, for this to work we need to receive a reference
            nodeToBeReplaced.parent = nodeToReplace.parent;
            nodeToBeReplaced.fValue = nodeToReplace.fValue;
            nodeToBeReplaced.gValue = nodeToReplace.gValue;
            nodeToBeReplaced.hValue = nodeToReplace.hValue;
        }

        public NodeRecord GetBestAndRemove()
        {
            var best = this.PeekBest();
            this.NodeRecords.Remove(best.node);
            return best;
        }

        public NodeRecord PeekBest()
        {
            //welcome to LINQ guys, for those of you that remember LISP from the AI course, the LINQ Aggregate method is the same as lisp's Reduce method
            //so here I'm just using a lambda that compares the first element with the second and returns the lowest
            //by applying this to the whole list, I'm returning the node with the lowest F value.
            return this.NodeRecords.Values.Aggregate((nodeRecord1, nodeRecord2) => nodeRecord1.fValue < nodeRecord2.fValue ? nodeRecord1 : nodeRecord2);
        }
    }
}