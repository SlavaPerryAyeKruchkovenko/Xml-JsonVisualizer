// Learn more about F# at http://fsharp.org

open System
open VisualizerLogic
open System.Threading
open System.IO
open Microsoft.FSharp.Core
open Exstension
open System.Text
open System.Linq
open System.Threading.Tasks

type Printer() = 
    interface IDrawer with             
       member this.Clear() = 
        let clear() = async {
            let! sleep = Task.Delay(200) |> Async.AwaitTask
            Console.Clear()
        } 
        clear() |> Async.RunSynchronously
       member this.GetText() = Console.ReadLine()
       member this.PrintMessage(message: string) = printfn "%s" message
       member this.PrintError(ex: Exception) = printfn "%s" ex.Message
       member this.GetWindowHeight(): int = Console.WindowHeight
       member this.GetWindowWeight(): int = Console.WindowWidth
       member this.SetCursorCentr(textSize:int): unit =  

        let halfSize = textSize/2
        let halfWindow = (Console.WindowWidth/2,Console.WindowHeight/2)
        if fst halfWindow - halfSize > 0 then
            Console.SetCursorPosition (((fst halfWindow) - halfSize),(snd halfWindow))
        else
            Console.SetCursorPosition (2,(snd halfWindow))

       member this.ResetCursor(): unit = Console.SetCursorPosition(0,0) 

    member this.printFrame height weight = 
        let printer = this :> IDrawer

        let printCells isLine size = 

            let addPoint(line : StringBuilder , times) = line.Append ("*" +* times)  
            let addEmpty(line : StringBuilder , times) = line.Append (" " +* times)

            let mutable line = new StringBuilder(String.Empty)
            if isLine then 
               addPoint(line,size)
            else
                line <- addPoint(line,2)
                line <- addEmpty(line,(size-4))
                line <- addPoint(line,2)
                line       
        printer.Clear()
        
        let print (printer : IDrawer) = 

            let printLine() = printer.PrintMessage ((printCells true weight).ToString())
            let printOnSides() = printer.PrintMessage ((printCells false weight).ToString())

            printLine()
            for y in 2..height-1 do
                printOnSides()
            printLine()

        print printer

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
       
    let print = 
        drawer.PrintMessage ("Open " + path())
        drawer.Clear()

    let GetList(haveFile , file: ProjectFile) = 
        if haveFile then
            file.GetFields()
        else 
            ["this file can not be visualized"]
    
    let printFile (list : string list, drawer: IDrawer)= 
        let printer = drawer :?> Printer              
        for value in list do 
            let height = drawer.GetWindowHeight()
            let weight = drawer.GetWindowWeight()
            printer.printFrame height weight
            drawer.SetCursorCentr value.Length
            drawer.PrintMessage value
            drawer.ResetCursor()
            drawer.Clear()

    let fileTuple = getFile (fst pathTuple)
    let file = fileTuple.Value
    let haveFile = fileTuple.IsSome    
    
    let list = GetList(haveFile,file) |> List.rev
    printFile(list,drawer)
    0
 
  