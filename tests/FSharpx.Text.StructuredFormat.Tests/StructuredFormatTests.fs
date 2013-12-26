namespace FSharpx.PowerPack.Unittests

open NUnit.Framework
open Microsoft.FSharp.Quotations
open Microsoft.FSharp.Text.StructuredFormat
open System
open FsUnit

module StructuredFormat_MiscBitsAndPiecesToCompiler = 
    let opts = { FormatOptions.Default with 
                   PrintWidth=30; 
                   ShowIEnumerable=true;
                   ShowProperties=true }

[<TestFixture>]
type ``StructuredFormat Tests``() = 

    [<Test>]
    let t1 =
        let pprint = Display.layout_as_string
                        { FormatOptions.Default
                            with PrintDepth = 10
                                 PrintLength = 11
                                 PrintSize = 12 }
        let result = (pprint [1..10])
        printfn "%s" result
        result |> should equal "[1; 2; 3; 4; 5; 6; 7; 8; 9; 10]"

    [<Test>]
    let t2 =
        let pprint = Display.layout_as_string
                        { FormatOptions.Default
                            with PrintDepth = Int32.MaxValue
                                 PrintLength = Int32.MaxValue
                                 PrintSize = Int32.MaxValue }
        let result = (pprint [5..10000])
        printfn "%s" result
