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
using Remotion.Utilities;

namespace Remotion.IO
{
  public class ArchiveBuilderProgressEventArgs : EventArgs
  {
    private readonly long _currentFileValue;
    private readonly long _currentTotalValue;
    private readonly int _currentFileIndex;
    private readonly string _currentFileFullName;
    private readonly long _currentEstimatedFileSize;
    private readonly int? _currentEstimatedFileCount;
    private bool _cancel;

    public ArchiveBuilderProgressEventArgs (
        long currentFileValue,
        long currentTotalValue,
        int currentFileIndex,
        string currentFileFullName,
        long currentEstimatedFileSize,
        int? currentEstimatedFileCount)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("currentFileFullName", currentFileFullName);

      _currentFileValue = currentFileValue;
      _currentTotalValue = currentTotalValue;
      _currentFileIndex = currentFileIndex;
      _currentFileFullName = currentFileFullName;
      _currentEstimatedFileSize = currentEstimatedFileSize;
      _currentEstimatedFileCount = currentEstimatedFileCount;
      _cancel = false;
    }

    public long CurrentFileValue
    {
      get { return _currentFileValue; }
    }

    public bool Cancel
    {
      get { return _cancel; }
      set { _cancel = value; }
    }

    public long CurrentTotalValue
    {
      get { return _currentTotalValue; }
    }

    public int CurrentFileIndex
    {
      get { return _currentFileIndex; }
    }

    public string CurrentFileFullName
    {
      get { return _currentFileFullName; }
    }

    public long CurrentEstimatedFileSize
    {
      get { return _currentEstimatedFileSize; }
    }

    public int? CurrentEstimatedFileCount
    {
      get { return _currentEstimatedFileCount; }
    }
  }
}