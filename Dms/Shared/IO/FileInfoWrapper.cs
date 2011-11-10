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
using System.IO;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.Shared.IO
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

  }
}