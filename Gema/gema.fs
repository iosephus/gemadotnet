
namespace Gema

module Main =

    open System
    open System.Diagnostics
    open MathNet.Numerics.Random
    open Gema.Models

    let rec GetStartPoint startPosFunction checkStartPosFunction =
        let newPosition = startPosFunction()
        match checkStartPosFunction newPosition with
        | Some(state) -> { Step = 0;  State = state; Position = newPosition; }
        | None -> GetStartPoint startPosFunction checkStartPosFunction 

    let rec GetNextPoint getDispFunction checkDispFunction previousPoint =
        let displacement = getDispFunction previousPoint
        let newPosition = Array.map2 (fun x y -> x + y) previousPoint.Position displacement
        match checkDispFunction previousPoint displacement with
        | Some(state) -> { Step = previousPoint.Step + 1;  State = state; Position = newPosition; }
        | None -> GetNextPoint getDispFunction checkDispFunction previousPoint
        

    let WalkOneParticle numberOfSteps storageInterval (modelFunctions: ModelFunctions) =
        let rec _walkOneParticle (acc:List<Point>) (previousPoint:Point) (stepsToDo:int) (modelFunctions: ModelFunctions) =
            match stepsToDo with
            | 0 -> acc
            | todo ->
                let newPoint = GetNextPoint modelFunctions.GetDisplacement modelFunctions.CheckDisplacement previousPoint
                let newAcc = match newPoint.Step % storageInterval with | 0 -> (List.append acc [newPoint]) | _ -> acc
                _walkOneParticle newAcc newPoint (todo - 1) modelFunctions
        let startPoint = GetStartPoint modelFunctions.GetStartPosition modelFunctions.CheckStartPosition
        _walkOneParticle [startPoint] startPoint numberOfSteps modelFunctions

    let run simulationInfo =
        let modelInputParameters = Map.ofList [("Radius", "1.0")]
        let model = new SphereModel2(modelInputParameters, simulationInfo, 93849329)
        let modelFunctions = { GetStartPosition = model.GetStartPosition;
                               CheckStartPosition = model.CheckStartPosition;
                               GetDisplacement = model.GetDisplacement;
                               CheckDisplacement = model.CheckDisplacement; }
        let sw = new Stopwatch()
        sw.Start()
        let results = WalkOneParticle simulationInfo.NumberOfSteps simulationInfo.StorageInterval modelFunctions
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
