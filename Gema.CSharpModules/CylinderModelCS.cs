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
    public class CylinderModelCS : IGemaModel
    {
        readonly double Radius;
        readonly double RadiusSquared;
        readonly double CylinderLength;
        readonly int RandomSeed;
        readonly Random RandomGenerator;
        readonly IContinuousDistribution StartPDF;
        readonly IContinuousDistribution StepPDF;
        
        public CylinderModelCS(FSharpMap<String, String> inputPars, SimulationParameters simulationInfo, int randomSeed)
        {

            this.Radius = Convert.ToDouble(inputPars["Radius"]);
            this.RadiusSquared = Radius * Radius;
            this.CylinderLength = Convert.ToDouble(inputPars["CylinderLength"]);
            this.RandomSeed = randomSeed;
            this.RandomGenerator = new MersenneTwister(randomSeed);
            this.StartPDF = new ContinuousUniform(0, 1, RandomGenerator);
            this.StepPDF = new Normal(0.0, simulationInfo.StepSize, RandomGenerator);
        }

        public double[] GetStartPosition()
        {   
            double x = 2 * Radius * StartPDF.Sample() - Radius;
            double y = 2 * Radius * StartPDF.Sample() - Radius;
            double z = CylinderLength * StartPDF.Sample();

            var newPosition = new double[3] { x, y, z };

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

            if ((position[0] * position[0] + position[1] * position[1] < RadiusSquared) && (position[2] > 0) && (position[2] < CylinderLength))
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
