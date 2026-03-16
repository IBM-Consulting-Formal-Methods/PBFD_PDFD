Algorithm A.x.x. Dependency-Aware Development (DAD)

Procedure DAD(G: DAG, v1_root: Node)
Input:
    G          -- directed acyclic graph
    v1_root    -- root node of G
Output:
    Fully processed DAG with validated dependencies

Constants:
    allInitialNodes -- set of all nodes in G at load time
    v_new_param     -- fixed node ID reserved for graph extension

Variables:
    Q          -- processing queue
    processed  -- set of processed nodes
    node       -- current node being processed
    restQueue  -- remaining queue after dequeue
    childSet   -- children of the current node


// ============================================================
// State S0 -- Initialization
// DA1: S0 -> S1(<v1_root>, {})
// Operation:
//   load_dag_actual!g_initial ->
//   initialize_queue_actual!v1_root
// ============================================================

1.  LoadDAG(G)                                // load_dag_actual!g_initial
2.  Q <- <v1_root>                            // initialize_queue_actual!v1_root
3.  processed <- {}


// ============================================================
// State S1 -- Node Processing Loop
// ============================================================

4.  while true do

    // --------------------------------------------------------
    // DA6a / DA6b -- Empty-queue termination branch
    // Source: S1(<>, processed)
    // --------------------------------------------------------

5.      if Q = <> then

6.          if diff(allInitialNodes, processed) = {} then
                // DA6a: S1(<>, processed) -> T_SUCCESS
                // Condition: diff(allInitialNodes, processed) = {}
                // Operation:
                //   all_nodes_processed ->
                //   perform_final_validation_actual ->
                //   terminate_successfully_actual
7.              AllNodesProcessed()           // all_nodes_processed
8.              FinalValidation()             // perform_final_validation_actual
9.              TerminateSuccessfully()       // terminate_successfully_actual
10.             return

11.         else
                // DA6b: S1(<>, processed) -> T_ERROR
                // Condition: diff(allInitialNodes, processed) != {}
                // Operation:
                //   terminate_with_error_actual
12.             TerminateWithError()          // terminate_with_error_actual
13.             return
14.         end if

15.     end if


    // --------------------------------------------------------
    // DA2 -- Dequeue, process, and validate dependencies
    // Source: S1(<node> ^ restQueue, processed)
    // Target: S2ValidateOutcome(node, restQueue, processed U {node})
    // Operation:
    //   queue_not_empty ->
    //   dequeue_actual!node ->
    //   process_actual!node ->
    //   validate_dependencies_actual!node
    // --------------------------------------------------------

16.     QueueNotEmpty()                       // queue_not_empty
17.     node      <- head(Q)                  // dequeue_actual!node
18.     restQueue <- tail(Q)
19.     Process(node)                         // process_actual!node
20.     ValidateDependencies(node)            // validate_dependencies_actual!node
21.     processed <- processed U {node}
        // -> Enter S2ValidateOutcome(node, restQueue, processed U {node})


    // ========================================================
    // State S2 -- Dependency Check
    // ========================================================

22.     if diff(dependencies(node), processed) = {} then

            // DA3: S2ValidateOutcome(node, restQueue, processed) ->
            //      S1(restQueue ⧺ seq(diff(children(node), processed)), processed)
            // Condition: diff(dependencies(node), processed) = {}
            // Operation:
            //   all_dependencies_processed!node ->
            //   generate_children_actual!node ->
            //   enqueue_nodes_actual!children(node)

23.         AllDependenciesProcessed(node)    // all_dependencies_processed!node
24.         GenerateChildren(node)            // generate_children_actual!node
25.         childSet <- children(node)
26.         EnqueueNodes(childSet)            // enqueue_nodes_actual!children(node)
27.         Q <- restQueue ⧺ seq(diff(childSet, processed))
            // -> Enter S1(restQueue ⧺ seq(diff(childSet, processed)), processed)

28.     else

            // DA4: S2ValidateOutcome(node, restQueue, processed) ->
            //      S3ExtendCompletion(v_new_param, restQueue, processed)
            // Condition: diff(dependencies(node), processed) != {}
            // Operation:
            //   missing_dependency!node ->
            //   extend_graph_actual!node!v_new_param

29.         MissingDependency(node)           // missing_dependency!node
30.         ExtendGraph(node, v_new_param)    // extend_graph_actual!node!v_new_param
            // -> Enter S3ExtendCompletion(v_new_param, restQueue, processed)


        // ====================================================
        // State S3 -- Graph Extension Completion
        // DA5: S3ExtendCompletion(v_new_param, restQueue, processed) ->
        //      S1(<v_new_param> ^ restQueue, processed)
        // Operation:
        //   enqueue_nodes_actual!{v_new_param}
        // ====================================================

31.         EnqueueNodes({v_new_param})       // enqueue_nodes_actual!{v_new_param}
32.         Q <- <v_new_param> ^ restQueue
            // -> Enter S1(<v_new_param> ^ restQueue, processed)

33.     end if

34. end while


// ============================================================
// State T -- Termination
// T_SUCCESS : reached via DA6a after FinalValidation
// T_ERROR   : reached via DA6b when unprocessed nodes remain
// ============================================================

End Procedure
