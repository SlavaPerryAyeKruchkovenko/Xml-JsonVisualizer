

module Exstension 
    
open System.Linq
open System.Collections
open System
 
type ListExtension = ListExtension with
    static member        (?<-) (ListExtension, a , b) = a @ b
    static member inline (?<-) (ListExtension, a , b) = a + b

let inline (+) a b = (?<-) ListExtension a b

type StringExtension = StringExtension with 
    static member (?<-) 
        (StringExtension, a: string , b: int) = String.Concat(Enumerable.Repeat(a,b))
    static member inline (?<-) 
        (StringExtension, a, b): string = b +* a
         
let inline (+*) a b = (?<-) StringExtension a b
