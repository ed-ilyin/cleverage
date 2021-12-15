module Cleverage.Client.Eye
open Microsoft.AspNetCore.SignalR.Client
open Elmish
open System.Threading.Tasks
open Bolero.Html
open Cleverage
open Microsoft.Extensions.Logging
open Thoth.Json.Net
open Cleverage.Helpers

type Message = AddMessage of item: Result<Message * string, string>
type Model = Result<Message * string, string> list

let init = []

let update message (model: Model) =
    match message with
    | AddMessage msg -> msg :: model

let log tag a =
    printfn "%s: %+A" tag a
    a

let start (task: Task) = task.Start ()

let decode dispatch json =
    Decode.Auto.fromString<Result<Message * string, string>> json
    |> Result.join
    |> log "m"
    |> AddMessage
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

let item result =
    li [ attr.title result ] [
        match i with
        | Error e -> d "is-danger" [ text e ]
        | Ok (m: Shared.Message) -> sprintf "%+A" i |> text
    ]

let view (model: Model) = List.map item model |> ol []
