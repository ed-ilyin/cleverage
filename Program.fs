open System

type Role = Savior | Mason | Reptiloid | Demon

let rnd = Random ()

let players =
    [   "🍑Забава"; "🤬Щегол"; "👣Маркиза"; "🔱Карабас"; "🪵Гуманоид"
        "🦖Буратино"; "🌈Хитрость Ума"; "🎃Аватар"; "⚧ЗабавныйЖмыхоМаркиз"
        "🍌Командор"
    ]
    |> List.map (fun player -> player, None)
    |> Map.ofList

let times = 10000

let assignRole role table =
    let unassigned =
        Map.filter (fun _ (someRole: _ option) -> someRole.IsNone) table
        |> Map.toList
    let item = List.item (List.length unassigned |> rnd.Next) unassigned |> fst
    Map.add item (Some role) table

let allOthers role =
    Map.map (fun _ someRole ->
        match someRole with None -> role | Some otherRole -> otherRole
    )

let assignRoles table =
    table |> assignRole Savior |> assignRole Reptiloid |> assignRole Reptiloid
    |> assignRole Demon |> allOthers Mason |> Map.toList
    
let step = 100. / float times

let addToStats statistics listOfPlayersRoles =
    List.fold (fun stats playerAndRole ->
        Map.add playerAndRole (
            match Map.tryFind playerAndRole stats with
            None -> step | Some value -> value + step
        ) stats
    ) statistics listOfPlayersRoles

[<EntryPoint>]
let main argv =
    List.init times ignore |> List.fold (fun stats _ ->
        assignRoles players |> addToStats stats
    ) Map.empty |> Map.toList |> List.sortBy fst |> printfn "%A"
    0 // return an integer exit code