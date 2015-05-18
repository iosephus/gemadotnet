
namespace Gema.Common

open System
open MathNet.Numerics.Distributions

type SimulationParameters = { NumberOfParticles: int;
                        NumberOfSteps: int;
                        StorageInterval: int;
                        StepSize: float; }

type Point = { Step: int; State: int; Position: float array; }

type ModelPDFs = { StartPDF: IContinuousDistribution; StepPDF: IContinuousDistribution; RandomGenerator: Random; }

type ModelFunctions = { GetStartPosition: unit -> float array;
                        CheckStartPosition: float array -> int option;
                        GetDisplacement: Point -> float array;
                        CheckDisplacement: Point -> float array -> int option;
                       }

type IGemaModelInputParameters = interface end

type IGemaModelParameters = interface end

type IGemaModel =
    abstract member GetStartPosition: unit -> float array;
    abstract member CheckStartPosition: float array -> int option;
    abstract member GetDisplacement: Point -> float array;
    abstract member CheckDisplacement: Point -> float array -> int option;