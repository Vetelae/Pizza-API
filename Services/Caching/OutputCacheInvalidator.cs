using Microsoft.AspNetCore.OutputCaching;

namespace Pizza_API.Services
{
    public interface IOutputCacheInvalidator
    {
        Task EvictByTagAsync(string tag, CancellationToken cancellationToken = default);
    }

    public class OutputCacheInvalidator : IOutputCacheInvalidator
    {
        private readonly IOutputCacheStore _outputCacheStore;
        private readonly ILogger<OutputCacheInvalidator> _logger;

        public OutputCacheInvalidator(
            IOutputCacheStore outputCacheStore,
            ILogger<OutputCacheInvalidator> logger)
        {
            _outputCacheStore = outputCacheStore;
            _logger = logger;
        }

        public async Task EvictByTagAsync(
            string tag,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _outputCacheStore.EvictByTagAsync(tag, cancellationToken);
            }
            catch (Exception exception)
            {
                _logger.LogError(
                    exception,
                    "Failed to evict output cache entries tagged {OutputCacheTag}",
                    tag);
            }
        }
    }
}
