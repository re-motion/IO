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
using System.Collections.Generic;
using System.IO;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// Responsible for taking a full path and turning it into a hierarchy directories (<see cref="IDirectoryInfo"/>) and files (<see cref="IFileInfo"/>).
  /// </summary>
  public class DirectoryTreeResolver
  {
    public delegate IFileInfo FileInfoFactory (string fileName, IDirectoryInfo directory);

    private readonly Dictionary<string, InMemoryDirectoryInfo> _rootDirectories = new Dictionary<string, InMemoryDirectoryInfo>();
    private readonly DateTime _directoryDateTime;

    public DirectoryTreeResolver (DateTime dateTime)
    {
      _directoryDateTime = dateTime;
    }

    public IFileInfo GetFileInfoWithPath (string filePath, FileInfoFactory fileInfoFactory)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("filePath", filePath);
      ArgumentUtility.CheckNotNull ("fileInfoFactory", fileInfoFactory);

      var pathParts = GetPathPaths (filePath);
      var fileName = pathParts[pathParts.Length - 1];

      InMemoryDirectoryInfo directory = null;
      if (pathParts.Length > 1)
        directory = GetLeafDirectoryInfo (GetElementsExceptLast (pathParts));

      var fileInfo = fileInfoFactory (fileName, directory);

      if (directory != null)
        directory.Files.Add (fileInfo);

      return fileInfo;
    }

    public IDirectoryInfo GetLeafDirectoryInfo (string directoryPath)
    {
      ArgumentUtility.CheckNotNull ("directoryPath", directoryPath);

      return GetLeafDirectoryInfo (GetPathPaths (directoryPath));
    }

    public IDirectoryInfo[] GetRootDirectories ()
    {
      var rootDirectoryArray = new InMemoryDirectoryInfo[_rootDirectories.Count];
      _rootDirectories.Values.CopyTo (rootDirectoryArray, 0);
      return rootDirectoryArray;
    }

    private InMemoryDirectoryInfo GetLeafDirectoryInfo (string[] directoryPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("directoryPath", directoryPath);


      if (directoryPath.Length == 1)
      {
        var name = directoryPath[0];
        if (!_rootDirectories.ContainsKey (name))
          _rootDirectories.Add (name, CreateDirectoryInfo (name, null));

        return _rootDirectories[name];
      }
      else
      {
        var parent = GetLeafDirectoryInfo (GetElementsExceptLast (directoryPath));
        var name = string.Join (Path.DirectorySeparatorChar.ToString(), directoryPath);
        var directoryInfo = (InMemoryDirectoryInfo) parent.Directories.Find (d => d.FullName == name);
        if (directoryInfo == null)
          directoryInfo = CreateDirectoryInfo (name, parent);
        parent.Directories.Add (directoryInfo);

        return directoryInfo;
      }
    }

    private InMemoryDirectoryInfo CreateDirectoryInfo (string fullName, InMemoryDirectoryInfo parent)
    {
      return new InMemoryDirectoryInfo (fullName, parent, _directoryDateTime, _directoryDateTime, _directoryDateTime);
    }

    private string[] GetPathPaths (string path)
    {
      return path.Split (Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
    }

    private string[] GetElementsExceptLast (string[] nameParts)
    {
      var directoryNames = new string[nameParts.Length - 1];
      Array.Copy (nameParts, directoryNames, directoryNames.Length);
      return directoryNames;
    }
  }
}