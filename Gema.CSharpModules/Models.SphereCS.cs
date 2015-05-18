using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using Gema.Common;

namespace Gema.Models
{
    public class SphereModelCS: IGemaModel
    {
        readonly double Radius;
        readonly double RadiusSquared;
        readonly int RandomSeed;
        readonly Random RandomGenerator;
        readonly IContinuousDistribution StartPDF;
        readonly IContinuousDistribution StepPDF;

        public SphereModelCS (FSharpMap<String, String> inputPars, SimulationParameters simulationInfo, int randomSeed)
        {
            double radius = Convert.ToDouble(inputPars["Radius"]);
            Random randomGenerator = new MersenneTwister(randomSeed);

            this.Radius = radius;
            this.RadiusSquared = radius * radius;
            this.RandomSeed = randomSeed;
            this.RandomGenerator = randomGenerator;
            this.StartPDF = new ContinuousUniform(-radius, radius, randomGenerator);
            this.StepPDF = new Normal(0.0, simulationInfo.StepSize, randomGenerator);
        }

        public double[] GetStartPosition()
        {   
            double x = StartPDF.Sample();
            double y = StartPDF.Sample();
            double z = StartPDF.Sample();

            var newPosition = new double[3] { x, y, z };

            return newPosition;
        }

        public FSharpOption<int> CheckStartPosition(double[] position)
        {
            double distanceSquared = position[0] * position[0] + position[1] * position[1] + position[2] * position[2];
 
            if (distanceSquared < this.RadiusSquared) {
                return FSharpOption<int>.Some(0);
            } else {
                return FSharpOption<int>.None;
            }
        }
 
        public double[] GetDisplacement(Point previousPoint)
        {  
            double dispX = StepPDF.Sample();
            double dispY = StepPDF.Sample();
            double dispZ = StepPDF.Sample();

            var newDisplacement = new double[3] { dispX, dispY, dispZ };

            return newDisplacement;
        }

        public FSharpOption<int> CheckDisplacement(Point previousPoint, double[] displacement)
        {
            var position = new double[3];

            for (int i = 0; i < 3; i++) {

                position[i] = previousPoint.Position[i] + displacement[i];
            }

            double distanceSquared = position[0] * position[0] + position[1] * position[1] + position[2] * position[2];
 
            if (distanceSquared < this.RadiusSquared) {
                return FSharpOption<int>.Some(previousPoint.State);
            } else {
                return FSharpOption<int>.None;
            }
        }
 
    }
}
