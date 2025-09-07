using PoeCurrencyTracker.Api.Enums;
using PoeCurrencyTracker.Api.Exceptions;
using PoeCurrencyTracker.Api.Models;
using System.Text.Json;

namespace PoeCurrencyTracker.Api.Client
{

    /// <summary>
    /// Client for interacting with the Path of Exile 2 Ninja API
    /// </summary>
    public class Poe2NinjaClient : IDisposable
    {
        private readonly HttpClient _httpClient;
        private readonly string _baseUrl = "https://poe.ninja/poe2/api/economy/temp/overview";
        private readonly Dictionary<EconomyCategory, EconomyEndpoint> _endpoints;
        private bool _disposed = false;

        /// <summary>
        /// Initializes a new instance of the Poe2NinjaClient
        /// </summary>
        /// <param name="httpClient">Optional HttpClient instance. If not provided, a new one will be created.</param>
        public Poe2NinjaClient(HttpClient? httpClient = null)
        {
            _httpClient = httpClient ?? new HttpClient();
            _httpClient.Timeout = TimeSpan.FromSeconds(30);

            if (!_httpClient.DefaultRequestHeaders.Contains("User-Agent"))
            {
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "SimplePoe2NinjaClient/1.0");
            }

            _endpoints = InitializeEndpoints();
        }

        /// <summary>
        /// Initializes the endpoint configurations
        /// </summary>
        private static Dictionary<EconomyCategory, EconomyEndpoint> InitializeEndpoints()
        {
            return new Dictionary<EconomyCategory, EconomyEndpoint>
            {
                [EconomyCategory.Currency] = new EconomyEndpoint
                {
                    Id = 1,
                    Name = "Currency",
                    OverviewName = "Currency",
                    Description = "Basic currencies and orbs"
                },
                [EconomyCategory.Expedition] = new EconomyEndpoint
                {
                    Id = 2,
                    Name = "Expedition",
                    OverviewName = "Expedition",
                    Description = "Expedition artifacts and currencies"
                },
                [EconomyCategory.Essences] = new EconomyEndpoint
                {
                    Id = 3,
                    Name = "Essences",
                    OverviewName = "Essences",
                    Description = "Essences for crafting"
                },
                [EconomyCategory.Fragments] = new EconomyEndpoint
                {
                    Id = 4,
                    Name = "Fragments",
                    OverviewName = "Fragments",
                    Description = "Map fragments"
                },
                [EconomyCategory.UniqueWeapons] = new EconomyEndpoint
                {
                    Id = 10,
                    Name = "Unique Weapons",
                    OverviewName = "UniqueWeapons",
                    Description = "Unique weapons"
                },
                [EconomyCategory.UniqueArmour] = new EconomyEndpoint
                {
                    Id = 11,
                    Name = "Unique Armour",
                    OverviewName = "UniqueArmour",
                    Description = "Unique armor pieces"
                },
                [EconomyCategory.UniqueAccessories] = new EconomyEndpoint
                {
                    Id = 12,
                    Name = "Unique Accessories",
                    OverviewName = "UniqueAccessories",
                    Description = "Unique jewelry and accessories"
                },
                [EconomyCategory.UniqueJewels] = new EconomyEndpoint
                {
                    Id = 13,
                    Name = "Unique Jewels",
                    OverviewName = "UniqueJewels",
                    Description = "Unique jewels"
                },
                [EconomyCategory.SkillGems] = new EconomyEndpoint
                {
                    Id = 14,
                    Name = "Skill Gems",
                    OverviewName = "SkillGems",
                    Description = "Skill and support gems"
                },
                [EconomyCategory.BaseTypes] = new EconomyEndpoint
                {
                    Id = 15,
                    Name = "Base Types",
                    OverviewName = "BaseTypes",
                    Description = "High-value base items"
                },
                [EconomyCategory.Waystones] = new EconomyEndpoint
                {
                    Id = 16,
                    Name = "Waystones",
                    OverviewName = "Waystones",
                    Description = "PoE 2 maps (waystones)"
                },
                [EconomyCategory.Runes] = new EconomyEndpoint
                {
                    Id = 17,
                    Name = "Runes",
                    OverviewName = "Runes",
                    Description = "PoE 2 runes"
                },
                [EconomyCategory.SoulCores] = new EconomyEndpoint
                {
                    Id = 18,
                    Name = "Soul Cores",
                    OverviewName = "SoulCores",
                    Description = "PoE 2 soul cores"
                },
                [EconomyCategory.Charms] = new EconomyEndpoint
                {
                    Id = 19,
                    Name = "Charms",
                    OverviewName = "Charms",
                    Description = "PoE 2 charms"
                },
                [EconomyCategory.Tablets] = new EconomyEndpoint
                {
                    Id = 20,
                    Name = "Tablets",
                    OverviewName = "Tablets",
                    Description = "PoE 2 tablets"
                }
            };
        }

        /// <summary>
        /// Gets an endpoint by its enum value
        /// </summary>
        /// <param name="category">Economy category</param>
        /// <returns>Endpoint configuration</returns>
        public EconomyEndpoint GetEndpoint(EconomyCategory category)
        {
            return _endpoints[category];
        }

        /// <summary>
        /// Gets all available economy endpoints
        /// </summary>
        /// <returns>List of available endpoints</returns>
        public List<EconomyEndpoint> GetAvailableEndpoints()
        {
            return new List<EconomyEndpoint>(_endpoints.Values);
        }

