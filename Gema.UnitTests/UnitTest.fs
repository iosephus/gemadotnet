namespace Gema.UnitTests

open System
open Microsoft.VisualStudio.TestTools.UnitTesting
open Gema

[<TestClass>]
type UnitTest() = 
    [<TestMethod>]
    member x.RunGema () = 
        let returnValue = Main.main [||]
        Assert.AreEqual(0, returnValue)
