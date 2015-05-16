
module Sphere

    open System
    open MathNet.Numerics.Random
    open MathNet.Numerics.Distributions

    type SphereInfo = { radius: float; radiusSquared: float; }
    type modelPDFs = { initPDF: IContinuousDistribution; stepPDF: IContinuousDistribution; randomGenerator: Random; }
    type SimulationInfo = { stepSize: float; }

    let CreateModelInfo modelPars =
        { radius = modelPars.radius; radiusSquared = modelPars.radius * modelPars.radius; }

    let createPDF simulationInfo modelInfo randomSeed =
        let randomGenerator = new MersenneTwister(randomSeed, false)
        let startPDF = new ContinuousUniform(-modelInfo.radius, modelInfo.radius, randomGenerator)
        let stepPDF = new Normal(0.0, simulationInfo.stepSize, randomGenerator)
        { initPDF = startPDF; stepPDF = stepPDF; randomGenerator = randomGenerator; }
