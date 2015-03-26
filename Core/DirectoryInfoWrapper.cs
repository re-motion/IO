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
  /// The <see cref="DirectoryInfoWrapper"/> implements <see cref="IDirectoryInfo"/> and exposes methods for creating, moving, and enumerating 
  /// through directories via <see cref="DirectoryInfo"/>.
  /// </summary>
  public class DirectoryInfoWrapper : IDirectoryInfo
  {
    private readonly DirectoryInfo _wrappedInstance;

    public DirectoryInfoWrapper (DirectoryInfo directoryInfo)
    {
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      _wrappedInstance = directoryInfo;
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
      get
      {
        var folders = _wrappedInstance.FullName.Split (Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
        return folders.GetValue (folders.Length - 1).ToString();
      }
    }

    public bool Exists
    {
      get { return _wrappedInstance.Exists; }
    }

    public bool IsReadOnly
    {
      get { return _wrappedInstance.Attributes.Equals (FileAttributes.ReadOnly); }
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

    public IDirectoryInfo Directory
    {
      get { return this; }
    }

    public IFileInfo[] GetFiles ()
    {
      FileInfoWrapper[] fileInfo = new FileInfoWrapper[_wrappedInstance.GetFiles().Length];
      for (int i = 0; i < _wrappedInstance.GetFiles().Length; i++)
      {
        fileInfo[i] = new FileInfoWrapper(_wrappedInstance.GetFiles()[i]);
      }
      return fileInfo;
    }

    public IDirectoryInfo[] GetDirectories ()
    {
      DirectoryInfoWrapper[] directoryInfo = new DirectoryInfoWrapper[_wrappedInstance.GetDirectories().Length];

      for (int i = 0; i < _wrappedInstance.GetDirectories ().Length; i++)
      {
        directoryInfo[i] = new DirectoryInfoWrapper (_wrappedInstance.GetDirectories ()[i]);
      }
      return directoryInfo;
    }
  }
}