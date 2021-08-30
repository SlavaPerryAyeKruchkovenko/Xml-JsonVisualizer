namespace FileEncoder

open VisualizerLogic
open System.IO
open System.Diagnostics
open System
open System.Text

[<AbstractClass>]
type Command(drawer : IDrawer)  =
    abstract member Execute : unit -> unit

type HelpCommand(drawer : IDrawer) = 
    inherit Command(drawer)
    override this.Execute() =
        drawer.PrintMessage "\t-h|--help\t\tHelp information"
        drawer.PrintMessage "\t-v|--version -v\t\tProgram version"
     
type EmptyCommand(drawer : IDrawer) =
    inherit Command(drawer)
    override this.Execute() =
        drawer.PrintMessage "Unknown or empty argument"

type VersionCommand(drawer : IDrawer, path) =
    inherit Command(drawer)
    override this.Execute() =
        let file = new FileInfo(path)
        let procesers = Process.GetProcesses()
        for proces in procesers do
            try
                if proces.MainModule.FileName = file.Name then
                    try 
                        drawer.PrintMessage proces.MainModule.FileVersionInfo.FileVersion
                    with
                        | :? Exception as ex -> drawer.PrintError ex
            finally
                drawer.PrintMessage "Find"

type ParseFileCommand(drawer : IDrawer , path) = 
    inherit Command(drawer)
    override this.Execute() =       

        let getFile (path:string)=
            let json = ".json"
            let xml = ".xml"
            let txt = ".txt"
            let corPath = path.ToLower()

            if corPath.Contains json then
                Some (new JsonFile(path) :> ProjectFile)
            else if corPath.Contains xml then
                Some (new XmlFile(path) :> ProjectFile)
            else if corPath.Contains txt then
                Some (new TxtFile(path) :> ProjectFile)
            else 
                None

        let GetStringBuilder(file: ProjectFile) = 
                file.GetFields()
        
        let printFile (file : StringBuilder, drawer: IDrawer)=               
            for value in (file.ToString().Split('\n') |> Array.filter(fun x -> not (x = "" || Char.IsControl x.[0]))) do 
                let height = drawer.GetWindowHeight()
                let weight = drawer.GetWindowWeight()
                drawer.PrintFrame(height,weight)
                drawer.SetCursorCentr value.Length
                drawer.PrintMessage value
                drawer.ResetCursor()
                drawer.Clear()
              
        let checkFile(file : ProjectFile option) = 
            if file.IsSome then
                GetStringBuilder(file.Value)
            else 
                new StringBuilder("Uncorrect File")

        let file = getFile path
        let textParser = checkFile file
        printFile(textParser,drawer)   

       
module MoveCommand =
    let OpenFolder(path : string, folderPath) =
        let newPath = path + "\\" + folderPath
        if(File.Exists newPath) then
            newPath
        else
            raise (Exception("Incorrect folder"))
    let CloseFolder(path: string) = 
        let index = path.LastIndexOf("\\");
        if index >= 0 then
            path.Remove index
        else 
            raise (Exception("That is base directory"))

    let GetAllFilesInFolder(path) =
        try
            if(Directory.Exists(path)) then
                let files = Directory.GetFiles(path)
                let folders = Directory.GetDirectories(path)
                let mutable directories =[]
                for file in files do
                    directories <- file :: directories
                for folder in folders do
                    directories <- folder :: directories
                directories
            else
                raise (Exception("Incorrect Folder"))
        with
            | :? Exception as ex -> [ex.Message]