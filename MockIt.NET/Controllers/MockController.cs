using JsonMockServer.Models;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace JsonMockServer.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MockController : ControllerBase
    {
        private readonly IWebHostEnvironment _env;
        private readonly string _mockFolderPath;

        public MockController(IWebHostEnvironment env)
        {
            _env = env;
            _mockFolderPath = Path.Combine(_env.ContentRootPath, "MockRoutes");
            if (!Directory.Exists(_mockFolderPath))
                Directory.CreateDirectory(_mockFolderPath);
        }

        [HttpPost("create")]
        public IActionResult CreateMockRoute([FromBody] MockRoute mockRoute)
        {
            if (string.IsNullOrWhiteSpace(mockRoute.RouteName))
                return BadRequest("RouteName is required.");

            // Convert route name to a valid file format (removing path params)
            string fileName = Regex.Replace(mockRoute.RouteName, @"{[^}]+}", "_param_")
                                   .Trim('/')
                                   .Replace('/', '_') + ".json";
            string filePath = Path.Combine(_mockFolderPath, fileName);

            // Save all responses into the single file
            string jsonContent = JsonSerializer.Serialize(mockRoute, new JsonSerializerOptions { WriteIndented = true });
            System.IO.File.WriteAllText(filePath, jsonContent);

            return Ok(new { message = "Mock route created successfully.", route = mockRoute });
        }

        [HttpGet, HttpPost, HttpPut, HttpDelete, HttpPatch]
        [Route("{**route}")]
        public IActionResult HandleMockRequest(string route, [FromQuery] Dictionary<string, string> queryParams)
        {
            // Generate file name based on route (replace path params with `_param_`)
            string fileName = Regex.Replace(route, @"\d+", "_param_") // Replace numbers (IDs) with `_param_`
                                   .Replace("/", "_") + ".json";
            string filePath = Path.Combine(_mockFolderPath, fileName);

            if (!System.IO.File.Exists(filePath))
                return NotFound(new { message = "Mock route not found." });

            // Read JSON file
            string jsonData = System.IO.File.ReadAllText(filePath);
            MockRoute mockRoute = JsonSerializer.Deserialize<MockRoute>(jsonData);

            // Extract dynamic path parameters from request
            var routeSegments = route.Split('/');
            var definedSegments = mockRoute.RouteName.Split('/');
            var extractedParams = new Dictionary<string, string>();

            for (int i = 0; i < definedSegments.Length; i++)
            {
                if (definedSegments[i].StartsWith("{") && definedSegments[i].EndsWith("}"))
                {
                    string paramName = definedSegments[i].Trim('{', '}');
                    extractedParams[paramName] = routeSegments[i];
                }
            }

            // Find the best matching response
            foreach (var response in mockRoute.Responses)
            {
                bool pathMatch = true, queryMatch = true;

                // Match path params
                if (response.Conditions != null && response.Conditions.PathParams != null)
                {
                    foreach (var param in response.Conditions.PathParams)
                    {
                        if (param.Value != "*" && extractedParams.TryGetValue(param.Key, out string actualValue))
                        {
                            if (actualValue != param.Value)
                            {
                                pathMatch = false;
                                break;
                            }
                        }
                    }
                }

                // Match query params (if any)
                if (response.Conditions != null && response.Conditions.QueryParams != null)
                {
                    foreach (var param in response.Conditions.QueryParams)
                    {
                        if (!queryParams.TryGetValue(param.Key, out string actualValue) || actualValue != param.Value)
                        {
                            queryMatch = false;
                            break;
                        }
                    }
                }

                // If both path and query params match, return the response
                if (pathMatch && queryMatch)
                {
                    return StatusCode(response.StatusCode, response.Response);
                }
            }

            return NotFound(new { message = "No matching response found." });
        }
    }
}
