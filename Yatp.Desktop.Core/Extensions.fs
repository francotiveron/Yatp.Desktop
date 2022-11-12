module Extensions

open YatpHawaiiDotnet
open System.Net.Http
open System
open Yatp
open YatpHawaiiDotnet.Types
open System.Collections.Generic

type YatpClient(url: string) = 
    inherit YatpHawaiiDotnetClient(new HttpClient(BaseAddress = Uri(url)))
    override __.ToString() = url

type Dto.Mdp with
    static member FromApi(mdp: Mdp) = Dto.Mdp(
        Indicators = (mdp.Indicators |> Map.toSeq |> Seq.map (fun (sId, value) -> Dto.IndicatorId.Decode(sId), Nullable(value)) |> dict |> Dictionary)
        , Cards = (mdp.Cards |> Map.toSeq |> Seq.map (fun (sId, value) -> (Enum.Parse(typeof<Dto.CardId>, sId) :?> Dto.CardId, Dto.CardValue(value.Kind |> byte |> LanguagePrimitives.EnumOfValue, value.LevelOrSignOrMul25))) |> dict |> Dictionary)
        , Utc = mdp.Utc
        , Volume = mdp.Volume
        , Open = mdp.Open
        , High = mdp.High
        , Low = mdp.Low
        , Close = mdp.Close
        , Wave = (mdp.Wave |> Option.toNullable))

type Dto.Session with
    static member FromApi(session: Session) = Dto.Session(
        GridFracs = (session.GridFracs |> Map.toSeq |> Seq.map (fun (sId, value) -> Dto.IndicatorId.Decode(sId), Nullable(value)) |> dict |> Dictionary)
        , Episode = (session.Episode |> byte |> LanguagePrimitives.EnumOfValue)
        , Id = session.Id
        , MdpCount = session.MdpCount
        , StartUtc = session.StartUtc
        , Open = session.Open
        , High = session.High
        , Low = session.Low
        , Close = session.Close
        , ATR = session.ATR)

type Dto.Trade with
    static member FromApi(trade: Trade) = Dto.Trade(
        EntryUtc = trade.EntryUtc
        , EntryGrid = (sbyte)trade.EntryGrid
        , ExitUtc = (trade.ExitUtc |> Option.toNullable)
        , ExitGrid = (trade.ExitGrid |> Option.map sbyte |> Option.toNullable) )

type Dto.YatpData with
    member this.Add(mdps) = this.Mdps.Add(mdps |> List.map Dto.Mdp.FromApi)

    member this.Add(sessions) = 
        sessions
        |> Seq.map Dto.Session.FromApi
        |> Seq.groupBy (fun s -> s.Episode)
        |> Seq.iter (fun (episode, sessions) -> this.Sessions[episode].Add(sessions))
    
    member this.Add(trades) = this.Trades.Add(trades |> List.map Dto.Trade.FromApi)

    member this.Add(chartData: ChartData) = 
        this.Add(chartData.Mdps)
        this.Add(chartData.Sessions)
        this.Add(chartData.Trades)