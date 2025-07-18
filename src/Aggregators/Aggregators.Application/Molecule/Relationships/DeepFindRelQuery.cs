using System;
using System.Collections.Generic;
using MediatR;
using Daikon.Shared.VM.Horizon;

namespace Aggregators.Application.Molecule.Relationships
{
    /*
     * DeepFindRelQuery is a request class used to retrieve detailed relationship information
     * for a specified molecule, expanding upon its existing horizon-based relationships.
     */
    public class DeepFindRelQuery : IRequest<MoleculeRelationshipsVM>
    {
        /*
         * Unique identifier of the molecule whose relationships are to be retrieved.
         * This acts as the primary reference for expanding related compound data.
         */
        public Guid MoleculeId { get; set; }

        /*
         * A list of high-level compound relationship data (typically from a horizon view)
         * that will be used as input for deeper analysis and fetching additional details.
         */
        public List<CompoundRelationsVM> HorizonRelations { get; set; } = new();
    }
}
