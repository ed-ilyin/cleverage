module EdIlyin.Cleverage.Streaming
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

let private httpClient = new HttpClient()

[<FunctionName("index")>]
let Index
    ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequest)
    (ctx: ExecutionContext) =
    let path = Path.Combine(ctx.FunctionAppDirectory, "content", "index.html")
    ContentResult (Content = File.ReadAllText(path), ContentType = "text/html")

[<FunctionName("negotiate")>]
let Negotiate
    ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequest)
    ([<SignalRConnectionInfo(HubName = "CLeveRAge")>]
        connectionInfo: SignalRConnectionInfo) = connectionInfo

[<FunctionName("broadcast")>]
let Broadcast
    ([<HttpTrigger(AuthorizationLevel.Function)>] req: HttpRequestMessage)
    ([<SignalR(HubName = "CLeveRAge")>]
        signalRMessages: IAsyncCollector<SignalRMessage>) = task {

    let! content = req.Content.ReadAsStringAsync ()

    return!
        signalRMessages.AddAsync (
            SignalRMessage (Target = "newMessage", Arguments = [| content |])
        )
}
