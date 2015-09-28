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

    public bool CopyStream (Stream input, Stream output)
    {
      ArgumentUtility.CheckNotNull ("input", input);
      ArgumentUtility.CheckNotNull ("output", output);

      long bytesTransferred = 0;
      byte[] buffer = new byte[BufferSize];
      int bytesRead;
      var inputLength = input.CanSeek ? input.Length : (long?) null;
      do
      {
        bytesRead = input.Read (buffer, 0, buffer.Length);
        output.Write (buffer, 0, bytesRead);
        bytesTransferred += bytesRead;
        if (bytesTransferred > inputLength)
          throw new InvalidOperationException ("The stream returned more data than the specified length.");

        var args = new StreamCopyProgressEventArgs (bytesTransferred, inputLength);
        OnTransferProgress (args);
        if (args.Cancel)
          return false;
      } while (bytesRead != 0);


      if (inputLength.HasValue && bytesTransferred != inputLength)
        throw new InvalidOperationException ("The stream did not return the specified amount of data.");

      return true;
    }

    private void OnTransferProgress (StreamCopyProgressEventArgs args)
    {
      if (TransferProgress != null)
        TransferProgress (this, args);
    }
  }
}