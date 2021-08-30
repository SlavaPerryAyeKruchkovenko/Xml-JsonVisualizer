namespace FileEncoder

open VisualizerLogic
open System.IO
open System.Diagnostics
open System
open System.Text

[<AbstractClass>]
type Command(drawer : IDrawer)  =
    abstract member Execute : unit -> unit

type HelpCommand(drawer : IDrawer) = // print help info
    inherit Command(drawer)
    override this.Execute() =
        drawer.PrintMessage "-h That command get help info"
        drawer.PrintMessage "-v That command get version of file in it has version"
        drawer.PrintMessage "-cd *newPath* Set new path"
        drawer.PrintMessage "-d Go down the path"
        drawer.PrintMessage "-p that command patse selected file"
        drawer.PrintMessage "-up *folder/file* that command open foleder/file in this directory"
     
type EmptyCommand(drawer : IDrawer) = 
    inherit Command(drawer)
    override this.Execute() =
        drawer.PrintMessage "Unknown or empty argument"

type CompleteCommand(drawer : IDrawer) = // command where have not error in command
    inherit Command(drawer)
    override this.Execute() =
        drawer.PrintMessage "Complete"

type VersionCommand(drawer : IDrawer, path) = // try get file version
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
            with
                _ -> ()

type ParseFileCommand(drawer : IDrawer , path) = //command to get parse txt json xml and print parsing text in frame
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
    let GetAllFilesInFolder(path) = // get alld Files and Folders in directory
        try
            if Directory.Exists(path) then
                let files = Directory.GetFiles(path)
                let folders = Directory.GetDirectories(path)
                let mutable directories =[]
                for file in files do
                    directories <- file :: directories
                for folder in folders do
                    directories <- folder :: directories
                directories
            else if File.Exists(path) then
                ["this is file haven't child files"]
            else
                raise (Exception("Incorrect Folder"))
        with
            | :? Exception as ex -> [ex.Message]

    let GetCommand(args:string[], path , drawer:IDrawer) = //getCommand and new path

        let OpenFolder(newPath) =
            if (File.Exists newPath || Directory.Exists newPath) then
                newPath
            else
                raise (Exception("Incorrect folder"))

        let  CloseFolder(path: string) = 
            let index = path.LastIndexOf("\\");
            if index >= 0 then
                path.Remove index
            else 
                raise (Exception("That is base directory"))

        if(args.Length = 1) then
            match args.[0] with
            | "-h" -> new HelpCommand(drawer) :> Command , path
            | "-help" -> new HelpCommand(drawer) :> Command , path
            | "-v" -> new VersionCommand(drawer , path) :> Command , path
            | "-version" -> new VersionCommand(drawer, path) :> Command , path
            | "-p" -> new ParseFileCommand(drawer, path) :> Command , path
            | "-parse" -> new ParseFileCommand(drawer, path) :> Command , path
            | "-down" -> new CompleteCommand(drawer) :> Command ,CloseFolder(path)
            | "-d" -> new CompleteCommand(drawer) :> Command ,CloseFolder(path)
            | _ -> new EmptyCommand(drawer) :> Command , path        
        else if(args.Length = 2) then
            match args.[0] with
            | "-open" -> new CompleteCommand(drawer) :> Command , OpenFolder(path+"\\"+args.[1])
            | "-up" -> new CompleteCommand(drawer) :> Command , OpenFolder(path+"\\"+args.[1])
            | "-cd" -> new CompleteCommand(drawer) :> Command , OpenFolder args.[1]
            | _ -> new EmptyCommand(drawer) :> Command , path 
        else
            new EmptyCommand(drawer) :> Command,path