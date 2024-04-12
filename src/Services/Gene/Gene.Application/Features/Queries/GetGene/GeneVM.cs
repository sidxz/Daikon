
using Gene.Domain.Entities;

namespace Gene.Application.Features.Queries.GetGene
{
    public class GeneVM
    {
        public Guid Id { get; set; }
        public Guid StrainId { get; set; }
        public string AccessionNumber { get; set; }
        public string UniProtKB { get; set; }
        public string Name { get; set; }
        public string ProteinNameExpanded { get; set; }
        public string AlphaFoldId { get; set; }

        public object Product { get; set; }
        public object FunctionalCategory { get; set; }
        public object Comments { get; set; }

        public Tuple<string, string, string> Coordinates { get; set; } // (start, end, orientation)
        public List<Tuple<string, string>> Orthologues { get; set; } // (type, description)

        public object GeneSequence { get; set; }
        public object ProteinSequence { get; set; }
        public string GeneLength { get; set; }
        public string ProteinLength { get; set; }

        public List<GeneEssentialityVM> Essentialities { get; set; }
        public List<GeneProteinProductionVM> ProteinProductions { get; set; }
        public List<GeneProteinActivityAssayVM> ProteinActivityAssays { get; set; }
        public List<GeneHypomorphVM> Hypomorphs { get; set; }
        public List<GeneCrispriStrainVM> CrispriStrains { get; set; }
        public List<GeneResistanceMutationVM> ResistanceMutations { get; set; }
        public List<GeneVulnerabilityVM> Vulnerabilities { get; set; }
        public List<GeneUnpublishedStructuralInformationVM> UnpublishedStructuralInformations { get; set; }
        public List<ExpansionPropVM> ExpansionProps { get; set; }

    }
}