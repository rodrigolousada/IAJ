using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.DynamicMovement
{
    public class DynamicArrive : DynamicVelocityMatch
    {
        public float MaxSpeed { get; set; }
        public float DesiredSpeed { get; set; }
        public float StopRadius { get; set; }
        public float SlowRadius { get; set; }
        public float Distance { get; set; }

        public KinematicData DestinationTarget { get; set; }

        public Vector3 Direction { get; set; }

        public GameObject DebugTarget { get; set; }

        public DynamicArrive()
        {
            this.Target = new KinematicData();
            this.MaxSpeed = 10;
            this.StopRadius = 1.0f;
            this.SlowRadius = 15.0f;
            this.DestinationTarget = new KinematicData();
        }

        public override string Name
        {
            get { return "Arrive"; }
        }


        public override MovementOutput GetMovement()
        {
            Direction = DestinationTarget.Position - Character.Position;
            Distance = Direction.magnitude;
            if (Distance < StopRadius)
                DesiredSpeed = 0;
            else if (Distance > SlowRadius)
                DesiredSpeed = MaxSpeed;
            else
                DesiredSpeed = MaxSpeed * (Distance / SlowRadius);
            //set the target velocity of the base clase (Velocity Matching)
            base.Target.velocity = Direction.normalized * DesiredSpeed;

            if (this.DebugTarget != null)
            {
                this.DebugTarget.transform.position = this.DestinationTarget.Position;
            }

            //execute the getMovement of the base class
            return base.GetMovement();
        }
    }
}