module Cleverage.Client.Matrix
open Bolero.Html
open System

let initMatrix = List.replicate 10 [ 0.1; 0.6; 0.2; 0.1 ]
let rowSums = List.map List.sum
let targetRowSums = rowSums initMatrix
let zeroes = List.replicate (List.head initMatrix |> List.length) 0.
let colSums = List.fold (List.map2 (+)) zeroes
let targetColSums = colSums initMatrix

let setCell x y change =
    List.mapi (fun i row ->
        if i = y then
            List.mapi (fun i value -> if i = x then change value else value) row
        else row
    )

let balanceRows matrix =
    let kr = rowSums matrix |> List.map2 (/) targetRowSums
    List.map2 (fun kr row -> List.map ((*) kr) row) kr matrix

let balanceCols matrix =
    let kc = colSums matrix |> List.map2 (/) targetColSums
    List.map (List.map2 (*) kc) matrix

let log tag x =
    printfn "%s: %+A" tag x
    x

let isBalanced sums targetSums =
    sums
    // >> log "sums"
    >> List.map2 (-) targetSums
    >> List.map (abs >> (>) 0.001)
    >> List.reduce (&&)

let areRowsBalanced = isBalanced rowSums targetRowSums
let areColsBalanced = isBalanced colSums targetColSums

let rec balanceMatrix matrix =
    match areRowsBalanced matrix, areColsBalanced matrix with
    | false, _ -> balanceRows matrix |> balanceMatrix
    | _, false -> balanceCols matrix |> balanceMatrix
    | _ -> matrix

let changeMatrix x y change = setCell x y change >> balanceMatrix

let zero _ = 0.

let newMatrix =
    initMatrix
    |> changeMatrix 0 5 zero
    |> changeMatrix 1 5 zero
    |> changeMatrix 2 3 zero
    |> changeMatrix 2 5 zero
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

let view () = matrixTable newMatrix
