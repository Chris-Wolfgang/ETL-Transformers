type: fix

`ThrottleTransformer<T>` rejects a `minInterval` longer than `int.MaxValue` milliseconds in its constructor instead of failing mid-enumeration.
