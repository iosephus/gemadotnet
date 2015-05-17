
namespace Gema

open System
open MathNet.Numerics.Distributions

type SimulationParameters = { NumberOfParticles: int;
                        NumberOfSteps: int;
                        StorageInterval: int;
                        StepSize: float; }

type Point = { Step: int; State: int; Position: float array; }

type ModelPDFs = { StartPDF: IContinuousDistribution; StepPDF: IContinuousDistribution; RandomGenerator: Random; }

type ModelFunctions = { GetStartPosition: float array;
                        CheckStartPosition: float array -> int option;
                        GetDisplacement: Point -> float array;
                        CheckDisplacement: Point -> float array -> int option;
                       }

type IGemaModelInputParameters = interface end

type IGemaModelParameters = interface end

type IGemaModel<'T> =
    abstract member CreateModelInfo: Map<string, string> -> 'T
    abstract member CreatePDFs: SimulationParameters -> 'T -> int -> ModelPDFs
    abstract member GetStartPosition: SimulationParameters -> 'T -> ModelPDFs -> Point -> float array;
    abstract member CheckStartPosition: SimulationParameters -> 'T -> float array -> int option;
    abstract member GetDisplacement: SimulationParameters -> 'T -> ModelPDFs -> Point -> float array;
    abstract member CheckDisplacement: SimulationParameters -> 'T -> Point -> float array -> int option;
