using System;
using UnityEngine;
using Assets.Scripts.IAJ.Unity.Util;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicAvoidObstacle : DynamicSeek
    {
        public Collider CollisionDetector { get; set; }

        public float AvoidMargin { get; set; }

        public float MaxLookAhead { get; set; }

        public float WhiskersLength { get; set; }

        public float WhiskersSpan { get; set; }

        public DynamicAvoidObstacle(GameObject obstacle)
        {
            CollisionDetector = obstacle.GetComponent<Collider>();
            this.Target = new KinematicData();
        }

        // SINGLE RAY GET MOVEMENT
        //public override MovementOutput GetMovement()
        //{
        //    if (this.Character.velocity.magnitude != 0)
        //    {
        //        var rayVector = this.Character.velocity.normalized;
        //        RaycastHit hit;
        //        Ray ray = new Ray(this.Character.Position, rayVector.normalized);
        //        Debug.DrawRay(this.Character.Position, rayVector * MaxLookAhead, DebugColor);
        //        var colision = CollisionDetector.Raycast(ray, out hit, MaxLookAhead);
        //        if (!colision)
        //            return new MovementOutput();

        //        if (this.Character.velocity.normalized.Equals(hit.normal * -1f))
        //        {
        //            this.Target.Position = hit.point + Quaternion.Euler(0, 90f, 0) * hit.normal * AvoidMargin;
        //        }
        //        else
        //        {
        //            this.Target.Position = hit.point + hit.normal * AvoidMargin;
        //        }
        //    }
        //    return base.GetMovement();
        //}



        ////WHISKERS GET MOVEMENT
        public override MovementOutput GetMovement()
        {
            if (this.Character.velocity.magnitude != 0)
            {
                var centralRay = this.Character.velocity.normalized;
                Debug.DrawRay(this.Character.Position, centralRay * MaxLookAhead, DebugColor);

                var leftWhisker = this.Character.velocity.normalized;
                leftWhisker = Quaternion.Euler(0, WhiskersSpan / 2 * -1, 0) * leftWhisker;
                Debug.DrawRay(this.Character.Position, leftWhisker * WhiskersLength, DebugColor);

                var rightWhisker = this.Character.velocity.normalized;
                rightWhisker = Quaternion.Euler(0, WhiskersSpan / 2, 0) * rightWhisker;
                Debug.DrawRay(this.Character.Position, rightWhisker * WhiskersLength, DebugColor);

                RaycastHit hit;
                Ray ray = new Ray(this.Character.Position, centralRay.normalized);
                var collision = CollisionDetector.Raycast(ray, out hit, MaxLookAhead);

                if (!collision)
                {
                    ray = new Ray(this.Character.Position, leftWhisker.normalized);
                    collision = CollisionDetector.Raycast(ray, out hit, WhiskersLength);
                    if (!collision)
                    {
                        ray = new Ray(this.Character.Position, rightWhisker.normalized);
                        collision = CollisionDetector.Raycast(ray, out hit, WhiskersLength);
                        if (!collision)
                        {
                            return new MovementOutput();
                        }
                    }
                }

                if (this.Character.velocity.normalized.Equals(hit.normal * -1f))
                {
                    this.Target.Position = hit.point + Quaternion.Euler(0, 45f, 0) * hit.normal * AvoidMargin;
                }
                else
                {
                    this.Target.Position = hit.point + hit.normal * AvoidMargin;
                }
            }
            return base.GetMovement();
        }
    }
}