window.BENCHMARK_DATA = {
  "lastUpdate": 1782321704414,
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
      }
    ]
  }
}