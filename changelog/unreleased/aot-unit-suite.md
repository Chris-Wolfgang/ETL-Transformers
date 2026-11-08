type: internal

The unit suite now also runs as a Native AOT executable (`Tests.Aot`, xunit.v3.aot), so a trim or AOT break in the library fails a test instead of shipping; the library grants that test assembly the same `InternalsVisibleTo` access as `Tests.Unit`.
