module Cleverage.Client.Probability
open Bolero.Html
open System
open Types

let rnd = Random ()

let roleMapping (index, name) =
    name
    , match index with
        | i when i < 1  -> Спаситель
        | i when i < 7 -> Масон
        | i when i < 9 -> Рептилоид
        | _ -> Демон

let assignRolesAsAvatar =
    List.map <| fun player -> player, rnd.Next()
    >> List.sortBy snd
    >> List.map fst
    >> List.indexed
    >> List.map roleMapping

let countVote table stat player role =
    let vote = Map.remove player table |> Map.toList |> List.item (rnd.Next 9)
    let count = match Map.tryFind vote stat with None -> 1 | Some c -> c + 1
    Map.add vote count stat

let bench votes =
    let mx = List.maxBy snd votes |> snd |> float
    let maxHalfMinus1 = mx / 2. + 1.
    List.filter (snd >> float >> (<=) maxHalfMinus1) votes

let voting playersMap =
    Map.fold (countVote playersMap) Map.empty playersMap |> Map.toList

let numberOfBadRoles bad =
    function Спаситель | Масон -> bad | Рептилоид | Демон -> bad + 1

let игроки = List.map fst Игра.игроки

let run () =
    let playersWithRoles = assignRolesAsAvatar игроки |> Map.ofList
    let roles = voting playersWithRoles |> bench |> List.map (fst >> snd)
    let total = List.length roles
    let bad = List.fold numberOfBadRoles 0 roles
    bad, total

let addToStats stats badOfVoted =
    Map.add badOfVoted
    <| match Map.tryFind badOfVoted stats with None -> 1 | Some v -> v + 1
    <| stats

let stats times =
    let stats =
        List.init times ignore
        |> List.fold (fun stats _ -> run () |> addToStats stats) Map.empty
        |> Map.toList
        |> List.filter (fun ((_, total), _) -> total = 1)
    let k = List.sumBy snd stats |> float |> (/) 100.
    List.map (fun (o, v) -> o, float v * k) stats

// VIEW

let list list =
    List.map (sprintf "%+A" >> text >> List.singleton >> li []) list
    |> span [ attr.``class`` "column" ]

let view () =
    let playersWithRoles = assignRolesAsAvatar игроки
    let roles = Map.ofList playersWithRoles |> voting
    span [ attr.``class`` "columns" ] [
        list roles
        bench roles |> list
        stats 1000 |> list
    ]
