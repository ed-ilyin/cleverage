module Cleverage.Client.Eye
open Microsoft.AspNetCore.SignalR.Client
open Elmish
open System.Threading.Tasks
open Bolero.Html
open Cleverage.Shared
open Microsoft.Extensions.Logging
open Thoth.Json.Net
open Cleverage.Helpers

type Message = Update of update: Result<Update, string> * json: string
type Room = Map<uint64, Result<Update, string> * string>
type Model = Map<string, Room>

let init : Model = Map.ofList []

let update message (model: Model) : Model =
    match message with
    | Update (Ok update, json) ->
        match Map.tryFind update.Chat model with
        | Some (room: Room) -> room | None ->  Map.empty
        |> Map.add update.MessageId (Ok update, json)
        |> Map.add update.Chat
        <| model
    | Update (Error error, json) ->
        match Map.tryFind "" model with
        | Some (room: Room) -> room | None ->  Map.empty
        |> Map.add 0UL (Error error, json)
        |> Map.add ""
        <| model

let log tag a =
    printfn "%s: %+A" tag a
    a

let start (task: Task) = task.Start ()

let decode dispatch json =
    let decodeResult =
        Decode.Auto.fromString<Result<Update, string> * string>
            (json, PascalCase, Extra.withUInt64 Extra.empty)
    match decodeResult with
    | Ok (Ok update, json) -> Ok update, json
    | Ok (Error error, json) -> Error error, json
    | Error error -> Error json, ""
    |> Update
    |> dispatch

let sub (loggerProvider: ILoggerProvider) (dispatch: _ -> unit) =
    let hubConnection =
        HubConnectionBuilder()
            .WithUrl("https://cleverage-api.azurewebsites.net/api")
            .WithAutomaticReconnect()
            .ConfigureLogging(fun logging ->
                logging.AddProvider(loggerProvider) |> ignore
            )
            .Build()

    hubConnection.On<string> ("NewMessage", decode dispatch)
    |> ignore

    hubConnection.StartAsync () |> start

let subscription loggerProvider model = Cmd.ofSub <| sub loggerProvider

// VIEW

let d c = div [ attr.``class`` c ]

let message _ (message: Result<Update, string>, json: string) =
    div [ attr.title json ] [
        match message with
        | Ok m ->
            let t = m.Text |> List.map (fun t -> div [] [ text t ]) |> div []
            if m.From = "" then t else d "columns is-mobile" [
                d "column is-narrow" [ textf "%s ??? " m.From ]
                d "column" [ t ]
            ]
        | Error e -> d "is-danger" [ text e ]
    ]

let room name (room: Room) =
    d "column" [ d "card" [
        d "card-header" [ d "card-header-title" [ text name ] ]
        Map.map message room
        |> Map.toList
        |> List.map snd
        |> d "card-content"
    ] ]

let view (model: Model) =
    model
    |> Map.map room
    |> Map.toList
    |> List.map snd
    |> d "columns is-multiline content is-small"
