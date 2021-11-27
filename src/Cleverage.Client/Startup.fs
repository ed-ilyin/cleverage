namespace Cleverage.Client

open Microsoft.AspNetCore.Components.WebAssembly.Hosting
open Bolero.Remoting.Client
open Microsoft.AspNetCore.SignalR.Client
open Microsoft.AspNetCore.SignalR
open Microsoft.AspNetCore.SignalR

module Program =

    [<EntryPoint>]
    let Main args =
        let builder = WebAssemblyHostBuilder.CreateDefault(args)
        builder.Services.AddSignalR()
        builder.RootComponents.Add<Main.MyApp>("#main")
        builder.Services.AddRemoting(builder.HostEnvironment) |> ignore
        builder.Build().RunAsync() |> ignore
        0
