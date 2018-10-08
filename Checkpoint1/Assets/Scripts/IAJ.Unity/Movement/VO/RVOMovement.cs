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

        public Vector3 Vec3Subtraction(Vector3 vec1, Vector3 vec2){
            return new Vector3(vec1.x - vec2.x, vec1.y - vec2.y, vec1.z - vec2.z);
        }

        public Vector3 GetBestSample(Vector3 desiredVelocity, List<Vector3> Samples) {
            var bestSample = Vector3.zero; //default velocity if all samples suck
            var minimumPenalty = float.MaxValue; //INF
            foreach (var sample in Samples) {
                //penalty based on the distance to desired velocity

                var distancePenalty = Vec3Subtraction(desiredVelocity,sample).magnitude;

                var maximumTimePenalty = 2f;

                var characterPos = this.Character.Position;
                foreach (var b in this.Characters) {
                    var bPos = b.Position;

                    var deltaP = Vec3Subtraction( bPos, characterPos);


                    if (deltaP.magnitude > IgnoreDistance) //we can safely ignore this character
                        continue;
                    //test the collision of the ray λ(pA,2vA’-vA-vB) with the circle
                    var rayVector = Vec3Subtraction( Vec3Subtraction( 2 * sample , this.Character.velocity) , b.velocity);

                    var tc = MathHelper.TimeToCollisionBetweenRayAndCircle(characterPos, rayVector, bPos, CharacterSize * 2);
                    var timePenalty = 0f; //no collision
                    if (tc > 0)//future collision
                        timePenalty = Weight / tc;
                    else if (tc < 0.01f && tc >= 0) //immediate collision
                        timePenalty = float.MaxValue;
                    maximumTimePenalty = Math.Max(timePenalty, maximumTimePenalty);

  
                }
                foreach (var b in this.Obstacles)
                {
                    var bPos = b.Position;
                    var deltaP = Vec3Subtraction(bPos, characterPos);

                    if (deltaP.magnitude > IgnoreDistance) //we can safely ignore this character
                        continue;
                    //test the collision of the ray λ(pA,vA') with the circle
                    var rayVector = sample;
                    var tc = MathHelper.TimeToCollisionBetweenRayAndCircle(characterPos, rayVector, bPos, CharacterSize * 2);
                    var timePenalty = 0f; //no collision
                    if (tc > 0)//future collision
                        timePenalty = Weight / tc;
                    else if (tc < 0.01f && tc >= 0) //immediate collision
                        timePenalty = float.MaxValue;
                    maximumTimePenalty = Math.Max(timePenalty,maximumTimePenalty);

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
