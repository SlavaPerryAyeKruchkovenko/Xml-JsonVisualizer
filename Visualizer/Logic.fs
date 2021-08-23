namespace VisualizerLogic

open System
open System.IO

type IDrawer =
    abstract PrintError: message: Exception -> unit
    abstract PrintMessage: message: string -> unit
    abstract GetText:message: unit -> string
    abstract Clear:clear: unit -> unit

type ProjectFile(drawer: IDrawer ) = class
    let drawer = drawer   
    member this.getPath =   
        let mutable path = ""
        let mutable havePath = false
        while not havePath do
            try
                drawer.Clear()
                drawer.PrintMessage "Please Enter File"
                let file = drawer.GetText()
                if File.Exists file then
                    havePath <- true
                    path <- file.ToString()
                    drawer.PrintMessage "File found"
                else
                    failwith "File not found"
             with
             | :? Exception as ex -> drawer.PrintError ex
        path
end