        /// <summary>
        /// Gets an endpoint by its ID
        /// </summary>
        /// <param name="id">Endpoint ID</param>
        /// <returns>Endpoint or null if not found</returns>
        public EconomyEndpoint? GetEndpointById(int id)
        {
            return _endpoints.Values.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Gets an endpoint by its name
        /// </summary>
        /// <param name="name">Endpoint name</param>
        /// <returns>Endpoint or null if not found</returns>
        public EconomyEndpoint? GetEndpointByName(string name)
        {
            return _endpoints.Values.FirstOrDefault(e =>
                string.Equals(e.Name, name, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Fetches raw JSON data from a specific endpoint using enum
        /// </summary>
        /// <param name="league">League name (e.g., "Rise of the Abyssal")</param>
        /// <param name="category">Economy category enum</param>
        /// <returns>Raw JSON string</returns>
        public async Task<string> GetEconomyDataAsync(string league, EconomyCategory category)
        {
            var endpoint = GetEndpoint(category);
            return await GetEconomyDataAsync(league, endpoint);
        }

        /// <summary>
        /// Fetches raw JSON data from a specific endpoint by ID
        /// </summary>
        /// <param name="league">League name (e.g., "Rise of the Abyssal")</param>
        /// <param name="endpointId">ID of the endpoint to fetch from</param>
        /// <returns>Raw JSON string</returns>
        public async Task<string> GetEconomyDataAsync(string league, int endpointId)
        {
            var endpoint = GetEndpointById(endpointId);
            if (endpoint == null)
                throw new ArgumentException($"Endpoint with ID {endpointId} not found");

            return await GetEconomyDataAsync(league, endpoint);
        }

        /// <summary>
        /// Fetches raw JSON data from a specific endpoint by name
        /// </summary>
        /// <param name="league">League name (e.g., "Rise of the Abyssal")</param>
        /// <param name="endpointName">Name of the endpoint to fetch from</param>
        /// <returns>Raw JSON string</returns>
        public async Task<string> GetEconomyDataAsync(string league, string endpointName)
        {
            var endpoint = GetEndpointByName(endpointName);
            if (endpoint == null)
                throw new ArgumentException($"Endpoint with name '{endpointName}' not found");

            return await GetEconomyDataAsync(league, endpoint);
        }

        /// <summary>
        /// Fetches raw JSON data from a specific endpoint
        /// </summary>
        /// <param name="league">League name</param>
        /// <param name="endpoint">Endpoint configuration</param>
        /// <returns>Raw JSON string</returns>
        public async Task<string> GetEconomyDataAsync(string league, EconomyEndpoint endpoint)
        {
            var encodedLeague = Uri.EscapeDataString(league);
            var url = $"{_baseUrl}?leagueName={encodedLeague}&overviewName={endpoint.OverviewName}";

            try
            {
                var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (HttpRequestException ex)
            {
                throw new Poe2NinjaException($"Failed to fetch data from {endpoint.Name} (URL: {url}): {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Fetches and deserializes JSON data to a JsonDocument for easy navigation
        /// </summary>
        /// <param name="league">League name</param>
        /// <param name="category">Economy category enum</param>
        /// <returns>JsonDocument for navigation</returns>
        public async Task<JsonDocument> GetEconomyDataAsJsonAsync(string league, EconomyCategory category)
        {
            var jsonString = await GetEconomyDataAsync(league, category);
            return JsonDocument.Parse(jsonString);
        }

        /// <summary>
        /// Fetches and deserializes JSON data to a JsonDocument for easy navigation
        /// </summary>
        /// <param name="league">League name</param>
        /// <param name="endpointId">Endpoint ID</param>
        /// <returns>JsonDocument for navigation</returns>
        public async Task<JsonDocument> GetEconomyDataAsJsonAsync(string league, int endpointId)
        {
            var jsonString = await GetEconomyDataAsync(league, endpointId);
            return JsonDocument.Parse(jsonString);
        }

        /// <summary>
        /// Fetches and deserializes JSON data to a JsonDocument for easy navigation
        /// </summary>
        /// <param name="league">League name</param>
        /// <param name="endpointName">Endpoint name</param>
        /// <returns>JsonDocument for navigation</returns>
        public async Task<JsonDocument> GetEconomyDataAsJsonAsync(string league, string endpointName)
        {
            var jsonString = await GetEconomyDataAsync(league, endpointName);
            return JsonDocument.Parse(jsonString);
        }

        /// <summary>
        /// Saves endpoint configuration to a JSON file
        /// </summary>
        /// <param name="filePath">Path to save the configuration file</param>
        public async Task SaveEndpointsConfigAsync(string filePath)
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var endpointsList = _endpoints.Values.ToList();
            var json = JsonSerializer.Serialize(endpointsList, options);
            await File.WriteAllTextAsync(filePath, json);
        }

        /// <summary>
        /// Tests connectivity to the PoE 2 Ninja API
        /// </summary>
        /// <param name="league">League to test with</param>
        /// <returns>True if connection successful</returns>
        public async Task<bool> TestConnectionAsync(string league = "Rise of the Abyssal")
        {
            try
            {
                await GetEconomyDataAsync(league, EconomyCategory.Expedition);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Releases all resources used by the Poe2NinjaClient
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases the unmanaged resources and optionally releases the managed resources
        /// </summary>
        /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _httpClient?.Dispose();
                }
                _disposed = true;
            }
        }
    }
}
