open Funogram.Telegram.Bot
open Funogram.Telegram.Api

let accessToken = "мой токен"

let onStart (context: UpdateContext) = printfn "start %+A" context
let onHelp (context: UpdateContext) = printfn "help %+A" context

let onUpdate (context: UpdateContext) =
    processCommands context [
        cmd "/start" onStart
        cmd "/help" onHelp
        cmd "/msg" (fun _ -> sendMessage
    ] |> printfn "onUpdate %+A %b" context.Update.Message

[<EntryPoint>]
let main argv =
    startBot { defaultConfig with Token = accessToken } onUpdate None
    |> Async.RunSynchronously
    0 // return an integer exit code
