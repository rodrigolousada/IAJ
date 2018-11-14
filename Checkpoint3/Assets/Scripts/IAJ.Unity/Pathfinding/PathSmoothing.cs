using Assets.Scripts.IAJ.Unity.Pathfinding.DataStructures;
using Assets.Scripts.IAJ.Unity.Pathfinding.Heuristics;
using Assets.Scripts.IAJ.Unity.Pathfinding.Path;
using RAIN.Navigation.Graph;
using RAIN.Navigation.NavMesh;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Pathfinding
{
    public static class PathSmoothing
    {
        public static GlobalPath StraighLineSmoothing(GlobalPath globalPath, NavMeshPathGraph navmesh)
        {
            Vector3 startPosition = globalPath.PathPositions[0];
            Vector3 endPosition = globalPath.PathPositions[globalPath.PathPositions.Count - 1];
            var globalPathSmooth = new GlobalPath {};

            int nodesCount = globalPath.PathNodes.Count;

            var nrWaypoints = 3;
            Vector3 furtherWaypoint;
            Vector3 waypoint1;
            Vector3 waypoint2;
            Vector3 waypoint3;

            var currentPosition = startPosition;
            for (int i = 0; i < nodesCount; i++)
            {
                currentPosition = globalPath.PathNodes[i].LocalPosition;
                //FurtherPoint is 3 points after i-point, or endpoint if index out of range
                if ((i + nrWaypoints) >= nodesCount-1)
                    furtherWaypoint = endPosition;
                else
                    furtherWaypoint = globalPath.PathNodes[i + nrWaypoints].LocalPosition;

                //Check if there is collision in the middle
                bool collision = false;
                for (int x = 0; x < nrWaypoints; x++) {
                    waypoint1 = Vector3.Lerp(currentPosition, furtherWaypoint, 1 / nrWaypoints * x);
                    waypoint2 = Vector3.Lerp(currentPosition, furtherWaypoint, (1 / nrWaypoints * x) / 2);
                    waypoint3 = Vector3.Lerp(currentPosition, furtherWaypoint, (1 / nrWaypoints * x) * 2);

                    if (!(navmesh.IsPointOnGraph(waypoint1, 1)) || !(navmesh.IsPointOnGraph(waypoint2, 1) || !(navmesh.IsPointOnGraph(waypoint3, 1))))
                        collision=true;
                }

                //If there are no intersections we can smooth the path ignoring the middle points
                if (!collision) {
                    i = i + nrWaypoints - 1;
                }

                globalPathSmooth.PathPositions.Add(currentPosition);
            }
            globalPathSmooth.PathPositions.Add(endPosition);

			return globalPathSmooth;
		}
	}
}
