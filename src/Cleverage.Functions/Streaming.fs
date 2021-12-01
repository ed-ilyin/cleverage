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

let actionResultTaskOfAsync (log: ILogger) (task: Async<Result<_,string>>) =
    async {
    let! catch = Async.Catch task
    return
        match catch with
        | Choice1Of2 (Ok ok) ->
            log.LogInformation ("📤👌🏼{ok}", sprintf "%+A" ok)
            OkObjectResult ok :> IActionResult
        | Choice1Of2 (Error error) ->
            log.LogError ("📤💩{error}", error)
            BadRequestObjectResult error
        | Choice2Of2 error ->
            log.LogError ("📤💩{error}", error)
            BadRequestObjectResult error.Message
    } |> Async.StartAsTask

[<FunctionName("broadcast")>]
let Broadcast
    ([<HttpTrigger(AuthorizationLevel.Function)>] req: HttpRequest)
    ([<SignalR(HubName = "CLeveRAge")>]
        signalRMessages: IAsyncCollector<SignalRMessage>)
    (log: ILogger) =
    async {
        let! result, request = ofReq Message.Decoder req
        log.LogInformation ("📥{request}", request)

        let! signal =
            signalRMessages.AddAsync
                (SignalRMessage (
                    Target = "NewMessage",
                    Arguments = [| result, request |]
                ))
            |> Async.AwaitTask

        return Ok (result, request)
    }
    |> actionResultTaskOfAsync log
