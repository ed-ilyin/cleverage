module Cleverage.Shared
open Thoth.Json.Net

type Update =
    {   MessageId: uint64
        From: string
        Chat: string
        Text: string }
    static member Decoder : Decoder<Update> =
        let decodeMessage = Decode.object (fun get ->
            let from = get.Required.At [ "from"; "first_name" ] Decode.string
            let chat =
                get.Optional.At [ "chat"; "title" ] Decode.string
                |> Option.orElse
                <| get.Optional.At [ "chat"; "first_name" ] Decode.string
                |> Option.defaultValue ""
            {   MessageId = get.Required.Field "message_id" Decode.uint64
                From = if from = chat then "" else from
                Chat = chat
                Text =
                    get.Optional.Field "text" Decode.string
                    |> Option.defaultValue ""
            }
        )
        Decode.oneOf [
            Decode.map2
            <| fun sessionName message ->
                { message with Chat = sessionName; From = "" }
            <| Decode.field "SessionName" Decode.string
            <| Decode.field "Message" decodeMessage
            decodeMessage |> Decode.field "message"
        ]

    static member Encoder update =
        Encode.Auto.generateEncoderCached<Update>
            (PascalCase, Extra.withUInt64 Extra.empty)
