using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics.Random;
using MathNet.Numerics.Distributions;
using Microsoft.FSharp.Core;
using Microsoft.FSharp.Collections;
using Gema.Common;

namespace Gema.CSharpModules
{
    class CircleModelCS : IGemaModel
    {
        readonly double Radius;
        readonly double RadiusSquared;
        readonly int RandomSeed;
        readonly Random RandomGenerator;
        readonly IContinuousDistribution StartPDF;
        readonly IContinuousDistribution StepPDF;
        
        public CircleModelCS(FSharpMap<String, String> inputPars, SimulationParameters simulationInfo, int randomSeed)
        {

            this.Radius = Convert.ToDouble(inputPars["Radius"]);
            this.RadiusSquared = Radius * Radius;
            this.RandomSeed = randomSeed;
            this.RandomGenerator = new MersenneTwister(randomSeed);
            this.StartPDF = new ContinuousUniform(-Radius, Radius, RandomGenerator);
            this.StepPDF = new Normal(0.0, simulationInfo.StepSize, RandomGenerator);
        }

        public double[] GetStartPosition()
        {   
            double x = StartPDF.Sample();
            double y = StartPDF.Sample();

            var newPosition = new double[3] { x, y, 0 };

            return newPosition;
        }

        public FSharpOption<int> CheckStartPosition(double[] position)
        {
            double distanceSquared = position[0] * position[0] + position[1] * position[1];
 
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

            var newDisplacement = new double[3] { dispX, dispY, 0 };

            return newDisplacement;
        }

        public FSharpOption<int> CheckDisplacement(Point previousPoint, double[] displacement)
        {
            var position = new double[2];

            for (int i = 0; i < 2; i++) {

                position[i] = previousPoint.Position[i] + displacement[i];
            }

            if (position[0] * position[0] + position[1] * position[1] < RadiusSquared)
            {
                return FSharpOption<int>.Some(previousPoint.State);
            }
            else
            {
                return FSharpOption<int>.None;
            }
        }
    
    }
}
