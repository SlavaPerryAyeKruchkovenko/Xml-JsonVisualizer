namespace VisualizerLogic

open System
open System.IO
open System.Text
open Newtonsoft.Json.Linq
open Newtonsoft.Json

type IDrawer =
    abstract PrintError: message: Exception -> unit
    abstract PrintMessage: message: string -> unit
    abstract GetText:message: unit -> string
    abstract Clear:clear: unit -> unit    

[<AbstractClass>]    
type ProjectFile(path:string) =
    abstract member GetFields : unit -> StringBuilder

type JsonFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        let text = new StringBuilder(path)
        use file = File.OpenText(path)           
        use reader = new JsonTextReader(file) :> JsonReader
        let jsObject = JObject (JToken.ReadFrom reader)
        for field in jsObject.Properties() do 
            text.Append field.Name |> ignore
            text.Append field.Value |> ignore
        text
    new () = JsonFile (Environment.CurrentDirectory + "\Save.json")

type XmlFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        new StringBuilder()
        

