module FSExperiment.Reactive

type IObserver<'a> =
    abstract OnNext : 'a -> unit

type IObservable<'a> =
    abstract Subscribe : IObserver<'a> -> unit


[<AbstractClass; Sealed>]
type Observer =
    static member Create(action : 'a -> unit) =
        { new IObserver<'a> with
            member __.OnNext(x) = action(x)
        }

[<AbstractClass; Sealed>]
type Observable =
    static member Create(action : IObserver<'a> -> unit) =
        { new IObservable<'a> with
            member __.Subscribe(o) = action(o)
        }


[<AutoOpen>]
module Extension =
    type IObservable<'a> with
        member this.Subscribe(action) =
            this.Subscribe( Observer.Create(action) )

        member this.Select(f : 'a -> 'b) =
            Observable.Create(fun o ->
                this.Subscribe(f >> o.OnNext)
            )

        member this.Where(f : 'a -> bool) =
            Observable.Create(fun o ->
                this.Subscribe(fun x ->
                    if f x then
                        o.OnNext(x)
                )
            )