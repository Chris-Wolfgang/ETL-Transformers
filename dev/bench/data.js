window.BENCHMARK_DATA = {
  "lastUpdate": 1786458674924,
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
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "b5f5e4b39ac50ba25ddbd524838ba22e455e1e4d",
          "message": "docs: fix code-review findings (CHANGELOG date, TFM table, Then overloads, slnx cleanup), Closes #38\n\nCo-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>",
          "timestamp": "2026-06-20T21:58:30-04:00",
          "tree_id": "fc290f7ee731470dece59998c4808890dcf6195e",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/b5f5e4b39ac50ba25ddbd524838ba22e455e1e4d"
        },
        "date": 1782007371339,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10114524.380208334,
            "unit": "ns",
            "range": "± 461.0505842037959"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5157890.065104167,
            "unit": "ns",
            "range": "± 973.2994165686148"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10115749.018229166,
            "unit": "ns",
            "range": "± 2161.5897010028916"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5162029.990885417,
            "unit": "ns",
            "range": "± 5953.611772895032"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10117178.770833334,
            "unit": "ns",
            "range": "± 4244.802453037803"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5160375.342447917,
            "unit": "ns",
            "range": "± 7745.568008591503"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10128359.458333334,
            "unit": "ns",
            "range": "± 695.6076017614063"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5168326.203125,
            "unit": "ns",
            "range": "± 2421.8378780043113"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10117772.7421875,
            "unit": "ns",
            "range": "± 3197.0231535272706"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5158436.169270833,
            "unit": "ns",
            "range": "± 4389.95705946745"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10118194.244791666,
            "unit": "ns",
            "range": "± 1428.9954378034315"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5165702.427083333,
            "unit": "ns",
            "range": "± 850.4282759785341"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18840.404810587566,
            "unit": "ns",
            "range": "± 57.01713424900233"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 19164.552693684895,
            "unit": "ns",
            "range": "± 46.55991012102245"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25100.37939961751,
            "unit": "ns",
            "range": "± 58.83389059280607"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 28110.51368713379,
            "unit": "ns",
            "range": "± 87.32549551181557"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34852.095642089844,
            "unit": "ns",
            "range": "± 117.95920215897344"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 37836.61665852865,
            "unit": "ns",
            "range": "± 117.9084484241473"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1729309.8958333333,
            "unit": "ns",
            "range": "± 473.32859227174595"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1945505.62890625,
            "unit": "ns",
            "range": "± 972.4189839701761"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2484867.9615885415,
            "unit": "ns",
            "range": "± 1625.401324777465"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2776592.0182291665,
            "unit": "ns",
            "range": "± 1448.0890238268978"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3227592.6927083335,
            "unit": "ns",
            "range": "± 3775.1165213003337"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3602326.2799479165,
            "unit": "ns",
            "range": "± 2239.727275926281"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18456773.833333332,
            "unit": "ns",
            "range": "± 26772.394493368138"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19174258.96875,
            "unit": "ns",
            "range": "± 12158.939411169553"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26765776.645833332,
            "unit": "ns",
            "range": "± 8963.665152663587"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 28277123.40625,
            "unit": "ns",
            "range": "± 499099.49640699674"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34548026.844444446,
            "unit": "ns",
            "range": "± 34115.7655765929"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34342964.86666667,
            "unit": "ns",
            "range": "± 104088.73932674411"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "894abb8fa13cd57103f4521913b6ce16f88bcbf2",
          "message": "chore: bump version to 0.1.1, update CHANGELOG\n\nCo-Authored-By: Claude Sonnet 4.6 <noreply@anthropic.com>",
          "timestamp": "2026-06-20T22:15:00-04:00",
          "tree_id": "ff8974b7a99c63833cc7042655066720000a55d8",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/894abb8fa13cd57103f4521913b6ce16f88bcbf2"
        },
        "date": 1782008361058,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10128620.828125,
            "unit": "ns",
            "range": "± 4494.414122056079"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5165070.46484375,
            "unit": "ns",
            "range": "± 6292.131010060591"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10124152.46875,
            "unit": "ns",
            "range": "± 1156.493453782847"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5173102.768229167,
            "unit": "ns",
            "range": "± 2102.360433806741"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10122358.40625,
            "unit": "ns",
            "range": "± 353.0344850807779"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5186404.252604167,
            "unit": "ns",
            "range": "± 20845.03391226815"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10118288.583333334,
            "unit": "ns",
            "range": "± 1187.5243703639658"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5176984.143229167,
            "unit": "ns",
            "range": "± 16201.2962614731"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10124573.838541666,
            "unit": "ns",
            "range": "± 3675.43384839917"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5166243.6015625,
            "unit": "ns",
            "range": "± 1109.9720461238064"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10125850.291666666,
            "unit": "ns",
            "range": "± 3981.784901282512"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5169752.283854167,
            "unit": "ns",
            "range": "± 10041.638770266796"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 17362.93537394206,
            "unit": "ns",
            "range": "± 71.50565669099181"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 19967.099411010742,
            "unit": "ns",
            "range": "± 284.81211395160466"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25134.104258219402,
            "unit": "ns",
            "range": "± 51.61007189356248"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 26196.78970336914,
            "unit": "ns",
            "range": "± 52.181448717466196"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34526.13483683268,
            "unit": "ns",
            "range": "± 133.71841036693058"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 36370.42528279623,
            "unit": "ns",
            "range": "± 148.39428202707413"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1860447.1201171875,
            "unit": "ns",
            "range": "± 9473.53179848347"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1936129.3528645833,
            "unit": "ns",
            "range": "± 1347.2812422509196"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2673262.5416666665,
            "unit": "ns",
            "range": "± 928.4651144195193"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2788619.87109375,
            "unit": "ns",
            "range": "± 6374.239910226217"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3222223.109375,
            "unit": "ns",
            "range": "± 2960.7914940012774"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3604227.21875,
            "unit": "ns",
            "range": "± 5490.188696666359"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18493116.84375,
            "unit": "ns",
            "range": "± 15500.580771755578"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19487680.427083332,
            "unit": "ns",
            "range": "± 41040.97624048202"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26973631.59375,
            "unit": "ns",
            "range": "± 265526.60220117855"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26268687.09375,
            "unit": "ns",
            "range": "± 11535.205353307272"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34972617.288888894,
            "unit": "ns",
            "range": "± 528972.1091385602"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34459611.35555556,
            "unit": "ns",
            "range": "± 107299.26959565854"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "aadcb5213a155f2383aa83457fcc306651008d51",
          "message": "feat!: ChunkTransformer yields IReadOnlyList<T> instead of T[]\n\nBREAKING CHANGE: ChunkTransformer<T> now implements\nITransformAsync<T, IReadOnlyList<T>> and TransformAsync returns\nIAsyncEnumerable<IReadOnlyList<T>> (was IAsyncEnumerable<T[]>).\n\nThis hides the array backing store behind a read-only contract so\nconsumers can no longer mutate emitted chunks. Downstream code that\nhard-coded the element type (e.g. SelectTransformer<T[], _>,\nTestLoader<T[]>, or chunk.Length) must switch to IReadOnlyList<T> /\n.Count. The integration test and LinqOps example are updated to show\nthe migration.\n\nNote: ITransformAsync's TDestination is covariant, so consumers that\nonly need IReadOnlyList<T> were already served by the old T[] return.\nThis change trades that concrete-array capability for an immutable\npublic surface.\n\nUpdated PublicAPI.Shipped.txt for the new return type.\n\nCo-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>",
          "timestamp": "2026-06-24T13:17:19-04:00",
          "tree_id": "a580d35f0080189c1f801c1853866409b9e6b2f1",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/aadcb5213a155f2383aa83457fcc306651008d51"
        },
        "date": 1782321702508,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10131929.442708334,
            "unit": "ns",
            "range": "± 4236.167010373722"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5137979.981770833,
            "unit": "ns",
            "range": "± 1655.2792357813883"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10080852.765625,
            "unit": "ns",
            "range": "± 228.19903924791592"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5135981.684895833,
            "unit": "ns",
            "range": "± 2621.920709248613"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10086507.427083334,
            "unit": "ns",
            "range": "± 1938.7475341204029"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5142404.346354167,
            "unit": "ns",
            "range": "± 4214.4897743240135"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10113551.065104166,
            "unit": "ns",
            "range": "± 60515.740644238875"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5179445.888020833,
            "unit": "ns",
            "range": "± 4760.534245986503"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10080259.65625,
            "unit": "ns",
            "range": "± 3859.2340430673444"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5152383.442708333,
            "unit": "ns",
            "range": "± 7220.010291303378"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10080787.864583334,
            "unit": "ns",
            "range": "± 1438.0827930652276"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5157053.5625,
            "unit": "ns",
            "range": "± 14611.891540151964"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18502.279083251953,
            "unit": "ns",
            "range": "± 62.62003335734767"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 20143.796717325848,
            "unit": "ns",
            "range": "± 38.76422473626104"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 26227.358286539715,
            "unit": "ns",
            "range": "± 287.7738685129792"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 28186.79197184245,
            "unit": "ns",
            "range": "± 50.44695261537568"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 36075.918477376305,
            "unit": "ns",
            "range": "± 291.018391122539"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 36436.21920776367,
            "unit": "ns",
            "range": "± 117.67865035737381"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1824440.603515625,
            "unit": "ns",
            "range": "± 7490.871460054566"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1967725.9453125,
            "unit": "ns",
            "range": "± 1055.672016498423"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2634088.9309895835,
            "unit": "ns",
            "range": "± 7637.779107129612"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2657465.8958333335,
            "unit": "ns",
            "range": "± 2744.444033889259"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3493266.3059895835,
            "unit": "ns",
            "range": "± 1299.261539247725"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3651324.3658854165,
            "unit": "ns",
            "range": "± 5219.856648785816"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18552386.479166668,
            "unit": "ns",
            "range": "± 513485.0255328637"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19708151.4375,
            "unit": "ns",
            "range": "± 95669.13906103007"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26130232.15625,
            "unit": "ns",
            "range": "± 35776.111770769196"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26331691.895833332,
            "unit": "ns",
            "range": "± 112688.66067597768"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 39238785.948717944,
            "unit": "ns",
            "range": "± 265006.32151684735"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 36356045.61904762,
            "unit": "ns",
            "range": "± 528991.0050796294"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "9b8be85e044de753eed521125b438ed6c042e90e",
          "message": "feat: add optional IProgress<int> sink to ChunkTransformer\n\nAdds ChunkTransformer(int size, IProgress<int>? progress). When a sink is\nsupplied, the transformer reports the cumulative count of source items\nconsumed once per yielded chunk (including the partial final chunk). When\nno sink is supplied the hot path reports nothing, preserving the lean\ndefault.\n\nNote: the same per-chunk progress is already composable today via the\nexisting ProgressReportingTransformer<T[]> decorator and .Then(...). This\noverload is a built-in convenience for that pattern; it does not enable\nanything composition could not already do.\n\nAdds 5 unit tests (cumulative counts, partial final chunk, empty source,\nnull sink, size validation) and records the new ctor in\nPublicAPI.Unshipped.txt.\n\nCo-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>",
          "timestamp": "2026-06-24T14:19:02-04:00",
          "tree_id": "f45a28ea684b935bf884bf0ef55cca4c93017814",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/9b8be85e044de753eed521125b438ed6c042e90e"
        },
        "date": 1782325411047,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10068956.53125,
            "unit": "ns",
            "range": "± 2939.120261095093"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5166199.192708333,
            "unit": "ns",
            "range": "± 30200.288578897347"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10083268.40625,
            "unit": "ns",
            "range": "± 1459.8797883174802"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5142455.111979167,
            "unit": "ns",
            "range": "± 4997.33453517385"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10090637.411458334,
            "unit": "ns",
            "range": "± 10056.509028007864"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5148520.203125,
            "unit": "ns",
            "range": "± 603.8382649414656"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10082871.947916666,
            "unit": "ns",
            "range": "± 1191.5106794738058"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5139555.213541667,
            "unit": "ns",
            "range": "± 2177.302239332468"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10074353.270833334,
            "unit": "ns",
            "range": "± 5073.577093892362"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5155088.916666667,
            "unit": "ns",
            "range": "± 12799.668110350665"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10112223.919270834,
            "unit": "ns",
            "range": "± 53636.73374340319"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5152658.106770833,
            "unit": "ns",
            "range": "± 9015.659023421818"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18647.170013427734,
            "unit": "ns",
            "range": "± 43.176463551245796"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 19778.450424194336,
            "unit": "ns",
            "range": "± 52.1215015003805"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25904.43423461914,
            "unit": "ns",
            "range": "± 34.09837419986913"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 26885.938568115234,
            "unit": "ns",
            "range": "± 73.39603022938854"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 36271.85356648763,
            "unit": "ns",
            "range": "± 162.04831754373612"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 40094.843434651695,
            "unit": "ns",
            "range": "± 90.34461695255963"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1706629.1399739583,
            "unit": "ns",
            "range": "± 34378.29900487991"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1997199.5364583333,
            "unit": "ns",
            "range": "± 2248.3220554126447"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2584775.046875,
            "unit": "ns",
            "range": "± 1616.0297027114868"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2678628.9622395835,
            "unit": "ns",
            "range": "± 859.5586301177033"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3502269.6197916665,
            "unit": "ns",
            "range": "± 32572.027241963144"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3641265.1640625,
            "unit": "ns",
            "range": "± 55118.940633276165"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18368682.645833332,
            "unit": "ns",
            "range": "± 77205.85306928857"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19429004.854166668,
            "unit": "ns",
            "range": "± 47433.88129833716"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 27432576.041666668,
            "unit": "ns",
            "range": "± 201240.99056730972"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 27798289.489583332,
            "unit": "ns",
            "range": "± 106248.57719958358"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 36805164.69047619,
            "unit": "ns",
            "range": "± 320773.4981912297"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 37227059.666666664,
            "unit": "ns",
            "range": "± 126822.08308879255"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "49699333+dependabot[bot]@users.noreply.github.com",
            "name": "dependabot[bot]",
            "username": "dependabot[bot]"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "2ecd3237e2d2c3d5f7d0be4733932203645dabca",
          "message": "build(deps): bump actions/checkout in the github-actions group\n\nBumps the github-actions group with 1 update: [actions/checkout](https://github.com/actions/checkout).\n\n\nUpdates `actions/checkout` from 6 to 7\n- [Release notes](https://github.com/actions/checkout/releases)\n- [Changelog](https://github.com/actions/checkout/blob/main/CHANGELOG.md)\n- [Commits](https://github.com/actions/checkout/compare/v6...v7)\n\n---\nupdated-dependencies:\n- dependency-name: actions/checkout\n  dependency-version: '7'\n  dependency-type: direct:production\n  update-type: version-update:semver-major\n  dependency-group: github-actions\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>",
          "timestamp": "2026-06-24T21:19:01-04:00",
          "tree_id": "cc27607374f92e4a5c01e5d7d72bcbbe6d206993",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/2ecd3237e2d2c3d5f7d0be4733932203645dabca"
        },
        "date": 1782350612115,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10138742.958333334,
            "unit": "ns",
            "range": "± 3905.6178541730483"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5191732.627604167,
            "unit": "ns",
            "range": "± 58486.26560353997"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10088711.927083334,
            "unit": "ns",
            "range": "± 1274.061312347425"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5153206.65625,
            "unit": "ns",
            "range": "± 2521.2833963643825"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10078559.875,
            "unit": "ns",
            "range": "± 4836.905138474758"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5149503.885416667,
            "unit": "ns",
            "range": "± 2165.1703331783433"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10063746.197916666,
            "unit": "ns",
            "range": "± 2418.512310992281"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5141826.890625,
            "unit": "ns",
            "range": "± 2093.512508233793"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10132262.458333334,
            "unit": "ns",
            "range": "± 5361.01274441997"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5159848.80078125,
            "unit": "ns",
            "range": "± 7296.121497376533"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10079610.244791666,
            "unit": "ns",
            "range": "± 4081.062901421003"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5164490.80859375,
            "unit": "ns",
            "range": "± 13776.210018065674"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18251.460540771484,
            "unit": "ns",
            "range": "± 63.34598192540369"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 19661.94054667155,
            "unit": "ns",
            "range": "± 46.26334012471331"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25636.930231730144,
            "unit": "ns",
            "range": "± 51.639835208921006"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 28301.934326171875,
            "unit": "ns",
            "range": "± 111.27179717218236"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 35666.40889485677,
            "unit": "ns",
            "range": "± 97.8850182946363"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 37195.25663248698,
            "unit": "ns",
            "range": "± 228.97820541397994"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1811288.73046875,
            "unit": "ns",
            "range": "± 1237.7320006311352"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 1942377.2607421875,
            "unit": "ns",
            "range": "± 5778.665523943211"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2582029.7799479165,
            "unit": "ns",
            "range": "± 1274.692622791024"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 2685178.1627604165,
            "unit": "ns",
            "range": "± 1239.406912895287"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3462825.1263020835,
            "unit": "ns",
            "range": "± 1857.9919864138171"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 3612820.4088541665,
            "unit": "ns",
            "range": "± 8139.934405483232"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18563140.802083332,
            "unit": "ns",
            "range": "± 133132.79756840892"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 19601110.208333332,
            "unit": "ns",
            "range": "± 23177.35223516996"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 25778430.666666668,
            "unit": "ns",
            "range": "± 14141.744787890684"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26581841.354166668,
            "unit": "ns",
            "range": "± 20338.658362191898"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35956757.08888888,
            "unit": "ns",
            "range": "± 71115.84629897345"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 36559718.71428571,
            "unit": "ns",
            "range": "± 42314.88812794581"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "588c6bcbaeceafa6cd75636d1bd3553b30d632e2",
          "message": "chore: bump Abstractions to 0.14.1 and TestKit to 0.9.0\n\n- Wolfgang.Etl.Abstractions 0.12.0 -> 0.14.1 (src; two-minor jump, API compatible)\n- Microsoft.Bcl.AsyncInterfaces 10.0.5 -> 10.0.9 (src + both test projects)\n- Wolfgang.Etl.TestKit 0.8.0 -> 0.9.0 (examples: BasicChain, LinqOps)\n\nClean bump: no dispose-pattern or Report-property collisions. All 262\nunit tests pass; multi-TFM Release build and both examples build clean.\n\nCo-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>",
          "timestamp": "2026-06-25T20:57:32-04:00",
          "tree_id": "fb6de1389c0c274e7d4cdf30b225c97635b14952",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/588c6bcbaeceafa6cd75636d1bd3553b30d632e2"
        },
        "date": 1782435709396,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10078107.817708334,
            "unit": "ns",
            "range": "± 2952.97715206675"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5139556.010416667,
            "unit": "ns",
            "range": "± 2757.22547225103"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10132966.46875,
            "unit": "ns",
            "range": "± 7283.995752893726"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5151041.520833333,
            "unit": "ns",
            "range": "± 4905.201646257779"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10079492.140625,
            "unit": "ns",
            "range": "± 2077.349491812298"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5146661.15625,
            "unit": "ns",
            "range": "± 1754.8280909665239"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10090574.463541666,
            "unit": "ns",
            "range": "± 19649.61658870445"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5135990.53125,
            "unit": "ns",
            "range": "± 2042.3036224726102"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10081627.140625,
            "unit": "ns",
            "range": "± 1764.1286924998335"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5138603.260416667,
            "unit": "ns",
            "range": "± 1267.4248684397373"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10082130.979166666,
            "unit": "ns",
            "range": "± 1515.779160517879"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5161404.901041667,
            "unit": "ns",
            "range": "± 15967.815544898407"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18765.142690022785,
            "unit": "ns",
            "range": "± 247.6480029766746"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 21669.464345296223,
            "unit": "ns",
            "range": "± 99.4575841126269"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 26023.776412963867,
            "unit": "ns",
            "range": "± 64.53413012432696"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 40373.414723714195,
            "unit": "ns",
            "range": "± 439.24825170366415"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 35622.01066080729,
            "unit": "ns",
            "range": "± 119.76849248058"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 50997.72615559896,
            "unit": "ns",
            "range": "± 253.27008300999802"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1827672.4986979167,
            "unit": "ns",
            "range": "± 3906.2056904915316"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2160223.58984375,
            "unit": "ns",
            "range": "± 737.0182283028745"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2547430.8411458335,
            "unit": "ns",
            "range": "± 1020.8963944427655"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3453880.7942708335,
            "unit": "ns",
            "range": "± 5986.063705044598"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3494986.4296875,
            "unit": "ns",
            "range": "± 1385.4094795879673"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 4993933.088541667,
            "unit": "ns",
            "range": "± 2626.905299626612"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18389925.302083332,
            "unit": "ns",
            "range": "± 16204.526415327666"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21929189.375,
            "unit": "ns",
            "range": "± 22034.210458695496"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26674193.270833332,
            "unit": "ns",
            "range": "± 37817.43377913152"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 36063238.428571425,
            "unit": "ns",
            "range": "± 230260.59518271583"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35568579.11111111,
            "unit": "ns",
            "range": "± 240166.11605263228"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 50456481.96666667,
            "unit": "ns",
            "range": "± 160492.2325613995"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "committer": {
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com",
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang"
          },
          "distinct": true,
          "id": "8437f34b0c5eac56b2b0fc114bdb1555265bd8fa",
          "message": "chore: prepare v0.2.0 release\n\n- Bump <Version>/<AssemblyVersion>/<FileVersion> 0.1.1 -> 0.2.0\n- Add CHANGELOG [0.2.0] section (breaking ChunkTransformer IReadOnlyList<T>\n  return, optional IProgress<int> sink, Abstractions 0.14.1 / Bcl 10.0.9)\n\nMINOR bump: driven by the breaking ChunkTransformer return-type change on 0.x.\n\nCo-Authored-By: Claude Opus 4.8 <noreply@anthropic.com>",
          "timestamp": "2026-06-26T11:51:07-04:00",
          "tree_id": "63f5657193b0f3ca70fd083758ba28ae6bc24d5f",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/8437f34b0c5eac56b2b0fc114bdb1555265bd8fa"
        },
        "date": 1782489327799,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10061653.986979166,
            "unit": "ns",
            "range": "± 2046.1052218042603"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5166172.5234375,
            "unit": "ns",
            "range": "± 6910.239502024644"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10065230.494791666,
            "unit": "ns",
            "range": "± 11978.819063928679"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5176587.30859375,
            "unit": "ns",
            "range": "± 16264.68601353735"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10057377.190104166,
            "unit": "ns",
            "range": "± 4289.35037835501"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5182048.276041667,
            "unit": "ns",
            "range": "± 5430.541797768018"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10054152.364583334,
            "unit": "ns",
            "range": "± 5981.22262835653"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5169255.99609375,
            "unit": "ns",
            "range": "± 6977.41868151054"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10053660.9921875,
            "unit": "ns",
            "range": "± 367.50297983220065"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5167499.235677083,
            "unit": "ns",
            "range": "± 7846.316796210808"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10060167.1953125,
            "unit": "ns",
            "range": "± 3481.6187267206546"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5169782.567708333,
            "unit": "ns",
            "range": "± 9667.48685532702"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 20578.135904947918,
            "unit": "ns",
            "range": "± 238.28776024753458"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 29002.9341023763,
            "unit": "ns",
            "range": "± 25.36774492668921"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 28970.932647705078,
            "unit": "ns",
            "range": "± 11.34027820964479"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 44352.26678466797,
            "unit": "ns",
            "range": "± 131.69058507958275"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 38117.92785644531,
            "unit": "ns",
            "range": "± 330.9691889702068"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 67275.15201822917,
            "unit": "ns",
            "range": "± 379.2647012083641"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 2086069.689453125,
            "unit": "ns",
            "range": "± 17644.912002497113"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2927143.5143229165,
            "unit": "ns",
            "range": "± 11701.814631745168"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2849715.9329427085,
            "unit": "ns",
            "range": "± 20185.092357056375"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 4485120.32421875,
            "unit": "ns",
            "range": "± 15176.424171651188"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3846316.1100260415,
            "unit": "ns",
            "range": "± 4550.5973666247955"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 6168619.3203125,
            "unit": "ns",
            "range": "± 13290.842336012132"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 20464721.197916668,
            "unit": "ns",
            "range": "± 145623.76594576816"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 30089764.583333332,
            "unit": "ns",
            "range": "± 155218.06102060553"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 28205674.354166668,
            "unit": "ns",
            "range": "± 97198.95017876396"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 44176022.88888889,
            "unit": "ns",
            "range": "± 112626.21255810151"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 38224910.71428572,
            "unit": "ns",
            "range": "± 79169.7871661522"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 61928341,
            "unit": "ns",
            "range": "± 621835.2326088253"
          }
        ]
      },
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
          "id": "a491cfca5bc530dcb93320c574e3e880f0eaebc8",
          "message": "Merge pull request #141 from Chris-Wolfgang/dependabot/nuget/dotnet-dependencies-a2b6323b9f\n\nBump the dotnet-dependencies group with 7 updates",
          "timestamp": "2026-07-09T16:50:59-04:00",
          "tree_id": "e7dad5d345b92d5e5aad55fe4105a89feec17c73",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/a491cfca5bc530dcb93320c574e3e880f0eaebc8"
        },
        "date": 1783630515835,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10081139.4609375,
            "unit": "ns",
            "range": "± 1363.8789642419017"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5154235.75390625,
            "unit": "ns",
            "range": "± 2543.8218622637773"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10083793.229166666,
            "unit": "ns",
            "range": "± 3090.5892954180235"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5149975.515625,
            "unit": "ns",
            "range": "± 1611.8251424378893"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10084075.760416666,
            "unit": "ns",
            "range": "± 758.7536813796435"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5139242.49609375,
            "unit": "ns",
            "range": "± 1182.8816913640012"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10078222.427083334,
            "unit": "ns",
            "range": "± 1994.641938047749"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5149623.231770833,
            "unit": "ns",
            "range": "± 1970.1992115486314"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10081415.822916666,
            "unit": "ns",
            "range": "± 2602.972310447872"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5144827.731770833,
            "unit": "ns",
            "range": "± 10588.989449176388"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10060498.567708334,
            "unit": "ns",
            "range": "± 3543.5775240604926"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5154571.71875,
            "unit": "ns",
            "range": "± 23844.72240357092"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18715.807525634766,
            "unit": "ns",
            "range": "± 72.66632423453899"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 22122.324432373047,
            "unit": "ns",
            "range": "± 201.6372923549098"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25939.891031901043,
            "unit": "ns",
            "range": "± 34.58910371760876"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 44304.83435058594,
            "unit": "ns",
            "range": "± 105.73159517361566"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34976.46315511068,
            "unit": "ns",
            "range": "± 141.16273138385347"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 56200.64400227865,
            "unit": "ns",
            "range": "± 44.56202479313813"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1756003.8665364583,
            "unit": "ns",
            "range": "± 29015.29418664741"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2142259.5598958335,
            "unit": "ns",
            "range": "± 903.6853828288412"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2632695.08203125,
            "unit": "ns",
            "range": "± 1599.6302810609195"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3522327.4231770835,
            "unit": "ns",
            "range": "± 2145.4029638458705"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3496855.0234375,
            "unit": "ns",
            "range": "± 11578.665745565204"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 4979691.122395833,
            "unit": "ns",
            "range": "± 6004.354909662227"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18022619.40625,
            "unit": "ns",
            "range": "± 15848.353475111233"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21242010.78125,
            "unit": "ns",
            "range": "± 168017.2686093204"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 25384927.520833332,
            "unit": "ns",
            "range": "± 14136.034569977777"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35241424.97777778,
            "unit": "ns",
            "range": "± 239981.04388092857"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 36412721.62222222,
            "unit": "ns",
            "range": "± 29064.419325989256"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 49647365.63333333,
            "unit": "ns",
            "range": "± 158600.98569026266"
          }
        ]
      },
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
          "id": "e109dde3dab624742633143e8ae571f92c18d612",
          "message": "Merge pull request #142 from Chris-Wolfgang/chore/release-v0.2.1\n\nchore: release v0.2.1",
          "timestamp": "2026-07-11T20:53:42-04:00",
          "tree_id": "c9b71880283d29931cdfef23a918284180dc4117",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/e109dde3dab624742633143e8ae571f92c18d612"
        },
        "date": 1783817879832,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10119247.932291666,
            "unit": "ns",
            "range": "± 3996.5823605954606"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5137085.84375,
            "unit": "ns",
            "range": "± 3831.863103400314"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10071837.677083334,
            "unit": "ns",
            "range": "± 1764.1278891593704"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5131507.963541667,
            "unit": "ns",
            "range": "± 1503.8048592716011"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10116423.364583334,
            "unit": "ns",
            "range": "± 2220.68589891809"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5141748.78125,
            "unit": "ns",
            "range": "± 6137.909396047596"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10136638.869791666,
            "unit": "ns",
            "range": "± 15802.727379581735"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5136970.609375,
            "unit": "ns",
            "range": "± 1247.0197869561086"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10069491.736979166,
            "unit": "ns",
            "range": "± 3289.1415227418156"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5140161.8359375,
            "unit": "ns",
            "range": "± 1410.3822808592076"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10105810.09375,
            "unit": "ns",
            "range": "± 46183.0511615948"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5132655.395833333,
            "unit": "ns",
            "range": "± 688.5390982828027"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18842.178517659504,
            "unit": "ns",
            "range": "± 66.36902408613571"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 21701.29916890462,
            "unit": "ns",
            "range": "± 87.43910529232998"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25690.74351501465,
            "unit": "ns",
            "range": "± 60.10892664771992"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 35466.051920572914,
            "unit": "ns",
            "range": "± 90.50768022215435"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 36038.2705078125,
            "unit": "ns",
            "range": "± 259.3817847297438"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 50347.93295288086,
            "unit": "ns",
            "range": "± 9.535568766947561"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1923183.111328125,
            "unit": "ns",
            "range": "± 1370.8127831874565"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2147415.1901041665,
            "unit": "ns",
            "range": "± 3343.6449168499335"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2579491.0377604165,
            "unit": "ns",
            "range": "± 8863.11943599737"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3569357.5325520835,
            "unit": "ns",
            "range": "± 3545.13492918715"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3497683.6692708335,
            "unit": "ns",
            "range": "± 3186.660491900933"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5028976.567708333,
            "unit": "ns",
            "range": "± 40603.48308404308"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 17889243.6875,
            "unit": "ns",
            "range": "± 43982.69347155419"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21232057.041666668,
            "unit": "ns",
            "range": "± 67453.84562015416"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26213606.177083332,
            "unit": "ns",
            "range": "± 25751.987782429907"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 34296438.42222222,
            "unit": "ns",
            "range": "± 61842.73460883764"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35352375.666666664,
            "unit": "ns",
            "range": "± 53714.673707865506"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 50625022.9,
            "unit": "ns",
            "range": "± 210211.4845154061"
          }
        ]
      },
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
          "id": "d38f478c14226be0283dea637b2633e919201d70",
          "message": "Merge pull request #165 from Chris-Wolfgang/dependabot/github_actions/github-actions-25f4b29175\n\nbuild(deps): bump actions/setup-dotnet from 5 to 6 in the github-actions group",
          "timestamp": "2026-07-21T16:11:55-04:00",
          "tree_id": "96151f627db23562b6b4dc0c5622ae26ce955a4d",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/d38f478c14226be0283dea637b2633e919201d70"
        },
        "date": 1784664970702,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10123729.427083334,
            "unit": "ns",
            "range": "± 1041.1304781076487"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5168596.088541667,
            "unit": "ns",
            "range": "± 1583.7905187773342"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10117723.755208334,
            "unit": "ns",
            "range": "± 1219.2555127833025"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5189049.8515625,
            "unit": "ns",
            "range": "± 40574.36126845322"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10118781.34375,
            "unit": "ns",
            "range": "± 358.66442624589473"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5162980.899739583,
            "unit": "ns",
            "range": "± 6177.9226650404535"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10115104.447916666,
            "unit": "ns",
            "range": "± 2536.2085667634574"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5172848.385416667,
            "unit": "ns",
            "range": "± 7606.192412775392"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10114899.130208334,
            "unit": "ns",
            "range": "± 921.1762222319891"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5174730.252604167,
            "unit": "ns",
            "range": "± 13493.109778825574"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10115580.700520834,
            "unit": "ns",
            "range": "± 1213.805605194061"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5164511.3984375,
            "unit": "ns",
            "range": "± 1102.3344059390915"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18848.977920532227,
            "unit": "ns",
            "range": "± 23.139966798561634"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 20856.015655517578,
            "unit": "ns",
            "range": "± 110.82242654399907"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25119.689509073894,
            "unit": "ns",
            "range": "± 54.1019341841329"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 35963.45375569662,
            "unit": "ns",
            "range": "± 89.27825629253296"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34973.04619344076,
            "unit": "ns",
            "range": "± 181.46940551789288"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 54185.29717000326,
            "unit": "ns",
            "range": "± 148.92848721607123"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1865823.3645833333,
            "unit": "ns",
            "range": "± 683.3171771547803"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2050249.8111979167,
            "unit": "ns",
            "range": "± 836.7323407248293"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2498173.1888020835,
            "unit": "ns",
            "range": "± 4909.398760661274"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3582459.5208333335,
            "unit": "ns",
            "range": "± 2792.6129710688238"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3196038.77734375,
            "unit": "ns",
            "range": "± 2306.1417391277723"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5013544.028645833,
            "unit": "ns",
            "range": "± 1554.869181523386"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 17834975.125,
            "unit": "ns",
            "range": "± 56139.4634049655"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 20592255.354166668,
            "unit": "ns",
            "range": "± 29946.23527847601"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26631020.947916668,
            "unit": "ns",
            "range": "± 50331.06672270754"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35703019.73333333,
            "unit": "ns",
            "range": "± 3351.488513749574"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34644532.86666667,
            "unit": "ns",
            "range": "± 283015.9475878903"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 51094739.633333325,
            "unit": "ns",
            "range": "± 93081.95495929262"
          }
        ]
      },
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
          "id": "8c7ae368efce2bb5c9419031ca3033c64d289ce1",
          "message": "Merge pull request #168 from Chris-Wolfgang/release/0.3.0-code\n\nRelease 0.3.0 (1/2 — code, full CI)",
          "timestamp": "2026-07-21T22:44:23-04:00",
          "tree_id": "ec540d6408064e40cefc62117a44e23a75e4a6dc",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/8c7ae368efce2bb5c9419031ca3033c64d289ce1"
        },
        "date": 1784688516807,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10120374.015625,
            "unit": "ns",
            "range": "± 2101.8872834535514"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5163929.591145833,
            "unit": "ns",
            "range": "± 1401.937739239144"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10135738.78125,
            "unit": "ns",
            "range": "± 25799.651584906875"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5157461.5390625,
            "unit": "ns",
            "range": "± 2003.6058551497915"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10116511.078125,
            "unit": "ns",
            "range": "± 491.52385640578586"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5156098.536458333,
            "unit": "ns",
            "range": "± 2890.886764882682"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10113876.1875,
            "unit": "ns",
            "range": "± 1748.166839249493"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5173263.736979167,
            "unit": "ns",
            "range": "± 9986.674721678002"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10116778.567708334,
            "unit": "ns",
            "range": "± 285.0579934885053"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5164319.416666667,
            "unit": "ns",
            "range": "± 1978.7982199517"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10117090.096354166,
            "unit": "ns",
            "range": "± 1238.7940730615699"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5156494.733072917,
            "unit": "ns",
            "range": "± 1669.0335613980562"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 19115.001251220703,
            "unit": "ns",
            "range": "± 630.0023861908023"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 20881.226282755535,
            "unit": "ns",
            "range": "± 237.893276580392"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 24781.20863342285,
            "unit": "ns",
            "range": "± 64.41603367797768"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 40870.26963297526,
            "unit": "ns",
            "range": "± 71.17682797759812"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34917.55577596029,
            "unit": "ns",
            "range": "± 223.44055022432124"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 52090.550048828125,
            "unit": "ns",
            "range": "± 136.64584277110768"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1772732.2516276042,
            "unit": "ns",
            "range": "± 5057.706869767003"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2055070.64453125,
            "unit": "ns",
            "range": "± 4277.866447099318"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2660640.9700520835,
            "unit": "ns",
            "range": "± 6831.201726300503"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3554389.9401041665,
            "unit": "ns",
            "range": "± 2713.144296208918"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3212390.8151041665,
            "unit": "ns",
            "range": "± 4930.21018906876"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5087616.380208333,
            "unit": "ns",
            "range": "± 1757.3029782438098"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18538263.333333332,
            "unit": "ns",
            "range": "± 98461.53331700199"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 20450557.729166668,
            "unit": "ns",
            "range": "± 51588.7943316766"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26709487.53125,
            "unit": "ns",
            "range": "± 314015.6229424961"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35892338,
            "unit": "ns",
            "range": "± 463431.177478545"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35045476.26666667,
            "unit": "ns",
            "range": "± 244253.86519945046"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 50844526.4,
            "unit": "ns",
            "range": "± 40260.960337650875"
          }
        ]
      },
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
          "id": "072d75a403e76f39068355b04ec4e5d93ceaf110",
          "message": "Merge pull request #176 from Chris-Wolfgang/chore/abstractions-0.20\n\nbuild: bump Abstractions 0.16.0 → 0.20.0 (folds into 0.3.0)",
          "timestamp": "2026-08-02T20:34:26-04:00",
          "tree_id": "983aef869f32191299e7b99ee1612147be3cff13",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/072d75a403e76f39068355b04ec4e5d93ceaf110"
        },
        "date": 1785717530850,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10127391.768229166,
            "unit": "ns",
            "range": "± 1424.0065031724657"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5141567.854166667,
            "unit": "ns",
            "range": "± 3789.2280419405924"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10067235.96875,
            "unit": "ns",
            "range": "± 3842.2660188258474"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5144170.609375,
            "unit": "ns",
            "range": "± 1987.7359137791032"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10074916.489583334,
            "unit": "ns",
            "range": "± 653.832591119689"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5138793.203125,
            "unit": "ns",
            "range": "± 267.9315800796985"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10081322.0625,
            "unit": "ns",
            "range": "± 1442.3818672347952"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5151473.958333333,
            "unit": "ns",
            "range": "± 598.9394285796011"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10081693.1640625,
            "unit": "ns",
            "range": "± 2153.0281029199064"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5153685.479166667,
            "unit": "ns",
            "range": "± 845.3918112535188"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10076860.994791666,
            "unit": "ns",
            "range": "± 3650.1069434760193"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5158828.578125,
            "unit": "ns",
            "range": "± 25646.78518383082"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18606.659881591797,
            "unit": "ns",
            "range": "± 89.80880566148946"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 21791.674082438152,
            "unit": "ns",
            "range": "± 106.46334385100225"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25900.892812093098,
            "unit": "ns",
            "range": "± 73.66874737488598"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 34686.45884195963,
            "unit": "ns",
            "range": "± 155.41073571549717"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 35309.7805074056,
            "unit": "ns",
            "range": "± 117.08996595505697"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 55858.9052734375,
            "unit": "ns",
            "range": "± 75.55242668197972"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1812898.3385416667,
            "unit": "ns",
            "range": "± 24370.698049171704"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2093069.8671875,
            "unit": "ns",
            "range": "± 1347.6857502036148"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2532163.1640625,
            "unit": "ns",
            "range": "± 11769.381778560362"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3418301.671875,
            "unit": "ns",
            "range": "± 1460.6854371455418"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3532883.55859375,
            "unit": "ns",
            "range": "± 8656.628599591064"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5093428.9921875,
            "unit": "ns",
            "range": "± 8431.279182673823"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18428030.354166668,
            "unit": "ns",
            "range": "± 354751.70393958554"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21005043.864583332,
            "unit": "ns",
            "range": "± 42065.37524629554"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26003666.96875,
            "unit": "ns",
            "range": "± 115997.5662642855"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 34678624.02222222,
            "unit": "ns",
            "range": "± 132193.31542825012"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35537996.711111106,
            "unit": "ns",
            "range": "± 23997.667191934117"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 50614421,
            "unit": "ns",
            "range": "± 455326.6381465922"
          }
        ]
      },
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
          "id": "655b628b6168d8bac1df31c066303ee93441cbc1",
          "message": "Merge pull request #169 from Chris-Wolfgang/release/0.3.0-workflows\n\nRelease 0.3.0 (2/2 — workflows, admin-bypass)",
          "timestamp": "2026-08-08T07:40:58-04:00",
          "tree_id": "585f5b663cd801605bb27647403f6b35990c70a4",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/655b628b6168d8bac1df31c066303ee93441cbc1"
        },
        "date": 1786189514321,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10116313.018229166,
            "unit": "ns",
            "range": "± 9895.431824113442"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5135111.533854167,
            "unit": "ns",
            "range": "± 3690.898173420452"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10114100.1484375,
            "unit": "ns",
            "range": "± 1699.9501047508948"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5134185.765625,
            "unit": "ns",
            "range": "± 1098.8881136142459"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10111557.098958334,
            "unit": "ns",
            "range": "± 5374.052155351747"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5139848.572916667,
            "unit": "ns",
            "range": "± 3802.814402930811"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10069559.703125,
            "unit": "ns",
            "range": "± 3042.073143114189"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5133391.25,
            "unit": "ns",
            "range": "± 334.02075386669"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10116209.411458334,
            "unit": "ns",
            "range": "± 6892.143215971094"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5140558.861979167,
            "unit": "ns",
            "range": "± 8811.890390422928"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10066491.651041666,
            "unit": "ns",
            "range": "± 1794.9795960749861"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5145017.6171875,
            "unit": "ns",
            "range": "± 9100.794401739708"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 19307.270741780598,
            "unit": "ns",
            "range": "± 154.61865819519647"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 22165.631998697918,
            "unit": "ns",
            "range": "± 159.79421609059983"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 25960.097513834637,
            "unit": "ns",
            "range": "± 48.90948232309479"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 34148.02380371094,
            "unit": "ns",
            "range": "± 63.64830521398554"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 35393.18428548177,
            "unit": "ns",
            "range": "± 187.66129275831528"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 55883.90756225586,
            "unit": "ns",
            "range": "± 169.09943343876756"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1811328.564453125,
            "unit": "ns",
            "range": "± 1208.533433463549"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2129709.5494791665,
            "unit": "ns",
            "range": "± 124.59750784335432"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2571080.19140625,
            "unit": "ns",
            "range": "± 9543.870514045664"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3668802.6432291665,
            "unit": "ns",
            "range": "± 2970.9136442047966"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3541488.8743489585,
            "unit": "ns",
            "range": "± 4583.798257150378"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5252702.0625,
            "unit": "ns",
            "range": "± 12969.31865563328"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18223520.291666668,
            "unit": "ns",
            "range": "± 64176.13162772857"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21319498.635416668,
            "unit": "ns",
            "range": "± 60486.1427617004"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26147850.729166668,
            "unit": "ns",
            "range": "± 56712.54185878385"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 34390563.177777775,
            "unit": "ns",
            "range": "± 194005.7287351667"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 35868136.52380952,
            "unit": "ns",
            "range": "± 27324.04828162544"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 50044954.9,
            "unit": "ns",
            "range": "± 152179.4180938065"
          }
        ]
      },
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
          "id": "4f204e436f2bc1b7f0d44460caeeea812b9f1e53",
          "message": "Merge pull request #204 from Chris-Wolfgang/protected/0.4.0-workflows\n\nci: 0.4.0 workflow changes (protected-only, ahead of #203)",
          "timestamp": "2026-08-10T18:51:20-04:00",
          "tree_id": "d126bb885473ec6d3093f0840db80c03209b7960",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/4f204e436f2bc1b7f0d44460caeeea812b9f1e53"
        },
        "date": 1786402536543,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10122972.104166666,
            "unit": "ns",
            "range": "± 3482.8750630292816"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5158908.583333333,
            "unit": "ns",
            "range": "± 1161.7370200819714"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10117101.213541666,
            "unit": "ns",
            "range": "± 764.5484086206374"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5155989.38671875,
            "unit": "ns",
            "range": "± 1804.847891428485"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10158545.989583334,
            "unit": "ns",
            "range": "± 73034.68121621457"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5160574.291666667,
            "unit": "ns",
            "range": "± 9947.398440682431"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10120882.8046875,
            "unit": "ns",
            "range": "± 1606.0824138165212"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5156186.580729167,
            "unit": "ns",
            "range": "± 1209.8426962840213"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10129234.916666666,
            "unit": "ns",
            "range": "± 529.6226246990389"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5156364.815104167,
            "unit": "ns",
            "range": "± 939.9054600134106"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10122533.322916666,
            "unit": "ns",
            "range": "± 970.9475996635027"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5166095.803385417,
            "unit": "ns",
            "range": "± 1244.3569530905975"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 17470.490158081055,
            "unit": "ns",
            "range": "± 158.69428273731134"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 20941.854365030926,
            "unit": "ns",
            "range": "± 95.07665310908124"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 24975.779851277668,
            "unit": "ns",
            "range": "± 59.80013305267048"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 36160.72939046224,
            "unit": "ns",
            "range": "± 171.85186061412227"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34813.14833577474,
            "unit": "ns",
            "range": "± 198.179756949057"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 54850.45340983073,
            "unit": "ns",
            "range": "± 202.5428777786197"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1859166.5146484375,
            "unit": "ns",
            "range": "± 293.7513838780048"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2039897.3333333333,
            "unit": "ns",
            "range": "± 158.80699786919004"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2517504.8385416665,
            "unit": "ns",
            "range": "± 818.919461644946"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3446812.2252604165,
            "unit": "ns",
            "range": "± 382.0235566337231"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3209975.1653645835,
            "unit": "ns",
            "range": "± 28695.234633076507"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5696079.9140625,
            "unit": "ns",
            "range": "± 3549.743477094342"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18539260.0625,
            "unit": "ns",
            "range": "± 95701.340608268"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 20242746.947916668,
            "unit": "ns",
            "range": "± 38462.37178162195"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26625120.270833332,
            "unit": "ns",
            "range": "± 62305.177990908924"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35430273.78571429,
            "unit": "ns",
            "range": "± 47696.73250413913"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34374637.62222222,
            "unit": "ns",
            "range": "± 19283.05763125564"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 51855943.6,
            "unit": "ns",
            "range": "± 330753.2018009352"
          }
        ]
      },
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
          "id": "93843fca7ba03ad167305c254f2ae96cb0affb31",
          "message": "Merge pull request #203 from Chris-Wolfgang/vNext\n\nRelease 0.4.0 — observability operators (Tap/Log/Throttle) + test/supply-chain hardening",
          "timestamp": "2026-08-10T20:50:14-04:00",
          "tree_id": "45ae8c82e9471370097b38fb95ecfcb6fe9ef264",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/93843fca7ba03ad167305c254f2ae96cb0affb31"
        },
        "date": 1786409670354,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10082962.473958334,
            "unit": "ns",
            "range": "± 1936.9235499152119"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5152313.34375,
            "unit": "ns",
            "range": "± 2692.3065284804293"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10087771.317708334,
            "unit": "ns",
            "range": "± 2345.3294041036656"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5151435.76953125,
            "unit": "ns",
            "range": "± 4723.847969373734"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10086448.447916666,
            "unit": "ns",
            "range": "± 2023.4747499185512"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5175621.783854167,
            "unit": "ns",
            "range": "± 40701.855984506605"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10082055.942708334,
            "unit": "ns",
            "range": "± 1652.8267468080835"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5167202.135416667,
            "unit": "ns",
            "range": "± 6831.789702832012"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10090296.473958334,
            "unit": "ns",
            "range": "± 2002.3129231150579"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5152434.3828125,
            "unit": "ns",
            "range": "± 2715.240149511465"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10090029.130208334,
            "unit": "ns",
            "range": "± 1009.9184026509374"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5154899.94921875,
            "unit": "ns",
            "range": "± 19354.38886384449"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18625.760599772137,
            "unit": "ns",
            "range": "± 53.91980515782848"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 22217.880981445312,
            "unit": "ns",
            "range": "± 91.15483552305203"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 29629.756281534832,
            "unit": "ns",
            "range": "± 102.47035445499792"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 34627.977783203125,
            "unit": "ns",
            "range": "± 74.19642232270411"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 35567.93125406901,
            "unit": "ns",
            "range": "± 95.80969539037547"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 50351.213246663414,
            "unit": "ns",
            "range": "± 139.39912277031883"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1907111.4375,
            "unit": "ns",
            "range": "± 8472.233188004735"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2116060.0091145835,
            "unit": "ns",
            "range": "± 3962.714764334153"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2533590.0403645835,
            "unit": "ns",
            "range": "± 3293.728387313862"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3540537.0755208335,
            "unit": "ns",
            "range": "± 2166.1996522447353"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3590855.3893229165,
            "unit": "ns",
            "range": "± 16808.625100003777"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5193725.0546875,
            "unit": "ns",
            "range": "± 6239.440174387307"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 17919015.760416668,
            "unit": "ns",
            "range": "± 412087.7184855872"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 21310742.541666668,
            "unit": "ns",
            "range": "± 20081.699614732344"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 28562680.572916668,
            "unit": "ns",
            "range": "± 16861.655433391676"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35238171.199999996,
            "unit": "ns",
            "range": "± 211562.5099861919"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 36542906.06666667,
            "unit": "ns",
            "range": "± 129607.01608266756"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 52663025.333333336,
            "unit": "ns",
            "range": "± 184295.6383623967"
          }
        ]
      },
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
          "id": "876906aa825d2b3dd3fbd4ecd98ec31bbd16be01",
          "message": "Merge pull request #205 from Chris-Wolfgang/chore/baseline-0.4.0\n\nchore: bump PackageValidation baseline 0.3.0 → 0.4.0",
          "timestamp": "2026-08-11T10:26:56-04:00",
          "tree_id": "8e55e1d938503d9bddc1a6b7b1cc0060514acbd3",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/876906aa825d2b3dd3fbd4ecd98ec31bbd16be01"
        },
        "date": 1786458672799,
        "tool": "benchmarkdotnet",
        "benches": [
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 10123283.104166666,
            "unit": "ns",
            "range": "± 2360.3917803040013"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1)",
            "value": 5171107.026041667,
            "unit": "ns",
            "range": "± 2520.311289586917"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 10122903.078125,
            "unit": "ns",
            "range": "± 1606.0274632292203"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8)",
            "value": 5166715.3046875,
            "unit": "ns",
            "range": "± 10071.95932577948"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 10122619.971354166,
            "unit": "ns",
            "range": "± 829.680839829279"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 64)",
            "value": 5164454.958333333,
            "unit": "ns",
            "range": "± 8085.144928405799"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 10121583.783854166,
            "unit": "ns",
            "range": "± 1749.4931563971888"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 256)",
            "value": 5169760.87109375,
            "unit": "ns",
            "range": "± 2271.8061886581972"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 10122361.96875,
            "unit": "ns",
            "range": "± 2085.964489350209"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 1024)",
            "value": 5162913.4609375,
            "unit": "ns",
            "range": "± 4544.182797818305"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.NoBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 10110625.651041666,
            "unit": "ns",
            "range": "± 2674.533739772044"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.BufferedTransformerCapacityBenchmarks.WithBuffer(ItemCount: 100, SourceDelayMicroseconds: 50, SinkDelayMicroseconds: 50, Capacity: 8192)",
            "value": 5160253.192708333,
            "unit": "ns",
            "range": "± 1057.753300172915"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.1)",
            "value": 18842.138041178387,
            "unit": "ns",
            "range": "± 98.92228806972129"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.1)",
            "value": 20886.971974690754,
            "unit": "ns",
            "range": "± 135.10153059931437"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.5)",
            "value": 24689.964274088543,
            "unit": "ns",
            "range": "± 43.18230199473796"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.5)",
            "value": 40511.34216308594,
            "unit": "ns",
            "range": "± 146.39389977794931"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000, PassRate: 0.9)",
            "value": 34805.11414591471,
            "unit": "ns",
            "range": "± 110.41994535156844"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000, PassRate: 0.9)",
            "value": 51949.80821736654,
            "unit": "ns",
            "range": "± 159.68141236404733"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.1)",
            "value": 1862748.4583333333,
            "unit": "ns",
            "range": "± 8617.838879965091"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.1)",
            "value": 2066321.9095052083,
            "unit": "ns",
            "range": "± 188.852707778281"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.5)",
            "value": 2496526.2174479165,
            "unit": "ns",
            "range": "± 18086.69438539348"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.5)",
            "value": 3550429.8294270835,
            "unit": "ns",
            "range": "± 3484.33078608731"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 100000, PassRate: 0.9)",
            "value": 3235578.0846354165,
            "unit": "ns",
            "range": "± 3721.6295895882213"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 100000, PassRate: 0.9)",
            "value": 5118412.677083333,
            "unit": "ns",
            "range": "± 12574.027517991748"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.1)",
            "value": 18733586.364583332,
            "unit": "ns",
            "range": "± 99297.93713961013"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.1)",
            "value": 20428235.583333332,
            "unit": "ns",
            "range": "± 5082.036930947375"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.5)",
            "value": 26725101.083333332,
            "unit": "ns",
            "range": "± 27848.83184224905"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.5)",
            "value": 35509256.80952381,
            "unit": "ns",
            "range": "± 70208.02637211759"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.Lightweight(ItemCount: 1000000, PassRate: 0.9)",
            "value": 34421767.11111111,
            "unit": "ns",
            "range": "± 48653.67124690041"
          },
          {
            "name": "Wolfgang.Etl.Transformers.Benchmarks.WhereBenchmarks.WithBase(ItemCount: 1000000, PassRate: 0.9)",
            "value": 51172477.03703704,
            "unit": "ns",
            "range": "± 103567.79248039865"
          }
        ]
      }
    ]
  }
}