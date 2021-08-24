﻿// Learn more about F# at http://fsharp.org

open System
open VisualizerLogic
open System.Threading
open Newtonsoft.Json
open Newtonsoft.Json
open Newtonsoft.Json.Linq
open System.IO

type Printer() = 
    interface IDrawer with
       member this.Clear() = 
        Thread.Sleep(200) 
        Console.Clear()
       member this.GetText() = Console.ReadLine()
       member this.PrintMessage(message: string) = printfn "%s" message
       member this.PrintError(ex: Exception) = printfn "%s" ex.Message

let cells isLine size = 
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
(*let json = JObject.Parse(File.ReadAllText(fst path))*) 
let printFrame height weight = 
    Console.Clear()
    for y in 0..height do
        Console.WriteLine(cells true weight);
use json path = 
    let reader = new JsonTextReader (File.OpenText path)
    JObject (JToken.ReadFrom reader);
[<EntryPoint>]
let main argv =       
    let file = new ProjectFile(new Printer())
    let mutable isPlay = true
    let path = file.getPath
    while isPlay do
        if snd path then 
            printf "%s" ("Open " + fst path)
            isPlay <- false              
    0
 
