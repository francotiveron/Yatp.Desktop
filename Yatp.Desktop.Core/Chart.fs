module Yatp.Desktop.Chart

open Elmish.WPF

type Model = {
    Product: string
    N: int
}

type Msg = 
    | NewData of string

let init = {Product = "Design"; N = 0}

let update msg m = 
    match msg with
    | NewData product -> 
        Repo.data.Mdps.Add(Repo.data.Mdps.Count, obj()); 
        let d = Repo.getData m.Product
        let q = d.Mdps
        let n  = q.Count
        {m with N = n}

let mutable i = 0

let bindings() : Binding<Model, Msg> list = [
    "NewData" |> Binding.cmd (NewData "GCA@500")
    "Product" |> Binding.oneWay (fun m -> m.Product) 
    //"MdpsCount" |> Binding.oneWay (fun m -> (Repo.getData m.Product).Mdps.Count) ]
    //"MdpsCount" |> Binding.oneWay (fun m -> 
    //    let d = Repo.getData m.Product
    //    let q = d.Mdps
    //    let n  = q.Count
    //    n) 
    "MdpsCount" |> Binding.oneWay (fun m -> 
        let d = Repo.getData m.Product
        let q = d.Mdps
        let n  = q.Count
        i <- i + 1
        printfn "i = %d  n = %d" i n
        m.N) 
    ]

//let designVm = ViewModel.designInstance init (bindings())