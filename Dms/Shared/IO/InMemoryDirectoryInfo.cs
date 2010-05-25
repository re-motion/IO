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
using System.Collections.Generic;
using System.IO;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.Shared.IO
{
  /// <summary>
  /// In-memory implementation of the <see cref="IDirectoryInfo"/> interface.
  /// </summary>
  public class InMemoryDirectoryInfo : IDirectoryInfo
  {
    private readonly string _fullName;
    private readonly List<IDirectoryInfo> _directories = new List<IDirectoryInfo>();
    private readonly List<IFileInfo> _files = new List<IFileInfo>();
    private readonly IDirectoryInfo _parent;
    private readonly DateTime _creationTimeUtc;
    private readonly DateTime _lastAccessTimeUtc;
    private readonly DateTime _lastWriteTimeUtc;

    public InMemoryDirectoryInfo (
        string fullName,
        IDirectoryInfo parent,
        DateTime creationTimeUtc,
        DateTime lastAccessTimeUtc,
        DateTime lastWriteTimeUtc)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fullName", fullName);

      _fullName = fullName;
      _parent = parent;
      _creationTimeUtc = creationTimeUtc;
      _lastAccessTimeUtc = lastAccessTimeUtc;
      _lastWriteTimeUtc = lastWriteTimeUtc;
    }

    public string FullName
    {
      get { return _fullName; }
    }

    public string Name
    {
      get { return Path.GetFileName (_fullName); }
    }

    public string Extension
    {
      get { return string.Empty; }
    }

    public bool Exists
    {
      get { return true; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _creationTimeUtc; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _lastAccessTimeUtc; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _lastWriteTimeUtc; }
    }

    public IDirectoryInfo Directory
    {
      get { return _parent; }
    }

    public IFileInfo[] GetFiles ()
    {
      return _files.ToArray();
    }

    public IDirectoryInfo[] GetDirectories ()
    {
      return _directories.ToArray();
    }

    public List<IFileInfo> Files
    {
      get { return _files; }
    }

    public List<IDirectoryInfo> Directories
    {
      get { return _directories; }
    }

    public override string ToString ()
    {
      return _fullName;
    }
    //public void AddDirectory (IDirectoryInfo directoryInfo)
    //{
    //  ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
    //  _directories.Add (directoryInfo);
    //}

    //public void AddFile (IFileInfo fileInfo)
    //{
    //  ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
    //  _files.Add (fileInfo);
    //}
  }
}