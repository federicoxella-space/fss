# Systems

One file per phase of the settlement update, in the order given by
`docs/SIM-ECON.md`. These files are the only ones in the codebase that write
to simulation state.

Phase order is a design decision, not an implementation detail. Three of the
choices embedded in it are recorded at the top of SIM-ECON with the world each
produces. Do not reorder phases.

Every rate applied to a small integer needs a remainder accumulator. Without
one, small stocks never change while large ones decay normally.