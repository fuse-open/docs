using Builder.Models;
using Microsoft.Extensions.Logging;

namespace Builder.Services
{
    public class ApiIndices : ApiJsonParser<ApiReferenceIndex>
    {
        protected override string SourceDirectoryName { get; } = "indices";
        protected override string SourceDirectoryDisplayName { get; } = "index";

        public ApiIndices(BuilderSettings settings, ILogger<ApiIndices> logger) : base(settings, logger) {}
    }
}