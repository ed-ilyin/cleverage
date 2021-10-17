module Cleverage.Client.Table
open Bolero.Html

type Символ = string
type Имя = string
type Роль = Спаситель | Масон | Рептилоид | Демон

type Игрок = {
    Символ: Символ
    Имя: Имя
    Подозрительность: int
    Спаситель: float
    Масон: float
    Рептилоид: float
    Демон: float
    Раб: bool
}

type Модель = Map<Символ, Игрок>

type Действие =
    | Зевнул
    | Пробасил
    | Подозревает of Символ
    | ПолучилКлючНе of Роль * Символ
    | Пророчит of
        спаситель: Символ * рептиль1: Символ * рептиль2: Символ * демон: Символ
    | ГолосуетПротив of Символ
    | Заснул
    | Пощадил of Символ
    | ПорабощёнНе of Роль
    | ПоставилНа of Символ
    | ПорабощёнДемономНе of Роль * Роль

let игра = "MistyRose Union"

let игроки =
    [   "👣", "Маркиза"
        "К", "Казанова"
        "🤬", "Миледи"
        "🧞", "Властелин"
        "🐙", "Кракен"
        "🍑", "Забава"
        "👶", "Голодранец"
        "Н", "Негодяй"
        "😇", "Икона"
        "🌈", "Молния"
    ] |> Map.ofList

let события = [
        "🐙", Подозревает "👣"
        "🐙", Подозревает "🤬"
        "🐙", Подозревает "🧞"
        "🐙", Подозревает "👶"
        //ПолучилКлючНе
        "🐙", Пророчит ("😇", "👶", "🧞", "🤬")
    ]

let новыйИгрок символ подозрительность = {
        Символ = символ
        Имя = Map.tryFind символ игроки |> Option.defaultValue ""
        Подозрительность = подозрительность
        Спаситель = 0.1
        Масон = 0.6
        Рептилоид = 0.2
        Демон = 0.1
        Раб = false
    }

let подозрительней он состояние =
    Map.add он (
        match Map.tryFind он состояние with
        | None -> новыйИгрок он 1
        | Some игрок ->
            { игрок with Подозрительность = игрок.Подозрительность + 1 }
    ) состояние

let менееПодозрительный он состояние =
    Map.add он (
        match Map.tryFind он состояние with
        | None -> новыйИгрок он -1
        | Some игрок ->
            { игрок with Подозрительность = игрок.Подозрительность - 1 }
    ) состояние

let добавьПроцентыОтГкД а б в г д = д + г / (а + б + в) * д

let добавьПроцентыК _ _ _ модель игрок = Map.add игрок.Символ игрок модель

let ключНе неРоль он состояние =
    let игрок =
        Map.tryFind он состояние |> Option.defaultValue (новыйИгрок он 0)
    match неРоль with
    | Спаситель ->
        let д =
            добавьПроцентыОтГкД игрок.Масон игрок.Рептилоид игрок.Демон
                игрок.Спаситель
        { игрок with
            Спаситель = 0.
            Масон = д игрок.Масон
            Рептилоид = д игрок.Рептилоид
            Демон = д игрок.Демон
        } |> добавьПроцентыК игрок.Спаситель (fun и -> и.Спаситель)
            (fun и с -> { и with Спаситель = с }) состояние
    | Масон ->
        let д =
            добавьПроцентыОтГкД игрок.Спаситель игрок.Рептилоид игрок.Демон
                игрок.Масон
        { игрок with
            Спаситель = д игрок.Спаситель
            Масон = 0.
            Рептилоид = д игрок.Рептилоид
            Демон = д игрок.Демон
        } |> добавьПроцентыК игрок.Масон (fun и -> и.Масон)
            (fun и м -> { и with Масон = м }) состояние
    | Рептилоид ->
        let д =
            добавьПроцентыОтГкД игрок.Спаситель игрок.Масон игрок.Демон
                игрок.Рептилоид
        { игрок with
            Спаситель = д игрок.Спаситель
            Масон = д игрок.Масон
            Рептилоид = 0.
            Демон = д игрок.Демон
        } |> добавьПроцентыК игрок.Масон (fun и -> и.Масон)
            (fun и м -> { и with Масон = м }) состояние
    | Демон ->
        let д =
            добавьПроцентыОтГкД игрок.Спаситель игрок.Масон игрок.Рептилоид
                игрок.Демон
        { игрок with
            Спаситель = д игрок.Спаситель
            Масон = д игрок.Масон
            Рептилоид = д игрок.Рептилоид
            Демон = 0.
        } |> добавьПроцентыК игрок.Масон (fun и -> и.Масон)
            (fun и м -> { и with Масон = м }) состояние

