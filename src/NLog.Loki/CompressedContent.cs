using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace NLog.Loki;

/// <summary>
/// GZipped HTTP Content.
/// Inspired from https://programmer.help/blogs/httpclient-and-aps.net-web-api-compression-and-decompression-of-request-content.html
/// by arunmj82 (Tue, 18 Dec 2018).
/// </summary>
internal sealed class CompressedContent : HttpContent
{
    private readonly HttpContent _originalContent;
    private readonly CompressionLevel _level;

    public CompressedContent(HttpContent content, CompressionLevel level)
    {
        _originalContent = content ?? throw new ArgumentNullException("content");
        _level = level;

        // Copy the underlying content's headers
        foreach(var header in _originalContent.Headers)
            _ = Headers.TryAddWithoutValidation(header.Key, header.Value);

        // Add Content-Encoding header
        Headers.ContentEncoding.Add("gzip");
    }

    protected override bool TryComputeLength(out long length)
    {
        length = -1;
        return false;
    }

    protected async override Task SerializeToStreamAsync(Stream stream, TransportContext context)
    {
        using var gzipStream = new GZipStream(stream, _level, leaveOpen: true);
        await _originalContent.CopyToAsync(gzipStream).ConfigureAwait(false);
    }

    private bool _isDisposed;
    protected override void Dispose(bool isDisposing)
    {
        if(!_isDisposed)
        {
            if(isDisposing)
            {
                _originalContent?.Dispose();
            }
            _isDisposed = true;
        }
        base.Dispose(isDisposing);
    }
}
