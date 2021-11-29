module Cleverage.Streaming
open System
open System.IO
open System.Net.Http
open System.Threading.Tasks
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Azure.WebJobs
open Microsoft.Azure.WebJobs.Extensions.Http
open Microsoft.Azure.WebJobs.Extensions.SignalRService
open Newtonsoft.Json
open Microsoft.Extensions.Logging
open Thoth.Json.Net

type Message =
    { From: string
      Chat: string option
      Text: string }
    static member Decoder : Decoder<Message> =
        Decode.object (fun get -> {
            From = get.Required.At [ "from"; "first_name" ] Decode.string
            Chat = get.Optional.At [ "chat"; "title" ] Decode.string
            Text = get.Required.Field "text" Decode.string
        })
        |> Decode.field "message"

let private httpClient = new HttpClient()

[<FunctionName("index")>]
let Index
    ([<HttpTrigger(AuthorizationLevel.Function)>] req: HttpRequest)
    (ctx: ExecutionContext) =
    let path = Path.Combine(ctx.FunctionAppDirectory, "content", "index.html")
    ContentResult (Content = File.ReadAllText(path), ContentType = "text/html")

[<FunctionName("negotiate")>]
let Negotiate
    ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequest)
    ([<SignalRConnectionInfo(HubName = "CLeveRAge")>]
        connectionInfo: SignalRConnectionInfo) = connectionInfo

let ofReq decoder (req: HttpRequest) = task {
    use reader = new StreamReader(req.Body)
    let! body = reader.ReadToEndAsync()
    return Decode.fromString decoder body
}

[<FunctionName("broadcast")>]
let Broadcast
    ([<HttpTrigger(AuthorizationLevel.Function)>] req: HttpRequest)
    ([<SignalR(HubName = "CLeveRAge")>]
        signalRMessages: IAsyncCollector<SignalRMessage>)
    (log: ILogger) = task {
    let! message = ofReq Message.Decoder req
    let s = sprintf "%+A" message
    log.LogInformation s
    return!
        signalRMessages.AddAsync (
            SignalRMessage (Target = "NewMessage", Arguments = [| s |])
        )
}
