## Primary Depth-First Development (PDFD) - Semantic Predicates for the Generic Model
## Artifact Documentation (Conceptual Definitions Only)

---------------------------------------------------------------------

1. Purpose
----------

This document defines the semantic predicates used in the generic PDFD
state-machine model. These predicates appear in the generic diagrams and
operational semantics in the paper.

The CSP implementation included in this artifact is the instance-specific
version of PDFD over levels L1 ... L5, where all boundary conditions are
encoded directly (for example, "i == L1", "i != L5"). Therefore:

- These predicates are not implemented in CSP.
- No predicates.csp file is required.
- This file provides conceptual definitions only to support the generic model.

---------------------------------------------------------------------

2. Scope
--------

These predicates describe the generic semantics of PDFD over an arbitrary
finite hierarchy depth:

L1, L2, ..., Ln

The instance used in the CSP artifact (L1 ... L5) is one concrete
instantiation of this generic model.

---------------------------------------------------------------------

3. Level-Structure Predicates
-----------------------------

is_root(i)
    True iff i is the lowest-level index in the hierarchy (L1).

is_leaf(i)
    True iff i is the highest-level index in the hierarchy (Ln).

not_root(i)
    True iff i is not the lowest-level index.

not_leaf(i)
    True iff i is not the highest-level index.

has_children(i)
    True iff i has at least one child level (i < Ln).

has_no_children(i)
    True iff i has no child levels (i == Ln).

---------------------------------------------------------------------

4. Threshold and Validation Predicates
--------------------------------------

threshold_met(i)
    True iff the validation threshold Ki is satisfied at level i.

threshold_not_met(i)
    True iff the validation threshold Ki is not satisfied.

all_descendants_validated(i)
    True iff all nodes in the subtree rooted at i have been validated.

not_all_descendants_validated(i)
    True iff at least one descendant of i is not validated.

---------------------------------------------------------------------

5. Refinement-Availability Predicates
-------------------------------------

refinement_available(j)
    True iff refinement alternative j is available.

refinement_exhausted(j)
    True iff all refinement attempts for j have been used.

---------------------------------------------------------------------

6. Trace-Origin Predicates
--------------------------

trace_origin_exists(i)
    True iff a trace origin is available for level i.

trace_origin_not_exists(i)
    True iff no trace origin exists for level i.

---------------------------------------------------------------------

7. Index-Comparison Predicates
------------------------------

j_lt_i(j, i_orig)
    True iff refinement index j is strictly less than the original level
    index i_orig.

j_eq_i(j, i_orig)
    True iff refinement index j equals the original level index i_orig.

---------------------------------------------------------------------

8. Helper Functions
-------------------

Next(i)
    Returns the next deeper level after i.

Prev(i)
    Returns the next shallower level before i.

root
    Alias for L1.

leaf
    Alias for Ln.

---------------------------------------------------------------------

9. Relationship to the CSP Implementation
-----------------------------------------

- The CSP model in this artifact is instance-specific (L1 ... L5).
- All boundary conditions are encoded directly in the CSP code.
- These predicates are not used by the CSP implementation.
- This file documents the generic semantics referenced by the paper and diagrams.

---------------------------------------------------------------------

10. Summary
-----------

This file provides the conceptual predicate definitions for the generic PDFD
model. The CSP implementation uses a concrete instantiation and does not rely
on these predicates.

---------------------------------------------------------------------
