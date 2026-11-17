type: fix

`ChunkTransformer<T>` allocates its buffers lazily and its progress count saturates at `int.MaxValue` instead of wrapping negative.
