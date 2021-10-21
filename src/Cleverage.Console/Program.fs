module Cleverage.Console.Program
open System

type Role = Savior | Mason | Reptiloid | Demon

let rnd = Random ()

let players = [
    "🍑Забава"
    "🤬Щегол"
    "👣Маркиза"
    "🔱Карабас"
    "🪵Гуманоид"
    "🦖Буратино"
    "🌈Хитрость Ума"
    "🎃Аватар"
    "♟Жмых"
    "🍌Командор"
]

let times = 1

// let roleMapping (index, (name, savior, reptiloid, demon)) =
//     name
//     , match index with
//         | i when 0 <= i && i < mason -> Savior
//         | i when i < reptiloid -> Mason
//         | i when i < demon -> Reptiloid
//         | _ -> Demon

// let assignRolesAsAvatar table =
//     table
//     |> List.map (fun (player, weight) -> player, rnd.Next())
//     |> List.sortBy snd |> List.map fst |> List.indexed |> List.map roleMapping

// let step = 100. / float times

// let addToStats statistics listOfPlayersRoles =
//     List.fold (fun stats playerAndRole ->
//         Map.add playerAndRole (
//             match Map.tryFind playerAndRole stats with
//             None -> step | Some value -> value + step
//         ) stats
//     ) statistics listOfPlayersRoles

[<EntryPoint>]
let main argv =
    Cleverage.Client.Probability.stats 1000000 |> printfn "%+A"
    // List.init times ignore |> List.fold (fun stats _ ->
    //     assignRolesAsAvatar players |> addToStats stats
    // ) Map.empty |> Map.toList |> List.sortBy fst |> printfn "%A"
    0 // return an integer exit code
