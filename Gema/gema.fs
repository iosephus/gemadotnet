
namespace Gema

module Main =

    open System
    open System.Diagnostics
    open MathNet.Numerics.Random
    open Gema.Models

    let rec GetStartPoint modelFunctions =
        let newPosition = modelFunctions.GetStartPosition
        match modelFunctions.CheckStartPosition newPosition with
        | Some(state) -> { Step = 0;  State = state; Position = newPosition; }
        | None -> GetStartPoint modelFunctions

    let rec GetNextPoint modelFunctions previousPoint =
        let displacement = modelFunctions.GetDisplacement previousPoint
        let newPosition = Array.map2 (fun x y -> x + y) previousPoint.Position displacement
        match modelFunctions.CheckDisplacement previousPoint displacement with
        | Some(state) -> { Step = previousPoint.Step + 1;  State = state; Position = newPosition; }
        | None -> GetNextPoint modelFunctions previousPoint
        

    let WalkOneParticle simulationInfo modelFunctions =
        let rec _walkOneParticle (acc:List<Point>) (prevPoint:Point) (stepsToDo:int) modelFunctions =
            match stepsToDo with
            | 0 -> acc
            | todo ->
                let newPoint = GetNextPoint modelFunctions
                let newAcc = match newPoint.Step % simulationInfo.StorageInterval with | 0 -> (List.append acc [newPoint]) | _ -> acc
                _walkOneParticle newAcc newPoint (todo - 1) modelPDFs
        let startPoint = GetStartPoint modelFunctions
        _walkOneParticle [startPoint] startPoint simulationInfo.NumberOfSteps modelFunctions

    let run simulationInfo =
        let modelParameters = SphereModel.CreateModelInfo(Map.ofList [("Radius", "1.0")])
        let modelPDFs = SphereModel.CreatePDFs simulationInfo modelParameters 93849329
        let sw = new Stopwatch()
        sw.Start()
        let results = WalkOneParticle simulationInfo modelPDFs
        sw.Stop()
        printfn "Execution time: %d ms" sw.ElapsedMilliseconds
        results

    let processCLIOptions argv =
        let numberOfParticles = 100000
        let numberOfSteps = 100000
        let storageInterval = 1000
        let stepSize = 1.0 / float numberOfSteps
        { NumberOfParticles = numberOfParticles;
          NumberOfSteps = numberOfSteps;
          StorageInterval = storageInterval;
          StepSize = stepSize}


    [<EntryPoint>]
    let main argv = 
        let SimulInfo = processCLIOptions argv
        let results = run SimulInfo
        printfn "This is the .NET implementation of Gema version 0.1"
        printfn "Running with NumberOfParticles = %d, NumberOfSteps = %d; StorageInterval = %d" SimulInfo.NumberOfParticles SimulInfo.NumberOfSteps SimulInfo.StorageInterval
        printfn "Results = %s" (results.ToString())
        0 // return an integer exit code
