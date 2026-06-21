window.BENCHMARK_DATA = {
  "lastUpdate": 1782002002540,
  "repoUrl": "https://github.com/Chris-Wolfgang/ETL-Transformers",
  "entries": {
    "BenchmarkDotNet": [
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "noreply@github.com",
            "name": "GitHub",
            "username": "web-flow"
          },
          "distinct": true,
          "id": "20f9dfb67b5216e8120bd189be82c70dcec2bc08",
          "message": "Merge pull request #122 from Chris-Wolfgang/protected/benchmarks-canonical\n\nchore: adopt canonical benchmarks.yaml workflow",
          "timestamp": "2026-06-20T20:29:03-04:00",
          "tree_id": "27e2308ddca5afe7617116eea556eb5bd4dcb558",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/20f9dfb67b5216e8120bd189be82c70dcec2bc08"
        },
        "date": 1782002001583,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10149206.244791666,
            "unit": "ns",
            "range": "± 5651.901169273028"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5176340.131510417,
            "unit": "ns",
            "range": "± 6592.4246403128445"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10105998.109375,
            "unit": "ns",
            "range": "± 1840.0085863659415"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5169452.989583333,
            "unit": "ns",
            "range": "± 1906.6720486029226"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10126954.802083334,
            "unit": "ns",
            "range": "± 8492.113408950214"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5169924.747395833,
            "unit": "ns",
            "range": "± 1435.7894386553114"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10120951.125,
            "unit": "ns",
            "range": "± 1056.4476438837744"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5173818.444010417,
            "unit": "ns",
            "range": "± 7946.507635463784"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10130469.609375,
            "unit": "ns",
            "range": "± 350.3607212713286"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5159215.6171875,
            "unit": "ns",
            "range": "± 1943.717640091867"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10122764.953125,
            "unit": "ns",
            "range": "± 1093.9091617453391"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5186023.044270833,
            "unit": "ns",
            "range": "± 42272.06283392581"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18788.834243774414,
            "unit": "ns",
            "range": "± 56.61326503423595"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 19832.082407633465,
            "unit": "ns",
            "range": "± 63.09244221732247"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25063.928176879883,
            "unit": "ns",
            "range": "± 72.25816113653467"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 28077.52165730794,
            "unit": "ns",
            "range": "± 63.81759016261734"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34772.64653523763,
            "unit": "ns",
            "range": "± 231.9625769657525"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 38001.69812011719,
            "unit": "ns",
            "range": "± 168.17261402126488"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1781085.41796875,
            "unit": "ns",
            "range": "± 14689.021869436554"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1964157.9837239583,
            "unit": "ns",
            "range": "± 605.8855822267134"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2490856.4674479165,
            "unit": "ns",
            "range": "± 7472.827436407368"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2802767.3971354165,
            "unit": "ns",
            "range": "± 1758.1215238842149"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3247508.7721354165,
            "unit": "ns",
            "range": "± 16730.857994569684"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3616013.4778645835,
            "unit": "ns",
            "range": "± 6233.515789171199"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18413110.614583332,
            "unit": "ns",
            "range": "± 87657.01813891564"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19247743.572916668,
            "unit": "ns",
            "range": "± 37373.90796274257"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26572682.96875,
            "unit": "ns",
            "range": "± 46244.68981664062"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26293650.59375,
            "unit": "ns",
            "range": "± 18593.825629914467"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34769409.13333333,
            "unit": "ns",
            "range": "± 47575.75634562011"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34495836.88888889,
            "unit": "ns",
            "range": "± 164225.26807367"
          }
        ]
      }
    ]
  }
}