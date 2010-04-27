// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2009 rubicon informationstechnologie gmbh, www.rubicon.eu
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

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IDirectoryInfo"/> for a null-object scenario, e.g. for use with the <see cref="InMemoryFileInfo"/>.
  /// </summary>
  public class NullDirectoryInfo : IDirectoryInfo
  {
    public string FullName
    {
      get { return string.Empty; }
    }

    public string Extension
    {
      get { return string.Empty; }
    }

    public string Name
    {
      get { return string.Empty; }
    }

    public bool Exists
    {
      get { return false; }
    }

    public bool IsReadOnly
    {
      get { return false; }
    }

    public DateTime CreationTimeUtc
    {
      get { return DateTime.MinValue; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return DateTime.MinValue; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return DateTime.MinValue; }
    }

    public IDirectoryInfo Directory
    {
      get { return null; }
    }

    public IFileInfo[] GetFiles ()
    {
      return new IFileInfo[0];
    }

    public IDirectoryInfo[] GetDirectories ()
    {
      return new IDirectoryInfo[0];
    }
  }
}