open Funogram.Telegram.Bot
open Funogram.Telegram.Api
open Funogram.Api

let accessToken = ""
let config = { defaultConfig with Token = accessToken }

let onStart (context: UpdateContext) = printfn "start %+A" context
let onHelp (context: UpdateContext) = printfn "help %+A" context

let onSay arg (ctx: UpdateContext) =
    // let chatId = Option.map (fun m -> m.Chat.Id) ctx.Update.Message
    match ctx.Update.Message with
        | None -> ()
        | Some msg ->
            Some  msg.MessageId
            |> sendMessageReply msg.Chat.Id arg
            |> api config
            |> Async.RunSynchronously
            |> ignore

let onUpdate (ctx: UpdateContext) =
    processCommands ctx [
        cmd "/start" onStart
        cmd "/help" onHelp
        cmdScan "/say %s" onSay
    ] |> printfn "onUpdate %+A %b" ctx.Update.Message

[<EntryPoint>]
let main argv =
    startBot config onUpdate None |> Async.RunSynchronously
    0 // return an integer exit code
