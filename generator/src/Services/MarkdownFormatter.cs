using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Builder.Models;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using CommonMark;
using System.Text;

namespace Builder.Services;

public class MarkdownFormatter
{
    private readonly ReferenceMap _referenceMap;
    private readonly ILogger<MarkdownFormatter> _logger;
    private readonly ConcurrentDictionary<Guid, DeferredMarkdownQueueItem> _deferred = new ConcurrentDictionary<Guid, DeferredMarkdownQueueItem>();

    public MarkdownFormatter(ReferenceMap referenceMap, ILogger<MarkdownFormatter> logger)
    {
        _referenceMap = referenceMap;
        _logger = logger;
    }

    public Task<DeferredMarkdownRenderingResult> DeferredRenderAsync(string markdown, MarkdownPostProcessor postProcessor = null)
    {
        var id = Guid.NewGuid();
        _deferred.TryAdd(id, new DeferredMarkdownQueueItem(markdown, postProcessor));
        return Task.FromResult(new DeferredMarkdownRenderingResult(id, $"<markdown_{id}/>"));
    }

    public async Task PerformDeferredRenderingAsync()
    {
        var queue = _deferred.ToList();

        var sw = Stopwatch.StartNew();
        _logger.LogDebug($"Performing deferred markdown rendering of {queue.Count} fragments");

        foreach (var item in queue) {
            await ParseMarkdown(item.Key, item.Value.Markdown);
        }

        _logger.LogDebug($"Successfully rendered {queue.Count} markdown fragments in {sw.Elapsed.TotalMilliseconds} ms");
    }

    private async Task ParseMarkdown(Guid id, string markdown)
    {
        var html = CommonMarkConverter.Convert(markdown);
        await UpdateDeferredResultAsync(id, html);
    }

    private async Task UpdateDeferredResultAsync(Guid id, string html)
    {
        DeferredMarkdownQueueItem existing;
        if (!_deferred.TryGetValue(id, out existing)) return;

        // Call the post processing hook
        if (existing.PostProcessor != null)
        {
            html = await existing.PostProcessor.ProcessAsync(html);
        }
        existing.Html = NormalizeCodeLanguageAndIndentation(html);
    }

    public Task<DeferredMarkdownUpdateResult> ApplyDeferredAsync(string input, params Guid[] ids)
    {
        // Note: We work the IDs in reverse order, as there might have been rendered some DOM
        // that contains other markdown parsing result; parsing them in reverse order ensures
        // that we never leave dangling <markdown_<id>/> tags without properly parsing them.

        foreach (var id in ids.Reverse())
        {
            DeferredMarkdownQueueItem item;
            if (!_deferred.TryGetValue(id, out item))
            {
                throw new ArgumentException($"Tried applying deferred markdown rendering payload with id {id} - unknown id");
            }

            if (item.Html == null)
            {
                throw new ArgumentException($"Tried applying deferred markdown rendering payload with id {id} which has not yet been rendered");
            }

            input = input.Replace($"<markdown_{id}/>", item.Html);
        }

        return Task.FromResult(new DeferredMarkdownUpdateResult(input));
    }

    private class DeferredMarkdownQueueItem
    {
        public string Markdown { get; }
        public string Html { get; set; }
        public MarkdownPostProcessor PostProcessor { get; }

        public DeferredMarkdownQueueItem(string markdown, MarkdownPostProcessor postProcessor)
        {
            Markdown = markdown;
            PostProcessor = postProcessor;
        }
    }

    private static string NormalizeCodeLanguageAndIndentation(string html)
    {
        // Replace uno/ux languages with csharp/xml and more
        html = html.Replace("<code class=\"language-uno\"", "<code class=\"language-csharp\"")
                   .Replace("<code class=\"language-ux\"", "<code class=\"language-xml\"")
                   .Replace("<code class=\"language-cs\"", "<code class=\"language-csharp\"")
                   .Replace("<code class=\"language-sh\"", "<code class=\"language-shell\"")
                   // ".s" is usually used for Assembly languages, but we call it "shell" here
                   // https://github.com/github/linguist/blob/master/lib/linguist/languages.yml
                   .Replace("<code class=\"language-s\"", "<code class=\"language-shell\"");

        // Normalize indentation inside <code> tags
        return NormalizeCodeIndentation(html);
    }

    private static string NormalizeCodeIndentation(string html, int startIndex = 0)
    {
        // Find start tag
        startIndex = html.IndexOf("<code ", startIndex);
        if (startIndex == -1)
            return html;

        startIndex = html.IndexOf('>', startIndex + 6);
        if (startIndex == -1)
            return html;

        startIndex += 1; // Move past '>'

        // Find end tag
        var endIndex = html.IndexOf("</code>", startIndex);
        if (endIndex == -1)
            return html;

        // Extract code
        var original = html.Substring(startIndex, endIndex - startIndex);

        // Normalize spaces and line endings
        var code = original.Replace("\t", "    ")
                           .Replace("\r", "")
                           .TrimEnd();

        // Unindent while there's more left
        while (AllLinesStartsWith(code, "    "))
            code = code.Substring(4).Replace("\n    ", "\n");
        while (AllLinesStartsWith(code, "  "))
            code = code.Substring(2).Replace("\n  ", "\n");
        while (AllLinesStartsWith(code, " "))
            code = code.Substring(1).Replace("\n ", "\n");

        // Build new HTML if something changed
        if (code != original)
        {
            var sb = new StringBuilder();
            sb.Append(html, 0, startIndex);
            sb.Append(code);
            sb.Append(html, endIndex, html.Length - endIndex);

            // Update endIndex and html
            endIndex = startIndex + code.Length;
            html = sb.ToString();
        }

        // Continue searching (recursion)
        return NormalizeCodeIndentation(html, endIndex + 7);
    }

    private static bool AllLinesStartsWith(string code, string value)
    {
        if (code.IndexOf('\n') == -1)
            return code.StartsWith(value);

        foreach (var line in code.Split('\n'))
            if (!line.StartsWith(value) && line.Trim().Length > 0)
                return false;

        return true;
    }
}
