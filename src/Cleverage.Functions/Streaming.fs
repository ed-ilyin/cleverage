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

type private GitResult () =
    [<JsonRequired>]
    [<JsonProperty("stargazers_count")>]
    [<DefaultValue>] val mutable StarCount : string

let private httpClient = new HttpClient()

[<FunctionName("index")>]
let Index
    ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequest)
    (context: ExecutionContext) =

    let path =
        Path.Combine(context.FunctionAppDirectory, "content", "index.html")

    ContentResult (Content = File.ReadAllText(path), ContentType = "text/html")

[<FunctionName("negotiate")>]
let Negotiate
    ([<HttpTrigger(AuthorizationLevel.Anonymous)>] req: HttpRequest)
    ([<SignalRConnectionInfo(HubName = "serverlessSample")>]
        connectionInfo: SignalRConnectionInfo) = connectionInfo

[<FunctionName("broadcast")>]
let Broadcast
    // ([<TimerTrigger("*/5 * * * * *")>] myTimer: TimerInfo)
    ([<SignalR(HubName = "serverlessSample")>]
        signalRMessages: IAsyncCollector<SignalRMessage>) = task {

    let request =
        new HttpRequestMessage (
            HttpMethod.Get,
            "https://api.github.com/repos/azure/azure-signalr"
        )

    request.Headers.UserAgent.ParseAdd "Serverless"
    let! response = httpClient.SendAsync request
    let! content = response.Content.ReadAsStringAsync ()
    let result = JsonConvert.DeserializeObject<GitResult> content

    do! signalRMessages.AddAsync (
            SignalRMessage (
                Target = "newMessage",
                Arguments = [|
                    $"Current star count of https://github.com/Azure/azure-signalr is: {result.StarCount}"
                |]
            )
        )
}
