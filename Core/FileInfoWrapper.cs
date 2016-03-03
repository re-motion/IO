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
  /// The <see cref="FileInfoWrapper"/> implements <see cref="IFileInfo"/> and exposes methods for the creation, copying, deletion, moving, 
  /// and opening of files via <see cref="FileInfo"/>.
  /// </summary>
  public class FileInfoWrapper : IFileInfo
  {
    private readonly FileInfo _wrappedInstance;

    public FileInfoWrapper (FileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _wrappedInstance = fileInfo;
    }

    public string PhysicalPath
    {
      get { return _wrappedInstance.FullName; }
    }

    public string FullName
    {
      get { return _wrappedInstance.FullName; }
    }

    public string Extension
    {
      get { return _wrappedInstance.Extension; }
    }

    public string Name
    {
      get { return _wrappedInstance.Name; }
    }

    public long Length
    {
      get { return _wrappedInstance.Length; }
    }

    public IDirectoryInfo Directory
    {
      get { return new DirectoryInfoWrapper(_wrappedInstance.Directory); }
    }

    public bool IsReadOnly
    {
      get { return _wrappedInstance.IsReadOnly; }
    }

    public bool Exists
    {
      get { return _wrappedInstance.Exists; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _wrappedInstance.CreationTimeUtc; }
      set { _wrappedInstance.CreationTimeUtc = value; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _wrappedInstance.LastAccessTimeUtc; }
      set { _wrappedInstance.LastAccessTimeUtc = value; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _wrappedInstance.LastWriteTimeUtc; }
      set { _wrappedInstance.LastWriteTimeUtc = value; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _wrappedInstance.Open (mode, access, share);
    }

    public void Refresh ()
    {
      _wrappedInstance.Refresh();
    }
  }
}