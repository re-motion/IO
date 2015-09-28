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
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// Provides functionality for copying streams with progress report.
  /// </summary>
  public class StreamCopier
  {
    private readonly int _bufferSize;
    public const int DefaultCopyBufferSize = 1024 * 4;

    public event EventHandler<StreamCopyProgressEventArgs> TransferProgress;

    public StreamCopier ()
        : this (DefaultCopyBufferSize)
    {
    }

    public StreamCopier (int bufferSize)
    {
      _bufferSize = bufferSize;
    }

    public int BufferSize
    {
      get { return _bufferSize; }
    }

    /// <summary>
    /// Copies the <paramref name="input"/> <see cref="Stream"/> to the <paramref name="output"/> <see cref="Stream"/>.
    /// </summary>
    /// <param name="input">The input stream. Must not be <see langword="null" />.</param>
    /// <param name="output">The output stream. Must not be <see langword="null" />.</param>
    /// <param name="maxLength">If specified, the maximum length of the <paramref name="input"/> <see cref="Stream"/>.</param>
    /// <returns>
    /// <see langword="true" /> if all bytes were copied and <see langword="false" /> if the copying was either cancelled via the <see cref="TransferProgress"/> event 
    /// or the <paramref name="input"/> <see cref="Stream"/> returned less data than specified by its <see cref="Stream.Length"/> property.</returns>
    /// <exception cref="IOException">
    /// <para>
    /// Thrown if the <paramref name="input"/> <see cref="Stream"/> returned more bytes than specified via <paramref name="maxLength"/>
    /// </para>
    /// <para>- or -</para>
    /// <para>
    /// the <paramref name="input"/> <see cref="Stream"/> is seekable and returned more bytes than specified by its <see cref="Stream.Length"/> property.
    /// </para>
    /// </exception>
    public bool CopyStream (Stream input, Stream output, int? maxLength = null)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      ArgumentUtility.CheckNotNull ("output", output);

      long bytesTransferred = 0;
      byte[] buffer = new byte[BufferSize];
      int bytesRead;

      do
      {
        bytesRead = input.Read (buffer, 0, buffer.Length);
        output.Write (buffer, 0, bytesRead);
        bytesTransferred += bytesRead;

        if (bytesTransferred > maxLength)
          throw new IOException (string.Format ("The stream returned more data ({0} bytes) than the specified maximum ({1} bytes).", bytesTransferred, maxLength));

        var args = new StreamCopyProgressEventArgs (bytesTransferred);
        OnTransferProgress (args);
        if (args.Cancel)
          return false;
      } while (bytesRead != 0);

      var finalInputLength = input.CanSeek ? input.Length : (long?) null;

      if (bytesTransferred > finalInputLength)
        throw new IOException (string.Format ("The stream returned more data than the stream length ({0} bytes) specified.", finalInputLength));

      if (bytesTransferred < finalInputLength)
        return false;

      return true;
    }

    private void OnTransferProgress (StreamCopyProgressEventArgs args)
    {
      if (TransferProgress != null)
        TransferProgress (this, args);
    }
  }
}