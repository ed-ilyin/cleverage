#r "nuget: FParsec"
#r "nuget: Bolero"
type Markdown = Entity list
and Entity = Text of string | Bold of Markdown | Italic of Markdown
open FParsec

let l tag x =
    printf "%s: %+A" tag x
    x

let bp (p: Parser<_,_>) stream =
    p stream

let entity, entityRef = createParserForwardedToRef()
let markdown: Parser<Markdown,_> = many1 entity

let mbw char =
    let c = pstring char
    between c c markdown

let bold = mbw "*" |>> Bold

let text =
    notFollowedBy
    <| choice [ bp bold ]
    >>. anyChar
    |> many1Chars
    |>> Entity.Text

entityRef.Value <- choice [ text; bold ]

let test p str =
    match run p str with
    | Success(result, _, _)   -> $"%+A{result}"
    | Failure(errorMsg, _, _) -> failwith errorMsg

test markdown "*ifjs*disdj"
