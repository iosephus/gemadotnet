
namespace Gema.Models

open System
open MathNet.Numerics.Random
open MathNet.Numerics.Distributions
open Gema.Common

module SphereModel =

    type Parameters = { Radius: float; RadiusSquared: float }

    let CreateModelInfo (modelPars: Map<string, string>) =
        let radius = float modelPars.["Radius"]
        { Radius = radius; RadiusSquared = radius * radius }

    let CreatePDFs (simulationInfo:SimulationParameters) (modelInfo:Parameters) (randomSeed:int) : ModelPDFs =
        let randomGenerator = new MersenneTwister(randomSeed, false)
        let startPDF = new ContinuousUniform(-modelInfo.Radius, modelInfo.Radius, randomGenerator)
        let stepPDF = new Normal(0.0, simulationInfo.StepSize, randomGenerator)
        { StartPDF = startPDF; StepPDF = stepPDF; RandomGenerator = randomGenerator; }

    let GetStartPosition simulationParameters modelParameters modelPDFs fromPoint : float array =
        let x = modelPDFs.StartPDF.Sample()
        let y = modelPDFs.StartPDF.Sample()
        let z = modelPDFs.StartPDF.Sample()
        [|x ; y; z|]

    let CheckStartPosition simulationParameters modelParameters (position:float array) : int option =
        let distance = position |> Array.map (fun x -> x * x) |> Array.sum
        if distance < modelParameters.RadiusSquared then Some 0 else None

    let GetDisplacement simulationParameters modelParameters modelPDFs fromPoint =
        let dispX = modelPDFs.StepPDF.Sample()
        let dispY = modelPDFs.StepPDF.Sample()
        let dispZ = modelPDFs.StepPDF.Sample()
        [| dispX; dispY; dispZ |]

    let CheckDisplacement simulationParameters modelParameters fromPoint displacement =
        let newPosition = Array.map2 (fun x y -> x + y) fromPoint.Position displacement
        let newDistance = newPosition |> Array.map (fun x -> x * x) |> Array.sum
        if newDistance < modelParameters.RadiusSquared then Some 0 else None
    