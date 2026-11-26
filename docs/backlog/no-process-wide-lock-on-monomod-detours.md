---
worth: yes
where: src/StaticMock/Hooks/Factories/Implementation/MonoModHookFactory.cs:15
added: 2026-11-26
---
# no process-wide lock on MonoMod.Core detour install and undo

SMock creates and disposes MonoMod.Core detours (`DetourFactory.Current.CreateDetour` in
`MonoModHookFactory.CreateHook`, `_hook?.Dispose()` in `MonoModHookManager.Return`) with no
synchronization of its own. A detour writes a jump into the native code of the original method, and that
change is visible to every thread in the process. Two setups or disposals of the same method from
different threads (parallel tests, or a test and a background thread) can race. The "last mock wins"
behavior is also undefined when the calls overlap.

Investigate, then add a global lock inside SMock around every MonoMod.Core detour install and undo, so
that method substitution is serialized within one process:

- a single static lock object (or one lock per `MethodBase`) taken in `MonoModHookFactory.CreateHook`
  and in `MonoModHookManager.Return`/`Dispose`;
- check what MonoMod.Core 1.4 already locks internally, so the SMock lock does not duplicate it or
  cause a deadlock with it;
- decide whether a second setup of a method that already has an active hook should wait, replace the
  hook or throw;
- add a test that sets up and disposes the same method from several threads at the same time.

A lock serializes install and undo only. It cannot stop other threads from calling a hooked method while
the mock is active. That is the cause of the intermittent test host crash on .NET Framework in
`TestGenericSetupReturnsDateTimeNow` / `TestGenericSetupReturnsDateTimeUtcNow` (PR #57, Windows Debug
and Release): the NUnit adapter reads `DateTimeOffset.Now` on the NUnit `EventPump` thread while the
throwing mock is active. `src/StaticMock.Tests/StaticMock.Tests.runsettings` sets
`NumberOfTestWorkers=0` to remove that thread. The lock does not fix that crash.

Keep the hook code logically identical, as `CLAUDE.md` requires: the lock wraps the MonoMod calls and
changes no IL emit or replacement-method logic.
