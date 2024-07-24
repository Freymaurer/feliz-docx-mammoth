namespace App

open Feliz
open Feliz.Router

module FileReaderHelper =
  open Fable.Core
  open Fable.Core.JsInterop

  [<Emit("new FileReader()")>]
  let newFileReader(): Browser.Types.FileReader = jsNative

  let readFromFile (file: Browser.Types.File) setState=

    let reader = newFileReader()
    reader.onload <- fun e ->
      let arrayBuffer = e.target?result
      promise {
        let! r = Mammoth.mammoth.convertToHtml({|arrayBuffer = arrayBuffer|})
        setState (Some r.value)
      }
      |> Promise.start

    reader.onerror <- fun e ->
      Browser.Dom.console.error ("Error reading file", e)
    reader.readAsArrayBuffer(file)

type Components =
    /// <summary>
    /// The simplest possible React component.
    /// Shows a header with the text Hello World
    /// </summary>
    [<ReactComponent>]
    static member DisplayHtml(htmlString: string) = 
      Html.div [
        prop.className "content"
        prop.children [
          Html.h1 "Display Html"
          Html.div [
            prop.innerHtml htmlString
          ]
        ]
      ]

    /// <summary>
    /// A stateful React component that maintains a counter
    /// </summary>
    [<ReactComponent>]
    static member UploadDisplay() =
        let filehtml, setFilehtml = React.useState(None)
        let ref = React.useInputRef()
        Html.div [
          prop.className "section" 
          prop.children [
              Html.div [
                  prop.className "container"
                  prop.children [
                    Html.div [
                      prop.className "field"
                      prop.children [
                        Html.input [
                          prop.ref ref
                          prop.type'.file
                          prop.onChange (fun (f: Browser.Types.File) -> 
                            printfn "Hello!"
                            FileReaderHelper.readFromFile f setFilehtml
                            if ref.current.IsSome then
                              ref.current.Value.value <- null
                          )
                        ]
                      ]
                    ]
                    Html.div [
                      prop.className "field"
                      prop.children [
                        match filehtml with
                        | None -> Html.p "No file uploaded"
                        | Some filehtml ->
                          Components.DisplayHtml(filehtml)
                      ]
                    ]
                    
                  ]
              ]
          ]
        ]