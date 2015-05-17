
namespace Gema.Models

open System
open MathNet.Numerics.Random
open MathNet.Numerics.Distributions
open Gema

type SphereModel2(modelInputPars: Map<string, string>, simulationInfo: SimulationParameters, randomSeed: int) =
        member this.Radius = float modelInputPars.["Radius"]
        member this.RadiusSquared = this.Radius * this.Radius
        member this.RandomSeed = randomSeed
        member this.RandomGenerator = new MersenneTwister(this.RandomSeed, false)
        member this.StartPDF = new ContinuousUniform(-this.Radius, this.Radius, this.RandomGenerator)
        member this.StepPDF = new Normal(0.0, simulationInfo.StepSize, this.RandomGenerator)

        interface IGemaModel with
            member this.GetStartPosition (fromPoint : float array =
                let x = this.StartPDF.Sample()
                let y = this.StartPDF.Sample()
                let z = this.StartPDF.Sample()
                [|x ; y; z|]

            member this.CheckStartPosition (position:float array) =
                let distance = position |> Array.map (fun x -> x * x) |> Array.sum
                match distance < this.RadiusSquared with
                | true -> Some(0)
                | false -> None

            member this.GetDisplacement fromPoint =
                let dispX = this.StepPDF.Sample()
                let dispY = this.StepPDF.Sample()
                let dispZ = this.StepPDF.Sample()
                [| dispX; dispY; dispZ |]

            member this.CheckDisplacement fromPoint displacement =
                let newPosition = Array.map2 (fun x y -> x + y) fromPoint.Position displacement
                let newDistance = newPosition |> Array.map (fun x -> x * x) |> Array.sum
                match newDistance < this.RadiusSquared with
                | true -> Some(fromPoint.State)
                | false -> None
            