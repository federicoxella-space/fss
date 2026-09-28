# Systems

Every file that writes simulation state, and nothing outside this folder does.
Most of them are the phases of the settlement update, one file each, in the
order given by `docs/SIM-ECON.md`; the few writers that are not phases — the
tick loop, the command drain, the generator — are named for what they do.

Phase order is a design decision, not an implementation detail. Three of the
choices embedded in it are recorded at the top of SIM-ECON with the world each
produces. Do not reorder phases.

Every rate applied to a small integer needs a remainder accumulator. Without
one, small stocks never change while large ones decay normally.