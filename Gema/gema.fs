﻿
open System
open System.Diagnostics
open MathNet.Numerics.Random

type Config = { NumberOfParticles: int; NumberOfSteps: int; StorageInterval: int; }
type Point = { Step: int; State: int; Position: float array; }


let WalkOneParticle (options:Config) (randomGenerator:MersenneTwister) =
    let rec _walkOneParticle (acc:List<Point>) (prevPoint:Point) (stepsToDo:int) (randomGenerator:MersenneTwister) =
        if stepsToDo = 0 then
            acc
        else
            let newX = prevPoint.Position.[0] + randomGenerator.NextDouble()
            let newY = prevPoint.Position.[1] + randomGenerator.NextDouble()
            let newZ = prevPoint.Position.[2] + randomGenerator.NextDouble()
            let newPoint = { Step = prevPoint.Step + 1;  State = 0; Position = [|newX; newY; newZ|]; }
            let newAcc = match newPoint.Step % options.StorageInterval with | 0 -> (List.append acc [newPoint]) | _ -> acc
            _walkOneParticle newAcc newPoint (stepsToDo - 1) randomGenerator
    let initPoint = { Step = 0;  State = 0; Position = [|0.0; 0.0; 0.0|]; }
    _walkOneParticle [initPoint] initPoint options.NumberOfSteps randomGenerator

let run config =
    let randomGenerator = new MersenneTwister(93849329 )
    let sw = new Stopwatch()
    sw.Start()
    let results = WalkOneParticle config randomGenerator
    sw.Stop()
    printfn "Execution time: %d ms" sw.ElapsedMilliseconds
    results

[<EntryPoint>]
let main argv = 
    let Options = { NumberOfParticles = 10000; NumberOfSteps = 100000000; StorageInterval = 100000000; }
    let results = run Options
    printfn "This is the .NET implementation of Gema version 0.1"
    printfn "Running with NumberOfParticles = %d, NumberOfSteps = %d; StorageInterval = %d" Options.NumberOfParticles Options.NumberOfSteps Options.StorageInterval
    printfn "Results = %s" (results.ToString())
    0 // return an integer exit code
