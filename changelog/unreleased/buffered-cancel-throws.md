type: fix

Cancelling a `BufferedTransformer` stage now always ends the enumeration with `OperationCanceledException`. Previously the sequence could end quietly instead, depending on the target framework and on whether the source honoured the token by returning early, so a cancelled run could be mistaken for a complete one.
