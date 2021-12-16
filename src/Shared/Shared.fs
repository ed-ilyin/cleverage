module Cleverage.Shared
open Thoth.Json.Net

type Update =
    {   MessageId: uint64
        From: string
        Chat: string
        Text: string list
    }
    static member Decoder : Decoder<Update> =
        let decodeCleverageMessage =
            Decode.object (fun get ->
                let text =
                    get.Required.At [ "Event"; "Message"; "Text" ] Decode.string
                {   MessageId = get.Required.Field "MessageID" Decode.uint64
                    From =
                        get.Required.At
                            [ "Event"; "Message"; "ChannelUsername" ]
                            Decode.string
                    Chat = get.Required.Field "SessionName" Decode.string
                    Text = text.Split '\n' |> List.ofArray
                }
            )
        let decodeTelegramMessage =
            Decode.field "message"
            <| Decode.object (fun get ->
                let from =
                    get.Required.At [ "from"; "first_name" ] Decode.string
                let chat =
                    get.Optional.At [ "chat"; "title" ] Decode.string
                    |> Option.orElse
                    <| get.Optional.At [ "chat"; "first_name" ] Decode.string
                    |> Option.defaultValue ""
                let text =
                    get.Optional.Field "text" Decode.string
                    |> Option.defaultValue ""
                {   MessageId = get.Required.Field "message_id" Decode.uint64
                    From = if from = chat then "" else from
                    Chat = chat
                    Text = text.Split '\n' |> List.ofArray
                }
            )
        Decode.oneOf [ decodeCleverageMessage; decodeTelegramMessage ]

    static member Encoder update =
        Encode.Auto.generateEncoderCached<Update>
            (PascalCase, Extra.withUInt64 Extra.empty)
