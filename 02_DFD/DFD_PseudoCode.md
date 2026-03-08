Algorithm DFD
Procedure DFD(T: Tree)
Input:  T, a hierarchical tree with root node c_root
Output: All nodes processed and validated

------------------------------------------------------------
-- STATE S0: Initialization (DF1)
------------------------------------------------------------
1.  LoadProject(T)                     // load_tree_actual!t_initial
2.  stack ← [c_root]                   // initialize_stack_actual!c_root
3.  Processed ← ∅

------------------------------------------------------------
-- STATE S1: Vertical Processing (DF2, DF3, DF8)
------------------------------------------------------------
4.  while stack is not empty:          // DF8 when empty
5.      c ← pop(stack)                 // dequeue_actual!c
6.      Process(c)                     // process_actual!c
7.      Processed ← Processed ∪ {c}

8.      if c is non-leaf then          // DF2: is_non_leaf!c
9.          for each child in reverse(children(c)):
10.             push(child, stack)     // process_child_actual!c, push_children_actual!c
11.         continue                   // remain in S1

12.     else                           // DF3: is_leaf!c
13.         b_j ← parent(c)            // set_backtrack_point_actual!parent(c)

------------------------------------------------------------
-- STATE S2: Backtracking (DF4, DF5)
------------------------------------------------------------
14.         while true:
15.             unproc ← children(b_j) \ Processed

16.             if unproc ≠ ∅ then     // DF4: has_unprocessed_sibling!b_j
17.                 sibling ← choose_one(unproc)
18.                 push(sibling, stack) // get_unprocessed_sibling_actual!b_j,
                                       // push_sibling_actual!sibling
19.                 break               // return to S1

20.             else                   // DF5: no_unprocessed_sibling!b_j
21.                 ValidateSubtree(b_j) // validate_subtree_actual!b_j

------------------------------------------------------------
-- STATE S3: Validation (DF6, DF7)
------------------------------------------------------------
22.                 if b_j = c_root then   // DF7: no_more_backtrack_points_above!b_j
23.                     Terminate()        // terminate_successfully_actual
24.                     return             // T (SKIP)

25.                 else                   // DF6: subtree_validated!b_j
26.                     b_j ← parent(b_j)  // backtrack_to_actual!b_j!parent(b_j)
                                           // continue S2 loop

------------------------------------------------------------
-- DF8: S1(<>, processed) → T
------------------------------------------------------------
27. Terminate()
End Procedure
