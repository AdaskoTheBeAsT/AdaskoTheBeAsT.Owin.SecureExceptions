using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace AdaskoTheBeAsT.Owin.SecureExceptions.WebApi2;

public class OutputStreamAdapter
        : Stream
{
#pragma warning disable MA0023 // Add RegexOptions.ExplicitCapture
    private static readonly Regex FindMessageWithUriRegex = new Regex(
        @"^(.*""[Mm]essage"":\s*""No\sHTTP\sresource\swas\sfound\sthat\smatches\sthe\srequest\sURI)(.*)",
        RegexOptions.Compiled | RegexOptions.Multiline,
        TimeSpan.FromSeconds(5));

    private static readonly Regex ReplaceUriRegex = new Regex(
        @"^(.*""[Mm]essage"":\s*""No\sHTTP\sresource\swas\sfound\sthat\smatches\sthe\srequest\sURI)\s*'.*'([^']*)$",
        RegexOptions.Compiled | RegexOptions.Multiline,
        TimeSpan.FromSeconds(5));

    private static readonly Regex FindMessageDetailWithControllerNameRegex = new Regex(
        @"^(.*""[Mm]essageDetail"":\s*""No\stype\swas\sfound\sthat\smatches\sthe\scontroller\snamed)(.*)",
        RegexOptions.Compiled | RegexOptions.Multiline,
        TimeSpan.FromSeconds(5));

    private static readonly Regex ReplaceControllerNameRegex = new Regex(
        @"^(.*""[Mm]essageDetail"":\s*""No\stype\swas\sfound\sthat\smatches\sthe\scontroller\sname)d\s*'.*'([^']*)$",
        RegexOptions.Compiled | RegexOptions.Multiline,
        TimeSpan.FromSeconds(5));
#pragma warning restore MA0023 // Add RegexOptions.ExplicitCapture

    private readonly Stream _originalStream;
#pragma warning disable CC0033 // Dispose Fields Properly
    private readonly MemoryStream _initialBlockStream = new MemoryStream();
#pragma warning restore CC0033 // Dispose Fields Properly
    private readonly int _initialBlockSize = 8192;
    private bool _initialBlockProcessed;

    public OutputStreamAdapter(
        Stream originalStream)
    {
        _originalStream = originalStream ?? throw new ArgumentNullException(nameof(originalStream));
    }

    public override bool CanRead => _originalStream.CanRead;

    public override bool CanSeek => _originalStream.CanSeek;

    public override bool CanWrite => _originalStream.CanWrite;

    public override long Length => _originalStream.Length;

    public override long Position
    {
        get => _originalStream.Position;
        set => _originalStream.Position = value;
    }

    public override void Write(
        byte[] buffer,
        int offset,
        int count)
    {
        if (!_initialBlockProcessed)
        {
            // Write to the initial block buffer until it's full or the first write is completed
            var bytesToWrite = Math.Min(count, _initialBlockSize);
            _initialBlockStream.Write(buffer, offset, bytesToWrite);

            ProcessInitialBlock();
            _initialBlockProcessed = true;

            // Write the remainder of the buffer if any
            if (bytesToWrite < count)
            {
                _originalStream.Write(buffer, offset + bytesToWrite, count - bytesToWrite);
            }
        }
        else
        {
            // Directly write to the original stream once the initial block has been processed
            _originalStream.Write(buffer, offset, count);
        }
    }

    public override async Task WriteAsync(
        byte[] buffer,
        int offset,
        int count,
        CancellationToken cancellationToken)
    {
        if (!_initialBlockProcessed)
        {
            var bytesToWrite = Math.Min(count, _initialBlockSize);
            await _initialBlockStream.WriteAsync(buffer, offset, bytesToWrite, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);

            await ProcessInitialBlockAsync()
                .ConfigureAwait(continueOnCapturedContext: false);
            _initialBlockProcessed = true;

            if (bytesToWrite < count)
            {
                await _originalStream.WriteAsync(
                    buffer,
                    offset + bytesToWrite,
                    count - bytesToWrite,
                    cancellationToken)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
        else
        {
            await _originalStream.WriteAsync(buffer, offset, count, cancellationToken)
                .ConfigureAwait(continueOnCapturedContext: false);
        }
    }

    public override void Flush() => _originalStream.Flush();

    public override Task FlushAsync(CancellationToken cancellationToken) => _originalStream.FlushAsync(cancellationToken);

    public override long Seek(
        long offset,
        SeekOrigin origin) => _originalStream.Seek(offset, origin);

    public override void SetLength(long value) => _originalStream.SetLength(value);

    public override int Read(
        byte[] buffer,
        int offset,
        int count) => _originalStream.Read(buffer, offset, count);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _initialBlockStream.Dispose();
        }

        base.Dispose(disposing);
    }

    private static bool CheckIfContainUri(string content) =>
        !string.IsNullOrEmpty(content) &&
        FindMessageWithUriRegex.IsMatch(content);

    private static string RemoveUri(string content) =>
        ReplaceUriRegex.Replace(
            content,
            "$1$2");

    private static bool CheckIfContainControllerName(string content) =>
        !string.IsNullOrEmpty(content) &&
        FindMessageDetailWithControllerNameRegex.IsMatch(content);

    private static string RemoveControllerName(string content) =>
        ReplaceControllerNameRegex.Replace(
            content,
            "$1$2");

    private void ProcessInitialBlock()
    {
        // Reset the position to read from the beginning
        _initialBlockStream.Position = 0;
        var originalBytes = _initialBlockStream.ToArray();
        using (var sr = new StreamReader(_initialBlockStream))
        {
            var content = sr.ReadToEnd();

            var found = false;
            if (CheckIfContainUri(content))
            {
                found = true;
                content = RemoveUri(content);
            }

            if (CheckIfContainControllerName(content))
            {
                found = true;
                content = RemoveControllerName(content);
            }

            if (!found)
            {
                _originalStream.Write(originalBytes, 0, originalBytes.Length);
            }
            else
            {
                var modifiedBytes = Encoding.UTF8.GetBytes(content);
                _originalStream.Write(modifiedBytes, 0, modifiedBytes.Length);
            }
        }
    }

    private async Task ProcessInitialBlockAsync()
    {
        _initialBlockStream.Position = 0;
        var originalBytes = _initialBlockStream.ToArray();
        using (var sr = new StreamReader(_initialBlockStream))
        {
            var content = await sr.ReadToEndAsync()
                .ConfigureAwait(continueOnCapturedContext: false);

            var found = false;
            if (CheckIfContainUri(content))
            {
                found = true;
                content = RemoveUri(content);
            }

            if (CheckIfContainControllerName(content))
            {
                found = true;
                content = RemoveControllerName(content);
            }

            if (!found)
            {
                await _originalStream.WriteAsync(originalBytes, 0, originalBytes.Length)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
            else
            {
                var modifiedBytes = Encoding.UTF8.GetBytes(content);
                await _originalStream.WriteAsync(modifiedBytes, 0, modifiedBytes.Length)
                    .ConfigureAwait(continueOnCapturedContext: false);
            }
        }
    }
}
