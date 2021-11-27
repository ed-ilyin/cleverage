namespace BlazorWebAssemblySignalRApp.Server.Hubs
open System.Threading.Tasks
open Microsoft.AspNetCore.SignalR

type ChatHub =
    inherit Hub
    member x.SendMessage user message =
        Clients.All.SendAsync("ReceiveMessage", user, message)

