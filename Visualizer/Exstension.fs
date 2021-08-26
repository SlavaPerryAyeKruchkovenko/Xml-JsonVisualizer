

module Exstension 
    
open System.Linq
open System.Collections
 
type ListExtension = ListExtension with
    static member        (?<-) (ListExtension, a , b) = a @ b
    static member inline (?<-) (ListExtension, a , b) = a + b

let inline (+) a b = (?<-) ListExtension a b
