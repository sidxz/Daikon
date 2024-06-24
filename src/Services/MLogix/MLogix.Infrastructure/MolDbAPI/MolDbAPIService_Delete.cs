
using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MLogix.Application.Contracts.Infrastructure;
using MLogix.Application.DTOs.MolDbAPI;
using CQRS.Core.Infrastructure;
using MediatR;
using System.Net;
namespace MLogix.Infrastructure.MolDbAPI
{
    public partial class MolDbAPIService : IMolDbAPIService
    {
        public async Task<HttpStatusCode> DeleteMoleculeById(Guid id, IDictionary<string, string> headers)
        {
            try
            {
                string apiUrl = $"{_molDbApiUrl}/molecule/{id}";
                var request = new HttpRequestMessage(HttpMethod.Delete, apiUrl);
                request.AddHeaders(headers);
                HttpResponseMessage response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Molecule deleted successfully by ID: {MoleculeId}", id);
                    return HttpStatusCode.OK;
                }
                else
                {
                    // Handle non-success status code
                    _logger.LogWarning("Failed to delete molecule by ID. Status Code: {StatusCode}", response.StatusCode);
                    return response.StatusCode;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in DeleteMoleculeById");
                return HttpStatusCode.InternalServerError;
            }
        }


    }
}