Algorithm TLE(Pages)
Procedure TLE_EventDriven(Units)
Input: Units – list of TLE data units (e.g., grandparent entities) to process
Output: Tree with bitmask-encoded children selections finalized
01  WaitForEvent(start)                 // TLE1: start → S0
02  currentState ← S0
03  currentUnit  ← NULL

04  while System_Running do
05      case currentState of

        ------------------------------------------------------------
        -- S0: Idle (TLE_S0)
        -- TLE2:  load?u         → S1(u)
        -- TLE11: no_next_unit?u → S6(u)
        ------------------------------------------------------------
06      S0:
07          event ← WaitForEvent({load, no_next_unit})
08          if event.type == load then
09              currentUnit ← event.Unit
10              currentState ← S1(currentUnit)
11          else if event.type == no_next_unit then
12              currentUnit ← event.Unit
13              currentState ← S6(currentUnit)

        ------------------------------------------------------------
        -- S1(u): Data Loaded (TLE_S1)
        -- TLE3: hierarchy_resolved.u → S2(u)
        ------------------------------------------------------------
14      S1(u):
15          event ← WaitForEvent({hierarchy_resolved})
16          if event.Unit == u then
17              currentState ← S2(u)

        ------------------------------------------------------------
        -- S2(u): Hierarchy Resolved (TLE_S2)
        -- TLE4: children_evaluated.u → S3(u)
        ------------------------------------------------------------
18      S2(u):
19          event ← WaitForEvent({children_evaluated})
20          if event.Unit == u then
21              currentState ← S3(u)

        ------------------------------------------------------------
        -- S3(u): Children Evaluated (TLE_S3)
        -- TLE5: children_updated.u → S4(u)
        -- TLE6: skip_update.u      → S5(u)
        ------------------------------------------------------------
22      S3(u):
23          event ← WaitForEvent({children_updated, skip_update})
24          if event.Unit == u then
25              if event.type == children_updated then
26                  currentState ← S4(u)
27              else if event.type == skip_update then
28                  currentState ← S5(u)

        ------------------------------------------------------------
        -- S4(u): Children Updated (TLE_S4)
        -- TLE7: changes_committed.u → S5(u)
        ------------------------------------------------------------
29      S4(u):
30          event ← WaitForEvent({changes_committed})
31          if event.Unit == u then
32              currentState ← S5(u)

        ------------------------------------------------------------
        -- S5(u): Changes Committed (TLE_S5)
        -- TLE8: has_next_unit   → S0
        -- TLE9: no_next_unit.u  → S6(u)
        ------------------------------------------------------------
33      S5(u):
34          event ← WaitForEvent({has_next_unit, no_next_unit})
35          if event.type == has_next_unit then
36              currentState ← S0
37          else if event.type == no_next_unit and event.Unit == u then
38              currentState ← S6(u)

        ------------------------------------------------------------
        -- S6(u): Workflow Finalized (TLE_S6)
        -- TLE10: finalize_process.u → S0
        ------------------------------------------------------------
39      S6(u):
40          event ← WaitForEvent({finalize_process})
41          if event.Unit == u then
42              currentState ← S0

43      end case
44  end while

45  return
End Procedure