let раб символ модель =
    match Map.tryFind символ модель with
    | None -> модель
    | Some игрок -> Map.add символ { игрок with Раб = true } модель

let расколбас состояние = function
    | он, Зевнул | он, Пробасил -> подозрительней он состояние
    | _, Подозревает его  -> подозрительней его состояние
    | он, ПолучилКлючНе (роль, кто) ->
        ключНе роль он состояние |> ключНе роль кто
    | он, Пророчит (спаситель, рептиль1, рептиль2, демон) ->
        менееПодозрительный спаситель состояние
        |> подозрительней рептиль1
        |> подозрительней рептиль2
        |> подозрительней демон
    | он, ГолосуетПротив него -> подозрительней него состояние
    | он, Пощадил его -> менееПодозрительный его состояние
    | он, ПорабощёнНе роль -> ключНе роль он состояние |> раб он
    | он, ПорабощёнДемономНе (роль1, роль2) ->
        ключНе роль1 он состояние |> ключНе роль2 он |> раб он
    | _ -> состояние

let рейтинг =
    List.fold расколбас (Map.map (fun символ _ -> новыйИгрок символ 0) игроки)
        события
    |> Map.toList
    |> List.map snd

// VIEW

let ключиВЯчейку ключи =
    match Set.toList ключи |> List.map (sprintf "Не-%A") with
    | [] -> ""
    | ключи -> List.reduce (sprintf "%s, %s") ключи
    |> text |> List.singleton |> td []

let вероятность = (*) 100. >> sprintf "%.0f" >> text >> List.singleton >> td []
let h = text >> List.singleton >> th []

let view =
    table [ attr.``class`` "table" ] [
        thead [] [ h "С"; h "Имя"; h "П"; h "С"; h "М"; h "Р"; h "Д"; h "∑" ]
        List.sortBy (fun игрок -> игрок.Подозрительность) рейтинг
        |> List.map (fun игрок ->
            tr [] [
                td [] [ text игрок.Символ ]
                td [
                    if игрок.Раб then "is-black" else ""
                    |> attr.``class``
                ] [ Map.tryFind игрок.Символ игроки
                    |> Option.defaultValue ""
                    |> text
                ]
                td [] [ string игрок.Подозрительность |> text ]
                вероятность игрок.Спаситель
                вероятность игрок.Масон
                вероятность игрок.Рептилоид
                вероятность игрок.Демон
                вероятность (
                    игрок.Спаситель
                    + игрок.Масон
                    + игрок.Рептилоид
                    + игрок.Демон
                )
                // ключиВЯчейку ключи
            ]
        )
        |> tbody []
        List.fold (fun (с, м, р, д) игрок ->
            с + игрок.Спаситель
            , м + игрок.Масон
            , р + игрок.Рептилоид
            , д + игрок.Демон
        ) (0., 0., 0., 0.) рейтинг
        |> fun (с, м, р, д) -> tfoot [] [
            td [] []
            td [] []
            td [] []
            с / 100. |> вероятность
            м / 100. |> вероятность
            р / 100. |> вероятность
            д / 100. |> вероятность
            td [] []
        ]
    ]
