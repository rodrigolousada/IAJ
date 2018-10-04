using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    class DynamicAvoidCharacter : DynamicSeek //DynamicMovement
    {
        //public override KinematicData Target { get; set; }
        public float AvoidMargin { get; set; }
        public float MaxTimeLookAhead { get; set; }
        public float CollisionRadius { get; set; }

        //public List<KinematicData> Targets { get; set; }
        //public KinematicData ClosestTarget { get; set; }
        //public float ClosestFutureDistance { get; set; }
        //public Vector3 ClosestFutureDeltaPos { get; set; }
        //public Vector3 ClosestDeltaPos { get; set; }
        //public Vector3 ClosestDeltaVel { get; set; }

        public override string Name
        {
            get { return "AvoidCharacter"; }
        }
        public DynamicAvoidCharacter(KinematicData target)
        {
            this.Target = target;
        }

        public override MovementOutput GetMovement()
        {
            //Avoidance Character
            var output = new MovementOutput();
            var deltaPos = this.Target.Position - this.Character.Position;
            var deltaVel = this.Target.velocity - this.Character.velocity;
            var deltaSqrSpeed = deltaVel.sqrMagnitude;
            if (deltaSqrSpeed < 0.01)
                return output;
            var timeToClosest = -Vector3.Dot(deltaPos, deltaVel) / deltaSqrSpeed;
            if (timeToClosest > MaxTimeLookAhead)
                return output;
            //for efficiency reasons I use the deltas instead of character and target
            var futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            var futureDistance = futureDeltaPos.magnitude;
            if (futureDistance > 2 * CollisionRadius)
                return output;

            if (futureDistance <= 0 || deltaPos.magnitude < 2 * CollisionRadius)
                //deals with exact or immediate collisions
                output.linear = this.Character.Position - this.Target.Position;
            else
                output.linear = futureDeltaPos * -1;

            output.linear = output.linear.normalized * MaxAcceleration;
            Debug.DrawRay(this.Character.Position, output.linear.normalized, DebugColor);
            return output;




            ////Multiple Characters Avoidance
            //var INF = float.MaxValue;
            //var shortestTime = INF;
            //foreach (var t in Targets)
            //{
            //    var deltaPos = t.Position - this.Character.Position;
            //    var deltaVel = t.velocity - this.Character.velocity;
            //    var deltaSqrSpeed = deltaVel.sqrMagnitude;

            //    if (deltaSqrSpeed == 0)
            //        continue;
            //    var timeToClosest = -(Vector3.Dot(deltaPos,deltaVel)) / deltaSqrSpeed;
            //    if (timeToClosest > MaxTimeLookAhead)
            //        continue;
            //    var futureDeltaPos = deltaPos + deltaVel * timeToClosest;
            //    var futureDistance = futureDeltaPos.magnitude;
            //    if (futureDistance > 2 * CollisionRadius)
            //        continue;

            //    if (timeToClosest > 0 && timeToClosest < shortestTime)
            //    {
            //        shortestTime = timeToClosest;
            //        ClosestTarget = t;
            //        ClosestFutureDistance = futureDistance;
            //        ClosestFutureDeltaPos = futureDeltaPos;
            //        ClosestDeltaPos = deltaPos;
            //        ClosestDeltaVel = deltaVel;
            //    }
            //}

            //var output = new MovementOutput();
            //if (shortestTime == INF)
            //    return output;
            //var avoidanceDirection = ClosestFutureDeltaPos * -1;
            //if (ClosestFutureDistance <= 0 || ClosestDeltaPos.magnitude < 2 * CollisionRadius)
            //    avoidanceDirection = this.Character.Position - ClosestTarget.Position;
            //output.linear = avoidanceDirection.normalized * MaxAcceleration;
            //return output;
        }
    }
}