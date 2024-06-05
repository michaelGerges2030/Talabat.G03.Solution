using StackExchange.Redis;
using System.Text.Json;
using Talabat.Core.Services.Contract;

namespace Talabat.APIs.CacheService
{
	public class ResponseCacheService : IResponseCacheService
	{
		private readonly IDatabase _database;
        public ResponseCacheService(IConnectionMultiplexer resis)
        {
            _database = resis.GetDatabase();
        }

        public async Task CacheResponseAsync(string Key, object Response, TimeSpan timeToLive)
		{
			if (Response is null) return;

			var serializeOptions = new JsonSerializerOptions() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

			var serializedResponse = JsonSerializer.Serialize(Response, serializeOptions);

			await _database.StringSetAsync(Key, serializedResponse, timeToLive);

		}

		public async Task<string?> GetCachedResponseAsync(string Key)
		{
		    var response = await _database.StringGetAsync(Key);

			if (response.IsNullOrEmpty) return null;

			return response;
		}
	}
}
