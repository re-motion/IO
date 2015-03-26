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

namespace Remotion.IO
{
  /// <summary>
  /// This class wraps around a <see cref="Stream"/> and exposes an interface to a readonly stream.
  /// </summary>
  public abstract class ReadOnlyStream : Stream
  {
    protected ReadOnlyStream (Stream stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);
      _stream = stream;
    }
    private readonly Stream _stream;

    public override void Flush ()
    {
      Stream.Flush();
    }

    public override long Seek (long offset, SeekOrigin origin)
    {
      return Stream.Seek (offset, origin);
    }

    public override void SetLength (long value)
    {
      throw new NotImplementedException();
    }

    public override int Read (byte[] buffer, int offset, int count)
    {
      return Stream.Read (buffer, offset, count);
    }

    public override void Write (byte[] buffer, int offset, int count)
    {
      throw new NotImplementedException();
    }

    public override void Close ()
    {
      _stream.Close();
      base.Close ();
    }
    
    public override bool CanRead
    {
      get { return Stream.CanRead; }
    }

    public override bool CanSeek
    {
      get { return Stream.CanSeek; }
    }

    public override bool CanWrite
    {
      get { return false; }
    }

    public override long Length
    {
      get { return Stream.Length; }
    }

    public override long Position
    {
      get { return Stream.Position; }
      set { Stream.Position = value; }
    }

    protected Stream Stream
    {
      get { return _stream; }
    }

    
  }
}