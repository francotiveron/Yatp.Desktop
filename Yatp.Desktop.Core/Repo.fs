module internal Repo

open System
open System.Collections.Generic

module Option = 
    let apply a f_ = match f_ with Some f -> f a | None -> ()

[<AutoOpen>]
module private Internals =
    let mutable newDataSubscription_ : (string -> unit) option = None

    let newData product = newDataSubscription_ |> Option.apply product



let data = {| Mdps = SortedDictionary<int, obj>() |}

let subscribe f = newDataSubscription_ <- Some f

let addProduct product = ()

let getData product = data
