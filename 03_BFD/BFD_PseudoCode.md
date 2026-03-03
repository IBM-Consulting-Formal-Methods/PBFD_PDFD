Procedure BFD(T: Tree)
Input:  T, hierarchical tree with root node c_root
Output: Level-synchronized implementation (CSP-traceable)

------------------------------------------------------------
-- STATE S0: Initialization (Table 19)
-- BF1: S0 → S1 (Table 20)
-- CSP events: load_tree_actual!t_initial, initialize_level_queues_actual!c_root
------------------------------------------------------------
1.  load_tree_actual!t_initial
2.  initialize_level_queues_actual!c_root
3.  currentQ ← [c_root]          // Queue for current level
4.  nextQ   ← []                 // Queue for next level
5.  level   ← 0                  // Current level index

------------------------------------------------------------
-- MAIN LOOP: Alternating S1 (Processing) and S2 (Validation)
------------------------------------------------------------
6.  while true:

        --------------------------------------------------------
        -- STATE S1: Level Processing (Table 19)
        -- BF2a/BF2b: S1 → S1, BF3: S1 → S2
        -- CSP events: get_level_queue_actual!level,
        --             level_queue_not_empty!level,
        --             dequeue_actual!level!node,
        --             process_actual!node,
        --             append_new_queue_actual!next_level,
        --             enqueue_child_actual!next_level!child
        --------------------------------------------------------
7.      get_level_queue_actual!level

8.      while currentQ ≠ []:

9.          level_queue_not_empty!level
10.         node ← Dequeue(currentQ)
11.         dequeue_actual!level!node
12.         process_actual!node

13.         if children(node) = ∅ then
14.             // BF2a: leaf node
15.             continue

16.         // BF2b: internal node → enqueue children in order
17.         next_level ← level + 1

18.         append_new_queue_actual!next_level     // Unconditional (CSP-faithful)

19.         for each child in children(node) in order:
20.             enqueue_child_actual!next_level!child
21.             Enqueue(nextQ, child)

        --------------------------------------------------------
        -- BF3: currentQ empty → transition to S2
        --------------------------------------------------------
22.     level_queue_empty!level

        --------------------------------------------------------
        -- STATE S2: Level Validation (Table 19)
        -- BF4: S2 → S1, BF5: S2 → T
        -- CSP events: validate_level_actual!level,
        --             level_validated!level,
        --             advance_level_actual!level,
        --             no_more_levels!level,
        --             terminate_successfully_actual
        --------------------------------------------------------
23.     validate_level_actual!level
24.     level_validated!level

25.     if nextQ = [] then
26.         // BF5: no more levels → terminate
27.         no_more_levels!level
28.         terminate_successfully_actual
29.         return

30.     // BF4: advance to next level
31.     currentQ ← nextQ
32.     nextQ    ← []
33.     advance_level_actual!level
34.     level    ← level + 1

------------------------------------------------------------
-- Helper Function
------------------------------------------------------------
35. function children(node: NodeID) → Set of NodeID
36.     // Returns children of node from tree T
37.     return ...
