// This file is part of the re-motion Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// Decorates a <see cref="Stream"/>, so that a <see cref="TimeoutException"/> is thrown if <see cref="Stream.Read"/> takes longer than <see cref="Stream.ReadTimeout"/> or 
  /// if <see cref="Stream.Write"/> takes longer than  <see cref="Stream.WriteTimeout"/>.
  /// </summary>
  public class StreamWithTimeoutDecorator : Stream
  {
    private readonly Stream _inner;

    public StreamWithTimeoutDecorator (Stream inner)
    {
      ArgumentUtility.CheckNotNull ("inner", inner);

      _inner = inner;
    }

    public override void Flush ()
    {
      _inner.Flush();
    }

    public override long Seek (long offset, SeekOrigin origin)
    {
      return _inner.Seek (offset, origin);
    }

    public override void SetLength (long value)
    {
      _inner.SetLength (value);
    }

    public override int Read (byte[] buffer, int offset, int count)
    {
      if (_inner.CanTimeout)
      {
        var asyncResult = _inner.BeginRead (buffer, offset, count, null, null);
        if (!asyncResult.AsyncWaitHandle.WaitOne (TimeSpan.FromMilliseconds (_inner.ReadTimeout)))
          throw new TimeoutException (string.Format ("The read operation exceeded the timout of '{0}' ms.", _inner.ReadTimeout));
        return _inner.EndRead (asyncResult);
      }
      else
      {
        return _inner.Read (buffer, offset, count);
      }
    }

    public override void Write (byte[] buffer, int offset, int count)
    {
      if (_inner.CanTimeout)
      {
        var asyncResult = _inner.BeginWrite (buffer, offset, count, null, null);
        if (!asyncResult.AsyncWaitHandle.WaitOne (TimeSpan.FromMilliseconds (_inner.WriteTimeout)))
          throw new TimeoutException (string.Format ("The write operation exceeded the timout of '{0}' ms.", _inner.WriteTimeout));
        _inner.EndWrite (asyncResult);
      }
      else
      {
        _inner.Write (buffer, offset, count);
      }
    }

    public override IAsyncResult BeginRead (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return _inner.BeginRead (buffer, offset, count, callback, state);
    }

    public override IAsyncResult BeginWrite (byte[] buffer, int offset, int count, AsyncCallback callback, object state)
    {
      return _inner.BeginWrite (buffer, offset, count, callback, state);
    }

    public override int EndRead (IAsyncResult asyncResult)
    {
      return _inner.EndRead (asyncResult);
    }

    public override void EndWrite (IAsyncResult asyncResult)
    {
      _inner.EndWrite (asyncResult);
    }

    public override bool CanTimeout
    {
      get { return _inner.CanTimeout; }
    }

    protected override void Dispose (bool disposing)
    {
      _inner.Dispose();
    }

    public override void Close ()
    {
      _inner.Close();
    }

    public override Task CopyToAsync (Stream destination, int bufferSize, CancellationToken cancellationToken)
    {
      return _inner.CopyToAsync (destination, bufferSize, cancellationToken);
    }

    public override Task FlushAsync (CancellationToken cancellationToken)
    {
      return _inner.FlushAsync (cancellationToken);
    }

    public override Task<int> ReadAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return _inner.ReadAsync (buffer, offset, count, cancellationToken);
    }

    public override int ReadTimeout
    {
      get { return _inner.ReadTimeout; }
      set { _inner.ReadTimeout = value; }
    }

    public override Task WriteAsync (byte[] buffer, int offset, int count, CancellationToken cancellationToken)
    {
      return _inner.WriteAsync (buffer, offset, count, cancellationToken);
    }


    public override int WriteTimeout
    {
      get { return _inner.WriteTimeout; }
      set { _inner.WriteTimeout = value; }
    }

    public override bool CanRead
    {
      get { return _inner.CanRead; }
    }

    public override bool CanSeek
    {
      get { return _inner.CanSeek; }
    }

    public override bool CanWrite
    {
      get { return _inner.CanWrite; }
    }

    public override long Length
    {
      get { return _inner.Length; }
    }

    public override long Position
    {
      get { return _inner.Position; }
      set { _inner.Position = value; }
    }
  }
}