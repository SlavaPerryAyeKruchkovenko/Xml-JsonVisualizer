namespace VisualizerLogic

open System
open System.IO
open System.Text
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open Exstension
open System.Linq

type IDrawer =
    abstract PrintError: message: Exception -> unit
    abstract PrintMessage: message: string -> unit
    abstract GetText:message: unit -> string
    abstract Clear:clear: unit -> unit    

[<AbstractClass>]    
type ProjectFile(path:string) =
    abstract member GetFields : unit -> string list

type JsonFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        use file = File.OpenText(path)           
        use reader = new JsonTextReader(file) :> JsonReader
        let jsObject =  (JToken.ReadFrom reader) :?> JObject
        
        let rec getText (jsObject: JObject) : string list = 
            let mutable text = []
            for field in jsObject.Properties() do 
                let name  = field.Name
                let list: string list = []
                if not (name.Contains "$type") then
                    text <- field.Name :: text
                    let value = field.Value.ToString()
                    if value.Contains "$type" then
                        text <-  text + (getText (field.Value:?> JObject))
                    else if value = "" then
                        text <- "null" :: text
                    else
                        text <- field.Value.ToString() :: text
            text

        getText jsObject   
    new () = JsonFile (Environment.CurrentDirectory + "\Save.json")

type XmlFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        []
    new () = XmlFile (Environment.CurrentDirectory + "\Save.xml")
        

