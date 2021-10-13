open System

type Role = Savior | Mason | Reptiloid | Demon

let rnd = Random ()

let players = [
    "🍑Забава",       [ 1; 1; 1; 1 ]
    "🤬Щегол",        [ 1; 1; 1; 1 ]
    "👣Маркиза",      [ 1; 1; 1; 1 ]
    "🔱Карабас",      [ 1; 1; 1; 1 ]
    "🪵Гуманоид",     [ 1; 1; 1; 1 ]
    "🦖Буратино",     [ 1; 1; 1; 1 ]
    "🌈Хитрость Ума", [ 1; 1; 1; 1 ]
    "🎃Аватар",       [ 1; 1; 1; 1 ]
    "♟Жмых",         [ 1; 1; 1; 1 ]
    "🍌Командор",     [ 1; 1; 1; 1 ]
]

let times = 100000

let roleMapping (index, player) =
    player
    , match index with
        | i when i = 0 -> Savior
        | i when i < 7 -> Mason
        | i when i < 9 -> Reptiloid
        | _ -> Demon

let assignRolesAsAvatar table =
    table
    |> List.map (fun (player, weight) -> player, rnd.Next(0, 1000) + weight)
    |> List.sortBy snd |> List.map fst |> List.indexed |> List.map roleMapping

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
        assignRolesAsAvatar players |> addToStats stats
    ) Map.empty |> Map.toList |> List.sortBy fst |> printfn "%A"
    0 // return an integer exit code
