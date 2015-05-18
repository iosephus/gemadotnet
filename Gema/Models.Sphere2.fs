
namespace Gema.Models

//open System
open MathNet.Numerics.Random
open MathNet.Numerics.Distributions
open Gema.Common

type SphereModel2(modelInputPars: Map<string, string>, simulationInfo: SimulationParameters, randomSeed: int) =
        let radius = float modelInputPars.["Radius"]
        let randomGenerator = MersenneTwister(randomSeed)
        member this.RandomSeed = randomSeed
        member this.RandomGenerator = randomGenerator
        member this.Radius = radius
        member this.RadiusSquared = radius * radius
        member this.StartPDF = ContinuousUniform(-radius, radius, randomGenerator)
        member this.StepPDF = Normal(0.0, simulationInfo.StepSize, randomGenerator)

        interface IGemaModel with

            member this.GetStartPosition () =
                let x = this.StartPDF.Sample()
                let y = this.StartPDF.Sample()
                let z = this.StartPDF.Sample()
                [| x; y; z |]

            member this.CheckStartPosition position =
                let distance = position |> Array.map (fun x -> x * x) |> Array.sum
                if distance < this.RadiusSquared then Some 0 else None

            member this.GetDisplacement fromPoint =
                let dispX = this.StepPDF.Sample()
                let dispY = this.StepPDF.Sample()
                let dispZ = this.StepPDF.Sample()
                [| dispX; dispY; dispZ |]

            member this.CheckDisplacement fromPoint displacement =
                let newPosition = Array.map2 (fun x y -> x + y) fromPoint.Position displacement
                let newDistance = newPosition |> Array.map (fun x -> x * x) |> Array.sum
                if newDistance < this.RadiusSquared then Some 0 else None
            