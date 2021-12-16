module Cleverage.Server.Index

open Bolero
open Bolero.Html
open Bolero.Server.Html
open Cleverage

let page = doctypeHtml [] [
    head [] [
        meta [attr.charset "UTF-8"]
        meta [attr.name "viewport"; attr.content "width=device-width, initial-scale=1.0"]
        title [] [text "CLeveRAge | СОобРАЖатор"]
        ``base`` [attr.href "/"]
        link [attr.rel "stylesheet"; attr.href "https://cdn.jsdelivr.net/npm/bulma@0.9.3/css/bulma.min.css"]
        link [attr.rel "stylesheet"; attr.href "css/index.css"]
    ]
    body [] [
        nav [attr.classes ["navbar"; "is-dark"]; "role" => "navigation"; attr.aria "label" "main navigation"] [
            div [attr.classes ["navbar-brand"]] [
                a [attr.classes ["navbar-item"; "has-text-weight-bold"; "is-size-5"]; attr.href "https://fsbolero.io"] [
                    img [attr.style "height:40px"; attr.src "https://raw.githubusercontent.com/alex5250/cleverage/master/src/Cleverage.Server/logo.jpg"]
                    text "CLeveRAge | СОобРАЖатор"
                ]
            ]
        ]
        div [attr.id "main"] [rootComp<Client.Main.MyApp>]
        boleroScript
    ]
]
