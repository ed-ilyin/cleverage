type Markdown = Entity list
and Entity = Text of string | Bold of Markdown | Italic of Markdown
open FParsec

let (<!>) (p: Parser<_,_>) label : Parser<_,_> =
    fun stream ->
        printfn "%A: Entering %s" stream.Position label
        let reply = p stream
        printfn "%A: Leaving %s (%A)" stream.Position label reply.Status
        reply

let bp (p: Parser<_,_>) stream =
    p stream

let entity, entityRef = createParserForwardedToRef()
let markdown: Parser<Markdown,_> = many1 entity

let mbw char =
    let c = pstring char
    between c c markdown

let bold = mbw "*" <!> "mbw" |> bp |>> Bold

let text =
    notFollowedBy
    <| choice [ bold ]
    >>. anyChar
    |> many1Chars
    |>> Entity.Text

entityRef.Value <- choice [ text; bold ]

let test p str =
    match run p str with
    | Success(result, _, _)   -> $"%+A{result}"
    | Failure(errorMsg, _, _) -> failwith errorMsg

let x = test markdown "*ifjs*disdj"
