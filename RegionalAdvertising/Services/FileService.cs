using Microsoft.Extensions.Caching.Memory;

namespace Services
{
    public class FileService
    {
        private readonly IMemoryCache _cache;
        private const string CacheKey = "Advertising";
        public FileService(IMemoryCache cache)
        {
            _cache = cache;
        }
        
        public async Task<IResult> ReadFile(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                return Results.BadRequest("File path is required");
            }
            
            if (!File.Exists(path))
            {
               return Results.BadRequest("File not found");
            }
                
            var fileContents =await File.ReadAllLinesAsync(path);
            
            if (!_cache.TryGetValue(CacheKey, out Dictionary<string, List<string>>? advertisings))
                   advertisings = new Dictionary<string, List<string>>();
            
            foreach (var line in fileContents)
            {
                var separator = line.IndexOf(':');
                if (separator <= 0 || separator == line.Length - 1) 
                            continue;

                var key = line.Substring(0, separator).Trim(' ', '"');
                
                var valuesPart = line.Substring(separator + 1).Trim();
                var values  = valuesPart.Split(',', StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim(' ', '"'))
                    .ToList();
                
                if (!advertisings!.ContainsKey(key))
                    advertisings[key] = new List<string>();
                
                foreach (var value in values.Select(v => v.Trim()))
                {   
                    if(value.Contains(":"))
                        continue;
                    if (!advertisings[key].Contains(value))
                             advertisings[key].Add(value);
                }
            }
            _cache.Set(CacheKey, advertisings);
         return Results.Ok(advertisings);
        }

        public IResult GetAdvertisingsByLocation(string location)
        {
            if (string.IsNullOrWhiteSpace(location))
            {
                return Results.BadRequest("Location is required");
            }
            
            if (!_cache.TryGetValue(CacheKey, out Dictionary<string, List<string>>? advertisings))
                return Results.BadRequest("Upload correct file first! (key:value)");
            
            var result = advertisings!
                .Where(kv=> location.Contains(kv.Key, StringComparison.InvariantCultureIgnoreCase))
                .SelectMany(kvp => kvp.Value)
                .Distinct()
                .ToList();
            if (result.Count > 0)
                return Results.Ok(result);
         
            return Results.BadRequest($"No advertisings found for location: {location}");
        }
     }
}