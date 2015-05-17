
namespace Gema.Models

open System
open MathNet.Numerics.Random
open MathNet.Numerics.Distributions
open Gema

type SphereModelParameters(radius: float) =
    member this.Radius = radius
    member this.RadiusSquared = radius * radius
    interface IGemaModelParameters

type SphereModel2() =
    interface IGemaModel<SphereModelParameters> with
        member this.CreateModelInfo modelPars =
                   let radius = float modelPars.["Radius"]
                   SphereModelParameters radius

        member this.CreatePDFs (simulationInfo:SimulationParameters) (modelInfo:SphereModelParameters) (randomSeed:int) : ModelPDFs =
            let randomGenerator = new MersenneTwister(randomSeed, false)
            let startPDF = new ContinuousUniform(-modelInfo.Radius, modelInfo.Radius, randomGenerator)
            let stepPDF = new Normal(0.0, simulationInfo.StepSize, randomGenerator)
            { StartPDF = startPDF; StepPDF = stepPDF; RandomGenerator = randomGenerator; }


        member this.GetStartPosition simulationParameters modelParameters modelPDFs fromPoint : float array =
            let x = modelPDFs.StartPDF.Sample()
            let y = modelPDFs.StartPDF.Sample()
            let z = modelPDFs.StartPDF.Sample()
            [|x ; y; z|]

        member this.CheckStartPosition simulationParameters modelParameters (position:float array) =
            let distance = position |> Array.map (fun x -> x * x) |> Array.sum
            distance < modelParameters.RadiusSquared

        member this.GetDisplacement simulationParameters modelParameters modelPDFs fromPoint =
            let dispX = modelPDFs.StepPDF.Sample()
            let dispY = modelPDFs.StepPDF.Sample()
            let dispZ = modelPDFs.StepPDF.Sample()
            [| dispX; dispY; dispZ |]

        member this.CheckDisplacement simulationParameters (modelParameters:SphereModelParameters) fromPoint displacement =
            let newPosition = Array.map2 (fun x y -> x + y) fromPoint.Position displacement
            let newDistance = newPosition |> Array.map (fun x -> x * x) |> Array.sum
            newDistance < modelParameters.RadiusSquared
        