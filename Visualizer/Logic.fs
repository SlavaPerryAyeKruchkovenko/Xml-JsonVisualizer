namespace VisualizerLogic

open System
open System.IO
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open Exstension

type IDrawer =
    abstract PrintError: message: Exception -> unit
    abstract PrintMessage: message: string -> unit
    abstract GetText:message: unit -> string
    abstract Clear:clear: unit -> unit  
    abstract GetWindowHeight: unit -> int
    abstract GetWindowWeight: unit -> int
    abstract SetCursorCentr:textSize: int -> unit
    abstract ResetCursor: unit -> unit

[<AbstractClass>]    
type ProjectFile(path:string) =
    abstract member GetFields : unit -> string list

type JsonFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        use file = File.OpenText(path)           
        use reader = new JsonTextReader(file) :> JsonReader
        let jToken =  JToken.ReadFrom reader
        
        let rec getText (jsToken: JToken) : string list = 
            
            let parseJObject( jsObject : JObject) : string list =
                let mutable json = []
                for field in jsObject.Properties() do 

                    let name  = field.Name
                    json <- name :: json
                    let value = field.Value
                    if value.ToString() = "" then
                        json <- "null" :: json
                    else 
                        json <- getText value + json
                json

            let parseJArray( jsArray : JArray) : string list =
                let mutable json = []
                for _value in jsArray.Children() do 
                    json <- getText _value + json
                json
            
            let parseProperty( jsProperty : JProperty): string list = 
                let mutable json = []
                json <- jsProperty.Name :: json
                jsProperty.Value.ToString() :: json

            match jsToken with 
            | :? JObject as jobject -> parseJObject jobject    
            | :? JArray as jarray  -> parseJArray jarray
            | :? JProperty as jproperty -> parseProperty jproperty
            | _ -> [jsToken.ToString()]  
            
        getText jToken   
    new () = JsonFile (Environment.CurrentDirectory + "\Save.json")

type XmlFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        []
    new () = XmlFile (Environment.CurrentDirectory + "\Save.xml")
        

