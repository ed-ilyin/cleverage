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
open Microsoft.Extensions.Logging
open Thoth.Json.Net
open Shared

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

let ofReq decoder (req: HttpRequest) = async {
    use reader = new StreamReader(req.Body)
    let! body = reader.ReadToEndAsync () |> Async.AwaitTask
    return Decode.fromString decoder body, body
}

let actionResultTaskOfAsync (log: ILogger) computation =
    async {
    let! catch = Async.Catch computation
    return
        match catch with
        | Choice1Of2 ok ->
            log.LogInformation ("📤👌🏼{ok}", sprintf "%+A" ok)
            OkObjectResult ok :> IActionResult
        | Choice2Of2 error ->
            log.LogError ("📤💩{error}", error)
            BadRequestObjectResult error.Message
    } |> Async.StartAsTask

let asyncResult computation = async {
    let! catch = Async.Catch computation
    return
        match catch with
        Choice1Of2 ok -> Ok ok | Choice2Of2 error -> Result.Error error
}

let encode a = Encode.Auto.toString (4, a)

let cleverage (req: HttpRequestMessage) (log: ILogger) func =
    async {
        let! json = req.Content.ReadAsStringAsync () |> Async.AwaitTask
        log.LogInformation ("📥{json}", json)

        let! result =
            async {
                let cleverage = Decode.fromString Message.Decoder json
                log.LogInformation ("👓{cleverage}", cleverage)
                return! func cleverage json
            }
            |> asyncResult

        match result with
        | Ok ok ->
            log.LogInformation ("⚡👌🏼{ok}", sprintf "%+A" ok)
        | Error error ->
            log.LogError ("⚡💩{error}", error)

        return result, json
    }
    |> actionResultTaskOfAsync log

[<FunctionName("broadcast")>]
let Broadcast
    ([<HttpTrigger(AuthorizationLevel.Function)>] req: HttpRequestMessage)
    ([<SignalR(HubName = "CLeveRAge")>]
        signalRMessages: IAsyncCollector<SignalRMessage>)
    (log: ILogger) = cleverage req log <| fun decodeResult json -> async {
        do! signalRMessages.AddAsync
                (SignalRMessage (
                    Target = "NewMessage",
                    Arguments =
                        [| Encode.Auto.toString (4, decodeResult), json |]
                ))
            |> Async.AwaitTask

        return Ok decodeResult
    }
