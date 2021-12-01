module Cleverage.Client.Eye
open Microsoft.AspNetCore.SignalR.Client
open Elmish
open System.Threading.Tasks
open Bolero.Html
open Cleverage


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

let sub (dispatch: _ -> unit) =
    let hubConnection =
        HubConnectionBuilder()
            .WithUrl("https://cleverage-api.azurewebsites.net/api")
            .WithAutomaticReconnect()
            .Build()

    hubConnection.On<Shared.Item> (
        "NewMessage",
        log "m" >> AddMessage >> dispatch)
    |> log "on"
    |> ignore

    hubConnection.StartAsync () |> start

let subscription model = Cmd.ofSub sub

// VIEW

let d c = div [ attr.``class`` c ]

let item (i, json) =
    li [ attr.title json ] [
        match i with
        | Error e -> d "is-danger" [ text e ]
        | Ok (m: Shared.Message) -> sprintf "%+A" i |> text
    ]

let view (model: Model) = List.map item model |> ol []
