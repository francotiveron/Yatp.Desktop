module Yatp.Desktop.Program

open System
open Serilog
open Elmish
open Elmish.WPF
open Serilog.Extensions.Logging
open System.Windows

//type Model = { Chart_: Chart.Model option }
type Model = { Charts: Map<string, Chart.Model> }

type Msg = 
    | OpenChart of product:string
    | ChartMsg of Chart.Msg
    | NewData of string

//let init() = { Chart_ = None }
let init() = { Charts = Map.empty }

let update msg m = 
    match msg with
    //| OpenChart product -> { m with Chart_ = Some {Product = product} }
    | OpenChart product -> 
        Repo.addProduct product
        { m with Charts = m.Charts.Add(product, {Product = product}) }
    | ChartMsg (Chart.NewData product) -> Chart.update (Chart.NewData product) m.Charts[product]; m

let bindings (chartBuilder: unit -> #Window) () : Binding<Model, Msg> list = [
    "OpenChart" |> Binding.cmdParam (string >> OpenChart)
    "ChartWindow" |> Binding.subModelWin(
        //(fun m -> WindowState.ofOption m.Chart_),
        (fun m -> WindowState.ofOption (m.Charts |> Map.tryFind "GCA@500")),
        snd,
        ChartMsg,
        Chart.bindings,
        chartBuilder) ]

let newData dispatch = Repo.subscribe (fun product -> dispatch (NewData product) )

let main mainWindow (chartBuilder: Func<#Window>) =
    let logger =
        LoggerConfiguration()
            .MinimumLevel.Override("Elmish.WPF.Update", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Bindings", Events.LogEventLevel.Verbose)
            .MinimumLevel.Override("Elmish.WPF.Performance", Events.LogEventLevel.Verbose)
            .WriteTo.Console()
            .CreateLogger()

    WpfProgram.mkSimple init update (bindings (fun() -> chartBuilder.Invoke()))
    |> WpfProgram.withSubscription(fun _ -> Cmd.ofSub newData)
    |> WpfProgram.withLogger (new SerilogLoggerFactory(logger))
    |> WpfProgram.startElmishLoop mainWindow
