type: fix

net5.0 and later no longer pull `Microsoft.Bcl.AsyncInterfaces` and `System.Threading.Channels` 11.0.0, which are in-box there, so net8.0 and net9.0 consumers no longer get two "doesn't support net8.0" build warnings per target from those packages.
