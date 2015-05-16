
namespace Gema

module Main =

    open System
    open System.Diagnostics
    open MathNet.Numerics.Random

    open Gema.Models

    let GetStartPpoint (options:SimulationInfo) (modelPDFs:ModelPDFs) =
        let startX = modelPDFs.startPDF.Sample()
        let startY = modelPDFs.startPDF.Sample()
        let startZ = modelPDFs.startPDF.Sample()
        { Step = 0;  State = 0; Position = [|startX; startY; startZ|]; }

    let GetNextPoint (options:SimulationInfo) (prevPoint:Point) (modelPDFs:ModelPDFs) =
        let newX = prevPoint.Position.[0] + modelPDFs.stepPDF.Sample()
        let newY = prevPoint.Position.[1] + modelPDFs.stepPDF.Sample()
        let newZ = prevPoint.Position.[2] + modelPDFs.stepPDF.Sample()
        { Step = prevPoint.Step + 1;  State = 0; Position = [|newX; newY; newZ|]; }

    let WalkOneParticle (simulationInfo:SimulationInfo) modelPDFs =
        let rec _walkOneParticle (acc:List<Point>) (prevPoint:Point) (stepsToDo:int) modelPDFs =
            match stepsToDo with
            | 0 -> acc
            | todo ->
                let newPoint = GetNextPoint simulationInfo prevPoint modelPDFs
                let newAcc = match newPoint.Step % simulationInfo.StorageInterval with | 0 -> (List.append acc [newPoint]) | _ -> acc
                _walkOneParticle newAcc newPoint (todo - 1) modelPDFs
        let startPoint = GetStartPpoint simulationInfo modelPDFs
        _walkOneParticle [startPoint] startPoint simulationInfo.NumberOfSteps modelPDFs

    let run simulationInfo =
        let modelPars = { r = 1.0; }
        let modelInfo = Sphere.CreateModelInfo modelPars
        let modelPDFs = Sphere.createPDFs simulationInfo modelInfo 93849329
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
