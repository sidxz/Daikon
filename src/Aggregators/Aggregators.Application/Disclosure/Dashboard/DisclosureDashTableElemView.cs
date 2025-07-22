using System;
using System.Collections.Generic;
using Daikon.Shared.VM.Horizon;
using Daikon.Shared.VM.Screen;
using Daikon.Shared.VM.MLogix;
using CQRS.Core.Domain;
using Daikon.Shared.VM.HitAssessment;
using Daikon.Shared.VM.Project;

namespace Aggregators.Application.Disclosure.Dashboard
{
    /*
     * View model used in the Disclosure dashboard to represent
     * detailed molecule data along with its horizon relationships and screening context.
     * Inherits core molecule properties from MoleculeVM.
     */
    public class DisclosureDashTableElemView : MoleculeVM
    {
        /*
         * List of compound-level relationships from the horizon perspective
         * associated with the molecule.
         */
        public List<CompoundRelationsVM> HorizonRelations { get; set; } = new();

        /*
         * List of screening hits linked to this molecule.
         * Each HitVM contains detailed assay and activity metadata.
         */
        public List<HitVM> Hits { get; set; } = new();
        public List<HaCompoundEvolutionVM> HaCompoundEvolutions { get; set; } = new();
        public List<CompoundEvolutionVM> ProjectCompoundEvolution { get; set; } = new();

    }
}
