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
using System.IO;

namespace Remotion.Dms.Shared.Utilities
{
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

    public DateTime CreationTime
    {
      get { return _wrappedInstance.CreationTime; }
      set { _wrappedInstance.CreationTime = value; }
    }

    public DateTime LastAccessTime
    {
      get { return _wrappedInstance.LastAccessTime; }
      set { _wrappedInstance.LastAccessTime = value; }
    }

    public DateTime LastWriteTime
    {
      get { return _wrappedInstance.LastWriteTime; }
      set { _wrappedInstance.LastWriteTime = value; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _wrappedInstance.Open (mode, access, share);
    }

  }
}