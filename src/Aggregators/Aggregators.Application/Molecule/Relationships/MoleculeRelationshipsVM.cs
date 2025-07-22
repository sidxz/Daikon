using System;
using System.Collections.Generic;
using Daikon.Shared.VM.HitAssessment;
using Daikon.Shared.VM.Project;
using Daikon.Shared.VM.Screen;

namespace Aggregators.Application.Molecule.Relationships
{
    /*
     * View model representing detailed relationship data for a molecule,
     * including all associated hit results.
     */
    public class MoleculeRelationshipsVM
    {
        /*
         * List of hits associated with the molecule.
         * These may include data retrieved from various HitCollections.
         */
        public List<HitVM> Hits { get; set; } = new();

        public List<HaCompoundEvolutionVM> HaCompoundEvolutions { get; set; } = new();
        public List<CompoundEvolutionVM> ProjectCompoundEvolution { get; set; } = new();
    }
}
