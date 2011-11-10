// This file is part of re-vision (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.Dms.Shared.IO
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