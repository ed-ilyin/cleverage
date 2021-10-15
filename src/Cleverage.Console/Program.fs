open System

type Role = Savior | Mason | Reptiloid | Demon

let rnd = Random ()

let players = [
    "🍑Забава",       1, 7, 9
    "🤬Щегол",        1, 7, 9
    "👣Маркиза",      1, 7, 9
    "🔱Карабас",      1, 7, 9
    "🪵Гуманоид",     1, 7, 9
    "🦖Буратино",     1, 7, 9
    "🌈Хитрость Ума", 1, 7, 9
    "🎃Аватар",       1, 7, 9
    "♟Жмых",         1, 6, 9
    "🍌Командор",     1, 7, 9
]

let times = 100000

let roleMapping (index, (name, savior, reptiloid, demon)) =
    name
    , match index with
        | i when 0 <= i && i < mason -> Savior
        | i when i < reptiloid -> Mason
        | i when i < demon -> Reptiloid
        | _ -> Demon

let assignRolesAsAvatar table =
    table
    |> List.map (fun (player, weight) -> player, rnd.Next())
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
