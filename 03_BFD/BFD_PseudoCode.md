Procedure BFD(T: Tree)
Input:  T, a hierarchical tree with root node c_root
Output: Level-synchronized breadth-first traversal

------------------------------------------------------------
-- STATE S0: Initialization
------------------------------------------------------------
1.  LoadProject(T)
2.  currentQ ← [c_root]              // Queue for current level
3.  nextQ    ← []                    // Queue for next level
4.  level    ← 0                     // Current level index

------------------------------------------------------------
-- MAIN LOOP: Alternating S1 (Processing) and S2 (Validation)
------------------------------------------------------------
5.  while true do:
        ----------------------------------------------------
        -- STATE S1: Level Processing
        ----------------------------------------------------
6.      while currentQ ≠ [] do:
7.          node ← Dequeue(currentQ)
8.          Process(node)

9.          if children(node) = [] then
10.             continue             // BF2a: Leaf node

11.         for each child in children(node) in order do:
12.             Enqueue(nextQ, child) // BF2b: Internal node

        ----------------------------------------------------
        -- STATE S2: Level Validation
        ----------------------------------------------------
13.     ValidateLevel(level)

14.     if nextQ = [] then           // BF5: No more levels
15.         Terminate()
16.         return

17.     currentQ ← nextQ             // BF4: Advance to next level
18.     nextQ    ← []
19.     level    ← level + 1

------------------------------------------------------------
-- Helper Functions
------------------------------------------------------------
20. function children(node: NodeID) → Set of NodeID
21.     return tree_lookup(T, node)

22. procedure ValidateLevel(level: Integer)
23.     return
