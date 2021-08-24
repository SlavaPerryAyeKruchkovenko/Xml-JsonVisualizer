namespace VisualizerLogic

open System
open System.IO

type IDrawer =
    abstract PrintError: message: Exception -> unit
    abstract PrintMessage: message: string -> unit
    abstract GetText:message: unit -> string
    abstract Clear:clear: unit -> unit

type ProjectFile(drawer: IDrawer) = class
    let drawer = drawer   
    member this.getPath =   
        try
            drawer.Clear()
            drawer.PrintMessage "Please Enter File"
            let file = drawer.GetText()
            if File.Exists file then
                file.ToString(),true                  
            else
                "File not found",false
         with
         | :? Exception as ex -> ex.Message,false    
    member this.getFileExstension (path:string)=
        let json = ".json"
        let xml = ".xml"
        let corPath = path.ToLower()

        if path.Contains json then
            json.Replace(".",""),true
        else if path.Contains xml then
            xml.Replace("." ," "),true
        else 
            String.Empty,false
end
type JsonFile = class 

end
type XmlFile = class 
end

