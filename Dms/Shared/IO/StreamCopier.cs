// This file is part of re-vision (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Affero General Public License version 3.0 
// as published by the Free Software Foundation.
// 
// This program is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Affero General Public License for more details.
// 
// You should have received a copy of the GNU Affero General Public License
// along with this program; if not, see http://www.gnu.org/licenses.
// 
// Additional permissions are listed in the file re-motion_exceptions.txt.
// 
using System;
using System.IO;
using Remotion.Utilities;

namespace Remotion.Dms.Shared.IO
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

    public bool CopyStream (Stream input, Stream output, long inputLength)
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

        var args = new StreamCopyProgressEventArgs (bytesTransferred, inputLength);
        OnTransferProgress (args);
        if (args.Cancel)
          return false;
      } while (bytesRead != 0);
      return true;
    }

    private void OnTransferProgress (StreamCopyProgressEventArgs args)
    {
      if (TransferProgress != null)
        TransferProgress (this, args);
    }
  }
}