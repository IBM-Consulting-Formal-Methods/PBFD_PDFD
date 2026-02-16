------------------------------------------------------------
-- PDFD Validation Algorithm
-- Fully Traceable to Table A.6.1 PD Rules
-- Levels: L1 .. L5
-- Max Refinement Attempts: R_MAX
------------------------------------------------------------

01  Algorithm PDFD_Validation(T, L, R_MAX):
02      refinement_attempts[1..L] = 0
03      call S1_InitialProcess(L1)           // PD1: System Initialization

------------------------------------------------------------
-- Helper Functions
------------------------------------------------------------

04  NextLevel(i):
05      return i == L ? i : i + 1

06  PrevLevel(i):
07      return i == L1 ? i : i - 1

08  CheckRefinement(j):
09      if refinement_attempts[j] >= R_MAX:
10          call S5                        // refinement exhausted
11          return False
12      refinement_attempts[j] += 1
13      return True

14  CanRefine(i):
15      return Trace_Origin_Exists(i)

------------------------------------------------------------
-- Forward Processing: S1_init → S2_val
------------------------------------------------------------

16  S1_InitialProcess(i):                   // PD2 entry, PD8a
17      if refinement_attempts[i] >= R_MAX:
18          call S5                         // PD8a
19          return

20      Process_Level(i)                     // PD2
21      call S2_LevelValidation(i)

------------------------------------------------------------
-- Primary Validation: S2_val branching
------------------------------------------------------------

22  S2_LevelValidation(i):                  // PD2, PD4
23      is_threshold_met = Validate_Level(i)

24      if is_threshold_met:
25          if i == L or has_no_children(i):
26              call S3_BottomUpCompletion(i)        // PD4
27          else:
28              call S1_InitialProcess(NextLevel(i)) // PD2b
29      else:
30          if not CanRefine(i):
31              call S5                               // PD2a-fatal
32              return

33          j = Find_Refinement_Origin(i, L)
34          if j != null and CheckRefinement(j):
35              call S1_RefinementProcess(j, i)       // PD2a-refine
36          else:
37              call S5                               // PD2a-exhausted

------------------------------------------------------------
-- Refinement Processing: S1_ref → S2_ref
------------------------------------------------------------

38  S1_RefinementProcess(j, i_orig):        // PD3 entry
39      Process_Level(j)                     // PD3
40      call S2_RefinementValidation(j, i_orig)

------------------------------------------------------------
-- Refinement Validation: S2_ref branching
------------------------------------------------------------

41  S2_RefinementValidation(j, i_orig):     // PD3-res, PD3a/b/c
42      is_threshold_met = Validate_Level(j)

43      if is_threshold_met:
44          call S3_RefinementResolution(j, i_orig)   // PD3-res
45      else:
46          j_new = Find_Refinement_Origin(j, i_orig)
47          if j_new != null and CheckRefinement(j_new):
48              call S1_RefinementProcess(j_new, i_orig)  // PD3c-2
49          else:
50              call S5                                   // PD3c-1

------------------------------------------------------------
-- Refinement Resolution: S3_res branching
------------------------------------------------------------

51  S3_RefinementResolution(j, i_orig):      // PD3a / PD3b
52      Emit_Trace_Origin(j, i_orig)

53      if j < i_orig:
54          call S1_RefinementProcess(NextLevel(j), i_orig)  // PD3a
55      else:
56          call S2_LevelValidation(i_orig)                  // PD3b

------------------------------------------------------------
-- Bottom-Up Completion: S3_bup → S4_tdown
------------------------------------------------------------

57  S3_BottomUpCompletion(i):                // PD4, PD4a, PD4b, PD5
58      Finalize_Subtrees(i)
59      is_validated = Check_All_Descendants_Validated(i)

60      if is_validated:
61          if i != L1:
62              call S3_BottomUpCompletion(PrevLevel(i))  // PD4a
63          else:
64              call S4_TopDownCompletion(L1)             // PD5
65      else:
66          if not CanRefine(i):
67              call S5                                   // PD4b-fatal
68              return

69          j = Find_Refinement_Origin(i, L)
70          if j != null and CheckRefinement(j):
71              call S1_RefinementProcess(j, i)           // PD4b-refine
72          else:
73              call S5                                   // PD4b-exhausted

------------------------------------------------------------
-- Top-Down Finalization: S4_tdown → T/S5
------------------------------------------------------------

74  S4_TopDownCompletion(i):                 // PD6, PD6a, PD6b, PD7
75      Finalize_Unprocessed_Nodes(i)
76      is_validated = Check_All_Descendants_Validated(i)

77      if is_validated:
78          if i != L:
79              call S4_TopDownCompletion(NextLevel(i))   // PD6
80          else:
81              call T                                    // PD7
82      else:
83          if not CanRefine(i):
84              call S5                                   // PD6b-fatal
85              return

86          j = Find_Refinement_Origin(i, L)
87          if j != null and CheckRefinement(j):
88              call S1_RefinementProcess(j, i)           // PD6a-refine
89          else:
90              call S5                                   // PD6a-exhausted

------------------------------------------------------------
-- Termination
------------------------------------------------------------

91  T:
92      Signal SUCCESS

93  S5:
94      Signal ERROR
