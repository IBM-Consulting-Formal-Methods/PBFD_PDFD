
============================================================
STRUCTURAL HELPER FUNCTIONS
============================================================

-- Table A.7.1, Rules PB3/PB7a: Find trace origin level j
Function trace_origin(i: Integer, predicate: Function) Returns Integer
    -- j = min{ k | k < i ∧ predicate(Patternₖ, Patternᵢ) }
    j_list ← {k | k < i ∧ predicate(Patternₖ, Patternᵢ)}
    if j_list is empty then
        return UNDEFINED                     -- PB3c / PB7b: no trace origin
    else
        return min(j_list)
    end if
End Function


-- Table A.7.1, Rule PB5: Next level within refinement scope
Function next_refinement_level(j: Integer, i_orig: Integer) Returns Integer
    if j + 1 ≤ i_orig then
        return j + 1
    else
        return UNDEFINED                     -- Should not occur if PB5 condition holds
    end if
End Function


-- Table A.7.1, Rule PB4a: Select critical children for next pattern
Function select_critical_children(available: Set[Node], level: Integer) 
    Returns Set[Node]
    critical ← ∅
    for each child in available do
        if is_on_critical_path(child) ∨ 
           has_high_fanout(child) ∨ 
           is_foundational(child, level) then
            critical ← critical ∪ {child}
        end if
    end for
    return critical
End Function


-- Table A.7.1, Rules PB3/PB3c and PB7a/PB7b: Consolidated refinement handler
Function HandlePBFDFailureRefinement(
    current_level: Integer,
    R_MAX: Integer,
    predicate: Function
) Returns State
    -- Find trace origin j
    j ← trace_origin(current_level, predicate)
    
    -- PB3 / PB7a: if j exists and attempts remain → go to refinement
    if j ≠ UNDEFINED and refinement_attempts[j] < R_MAX then
        refinement_attempts[j] ← refinement_attempts[j] + 1  -- increment_refinement_attempts_actual!j
        return S1_RefinementProcess(j, current_level)
    
    -- PB3c / PB7b: otherwise → error termination
    else
        return S5
    end if
End Function


============================================================
MAIN PBFD PROCEDURE
============================================================

    Procedure PBFD(T: Tree, L: Integer, R_MAX: Integer)
        -- S0: Initialization (PB1)
01      Load tree T
02      Initialize refinement_attempts[1..L] ← 0
03      i ← 1
04      currentState ← S1_InitialProcess(i)   -- PB1: → S1_init(1)

05      while currentState ∉ {T, S5} do
06          case currentState of

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S1_init(i) – Forward Pattern Processing
            -- ─────────────────────────────────────────────────────────
07          S1_InitialProcess(i):
08              Process Patternᵢ                            -- CSP: process_pattern_actual!i
                
                -- Table A.7.1, Rule PB2: S1_init(i) → S2_val(i)
                -- Condition: cond_not_all_validated?i
09              if ∃n ∈ Patternᵢ: ¬validated(n) then
10                  currentState ← S2_ValidationInitial(i)
                
                -- Table A.7.1, Rule PB2a: S1_init(i) → S3_depth(i)
                -- Condition: cond_all_validated?i
11              else if ∀n ∈ Patternᵢ: validated(n) then
12                  currentState ← S3_DepthProgression(i)
13              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S2_val(i) – Primary Pattern Validation
            -- ─────────────────────────────────────────────────────────
14          S2_ValidationInitial(i):
15              Validate Patternᵢ                            -- CSP: validate_pattern_actual!i
                
                -- Table A.7.1, Rule PB4: S2_val(i) → S3_depth(i)
                -- Condition: cond_all_validated?i
16              if ∀n ∈ Patternᵢ: validated(n) then
17                  currentState ← S3_DepthProgression(i)
                
                -- Table A.7.1, Rules PB3/PB3c: failure → refinement or termination
                -- PB3 Condition: cond_not_all_validated?i ∧ cond_trace_origin?i?j ∧ 
                --                cond_ref_attempts_lt_Rmax?j
                -- PB3c Condition: cond_not_all_validated?i ∧ 
                --                 (cond_no_trace_origin?i ∨ cond_ref_attempts_ge_Rmax?j)
18              else if ∃n ∈ Patternᵢ: ¬validated(n) then
19                  currentState ← HandlePBFDFailureRefinement(i, R_MAX, affected_by)
20              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S1_ref(j,i) – Refinement Pattern Processing
            -- ─────────────────────────────────────────────────────────
21          S1_RefinementProcess(j, i_orig):
                -- Table A.7.1, Rule PB9: S1_ref(j,i) → S5
                -- Condition: cond_ref_attempts_ge_Rmax?j
22              if refinement_attempts[j] ≥ R_MAX then
23                  currentState ← S5
24              else
25                  Process Patternⱼ                          -- CSP: process_refinement_pattern_actual!j
                    
                    -- Table A.7.1, Rule PB3a: S1_ref(j,i) → S2_ref(j,i)
                    -- Condition: cond_ref_attempts_lt_Rmax?j ∧ cond_not_all_validated?j
26                  if ∃n ∈ Patternⱼ: ¬validated(n) then
27                      currentState ← S2_ValidationRefinement(j, i_orig)
                    
                    -- Table A.7.1, Rule PB3b: S1_ref(j,i) → S3_rdep(j,i)
                    -- Condition: cond_ref_attempts_lt_Rmax?j ∧ cond_all_validated?j
