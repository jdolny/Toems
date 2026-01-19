using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

/// <summary>
/// Class for streaming data with throttling support.
/// </summary>
/// <remarks>http://www.codeproject.com/Articles/18243/Bandwidth-throttling</remarks>
public class ThrottledStream : Stream
{
    /// <summary>
    /// A constant used to specify an infinite number of bytes that can be transferred per second.
    /// </summary>
    public const long Infinite = 0;

    /// <summary>
    /// The base stream.
    /// </summary>
    private Stream _baseStream;

    /// <summary>
    /// The maximum bytes per second that can be transferred through the base stream.
    /// </summary>
    private long _maximumBytesPerSecond;

    /// <summary>
    /// The number of bytes that has been transferred since the last throttle.
    /// </summary>
    private long _byteCount;

    protected Stopwatch _stopWatch;

    /// <summary>
    /// Gets or sets the maximum bytes per second that can be transferred through the base stream.
    /// </summary>
    /// <value>The maximum bytes per second.</value>
    /// <exception cref="T:System.ArgumentOutOfRangeException">Value is negative. </exception>
    public long MaximumBytesPerSecond
    {
        get
        {
            return _maximumBytesPerSecond;
        }
        set
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException("MaximumBytesPerSecond",
                    value, "The maximum number of bytes per second can't be negative.");
            }
            if (_maximumBytesPerSecond != value)
            {
                _maximumBytesPerSecond = value;
                Reset();
            }
        }
    }

    public int BlockSize { get; set; }

    /// <summary>
    /// Gets a value indicating whether the current stream supports reading.
    /// </summary>
    /// <returns>true if the stream supports reading; otherwise, false.</returns>
    public override bool CanRead
    {
        get
        {
            return _baseStream.CanRead;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current stream supports seeking.
    /// </summary>
    /// <value></value>
    /// <returns>true if the stream supports seeking; otherwise, false.</returns>
    public override bool CanSeek
    {
        get
        {
            return _baseStream.CanSeek;
        }
    }

    /// <summary>
    /// Gets a value indicating whether the current stream supports writing.
    /// </summary>
    /// <value></value>
    /// <returns>true if the stream supports writing; otherwise, false.</returns>
    public override bool CanWrite
    {
        get
        {
            return _baseStream.CanWrite;
        }
    }

    /// <summary>
    /// Gets the length in bytes of the stream.
    /// </summary>
    /// <value></value>
    /// <returns>A long value representing the length of the stream in bytes.</returns>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support seeking. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Length
    {
        get
        {
            return _baseStream.Length;
        }
    }

    /// <summary>
    /// Gets or sets the position within the current stream.
    /// </summary>
    /// <value></value>
    /// <returns>The current position within the stream.</returns>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support seeking. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Position
    {
        get
        {
            return _baseStream.Position;
        }
        set
        {
            _baseStream.Position = value;
        }
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ThrottledStream"/> class with an
    /// infinite amount of bytes that can be processed.
    /// </summary>
    /// <param name="baseStream">The base stream.</param>
    public ThrottledStream(Stream baseStream)
        : this(baseStream, ThrottledStream.Infinite)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="T:ThrottledStream"/> class.
    /// </summary>
    /// <param name="baseStream">The base stream.</param>
    /// <param name="maximumBytesPerSecond">The maximum bytes per second that can be transferred through the base stream.</param>
    /// <exception cref="ArgumentNullException">Thrown when <see cref="baseStream"/> is a null reference.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when <see cref="maximumBytesPerSecond"/> is a negative value.</exception>
    public ThrottledStream(Stream baseStream, long maximumBytesPerSecond)
    {
        if (baseStream == null)
        {
            throw new ArgumentNullException("baseStream");
        }

        _baseStream = baseStream;
        _stopWatch = Stopwatch.StartNew();
        MaximumBytesPerSecond = maximumBytesPerSecond;
        BlockSize = 8192;
        _byteCount = 0;
    }

    /// <summary>
    /// Clears all buffers for this stream and causes any buffered data to be written to the underlying device.
    /// </summary>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs.</exception>
    public override void Flush()
    {
        _baseStream.Flush();
    }

    /// <summary>
    /// Reads a sequence of bytes from the current stream and advances the position within the stream by the number of bytes read.
    /// </summary>
    /// <param name="buffer">An array of bytes. When this method returns, the buffer contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin storing the data read from the current stream.</param>
    /// <param name="count">The maximum number of bytes to be read from the current stream.</param>
    /// <returns>
    /// The total number of bytes read into the buffer. This can be less than the number of bytes requested if that many bytes are not currently available, or zero (0) if the end of the stream has been reached.
    /// </returns>
    /// <exception cref="T:System.ArgumentException">The sum of offset and count is larger than the buffer length. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support reading. </exception>
    /// <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
    public override int Read(byte[] buffer, int offset, int count)
    {
        int total = 0;
        while (count > BlockSize)
        {
            Throttle(BlockSize);
            int rb = _baseStream.Read(buffer, offset, BlockSize);
            total += rb;
            if (rb < BlockSize)
                return total;
            offset += BlockSize;
            count -= BlockSize;
        }
        Throttle(count);
        return total + _baseStream.Read(buffer, offset, count);
    }

    /// <summary>
    /// Sets the position within the current stream.
    /// </summary>
    /// <param name="offset">A byte offset relative to the origin parameter.</param>
    /// <param name="origin">A value of type <see cref="T:System.IO.SeekOrigin"></see> indicating the reference point used to obtain the new position.</param>
    /// <returns>
    /// The new position within the current stream.
    /// </returns>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support seeking, such as if the stream is constructed from a pipe or console output. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override long Seek(long offset, SeekOrigin origin)
    {
        return _baseStream.Seek(offset, origin);
    }

    /// <summary>
    /// Sets the length of the current stream.
    /// </summary>
    /// <param name="value">The desired length of the current stream in bytes.</param>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support both writing and seeking, such as if the stream is constructed from a pipe or console output. </exception>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    public override void SetLength(long value)
    {
        _baseStream.SetLength(value);
    }

    /// <summary>
    /// Writes a sequence of bytes to the current stream and advances the current position within this stream by the number of bytes written.
    /// </summary>
    /// <param name="buffer">An array of bytes. This method copies count bytes from buffer to the current stream.</param>
    /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
    /// <param name="count">The number of bytes to be written to the current stream.</param>
    /// <exception cref="T:System.IO.IOException">An I/O error occurs. </exception>
    /// <exception cref="T:System.NotSupportedException">The base stream does not support writing. </exception>
    /// <exception cref="T:System.ObjectDisposedException">Methods were called after the stream was closed. </exception>
    /// <exception cref="T:System.ArgumentNullException">buffer is null. </exception>
    /// <exception cref="T:System.ArgumentException">The sum of offset and count is greater than the buffer length. </exception>
    /// <exception cref="T:System.ArgumentOutOfRangeException">offset or count is negative. </exception>
    public override void Write(byte[] buffer, int offset, int count)
    {
        while (count > BlockSize)
        {
            Throttle(BlockSize);
            _baseStream.Write(buffer, offset, BlockSize);
            offset += BlockSize;
            count -= BlockSize;
        }
        Throttle(count);
        _baseStream.Write(buffer, offset, count);
    }

    public override int ReadByte()
    {
        Throttle(1);
        return _baseStream.ReadByte();
    }

    public override void WriteByte(byte value)
    {
        Throttle(1);
        _baseStream.WriteByte(value);
    }

    /// <summary>
    /// Returns a <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </summary>
    /// <returns>
    /// A <see cref="T:System.String"></see> that represents the current <see cref="T:System.Object"></see>.
    /// </returns>
    public override string ToString()
    {
        return _baseStream.ToString();
    }

    /// <summary>
    /// Throttles for the specified buffer size in bytes.
    /// </summary>
    /// <param name="bufferSizeInBytes">The buffer size in bytes.</param>
    protected void Throttle(int bufferSizeInBytes)
    {
        if (_maximumBytesPerSecond <= 0 || bufferSizeInBytes <= 0)
            return;

        _byteCount += bufferSizeInBytes;
        if (_byteCount < 8192) // Avoid throttling for small bursts
            return;

        long elapsedMilliseconds = _stopWatch.ElapsedMilliseconds;
        if (elapsedMilliseconds <= 0)
            return;

        long bps = _byteCount * 1000L / elapsedMilliseconds;
        if (bps <= _maximumBytesPerSecond)
            return;

        long wakeElapsed = _byteCount * 1000L / _maximumBytesPerSecond;
        int toSleep = (int)(wakeElapsed - elapsedMilliseconds);

        if (toSleep > 10) // Only sleep for significant durations
        {
            try
            {
                Thread.Sleep(toSleep);
            }
            catch (ThreadAbortException)
            {
                // Ignore
            }
            Reset();
        }
    }

    /// <summary>
    /// Will reset the bytecount to 0 and reset the start time to the current time.
    /// </summary>
    protected void Reset()
    {
        if (_stopWatch.ElapsedMilliseconds > 1000)
        {
            _byteCount = 0;
            _stopWatch.Restart();
        }
    }

}