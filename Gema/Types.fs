
namespace Gema

open System
open MathNet.Numerics.Distributions

type SimulationInfo = { NumberOfParticles: int;
                            NumberOfSteps: int;
                            StorageInterval: int;
                            StepSize: float; }

type ModelPars = { r: float; }

type Point = { Step: int; State: int; Position: float array; }

type ModelPDFs = { startPDF: IContinuousDistribution; stepPDF: IContinuousDistribution; randomGenerator: Random; }

type IModelInfo =
    abstract member Name: string

type IGemaModel =
    abstract member CreateModelInfo: ModelPars -> IModelInfo
    abstract member CreatePDFs: SimulationInfo -> IModelInfo -> int -> ModelPDFs
