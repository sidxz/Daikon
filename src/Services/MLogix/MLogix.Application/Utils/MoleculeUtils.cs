using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MLogix.Application.Contracts.Infrastructure.DaikonChemVault;
using MLogix.Application.Contracts.Persistence;

namespace MLogix.Application.Utils
{
    public class MoleculeUtils
    {
        private readonly IMoleculeRepository _moleculeRepository;
        private readonly IMoleculeAPI _moleculeAPI;

        public MoleculeUtils(IMoleculeRepository moleculeRepository, IMoleculeAPI moleculeAPI)
        {
            _moleculeRepository = moleculeRepository ?? throw new ArgumentNullException(nameof(moleculeRepository));
            _moleculeAPI = moleculeAPI ?? throw new ArgumentNullException(nameof(moleculeAPI));
        }
        public async Task<bool> IsNameAvailableAsync(string name, IDictionary<string, string> headers)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name must not be null, empty, or whitespace.", nameof(name));
            }

            try
            {
                var localMolecules = await _moleculeRepository.GetByNames([name]);
                var localNameSet = new HashSet<string>(
                    localMolecules.Select(m => m.Name.Value),
                    StringComparer.OrdinalIgnoreCase);

                if (localNameSet.Contains(name))
                {
                    return false;
                }

                var chemVaultResults = await _moleculeAPI.FindByNamesOrSynonymsExact([name], headers);
                var chemVaultNameSet = new HashSet<string>(
                    chemVaultResults
                        .Select(m => m.Name)
                        .Concat(chemVaultResults.SelectMany(m => StringUtilities.ExtractSynonyms(m.Synonyms))),
                    StringComparer.OrdinalIgnoreCase);

                if (chemVaultNameSet.Contains(name))
                {
                    return false;
                }

                return true;
            }
            catch (Exception)
            {
                throw new ApplicationException("Failed to check name availability. Please try again later.");
            }
        }





        public async Task<IDictionary<string, bool>> AreNamesAvailableAsync(IEnumerable<string> names, IDictionary<string, string> headers)
        {
            if (names == null || !names.Any())
                throw new ArgumentException("Name list must not be null or empty.", nameof(names));

            var nameList = names.Where(n => !string.IsNullOrWhiteSpace(n)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            var availabilityMap = nameList.ToDictionary(name => name, name => true, StringComparer.OrdinalIgnoreCase);

            try
            {
                // Local Mlogix check
                var localMolecules = await _moleculeRepository.GetByNames(nameList);
                var localNames = new HashSet<string>(
                    localMolecules.Select(m => m.Name.Value),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var name in nameList)
                {
                    if (localNames.Contains(name))
                        availabilityMap[name] = false;
                }

                // ChemVault check
                var chemVaultMatches = await _moleculeAPI.FindByNamesOrSynonymsExact(nameList, headers);
                var chemVaultNames = new HashSet<string>(
                    chemVaultMatches
                        .Select(m => m.Name)
                        .Concat(chemVaultMatches.SelectMany(m => StringUtilities.ExtractSynonyms(m.Synonyms))),
                    StringComparer.OrdinalIgnoreCase);

                foreach (var name in nameList)
                {
                    if (chemVaultNames.Contains(name))
                        availabilityMap[name] = false;
                }

                return availabilityMap;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Failed to check names availability.");
            }
        }

    }
}
