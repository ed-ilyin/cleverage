module Cleverage.Client.Eye
open Microsoft.AspNetCore.SignalR.Client
open Elmish
open System.Threading.Tasks
open Bolero.Html
open Cleverage
open Microsoft.Extensions.Logging
open Thoth.Json.Net

type Message = AddMessage of item: Shared.Item
type Model = Shared.Item list

let init = []

let update message (model: Model) =
    match message with
    | AddMessage msg -> msg :: model

let log tag a =
    printfn "%s: %+A" tag a
    a

let start (task: Task) = task.Start ()

let resultJoin result =
    match result with
    Ok (Ok a) -> Ok a | Ok (Error e) -> Error e | Error e -> Error e

let decode dispatch json =
    match Decode.Auto.fromString<Shared.Item> json with
    | Ok (Ok r, tele) -> Ok r, tele
    | Ok (Error e, tele) -> Error e, tele
    | Error e -> Error e

    >> log "m"
    >> resultJoin
    >> AddMessage
    >> dispatch

let sub (loggerProvider: ILoggerProvider) (dispatch: _ -> unit) =
    let hubConnection =
        HubConnectionBuilder()
            .WithUrl("https://cleverage-api.azurewebsites.net/api")
            .WithAutomaticReconnect()
            .ConfigureLogging(fun logging ->
                logging.AddProvider(loggerProvider) |> ignore
            )
            .Build()

    hubConnection.On<string,string> ("NewMessage",
    )
    |> ignore

    hubConnection.StartAsync () |> start

let subscription loggerProvider model = Cmd.ofSub <| sub loggerProvider

// VIEW

let d c = div [ attr.``class`` c ]

let item (i, json) =
    li [ attr.title json ] [
        match i with
        | Error e -> d "is-danger" [ text e ]
        | Ok (m: Shared.Message) -> sprintf "%+A" i |> text
    ]

let view (model: Model) = List.map item model |> ol []
