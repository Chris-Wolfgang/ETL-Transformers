window.BENCHMARK_DATA = {
  "lastUpdate": 1795357310628,
  "repoUrl": "https://github.com/Chris-Wolfgang/ETL-Transformers",
  "entries": {
    "Mutation score": [
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "1fcc92bc573218440c4f43f5b73d3570bb661c41",
          "message": "chore(pack): advance PackageValidation baseline to 0.8.0 (#345)\n\nv0.8.0 is published and indexed on the NuGet flatcontainer CDN, so the\nnext release validates its API surface against what actually shipped.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-26T20:30:16Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/1fcc92bc573218440c4f43f5b73d3570bb661c41"
        },
        "date": 1790490885150,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 92.06,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "1fcc92bc573218440c4f43f5b73d3570bb661c41",
          "message": "chore(pack): advance PackageValidation baseline to 0.8.0 (#345)\n\nv0.8.0 is published and indexed on the NuGet flatcontainer CDN, so the\nnext release validates its API surface against what actually shipped.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-09-26T20:30:16Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/1fcc92bc573218440c4f43f5b73d3570bb661c41"
        },
        "date": 1791101934043,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 92.06,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "dependabot[bot]",
            "username": "dependabot[bot]",
            "email": "49699333+dependabot[bot]@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "c147338d9456bb263dcd1f03dc574654e7a468c0",
          "message": "build(deps): bump urllib3 from 2.7.0 to 2.8.0 in /.github/requirements (#368)\n\nBumps [urllib3](https://github.com/urllib3/urllib3) from 2.7.0 to 2.8.0.\n- [Release notes](https://github.com/urllib3/urllib3/releases)\n- [Changelog](https://github.com/urllib3/urllib3/blob/main/CHANGES.rst)\n- [Commits](https://github.com/urllib3/urllib3/compare/2.7.0...2.8.0)\n\n---\nupdated-dependencies:\n- dependency-name: urllib3\n  dependency-version: 2.8.0\n  dependency-type: indirect\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-10-11T00:58:09Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/c147338d9456bb263dcd1f03dc574654e7a468c0"
        },
        "date": 1791700514473,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 92.06,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "c1a08beda8648fa546e1ded6e12595e9a59194d5",
          "message": "docs: say SkipItemCount/MaximumItemCount instead of \"Skip/Max\" in transformer remarks (#425)\n\n\"No Skip/Max\" read as if the error policy's ItemErrorAction.Skip were\nunsupported. Name the item-count properties, and on the six error-policy\ntransformers note that the limits are unrelated to ItemErrorAction.Skip.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-10-17T22:41:08Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/c1a08beda8648fa546e1ded6e12595e9a59194d5"
        },
        "date": 1792305287378,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 92.59,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "6a6e646ee08d1fa695b576d54411c0ea2fe0382c",
          "message": "ci(pr): coverage step names carry no percentage (#468)\n\nThe coverage steps are now just 'Enforce coverage thresholds' and the\nStage 1 summary line says 'coverage gate'; the values live only in the\nenv block (CODECOV_MINIMUM / CODECOV_TEST_MINIMUM), so changing a\nthreshold never means renaming a step. Step names are not required-check\ncontexts (job names are), so the ruleset is unaffected.\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-08T02:22:55Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/6a6e646ee08d1fa695b576d54411c0ea2fe0382c"
        },
        "date": 1794142970336,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score",
            "value": 91.86,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "dependabot[bot]",
            "username": "dependabot[bot]",
            "email": "49699333+dependabot[bot]@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "8136ee0082b61b6eb8dd329d470040b32e4fca8d",
          "message": "Bump the dotnet-dependencies group with 4 updates (#758)\n\nBumps dotnet-reportgenerator-globaltool from 5.5.12 to 5.6.0\nBumps dotnet-stryker from 5.0.0 to 5.1.0\nBumps Meziantou.Analyzer from 3.0.322 to 3.0.328\nBumps SonarAnalyzer.CSharp from 10.35.0.4138 to 10.36.0.15337\n\n---\nupdated-dependencies:\n- dependency-name: dotnet-reportgenerator-globaltool\n  dependency-version: 5.6.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: dotnet-stryker\n  dependency-version: 5.1.0\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n- dependency-name: Meziantou.Analyzer\n  dependency-version: 3.0.328\n  dependency-type: direct:production\n  update-type: version-update:semver-patch\n  dependency-group: dotnet-dependencies\n- dependency-name: SonarAnalyzer.CSharp\n  dependency-version: 10.36.0.15337\n  dependency-type: direct:production\n  update-type: version-update:semver-minor\n  dependency-group: dotnet-dependencies\n...\n\nSigned-off-by: dependabot[bot] <support@github.com>\nCo-authored-by: dependabot[bot] <49699333+dependabot[bot]@users.noreply.github.com>\nCo-authored-by: Chris Wolfgang <210299580+Chris-Wolfgang@users.noreply.github.com>",
          "timestamp": "2026-11-12T21:48:24Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/8136ee0082b61b6eb8dd329d470040b32e4fca8d"
        },
        "date": 1794750396770,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score (tests/Wolfgang.Etl.Transformers.Tests.Unit)",
            "value": 86.4,
            "unit": "%"
          }
        ]
      },
      {
        "commit": {
          "author": {
            "name": "Chris Wolfgang",
            "username": "Chris-Wolfgang",
            "email": "210299580+Chris-Wolfgang@users.noreply.github.com"
          },
          "committer": {
            "name": "GitHub",
            "username": "web-flow",
            "email": "noreply@github.com"
          },
          "id": "b051133512301b7e8c208bd8d2f40ecc64a87497",
          "message": "build: record the runner image in the reproducible-build manifest (#470) (#802)\n\n#470 asks the release manifest to record the SDK version, runner image and commit;\n#792 left the runner image out. generate-repro-manifest.py now writes runnerImage,\ndefaulting to the hosted runner's $ImageOS/$ImageVersion (override: --runner-image).\nThe verifier doc lists the field and no longer says global.json pins 10.0 (#795 moved\nit to 11.0.100).\n\nCo-authored-by: Claude Opus 5.5 <noreply@anthropic.com>",
          "timestamp": "2026-11-20T17:56:25Z",
          "url": "https://github.com/Chris-Wolfgang/ETL-Transformers/commit/b051133512301b7e8c208bd8d2f40ecc64a87497"
        },
        "date": 1795357303941,
        "tool": "customBiggerIsBetter",
        "benches": [
          {
            "name": "Mutation score (tests/Wolfgang.Etl.Transformers.Tests.Unit)",
            "value": 82.83,
            "unit": "%"
          }
        ]
      }
    ]
  }
}