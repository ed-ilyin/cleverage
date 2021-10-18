module Cleverage.Client.Matrix
open Bolero.Html
open System

let initMatrix = List.replicate 10 [ 0.1; 0.6; 0.2; 0.1 ]
let rowSums = List.map List.sum
let targetRowSums = rowSums initMatrix
let zeroes = List.replicate (List.head initMatrix |> List.length) 0.
let colSums = List.fold (List.map2 (+)) zeroes
let targetColSums = colSums initMatrix

let setCell x y newValue =
    List.mapi (fun j -> List.mapi (fun i value ->
        if i = x && j = y then newValue else value
    ))

let balanceRows matrix =
    let kr = rowSums matrix |> List.map2 (/) targetRowSums
    List.map2 (fun kr row -> List.map ((*) kr) row) kr matrix

let balanceCols matrix =
    let kc = colSums matrix |> List.map2 (/) targetColSums
    List.map (List.map2 (*) kc) matrix

let balancedRows =
    rowSums
    >> List.map2 (-) targetRowSums
    >> List.map (abs >> (>) 0.005)
    >> List.reduce (&&)

let balancedCols =
    colSums
    >> List.map2 (-) targetColSums
    >> List.map (abs >> (>) 0.005)
    >> List.reduce (&&)

let rec balanceMatrix matrix =
    match balancedRows matrix, balancedCols matrix with
    | false, _ -> balanceRows matrix |> balanceMatrix
    | _, false -> balanceCols matrix |> balanceMatrix
    | _ -> matrix

let changeMatrix x y newValue = setCell x y newValue >> balanceMatrix

let newMatrix =
    initMatrix
    |> changeMatrix 0 5 0.
    |> changeMatrix 1 5 0.
    |> changeMatrix 2 3 0.
    |> changeMatrix 2 5 0.
    // |> List.map (fun row -> row, List.item 2 row + List.item 3 row)
    // |> List.sortBy snd
    // |> List.map fst

// VIEW

let cell ``class`` =
    (*) 100. >> sprintf "%.0f" >> text >> List.singleton
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
