// using Xunit;
// using Moq;
// using System.Net.Http;
// using System.Text;
// using System.Text.Json;
// using System.Threading.Tasks;



// namespace Gene.Test
// {
//     public class GeneTests
// {
//     private readonly HttpClient _client;
//     private readonly Mock<IMediator> _mediatorMock;
//     private readonly Mock<ILogger<YourController>> _loggerMock; // Replace YourController with the actual controller class

//     public GeneTests()
//     {
//         // Set up your test server and client
//         // Initialize _mediatorMock and _loggerMock
//         // Configure your controller with these mocks
//     }

//     [Fact]
//     public async Task AddFiveGenes()
//     {
//         for (int i = 0; i < 5; i++)
//         {
//             var newGene = new NewGeneCommand
//             {
//                 AccessionNumber = $"ACC{i}",
//                 Name = $"Gene{i}",
//                 Function = $"Function{i}",
//                 Product = $"Product{i}",
//                 FunctionalCategory = $"Category{i}"
//             };

//             var content = new StringContent(JsonSerializer.Serialize(newGene), Encoding.UTF8, "application/json");

//             var response = await _client.PostAsync("/api/gene", content); // Adjust the URL as per your route

//             response.EnsureSuccessStatusCode();

//             var responseString = await response.Content.ReadAsStringAsync();
//             var addGeneResponse = JsonSerializer.Deserialize<AddGeneResponse>(responseString);

//             Assert.NotNull(addGeneResponse);
//             Assert.Equal($"Gene added successfully", addGeneResponse.Message);
//         }
//     }
// }

// }