module Cleverage.Client.Eye
open Microsoft.AspNetCore.SignalR.Client
open Elmish
open System.Threading.Tasks
open Bolero.Html

type Model = string list
type Message = AddMessage of string

let init = []

let update message model =
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

    hubConnection.On<string> ("NewMessage", AddMessage >> dispatch) |> ignore
    hubConnection.StartAsync () |> start

let subscription model = Cmd.ofSub sub

let view model = List.map (text >> List.singleton >> li []) model |> ol []
