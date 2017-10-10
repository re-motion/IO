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
using System.Threading;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// Implements <see cref="IFileSystemHelper"/>
  /// </summary>
  public class FileSystemHelper : IFileSystemHelper
  {
    public FileSystemHelper ()
    {
    }

    public FileStream FileOpen (string path, FileMode mode, FileAccess access, FileShare share)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.Open (path, mode, access, share);
    }

    public bool FileExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.Exists (path);
    }

    public void FileDelete (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      File.Delete (path);

      while (File.Exists (path))
        Thread.Sleep (10);
    }

    public void FileMove (string sourceFile, string destinationFile)
    {
      ArgumentUtility.CheckNotNull ("sourceFile", sourceFile);
      ArgumentUtility.CheckNotNull ("destinationFile", destinationFile);

      File.Move (sourceFile, destinationFile);

      while (File.Exists (sourceFile) || !File.Exists (destinationFile))
        Thread.Sleep (10);
    }

    public IDirectoryInfo DirectoryCreate (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      return new DirectoryInfoWrapper (Directory.CreateDirectory (path));
    }

    public bool DirectoryExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return Directory.Exists (path);
    }

    public void DirectoryDelete (string path, bool recursive)
    {
      Directory.Delete (path, recursive);
    }

    public IFileInfo[] GetFilesOfDirectory (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);

      if (!Directory.Exists (path))
        return new IFileInfo[0];

      List<IFileInfo> fileInfos = new List<IFileInfo>();
      return GetFilesInSubdirectories (path, fileInfos);
    }

    private IFileInfo[] GetFilesInSubdirectories (string path, List<IFileInfo> fileInfos)
    {
      DirectoryInfo directoryInfo = new DirectoryInfo (path);
      DirectoryInfo[] subDirectories = directoryInfo.GetDirectories();

      foreach (FileInfo fileInfo in directoryInfo.GetFiles())
        fileInfos.Add (new FileInfoWrapper (fileInfo));

      if (subDirectories.Length > 0)
      {
        foreach (DirectoryInfo subDirectoryInfo in subDirectories)
          GetFilesInSubdirectories (Path.Combine (path, subDirectoryInfo.Name), fileInfos); //Path.Combine
      }
      return fileInfos.ToArray();
    }
  }
}