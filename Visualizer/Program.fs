// Learn more about F# at http://fsharp.org

open System
open VisualizerLogic
open System.Threading
open System.IO
open Microsoft.FSharp.Core
open Exstension

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

    member this.printList (list: 'T list) = 
        let reverseLists = list |> List.rev
        for text in reverseLists do
            printfn "%s" (text.ToString())
        

(*let json = JObject.Parse(File.ReadAllText(fst path))*) 

[<EntryPoint>]
let main argv =           
    let drawer: IDrawer = upcast new Printer()

    let getPath (prtiner: IDrawer) =   
        try
            prtiner.Clear()
            prtiner.PrintMessage "Please Enter File"
            let file = prtiner.GetText()
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

    let mutable pathTuple = getPath drawer 
    let havePath() = snd pathTuple
    let path() = fst pathTuple

    while not (havePath()) do
         drawer.PrintMessage (path())
         pathTuple <- getPath drawer
    
    drawer.Clear()
    let print = drawer.PrintMessage ("Open " + path())

    let fileTuple = getFile (fst pathTuple)
    let file = fileTuple.Value
    let haveFile = fileTuple.IsSome

    let printer = drawer :?> Printer
    if haveFile then
        printer.printList (file.GetFields())
    else 
        drawer.PrintMessage "this file can not be visualized"
    0
 
  