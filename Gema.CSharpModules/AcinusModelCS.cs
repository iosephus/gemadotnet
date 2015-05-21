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
    public class AcinusModelCS : IGemaModel
    {
        readonly double InnerRadius;
        readonly double InnerRadiusSquared;
        readonly double OuterRadius;
        readonly double OuterRadiusSquared;
        readonly double CylinderLength;
        readonly double CellAngle;
        readonly double Aperture;
        readonly int RandomSeed;
        readonly Random RandomGenerator;
        readonly IContinuousDistribution StartPDF;
        readonly IContinuousDistribution StepPDF;

        public AcinusModelCS(FSharpMap<String, String> inputPars, SimulationParameters simulationInfo, int randomSeed)
        {

            this.InnerRadius = Convert.ToDouble(inputPars["InnerRadius"]);
            this.InnerRadiusSquared = InnerRadius * InnerRadius;
            this.OuterRadius = Convert.ToDouble(inputPars["OuterRadius"]);
            this.OuterRadiusSquared = OuterRadius * OuterRadius;
            this.CylinderLength = Convert.ToDouble(inputPars["CylinderLength"]);
            this.CellAngle = Convert.ToDouble(inputPars["CellAngle"]);
            this.Aperture = Convert.ToDouble(inputPars["Aperture"]);
            this.RandomSeed = randomSeed;
            this.RandomGenerator = new MersenneTwister(randomSeed);
            this.StartPDF = new ContinuousUniform(0, 1, RandomGenerator);
            this.StepPDF = new Normal(0.0, simulationInfo.StepSize, RandomGenerator);
        }

        public double[] GetStartPosition()
        {   
            double x = 2 * OuterRadius * StartPDF.Sample() - OuterRadius;
            double y = 2 * OuterRadius * StartPDF.Sample() - OuterRadius;
            double z = CylinderLength * StartPDF.Sample();

            var newPosition = new double[3] { x, y, z };

            return newPosition;
        }

        public FSharpOption<int> CheckStartPosition(double[] position)
        {
            double distanceSquared = position[0] * position[0] + position[1] * position[1];
 
            if (distanceSquared < this.OuterRadiusSquared) {
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

            double x, y, z;
            double x_next, y_next, z_next;
            double ro_square, theta, theta_reduced, z_reduced;
            double ro_square_next, theta_next;
            bool both_within_same_cell_angle, both_within_same_cell_z, within_mouth_angle, within_mouth_z;

            x = previousPoint.Position[0];
            y = previousPoint.Position[1];
            z = previousPoint.Position[2];

            x_next = position[0];
            y_next = position[1];
            z_next = position[2];

            /* Compute cylindrical coordinates (using squared ro) */
            ro_square = x * x + y * y;
            ro_square_next = x_next * x_next + y_next * y_next;

            /* Case of movement within the inner cylinder */
            if ( (ro_square < InnerRadiusSquared) && (ro_square_next < InnerRadiusSquared) ) 
            {
                return FSharpOption<int>.Some(previousPoint.State);
            }

            /* Taking the particle outside the whole structure is not allowed */
            if (ro_square_next > OuterRadiusSquared)
            {
                return FSharpOption<int>.None;
            }

            theta = Math.Atan2(x, y) + Math.PI;
            theta_next = Math.Atan2(x_next, y_next) + Math.PI;
            both_within_same_cell_angle = ((int)Math.Floor(theta / CellAngle) == (int)Math.Floor(theta_next / CellAngle));
            both_within_same_cell_z = ((int)Math.Floor(z / CylinderLength) == (int)Math.Floor(z_next / CylinderLength));

            if (both_within_same_cell_angle && both_within_same_cell_z)
            {
                if ((ro_square > InnerRadiusSquared) && (ro_square_next > InnerRadiusSquared))
                {
                    return FSharpOption<int>.Some(previousPoint.State);
                }
                theta_reduced = theta / CellAngle - Math.Floor(theta / CellAngle);
                z_reduced = z / CylinderLength - Math.Floor(z / CylinderLength);
                within_mouth_angle = ((theta_reduced > ((1 - Aperture) / 2)) && (theta_reduced < ((1 + Aperture) / 2)));
                within_mouth_z = ((z_reduced > ((1 - Aperture) / 2)) && (z_reduced < ((1 + Aperture) / 2)));
                if (within_mouth_angle && within_mouth_z)
                {
                    return FSharpOption<int>.Some(previousPoint.State);
                }
            }
            return FSharpOption<int>.None;


        }
     }
}
