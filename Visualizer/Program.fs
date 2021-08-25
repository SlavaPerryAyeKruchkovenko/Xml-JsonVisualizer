// Learn more about F# at http://fsharp.org

open System
open VisualizerLogic
open System.Threading
open System.IO
open Microsoft.FSharp.Core

type Printer() = 
    interface IDrawer with
       member this.Clear() = 
        Thread.Sleep(200) 
        Console.Clear()
       member this.GetText() = Console.ReadLine()
       member this.PrintMessage(message: string) = printfn "%s" message
       member this.PrintError(ex: Exception) = printfn "%s" ex.Message

    member this.printFrame height weight = 
        let printer = this :> IDrawer
        
        let printCells isLine size = 
            let point = "*"   
            let empty = " "
            let mutable line = ""
            if isLine then
                for i in  1..size do 
                    line <- line + point
            else
                line <- line + point
                for i in  2..size - 1 do
                    line <- line + empty
                line <- line + point
            line
        
        printer.Clear()
        
        for y in 0..height do
            printer.PrintMessage (printCells true weight)
        

(*let json = JObject.Parse(File.ReadAllText(fst path))*) 

[<EntryPoint>]
let main argv =       
    let mutable isPlay = true
    let printer: IDrawer = upcast new Printer()
    let getPath (prtiner: IDrawer) =   
        try
            let printer = Printer() :> IDrawer
            printer.Clear()
            printer.PrintMessage "Please Enter File"
            let file = printer.GetText()
            if File.Exists file then
                file.ToString(),true                  
            else
                "File not found",false
         with
         | :? Exception as ex -> ex.Message,false

    let getFile (path:string)=
        let json = ".json"
        let xml = ".xml"
        let corPath = path.ToLower()

        if corPath.Contains json then
            Some (new JsonFile(path) :> ProjectFile)
        else if corPath.Contains xml then
            Some (new XmlFile(path) :> ProjectFile)
        else 
            None

    let pathTuple = getPath printer    

    while isPlay do
        let path = fst pathTuple
        let havePath = snd pathTuple
        if havePath then 
            printer.PrintMessage ("Open " + path)
            isPlay <- false  
        else
            printer.PrintMessage path
            pathTuple = getPath printer |> ignore

    let fileTuple = getFile (fst pathTuple)
    let file = fileTuple.Value
    let haveFile = fileTuple.IsSome

    if haveFile then
        printer.PrintMessage (file.GetFields().ToString())
    else 
        printer.PrintMessage "this file can not be visualized"
    0
 
