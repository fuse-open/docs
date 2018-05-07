using System.IO;
using System.Text.Encodings.Web;

namespace Builder.Services
{
    public class Layout
    {
        private readonly string _template;
        private readonly string _buildVersion;
        private readonly string _baseUrl;

        public Layout(BuilderSettings settings)
        {
            _template = ReadTemplate(settings.RootPath);
            _buildVersion = settings.BuildVersion;
            _baseUrl = settings.BaseUrl;
        }

        public string Apply(string html, string outline, string title)
        {
            var fullTitle = "Fuse Documentation";
            if (!string.IsNullOrEmpty(title))
            {
                fullTitle = $"{title} - {fullTitle}";
            }

            html = _template.Replace("##BODY##", html)
                            .Replace("##NAVIGATION##", outline)
                            .Replace("##TITLE##", HtmlEncoder.Default.Encode(fullTitle))
                            .Replace("##BUILD_VERSION##", _buildVersion)
                            .Replace("##BASE_URL##", _baseUrl);
            return html;
        }

        private string ReadTemplate(string root) {
            return File.ReadAllText(Path.Combine(root, "layout.html"));
        }
    }
}