28                  else if ∀n ∈ Patternⱼ: validated(n) then
29                      currentState ← S3_RefinementDepthResolution(j, i_orig)
30                  end if
31              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S2_ref(j,i) – Refinement Pattern Validation
            -- ─────────────────────────────────────────────────────────
32          S2_ValidationRefinement(j, i_orig):
33              Validate Patternⱼ                            -- CSP: validate_refinement_pattern_actual!j
                
                -- Table A.7.1, Rule PB3a1: S2_ref(j,i) → S3_rdep(j,i)
                -- Condition: cond_all_validated?j
34              if ∀n ∈ Patternⱼ: validated(n) then
35                  currentState ← S3_RefinementDepthResolution(j, i_orig)
                
                -- Table A.7.1, Rule PB3a2: S2_ref(j,i) → S1_ref(j,i)
                -- Condition: cond_not_all_validated?j ∧ cond_ref_attempts_lt_Rmax?j
36              else if ∃n ∈ Patternⱼ: ¬validated(n) and 
37                        refinement_attempts[j] < R_MAX then
38                  refinement_attempts[j] ← refinement_attempts[j] + 1  -- increment_refinement_attempts_actual!j
39                  currentState ← S1_RefinementProcess(j, i_orig)
                
                -- Table A.7.1, Rule PB3a3: S2_ref(j,i) → S5
                -- Condition: cond_not_all_validated?j ∧ cond_ref_attempts_ge_Rmax?j
40              else if ∃n ∈ Patternⱼ: ¬validated(n) and 
41                        refinement_attempts[j] ≥ R_MAX then
42                  currentState ← S5
43              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S3_depth(i) – Depth Progression
            -- ─────────────────────────────────────────────────────────
44          S3_DepthProgression(i):
                -- CSP: resolve_depth_actual!i → select_critical_children_actual!i
45              Resolve node implementations in Patternᵢ
46              available_children ← {c ∈ V | ∃n ∈ Patternᵢ: (n,c) ∈ E}
47              Patternᵢ₊₁ ← select_critical_children(available_children, i)
                
                -- Table A.7.1, Rule PB4a: S3_depth(i) → S1_init(Next(i))
                -- Condition: cond_i_lt_L?i ∧ cond_pattern_next_nonempty?i
48              if i < L and Patternᵢ₊₁ ≠ ∅ then
49                  i ← i + 1
50                  currentState ← S1_InitialProcess(i)
                
                -- Table A.7.1, Rule PB4b: S3_depth(i) → S4_tdown(L1)
                -- Condition: cond_i_eq_L?i ∨ cond_pattern_next_empty?i
51              else if i = L or Patternᵢ₊₁ = ∅ then
52                  i ← 1
53                  currentState ← S4(i)
54              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S3_rdep(j,i) – Refinement Depth Resolution
            -- ─────────────────────────────────────────────────────────
55          S3_RefinementDepthResolution(j, i_orig):
                -- CSP: resolve_refinement_depth_actual!j
56              Resolve node implementations in Patternⱼ
                
                -- Table A.7.1, Rule PB5: S3_rdep(j,i) → S1_ref(Next(j),i)
                -- Condition: LessThan(j,i)
57              if j < i_orig then
58                  next_level ← next_refinement_level(j, i_orig)
59                  currentState ← S1_RefinementProcess(next_level, i_orig)
                
                -- Table A.7.1, Rule PB6: S3_rdep(j,i) → S3_depth(i)
                -- Condition: ¬LessThan(j,i)
60              else if j = i_orig then
61                  currentState ← S3_DepthProgression(i_orig)
62              end if

            -- ─────────────────────────────────────────────────────────
            -- Table 40_DDD: S4_tdown(i) – Top‑Down Completion Phase
            -- ─────────────────────────────────────────────────────────
63          S4(i):
64              Finalize Patternᵢ                             -- CSP: finalize_pattern_actual!i
                
                -- Table A.7.1, Rule PB7: S4_tdown(i) → S4_tdown(Next(i))
                -- Condition: cond_all_processed?i ∧ cond_i_lt_L?i
65              if ∀n ∈ Patternᵢ: processed(n) then
66                  if i < L then
67                      i ← i + 1
68                      currentState ← S4(i)
                    
                    -- Table A.7.1, Rule PB8: S4_tdown(i) → T
                    -- Condition: cond_all_processed?i ∧ cond_i_eq_L?i
69                  else if i = L then
70                      currentState ← T
71                  end if
                
                -- Table A.7.1, Rules PB7a/PB7b: failure → refinement or termination
                -- PB7a Condition: cond_not_all_processed?i ∧ cond_trace_origin?i?j ∧ 
                --                 cond_ref_attempts_lt_Rmax?j
                -- PB7b Condition: cond_not_all_processed?i ∧ 
                --                 (cond_no_trace_origin?i ∨ cond_ref_attempts_ge_Rmax?j)
72              else if ∃n ∈ Patternᵢ: ¬processed(n) then
73                  currentState ← HandlePBFDFailureRefinement(i, R_MAX, 
74                                                              affected_by_unprocessed)
75              end if

76          end case
77      end while

        -- Termination
78      if currentState = S5 then
79          Terminate(ERROR)
80      else if currentState = T then
81          Terminate(SUCCESS)
82      end if
83  End Procedure
