//class adapted from the HRVO library http://gamma.cs.unc.edu/HRVO/
//adapted to IAJ classes by João Dias

using System.Collections.Generic;
using System.Linq;
using System;
using System.Text;
using Assets.Scripts.IAJ.Unity.Movement.DynamicMovement;
using Assets.Scripts.IAJ.Unity.Util;
using UnityEngine;

namespace Assets.Scripts.IAJ.Unity.Movement.VO
{
    public class RVOMovement : DynamicMovement.DynamicVelocityMatch
    {
        public override string Name
        {
            get { return "RVO"; }
        }

        protected List<KinematicData> Characters { get; set; }
        protected List<StaticData> Obstacles { get; set; }
        public float CharacterSize { get; set; }
        public float IgnoreDistance { get; set; }
        public float MaxSpeed { get; set; }
        //create additional properties if necessary
        public float Weight { get; set; }
        public int NumSamples { get; set; }

        protected DynamicMovement.DynamicMovement DesiredMovement { get; set; }

        public RVOMovement(DynamicMovement.DynamicMovement goalMovement, List<KinematicData> movingCharacters, List<StaticData> obstacles)
        {
            this.DesiredMovement = goalMovement;
            this.Characters = movingCharacters;
            this.Obstacles = obstacles;
            base.Target = new KinematicData();

            //initialize other properties if you think is relevant
        }

        public Vector3 GetBestSample(Vector3 desiredVelocity, List<Vector3> Samples) {
            var bestSample = Vector3.zero; //default velocity if all samples suck
            var minimumPenalty = float.MaxValue; //INF
            foreach (var sample in Samples) {
                //penalty based on the distance to desired velocity
                var distancePenalty = (desiredVelocity - sample).magnitude;
                var maximumTimePenalty = 0f;
                foreach (var b in this.Characters) {
            //        var deltaP = b.Position - this.Character.Position;
            //        if (deltaP.magnitude > IgnoreDistance) //we can safely ignore this character
            //            continue;
            //        //test the collision of the ray λ(pA,2vA’-vA-vB) with the circle
            //        var rayVector = 2 * sample - this.Character.velocity - b.velocity;
            //        var tc = MathHelper.TimeToCollisionBetweenRayAndCircle(this.Character.Position, rayVector.normalized, b.Position, CharacterSize * 2);
            //        var timePenalty = 0f; //no collision
            //        if (tc > 0)//future collision
            //            timePenalty = Weight / tc;
            //        else if (tc == 0) //immediate collision
            //            timePenalty = float.MaxValue;
            //        if (timePenalty > maximumTimePenalty) //opportunity for optimization here
            //            maximumTimePenalty = timePenalty;
                }

                var penalty = distancePenalty + maximumTimePenalty;
                if (penalty < minimumPenalty) { //opportunity for optimization here
                    minimumPenalty = penalty;
                    bestSample = sample;
                }
            }
            return bestSample;
        }

        public override MovementOutput GetMovement()
        {
            //TODO: implement the method
            //1) calculate desired velocity
            var desiredOutput = this.DesiredMovement.GetMovement();
            //if movementOutput is acceleration we need to convert it to velocity
            var desiredVelocity = this.Character.velocity + desiredOutput.linear;
            //trim velocity if bigger than max
            if(desiredVelocity.magnitude > MaxSpeed) {
                desiredVelocity.Normalize();
                desiredVelocity*=MaxSpeed;
            }

            //2) generate samples
            //always consider the desired velocity as a sample
            var samples = new List<Vector3>
            {
                desiredVelocity
            };
            for (var i = 0; i < NumSamples; i++) {
                var angle = RandomHelper.RandomBinomial(MathConstants.MATH_2PI); //random angle between 0 and 2PI
                var magnitude = RandomHelper.RandomBinomial(MaxSpeed); //random magnitude between 0 and maxSpeed
                var velocitySample = MathHelper.ConvertOrientationToVector(angle)*magnitude;
                samples.Add(velocitySample);
            }
            
            //3) evaluate and get best sample
            base.Target.velocity = GetBestSample(desiredVelocity, samples);
            //4) let the base class take care of achieving the final velocity
            return base.GetMovement();
        }
    }
}
