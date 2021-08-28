namespace VisualizerLogic

open System
open System.IO
open Newtonsoft.Json.Linq
open Newtonsoft.Json
open Exstension
open System.Xml.Linq
open System.Xml
open System.Text

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
    abstract member GetFields : unit -> StringBuilder

type JsonFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        use file = File.OpenText(path)           
        use reader = new JsonTextReader(file) :> JsonReader
        let jToken =  JToken.ReadFrom reader
        
        let rec getText (jsToken: JToken) = 
            
            let parseJObject( jsObject : JObject) =
                let mutable json = new StringBuilder()
                for field in jsObject.Properties() do 

                    let name  = field.Name
                    json <- json.AppendLine name
                    let value = field.Value
                    if value.ToString() = "" then
                        json <- json.AppendLine "null"
                    else 
                        json <- json.AppendLine((getText value).ToString())
                json

            let parseJArray( jsArray : JArray) =
                let mutable json = new StringBuilder()
                for _value in jsArray.Children() do 
                    json <- json.Append((getText _value).ToString())
                json
            
            let parseProperty( jsProperty : JProperty) = 
                let mutable json = new StringBuilder()
                json <- json.AppendLine jsProperty.Name
                json.AppendLine(jsProperty.Value.ToString())

            match jsToken with 
            | :? JObject as jobject -> parseJObject jobject    
            | :? JArray as jarray  -> parseJArray jarray
            | :? JProperty as jproperty -> parseProperty jproperty
            | _ -> new StringBuilder(jsToken.ToString()) 
            
        getText jToken   
    new () = JsonFile (Environment.CurrentDirectory + "\Save.json")

type XmlFile(path) = 
    inherit ProjectFile(path)
    override this.GetFields() =
        use file = File.OpenText path           
        use reader =   
            XmlReader.Create(file)
        let node = XDocument.Load reader
        let rec getText (node : XNode) = 

            let parseXComment(comment: XComment) =  
                let mutable xml = new StringBuilder()
                xml<- xml.AppendLine "Comment"
                xml.AppendLine(comment.Value.ToString())

            let parseContainer(container: XContainer) = 
                let mutable xml = new StringBuilder()
                for element in container.Nodes() do  
                    if not (element = null || element.ToString() = "") then
                        let array = element.ToString().Split('>')
                        if array.Length > 2 then
                            xml <- xml.AppendLine(array.[0].Substring(1))
                            xml <- xml.AppendLine(getText(element).ToString())                      
                        else    
                            xml <- xml.AppendLine(getText(element).ToString())  
                xml

            let parseXDocumentType (document: XDocumentType) = 
                let mutable xml = new StringBuilder()
                xml <- xml.AppendLine document.Name
                xml <- xml.AppendLine document.PublicId
                xml.AppendLine document.SystemId
            
            let parseXText (text : XText) = 
                    new StringBuilder(text.Value)    
            
            let parseXProcessingInstruction (instruction : XProcessingInstruction) = 
                new StringBuilder(instruction.Data)
            
            match node with
            | :? XComment as comment -> parseXComment comment    
            | :? XContainer as container  -> parseContainer container
            | :? XDocumentType as documentType -> parseXDocumentType documentType
            | :? XText as text  -> parseXText text
            | :? XProcessingInstruction as instruction -> parseXProcessingInstruction instruction
            | _ -> new StringBuilder(node.ToString())
        getText node 
    new () = XmlFile (Environment.CurrentDirectory + "\Save.xml")
        

