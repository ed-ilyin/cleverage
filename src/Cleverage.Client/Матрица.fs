module Cleverage.Client.Matrix
open Bolero.Html
open System

let initMatrix = List.replicate 10 [ 0.1; 0.6; 0.2; 0.1 ]

let matrixSum matrix =
    List.fold
    <| List.map2 (+)
    <| List.replicate (List.head matrix |> List.length) 0.
    <| matrix

let total = matrixSum initMatrix

let replace x y newValue i j value = if i = y && j = x then newValue else value
let distributeRow x y k i j value = if i = y && j = x then value else value * k

let distribute x y newValue i row =
    let newRow = List.mapi (replace x y newValue i) row
    let rowBadSum = List.sum row
    let rowOtherSum = List.mapi (replace x y 0. i) row |> List.sum
    let change = rowBadSum - 1.
    let k = 1. - change / rowOtherSum
    let nr = List.mapi (distributeRow x y k i) newRow
    // printfn "%i %i %f %i %+A %+A %f %f %f %f %+A"
        // x y newValue i row newRow rowBadSum rowOtherSum change k nr
    nr

let zeroRow i = List.mapi (fun j row ->
    if i = j then List.replicate (List.length row) 0. else row
)

let distributeRows y k i row =
    let nr = if i = y then row else List.map2 (*) row k
    printfn "%i %+A %+A" i row nr
    nr

let changeMatrix x y newValue matrix =
    let matrix = List.mapi (distribute x y newValue) matrix
    let badSum = matrixSum matrix
    let otherSum = zeroRow y matrix |> matrixSum
    let k = List.map3 (fun b s o -> 1. - (b - s) / o) badSum total otherSum
    printfn "%i %+A" y k
    List.mapi (distributeRows y k) matrix

let newMatrix =
    initMatrix
    |> changeMatrix 0 5 0.
    |> changeMatrix 0 5 0.
    |> changeMatrix 1 5 0.
    |> changeMatrix 1 5 0.
    |> changeMatrix 2 3 0.
    |> changeMatrix 2 3 0.
    |> changeMatrix 2 5 0.
    |> changeMatrix 2 5 0.
    // |> List.map (fun row -> row, List.item 2 row + List.item 3 row)
    // |> List.sortBy snd
    // |> List.map fst

// VIEW

let cell ``class`` =
    (*) 100. >> sprintf "%.2f" >> text >> List.singleton
    >> td [ attr.``class`` ``class`` ]

let sum = List.map2 (+)

let row row =
    List.map (cell "") row
    @ [ List.fold (+) 0. row |> cell "is-info" ]
    |> tr []

let matrixTable matrix =
    table [ attr.``class`` "table" ] [
        List.map row matrix |> tbody []
        List.fold sum (List.replicate (List.head matrix |> List.length) 0.) matrix
        |> List.map (cell "is-info")
        |> tfoot []
    ]

let view = matrixTable newMatrix
