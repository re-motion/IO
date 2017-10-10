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
using System.Text;
using System.Threading;
using Remotion.Utilities;

namespace Remotion.IO
{
  /// <summary>
  /// Implements <see cref="IFileSystemHelper"/>
  /// </summary>
  [Serializable]
  public class FileSystemHelper : IFileSystemHelper
  {
    private const int c_maxPathLength = 259;

    public FileSystemHelper ()
    {
    }

    public FileStream OpenFile (string path, FileMode mode, FileAccess access, FileShare share)
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

    public string GetTempFileName ()
    {
      return Path.GetTempFileName();
    }

    public string GetTempFolder ()
    {
      return Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
    }

    public DateTime GetLastWriteTime (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.GetLastWriteTime (path);
    }

    public string MakeValidFileName (string path, string proposedFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("proposedFileName", proposedFileName);

      string rootedPath = Path.GetFullPath (path);
      string cleanedUpFileName = RemoveInvalidChars (proposedFileName);
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension (cleanedUpFileName);
      string extension = Path.GetExtension (cleanedUpFileName);

      return BuildShortFileName (rootedPath, fileNameWithoutExtension, extension, "");
    }

    public string MakeUniqueAndValidFileName (string path, string proposedFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("proposedFileName", proposedFileName);

      string rootedPath = Path.GetFullPath (path);
      string cleanedUpFileName = RemoveInvalidChars (proposedFileName);
      string fileNameWithoutExtension = Path.GetFileNameWithoutExtension (cleanedUpFileName);
      string extension = Path.GetExtension (cleanedUpFileName);

      int fileIndex = 0;
      string resultingFileName = BuildShortFileName (rootedPath, fileNameWithoutExtension, extension, "");

      while (File.Exists (resultingFileName))
      {
        fileIndex++;
        var fileIndexString = string.Format (" ({0})", fileIndex);
        resultingFileName = BuildShortFileName (rootedPath, fileNameWithoutExtension, extension, fileIndexString);
      }
      return resultingFileName;
    }

    private string BuildShortFileName (string rootedPath, string fileNameWithoutExtension, string extension, string fileIndexString)
    {
      string resultingFileName = Path.Combine (rootedPath, fileNameWithoutExtension + fileIndexString + extension);

      if (resultingFileName.Length > c_maxPathLength)
      {
        int numberOfExcessCharacters = resultingFileName.Length - c_maxPathLength;

        int shortFileNameLength = fileNameWithoutExtension.Length - numberOfExcessCharacters;
        if (shortFileNameLength < 1)
        {
          throw new ArgumentException (
              string.Format ("The filename '{0}' exceeds the maximum length for file names.", Path.GetFileName (resultingFileName)));
        }

        string shortFileNameWithoutExtension = fileNameWithoutExtension.Substring (0, shortFileNameLength);
        resultingFileName = Path.Combine (rootedPath, shortFileNameWithoutExtension + fileIndexString + extension);
      }
      return resultingFileName;
    }

    private string RemoveInvalidChars (string fileName)
    {
      var builder = new StringBuilder (fileName);
      foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
        builder.Replace (invalidFileNameChar.ToString(), "");
      fileName = builder.ToString();
      return fileName;
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