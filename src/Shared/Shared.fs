module Cleverage.Shared
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

type Item = Result<Message,string> * string
