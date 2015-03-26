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

namespace Remotion.IO
{
  public class StreamCopyProgressEventArgs : EventArgs
  {
    private readonly long _currentValue;
    private readonly long _fileLength;
    private bool _cancel;

    public StreamCopyProgressEventArgs (long currentValue, long fileLength)
    {
      _currentValue = currentValue;
      _fileLength = fileLength;
      _cancel = false;
    }

    public long CurrentValue
    {
      get { return _currentValue; }
    }

    public long StreamLength
    {
      get { return _fileLength; }
    }

    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }
  }
}