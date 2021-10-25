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
    Подозрительно: string list
}

type Модель = Map<Символ, Игрок>

type Действие =
    | Зевнул
    | Пробасил
    | Подозревает of Символ
    | ПолучилКлючНе of Роль * Символ
    | ГоворитЧтоПолучилКлючНе of Роль * Символ
    | ПризналсяЧтоНе of Роль
    | Пророчит of
        спаситель: Символ * рептиль1: Символ * рептиль2: Символ * демон: Символ
    | ГолосуетПротив of Символ
    | Пощадил of Символ
    | ПорабощёнНе of Роль
    | ПоставилНа of Символ
    | Соврал
    | СказалПравду
    | Топит of Символ
    | Подозрительно of string

let игра = "DeepSkyBlue Status"
let я = "🦑"

let игроки =
    Map.ofList [
        "🦑", "Кракен"
        "🦖", "Буратино"
        "🤡", "Джокер|Валерончег Кыберпанк"
        "👣", "Маркиза"
        "🎃", "Аватар"
        "🌈", "Лёгкость Бытия|Свёкла"
        "🍑", "Забава"
        "😎", "Голодранец|Согдиец"
        "♟", "Жмых"
        "🤬", "Властелин|Спектата"
    ]

let события = [
    // "🦑", ПолучилКлючНе (Спаситель, "🦑")
    // "🦑", ПолучилКлючНе (Рептилоид, "🦑")
    // "🦑", ПолучилКлючНе (Демон, "🦑")
    // "🦑", Пророчит ("🦑", "🎃", "🦖", "👣")
    // ТЗ
    "♟", ГолосуетПротив "🍑"
    "🤬", ГолосуетПротив "😎"
    "🦑", ГолосуетПротив "👣"
    "🦖", ГолосуетПротив "😎"
    "👣", ГолосуетПротив "🤬"
    "🍑", ГолосуетПротив "♟"
    "😎", ГолосуетПротив "🤡"
    "🎃", ГолосуетПротив "🌈"
    "🌈", ГолосуетПротив "😎"
    "🤡", ГолосуетПротив "🌈"

    "🦖", ГолосуетПротив "😎"
    "🦑", ГолосуетПротив "😎"
    "🎃", Подозрительно "напористо гасит 😎Голодранца"
    "🤬", ГолосуетПротив "😎"
    "🎃", ГолосуетПротив "🌈"
    "🤡", ГолосуетПротив "😎"
    "🍑", ГолосуетПротив "🌈"
    "👣", ГолосуетПротив "🌈"
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
        Подозрительно = []
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

let матрицаИзСостояния состояние =
    Map.toList состояние |> List.map (fun (_, игрок) ->
        [ игрок.Спаситель; игрок.Масон; игрок.Рептилоид; игрок.Демон ]
    )

let игрокИзМатрицы строкаМатрицы игрок =
    match строкаМатрицы with
    | [ спаситель; масон; рептилоид; демон ] ->
        { игрок with
            Спаситель = спаситель
            Масон = масон
            Рептилоид = рептилоид
            Демон = демон
        }
    | _ -> игрок

let состояниеИзМатрицы состояние матрица =
    Map.toList состояние
    |> List.map snd
    |> List.map2 игрокИзМатрицы матрица
    |> List.map (fun игрок -> игрок.Символ, игрок)
    |> Map.ofList

let ключНе неРоль он изменение состояние =
    let роль =
        match неРоль with
            Спаситель -> 0 | Масон -> 1 | Рептилоид -> 2 | Демон -> 3
    let игрок = Map.toList состояние |> List.map fst |> List.findIndex ((=) он)
    матрицаИзСостояния состояние
    |> Matrix.changeMatrix роль игрок изменение
    |> состояниеИзМатрицы состояние

let раб символ модель =
    match Map.tryFind символ модель with
    | None -> модель
    | Some игрок -> Map.add символ { игрок with Раб = true } модель

let всеВРабствеНеДемоны =
    Map.map
        (fun _ игрок -> if игрок.Раб then { игрок with Демон = 0. } else игрок)

let точно _ = 0.
let может значение = значение / 2.
let немного = ((*) 1.1)

let подозрительно он сделал мир =
    match Map.tryFind он мир with
    | None -> мир
    | Some игрок ->
        Map.add он
            { игрок with Подозрительно = игрок.Подозрительно @ [ сделал ] }
            мир

let расколбас мир = function
    | он, Зевнул | он, Пробасил -> подозрительней он мир
    | _, Подозревает его  -> подозрительней его мир
    | он, ПолучилКлючНе (роль, кто) ->
        ключНе роль он точно мир |> ключНе роль кто точно
    | он, ГоворитЧтоПолучилКлючНе (роль, кто) ->
        ключНе роль он может мир |> ключНе роль кто может
    | он, ПризналсяЧтоНе роль -> ключНе роль он может мир
    | он, Пророчит (спаситель, рептиль1, рептиль2, демон) ->
        менееПодозрительный спаситель мир
        |> подозрительней рептиль1
        |> подозрительней рептиль2
        |> подозрительней демон
        |> ключНе Спаситель спаситель немного
        |> ключНе Рептилоид рептиль1 немного
        |> ключНе Рептилоид рептиль2 немного
        |> ключНе Демон демон немного
    | он, ГолосуетПротив меня when меня = я -> подозрительней он мир
    | он, Топит его -> подозрительней он мир
    | он, ГолосуетПротив него -> подозрительней него мир
    | он, Пощадил его -> менееПодозрительный его мир
    | он, ПорабощёнНе Демон -> раб он мир |> всеВРабствеНеДемоны
    | он, ПорабощёнНе роль -> ключНе роль он точно мир |> раб он
    | он, Соврал -> подозрительней он мир
    | он, СказалПравду -> менееПодозрительный он мир
    | он, Подозрительно сделал ->
        подозрительно он сделал мир |> подозрительней он
    | он, ПоставилНа _ -> мир

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
    |> text //|> List.singleton |> td []

let вероятность = (*) 100. >> sprintf "%.0f" >> text //>> List.singleton >> td []

let плохость игрок =
    let проценты = (игрок.Рептилоид + игрок.Демон) * 100.
    progress [
        attr.``class`` "progress"
        attr.value проценты
        attr.max 100
    ] [ sprintf "%.0f" проценты |> text ]

let h = text >> List.singleton >> th []

let join delimeter = function
    | [] -> ""
    | [ e ] -> e
    | list -> List.reduce (fun s1 s2 -> s1 + delimeter + s2) list

let колонка ``class`` =
    span [ join " " [ "column pt-0 pb-0"; ``class`` ] |> attr.``class`` ]

let роль знак процент =
    колонка "is-narrow" [ sprintf "%s %.0f%%" знак (процент * 100.) |> text ]

let block = div [ attr.``class`` "block" ]
let d ``class`` = div [ attr.``class`` ``class`` ]

let игрок и =
    d "panel-block" [
        d "panel-icon is-size-1 mr-0" [ text и.Символ ]
        d "column" [
            d "columns is-mobile" [
                колонка "" [ text и.Имя ]
                роль "🧘" и.Спаситель
                роль "🕺" и.Масон
            ]
            d "columns is-mobile" [
                колонка "" [ плохость и ]
                колонка "is-narrow"
                    [ sprintf "😱 %i" и.Подозрительность |> text ]
                роль "🦖" и.Рептилоид
                роль "😈" и.Демон
            ]
            List.map (text >> List.singleton >> колонка "") и.Подозрительно
            |> d "columns is-mobile"
        ]
    ]

let view () =
    рейтинг
    |> List.sortBy (fun игрок -> игрок.Рептилоид + игрок.Демон)
    |> List.sortBy (fun игрок -> игрок.Подозрительность)
    |> List.map игрок
    |> d "panel"

// let view () =
//     table [ attr.``class`` "table" ] [
//         thead []
//             [   h ""
//                 h "🪆"
//                 h "😱"
//                 h "🧘"
//                 h "🕺"
//                 h "🦖🐊🦎🐍😈"
//                 h "🦖"
//                 h "😈"
//                 h "🧐"
//             ]
//         рейтинг
//         |> List.sortBy (fun игрок -> игрок.Рептилоид + игрок.Демон)
//         |> List.sortBy (fun игрок -> игрок.Подозрительность)
//         |> List.map (fun игрок ->
//             tr [] [
//                 td [ attr.style "padding:0px;font-size:3rem"] [ text игрок.Символ ]
//                 td [
//                     if игрок.Раб then "is-black" else ""
//                     |> attr.``class``
//                 ] [ Map.tryFind игрок.Символ игроки
//                     |> Option.defaultValue ""
//                     |> text
//                 ]
//                 td [] [ string игрок.Подозрительность |> text ]
//                 вероятность игрок.Спаситель
//                 вероятность игрок.Масон
//                 плохость игрок
//                 вероятность игрок.Рептилоид
//                 вероятность игрок.Демон
//                 // вероятность (
//                 //     игрок.Спаситель
//                 //     + игрок.Масон
//                 //     + игрок.Рептилоид
//                 //     + игрок.Демон
//                 // )
//                 // ключиВЯчейку ключи
//                 td [] [
//                     List.map (text >> List.singleton >> li [])
//                         игрок.Подозрительно
//                     |> ol []
//                 ]
//             ]
//         )
//         |> tbody []
//         // List.fold (fun (с, м, р, д) игрок ->
//         //     с + игрок.Спаситель
//         //     , м + игрок.Масон
//         //     , р + игрок.Рептилоид
//         //     , д + игрок.Демон
//         // ) (0., 0., 0., 0.) рейтинг
//         // |> fun (с, м, р, д) -> tfoot [] [
//         //     td [] []
//         //     td [] []
//         //     td [] []
//         //     с / 100. |> вероятность
//         //     м / 100. |> вероятность
//         //     td [] []
//         //     р / 100. |> вероятность
//         //     д / 100. |> вероятность
//         //     td [] []
//         // ]
//     ]
