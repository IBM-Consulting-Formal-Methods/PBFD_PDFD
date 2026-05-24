Procedure DAD(G: DAG, v1_root: Node)
Input:
    G          -- directed acyclic graph
    v1_root    -- root node of G
Output:
    Fully processed DAG with validated dependencies
Constants:
    allInitialNodes -- set of nodes present at load time
    v_new_param     -- reserved node for dependency extension
Variables:
    Q          -- processing queue
    processed  -- set of processed nodes
    node       -- current node
    restQueue  -- remaining queue after dequeue
    childSet   -- children of current node
// DA1: initialization
1.  LoadDAG(G)
2.  Q <- <v1_root>
3.  processed <- {}
4.  while true do
        // DA6a: Successful completion condition
5.      if diff(allInitialNodes, processed) = {} then
6.          AllNodesProcessed()
7.          FinalValidation()
8.          TerminateSuccessfully()
9.          return
10.     end if
        // DA6b: Empty queue before completion
11.     if Q = <> then
12.         TerminateWithError()
13.         return
14.     end if
        // DA2: Node processing transition
15.     QueueNotEmpty()
16.     node <- head(Q)
17.     restQueue <- tail(Q)
18.     Process(node)
19.     ValidateDependencies(node)
20.     processed <- processed U {node}
        // DA3 / DA4: Dependency evaluation
21.     if diff(dependencies(node), processed) = {} then
                // DA3: dependency satisfied
22.         AllDependenciesProcessed(node)
23.         GenerateChildren(node)
24.         childSet <- children(node)
25.         EnqueueNodes(childSet)
26.         Q <- restQueue ⧺
                   seq(diff(childSet, processed))
27.     else
                // DA4: missing dependency
28.         MissingDependency(node)
29.         ExtendGraph(node, v_new_param)
                // DA5: graph extension continuation
30.         EnqueueNodes({v_new_param})
31.         Q <- <v_new_param> ^ restQueue
32.     end if
33. end while
End Procedure
