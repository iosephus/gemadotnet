
namespace Gema.Models

module Sphere =

    open System
    open MathNet.Numerics.Random
    open MathNet.Numerics.Distributions
    open Gema

    type SphereInfo = { radius: float; radiusSquared: float }

    let CreateModelInfo (modelPars:ModelPars) =
        { radius = modelPars.r; radiusSquared = modelPars.r* modelPars.r; }

    let createPDFs (simulationInfo:SimulationInfo) modelInfo randomSeed =
        let randomGenerator = new MersenneTwister(randomSeed, false)
        let startPDF = new ContinuousUniform(-modelInfo.radius, modelInfo.radius, randomGenerator)
        let stepPDF = new Normal(0.0, simulationInfo.StepSize, randomGenerator)
        { startPDF = startPDF; stepPDF = stepPDF; randomGenerator = randomGenerator; }
