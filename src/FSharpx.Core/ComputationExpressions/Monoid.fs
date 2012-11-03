﻿module FSharpx.Monoid

type ISemigroup<'a> =
    /// <summary>
    /// Associative operation
    /// </summary>
    abstract append : 'a -> 'a -> 'a
    
/// Monoid (associative binary operation with identity)
/// The monoid implementation comes from Matthew Podwysocki's http://codebetter.com/blogs/matthew.podwysocki/archive/2010/02/01/a-kick-in-the-monads-writer-edition.aspx.
[<AbstractClass>]
type Monoid<'a>() as m =
    /// <summary>
    /// Identity
    /// </summary>
    abstract mempty : 'a

    /// <summary>
    /// Associative operation
    /// </summary>
    abstract mappend : 'a -> 'a -> 'a

    /// <summary>
    /// Fold a list using this monoid
    /// </summary>
    abstract mconcat : 'a seq -> 'a
    default x.mconcat a = Seq.fold x.mappend x.mempty a

    interface ISemigroup<'a> with
        member x.append a b = m.mappend a b

/// List monoid
type ListMonoid<'a>() =
    inherit Monoid<'a list>()
        override this.mempty = []
        override this.mappend a b = a @ b

/// The dual of a monoid, obtained by swapping the arguments of 'mappend'.
type DualMonoid<'a>(m: 'a Monoid) =
    inherit Monoid<'a>()
        override this.mempty = m.mempty
        override this.mappend a b = m.mappend b a

/// Option wrapper monoid
type OptionMonoid<'a>(m: 'a Monoid) =
    inherit Monoid<'a option>()
        override this.mempty = None
        override this.mappend a b = 
            match a,b with
            | Some a, Some b -> Some (m.mappend a b)
            | Some a, None   -> Some a
            | None, Some a   -> Some a
            | None, None     -> None

type Tuple2Monoid<'a,'b>(a: 'a Monoid, b: 'b Monoid) =
    inherit Monoid<'a * 'b>()
        override this.mempty = a.mempty, b.mempty
        override this.mappend x y = a.mappend (fst x) (fst y), b.mappend (snd x) (snd y)

type Tuple3Monoid<'a,'b,'c>(a: 'a Monoid, b: 'b Monoid, c: 'c Monoid) =
    inherit Monoid<'a * 'b * 'c>()
        override this.mempty = a.mempty, b.mempty, c.mempty
        override this.mappend x y =
            let a1,b1,c1 = x
            let a2,b2,c2 = y
            a.mappend a1 a2, b.mappend b1 b2, c.mappend c1 c2
            
/// Monoid (int,0,+)
let IntSumMonoid = 
    { new Monoid<int>() with
        override this.mempty = 0
        override this.mappend a b = a + b }

/// Monoid (int,1,*)
let IntProductMonoid =
    { new Monoid<int>() with
        override this.mempty = 1
        override this.mappend a b = a * b }

let StringMonoid =
    { new Monoid<string>() with
        override this.mempty = ""
        override this.mappend a b = a + b }

let AllMonoid =
    { new Monoid<bool>() with
        override this.mempty = true
        override this.mappend a b = a && b }

let AnyMonoid = 
    { new Monoid<bool>() with
        override this.mempty = false
        override this.mappend a b = a || b }

// doesn't compile due to this F# bug http://stackoverflow.com/questions/4485445/f-interface-inheritance-failure-due-to-unit
(*
let UnitMonoid =
    { new Monoid<unit>() with
        override this.mempty = ()
        override this.mappend _ _ = () }
*)

type NonEmptyListSemigroup<'a>() =
    interface ISemigroup<'a NonEmptyList> with
        member x.append a b = NonEmptyList.append a b