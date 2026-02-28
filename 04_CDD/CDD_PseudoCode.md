Procedure CDD(G: Graph, Rmax: Integer, L_max: Integer)

Input:  G      - Directed development graph with dependency structure
Input:  Rmax   - Maximum refinement attempts per node
Input:  L_max  - Total number of milestones (M1..M_Lmax)
Output: Continuous cyclic execution with restart on completion or error

---------------------------------------------------------------------------
HELPER FUNCTIONS (generic, CSP-consistent)
---------------------------------------------------------------------------

Function all_nodes_completed(status: Map) -> Boolean
    for each node in Nodes(G) do
        if status[node] != Completed then
            return False
    return True
End Function

Function deps_satisfied(node: NodeID, status: Map) -> Boolean
    for each dep in Dependencies(node, G) do
        if status[dep] != Completed then
            return False
    return True
End Function

Function select_ready_node(status: Map) -> NodeID
    for each node in Nodes(G) do
        if status[node] != Completed
           and deps_satisfied(node, status) then
            return node
    return NULL
End Function

Function milestone_lt(m1: MilestoneID, m2: MilestoneID) -> Boolean
    return index(m1) < index(m2)
End Function

Function Next_Milestone(m: MilestoneID) -> MilestoneID
    if index(m) < L_max then
        return milestone(index(m) + 1)
    else
        return milestone(L_max)
End Function

---------------------------------------------------------------------------
MAIN PROCEDURE
---------------------------------------------------------------------------

01  while System_Running do

        ---------------------------------------------------------------
        STATE S0: INITIALIZATION (CD1)
        ---------------------------------------------------------------

02      load_graph_actual(G)
03      initialize_dependencies_actual()

04      current_milestone <- milestone(1)

05      for each node in Nodes(G) do
            node_status[node] <- NotCompleted

06      refinement_attempts <- empty_map()
07      current_node <- NULL
08      SystemState <- S1

        ---------------------------------------------------------------
        INNER STATE MACHINE LOOP
        ---------------------------------------------------------------

09      while SystemState != RESTART do
10          case SystemState of

                -------------------------------------------------------
                STATE S1: NODE PROCESSING (CD2, CD3a, CD3b, CD5)
                -------------------------------------------------------

11          S1:

12              if all_nodes_completed(node_status) then
                    // CD5: S1 -> S3
13                  all_components_written_actual(current_milestone)
14                  validate_increment_actual(current_milestone)
15                  validation_confirmed_actual(current_milestone)
16                  SystemState <- S3

17              else
                    // Select a node whose dependencies are satisfied
18                  candidate_node <- select_ready_node(node_status)

19                  process_node_actual(candidate_node)

20                  if test_failed(candidate_node) then
                        // CD3a: S1 -> S2
21                      test_failed_actual(candidate_node)
22                      current_node <- candidate_node
23                      refinement_attempts[current_node] <- 0
24                      SystemState <- S2

25                  else if feedback_triggered(candidate_node) then
                        // CD3b: S1 -> S2
26                      feedback_triggered_actual(candidate_node)
27                      current_node <- candidate_node
28                      refinement_attempts[current_node] <- 0
29                      SystemState <- S2

30                  else
                        // CD2: S1 -> S1
31                      node_status[candidate_node] <- Completed

                -------------------------------------------------------
                STATE S2: REFINEMENT (CD4a, CD4c, CD4b)
                -------------------------------------------------------

32          S2:

33              refine_component_actual(current_node)
34              refinement_confirmed_actual(current_node)

35              if refinement_complete(current_node) then
                        // CD4a: S2 -> S1
36                  refinement_complete_actual(current_node)
37                  SystemState <- S1

38              else if refinement_failed(current_node) then
39                  refinement_failed_actual(current_node)

40                  if refinement_attempts[current_node] + 1 >= Rmax then
                        // CD4b: S2 -> S0 (restart)
41                      terminate_with_error_actual(current_node)
42                      SystemState <- RESTART

43                  else
                        // CD4c: S2 -> S2 (retry)
44                      refinement_attempts[current_node] <-
                              refinement_attempts[current_node] + 1

                -------------------------------------------------------
                STATE S3: VALIDATION (CD6a, CD6b, CD8, CD7)
                -------------------------------------------------------

45          S3:

46              if validation_failed(current_milestone) then
                        // CD6a: S3 -> S2
47                  validation_failed_actual(current_milestone)
48                  flawed_node <- identify_flaw_actual()
49                  current_node <- flawed_node
50                  node_status[current_node] <- NotCompleted
51                  refinement_attempts[current_node] <- 0
52                  SystemState <- S2

53              else if feedback_received(current_milestone) then
                        // CD6b: S3 -> S2
54                  feedback_received_actual(current_milestone)
55                  flawed_node <- identify_flaw_actual()
56                  current_node <- flawed_node
57                  node_status[current_node] <- NotCompleted
58                  refinement_attempts[current_node] <- 0
59                  SystemState <- S2

60              else
61                  if milestone_lt(current_milestone, milestone(L_max)) then
                        // CD8: S3 -> S1
62                      advance_milestone_actual(
                            Next_Milestone(current_milestone))
63                      current_milestone <- Next_Milestone(current_milestone)

64                      for each node in Nodes(G) do
                            node_status[node] <- NotCompleted

65                      SystemState <- S1

66                  else
                        // CD7: S3 -> S0 (restart)
67                      final_development_actual()
68                      terminate_successfully_actual()
69                      SystemState <- RESTART

70          end case
71      end while

72  end while

End Procedure
