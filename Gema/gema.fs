
type Config = { NumberOfParticles: int; NumberOfSteps: int; }
type Point = { Step: int; State: int; Position: float array; }

let run config =
    { Step = 0;  State = 0; Position = [|0.0; 0.0; 0.0|]; }

[<EntryPoint>]
let main argv = 
    let Options = { NumberOfParticles = 1000; NumberOfSteps = 1000; }
    let results = run Options
    printfn "This is the .NET implementation of Gema version 0.1"
    printfn "Running with NumberOfParticles = %d, NumberOfSteps = %d" Options.NumberOfParticles Options.NumberOfSteps
    printfn "Results = { Step = %d; State = %d; Position = [|%f; %f, %f|]; }" results.Step results.State results.Position.[0] results.Position.[1] results.Position.[2]
    0 // return an integer exit code
