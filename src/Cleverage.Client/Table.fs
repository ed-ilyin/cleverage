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
    | ПризналсяЧтоНе of Роль
    | Пророчит of
        спаситель: Символ * рептиль1: Символ * рептиль2: Символ * демон: Символ
    | ГолосуетПротив of Символ
    | Заснул
    | Пощадил of Символ
    | ПорабощёнНе of Роль
    | ПоставилНа of Символ
    | ПорабощёнДемономНе of Роль * Роль

let игра = "MistyRose Union"
let я = "🐙"

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
        "🐙", ПолучилКлючНе (Спаситель, "🐙")
        "🐙", ПолучилКлючНе (Рептилоид, "🐙")
        "🐙", ПолучилКлючНе (Демон, "🐙")
        "🐙", Подозревает "👣"
        "🐙", Подозревает "🤬"
        "🐙", Подозревает "🧞"
        "🐙", Подозревает "👶"
        //ПолучилКлючНе
        "🐙", Пророчит ("😇", "👶", "🧞", "🤬")
        "К", ГолосуетПротив "😇"
        "👣", ГолосуетПротив "🌈"
        "🤬", ГолосуетПротив "😇"
        "👶", ГолосуетПротив "🌈"
        "Н", ГолосуетПротив "🐙"
        "🐙", ГолосуетПротив "👶"
        "🧞", ГолосуетПротив "🐙"
        "😇", ГолосуетПротив "🍑"
        "🍑", ГолосуетПротив "😇"
        "🌈", ГолосуетПротив "🐙"
        "🐙", ПолучилКлючНе (Спаситель, "🧞")
        "🤬", ГолосуетПротив "😇"
        "🍑", ГолосуетПротив "😇"
        "К", ГолосуетПротив "🐙"
        "Н", ГолосуетПротив "🐙"
        "🌈", ГолосуетПротив "🐙"
        "👣", ГолосуетПротив "🐙"
        "😇", ГолосуетПротив "🐙"
        "🧞", ГолосуетПротив "🐙"
        "🐙", ГолосуетПротив "🌈"
        "👶", ГолосуетПротив "😇"
        "🐙", ПорабощёнНе Рептилоид
        "👣", ПорабощёнНе Рептилоид
        "👣", ПорабощёнНе Демон
        "😇", ПорабощёнНе Демон
        "😇", ПризналсяЧтоНе Масон
        "😇", ПризналсяЧтоНе Спаситель
        "К", ГолосуетПротив "👶"
        "🤬", ГолосуетПротив "К"
        "Н", ГолосуетПротив "🍑"
        "🍑", ГолосуетПротив "👶"
        "🌈", ГолосуетПротив "🧞"
        "🧞", ГолосуетПротив "К"

        "🧞", ГолосуетПротив "К"
        "🤬", ГолосуетПротив "К"
        "🍑", ГолосуетПротив "👶"
        "🌈", ГолосуетПротив "👶"
        "🧞", ПолучилКлючНе (Спаситель, "🍑")
        "К", ПризналсяЧтоНе Масон
        "К", ПризналсяЧтоНе Демон
        "К", ПорабощёнНе Спаситель
        "👶", ПорабощёнНе Рептилоид

        "🤬", ПорабощёнНе Демон
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

let ключНе неРоль он состояние =
    let роль =
        match неРоль with
            Спаситель -> 0 | Масон -> 1 | Рептилоид -> 2 | Демон -> 3
    let игрок = Map.toList состояние |> List.map fst |> List.findIndex ((=) он)
    матрицаИзСостояния состояние
    |> Matrix.changeMatrix роль игрок 0.
    |> состояниеИзМатрицы состояние

let раб символ модель =
    match Map.tryFind символ модель with
    | None -> модель
    | Some игрок -> Map.add символ { игрок with Раб = true } модель

let расколбас состояние = function
    | он, Зевнул | он, Пробасил -> подозрительней он состояние
    | _, Подозревает его  -> подозрительней его состояние
    | он, ПолучилКлючНе (роль, кто) ->
        ключНе роль он состояние |> ключНе роль кто
    | он, ПризналсяЧтоНе роль -> ключНе роль он состояние
    | он, Пророчит (спаситель, рептиль1, рептиль2, демон) ->
        менееПодозрительный спаситель состояние
        |> подозрительней рептиль1
        |> подозрительней рептиль2
        |> подозрительней демон
    | он, ГолосуетПротив меня when меня = я -> подозрительней он состояние
    | он, ГолосуетПротив меня when меня = "👣" -> подозрительней он состояние
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
let плохость игрок =
    let проценты = (игрок.Рептилоид + игрок.Демон) * 100.
    let текст = sprintf "%.0f" проценты |> text
    td [ attr.width "200px" ] [
        progress [
            attr.``class`` "progress"
            attr.value проценты
            attr.max 100
        ] [ текст ]
    ]

let h = text >> List.singleton >> th []

let view =
    table [ attr.``class`` "table" ] [
        thead []
            [ h ""; h "Игрок"; h "По"; h "Сп"; h "Ма"; h "Зло"; h "Ре"; h "Де" ]
        рейтинг
        |> List.sortBy (fun игрок -> игрок.Рептилоид + игрок.Демон)
        |> List.sortBy (fun игрок -> игрок.Подозрительность)
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
                плохость игрок
                вероятность игрок.Рептилоид
                вероятность игрок.Демон
                // вероятность (
                //     игрок.Спаситель
                //     + игрок.Масон
                //     + игрок.Рептилоид
                //     + игрок.Демон
                // )
                // ключиВЯчейку ключи
            ]
        )
        |> tbody []
        // List.fold (fun (с, м, р, д) игрок ->
        //     с + игрок.Спаситель
        //     , м + игрок.Масон
        //     , р + игрок.Рептилоид
        //     , д + игрок.Демон
        // ) (0., 0., 0., 0.) рейтинг
        // |> fun (с, м, р, д) -> tfoot [] [
        //     td [] []
        //     td [] []
        //     td [] []
        //     с / 100. |> вероятность
        //     м / 100. |> вероятность
        //     р / 100. |> вероятность
        //     д / 100. |> вероятность
        //     td [] []
        // ]
    ]
