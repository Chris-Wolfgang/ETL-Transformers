type: fix

`SelectManyTransformer` now surfaces the same exception as LINQ's `SelectMany` when an item fails and disposing its inner sequence also throws: with the default error policy the disposal exception replaces the original one, as with a `foreach` loop. Previously the original exception was kept and the disposal error dropped. A disposal error is still ignored for an item the policy chose to `Skip`.
