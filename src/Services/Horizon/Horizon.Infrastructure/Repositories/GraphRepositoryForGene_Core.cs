
using CQRS.Core.Exceptions;
using Horizon.Application.Contracts.Persistance;
using Horizon.Domain.Genes;
using Microsoft.Extensions.Logging;

namespace Horizon.Infrastructure.Repositories
{
    public partial class GraphRepositoryForGene : IGraphRepositoryForGene
    {

        public async Task AddGene(Gene gene)
        {
            _logger.LogInformation("AddGene(): Adding gene with id {id} strainId {strainId} accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", gene.GeneId, gene.StrainId, gene.AccessionNumber, gene.Name, gene.Function, gene.Product, gene.FunctionalCategory);
            try
            {
                var query = @"
                   MERGE (g:Gene {accessionNumber: $accessionNumber})
                                ON CREATE SET
                                    g.uniId = $uniId,
                                    g.name = $name,
                                    g.function = $function,
                                    g.product = $product,
                                    g.functionalCategory = $functionalCategory
                                ON MATCH SET
                                    g.uniId = $uniId,
                                    g.name = $name,
                                    g.function = $function,
                                    g.product = $product,
                                    g.functionalCategory = $functionalCategory
                                WITH g
                                MERGE (s:Strain {uniId: $strainId})
                                MERGE (g)-[:PART_OF]->(s)
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = gene.UniId,
                                 geneId = gene.GeneId,
                                 strainId = gene.StrainId,
                                 accessionNumber = gene.AccessionNumber,
                                 name = gene.Name,
                                 function = gene.Function,
                                 product = gene.Product,
                                 functionalCategory = gene.FunctionalCategory
                             }).ExecuteAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AddGene");
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Adding Gene To Graph", ex);
            }
        }

        public async Task UpdateGene(Gene gene)
        {
            _logger.LogInformation("UpdateGene(): Updating gene with id {id} strainId {strainId} accession number {accessionNumber} and name {name} and function {function} and product {product} and functional category {functionalCategory}", gene.GeneId, gene.StrainId, gene.AccessionNumber, gene.Name, gene.Function, gene.Product, gene.FunctionalCategory);

            try
            {

                var query = @"
                    MATCH (g:Gene {uniId: $uniId})
                                SET 
                                    g.name = $name,
                                    g.function = $function,
                                    g.product = $product,
                                    g.functionalCategory = $functionalCategory
                                WITH g
                                    OPTIONAL MATCH (g)-[r2:PART_OF]->(oldStrain:Strain)
                                    WHERE oldStrain.uniId <> $strainId OR oldStrain IS NULL
                                    DELETE r2
                                WITH g
                                    MERGE (newStrain:Strain {uniId: $strainId})
                                    MERGE (g)-[:PART_OF]->(newStrain)";

                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 uniId = gene.UniId,
                                 strainId = gene.StrainId,
                                 accessionNumber = gene.AccessionNumber,
                                 name = gene.Name,
                                 function = gene.Function,
                                 product = gene.Product,
                                 functionalCategory = gene.FunctionalCategory
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in UpdateGene");
                _logger.LogError(ex, "All retry attempts failed for updating gene with accession number {AccessionNumber}", gene.AccessionNumber);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Updating Gene Of Graph", ex);
            }

        }

        // Delete a gene from the graph database
        public async Task DeleteGene(string geneId)
        {
            _logger.LogInformation("DeleteGene(): Deleting gene with accession number {geneId}", geneId);

            try
            {
                var query = @"
                   MATCH (g:Gene {uniId: $_geneId})
                            DETACH DELETE g
                ";
                var (queryResults, _) = await _driver
                             .ExecutableQuery(query).WithParameters(new
                             {
                                 _geneId = geneId
                             }).ExecuteAsync()
                             ;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteGene");
                _logger.LogError(ex, "All retry attempts failed for deleting gene with accession number {geneId}", geneId);
                throw new RepositoryException(nameof(GraphRepositoryForGene), "Error Deleting Gene From Graph", ex);
            }
        }
    }
}
