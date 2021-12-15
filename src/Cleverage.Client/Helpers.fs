namespace Cleverage.Helpers

module Result =
    let join result =
        match result with
        | Ok (Ok ok) -> Ok ok
        | Ok (Error er) | Error er -> Error er
