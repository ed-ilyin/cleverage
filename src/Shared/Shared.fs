﻿module Cleverage.Shared
open Thoth.Json.Net

type Update =
    {   MessageId: uint64
        From: string
        Chat: string option
        Text: string option }
    static member Decoder : Decoder<Update> =
        Decode.object (fun get -> {
            MessageId =
                get.Required.At [ "message"; "message_id" ] Decode.uint64
            From = get.Required.At [ "from"; "first_name" ] Decode.string
            Chat = get.Optional.At [ "chat"; "title" ] Decode.string
            Text = get.Optional.Field "text" Decode.string
        })
        |> Decode.field "message"