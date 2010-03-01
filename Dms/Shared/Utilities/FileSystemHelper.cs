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
using System.Text;
using System.Threading;
using Remotion.Dms.Shared.Globalization;

namespace Remotion.Dms.Shared.Utilities
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

    public void CopyFile (string sourceFileName, string destFileName, bool overwrite)
    {
      ArgumentUtility.CheckNotNull ("sourceFileName", sourceFileName);
      ArgumentUtility.CheckNotNull ("destFileName", destFileName);

      File.Copy (sourceFileName, destFileName, overwrite);

      while (!File.Exists (destFileName))
        Thread.Sleep (10);
    }

    public bool FileExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path); 
      return File.Exists (path);
    }

    public bool DirectoryExists (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return Directory.Exists (path);
    }

    public string GetTempFileName ()
    {
      return Path.GetTempFileName();
    }

    public DateTime GetLastWriteTime (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      return File.GetLastWriteTime (path);
    }

    public void Delete (string path)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      File.Delete (path);

      while (File.Exists (path))
        Thread.Sleep (10);
    }

    public void Move (string sourceFile, string destinationFile)
    {
      ArgumentUtility.CheckNotNull ("sourceFile", sourceFile);
      ArgumentUtility.CheckNotNull ("destinationFile", destinationFile);

      File.Move (sourceFile, destinationFile);

      while (File.Exists (sourceFile) || !File.Exists (destinationFile))
        Thread.Sleep (10);
    }

    public string MakeUniqueAndValidFileName (string path, string proposedFileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("path", path);
      ArgumentUtility.CheckNotNullOrEmpty ("proposedFileName", proposedFileName);

      if (!path.EndsWith (@"\"))
        path = path + @"\";

      var extensionStartIndex = proposedFileName.LastIndexOf ('.');
      var extension = extensionStartIndex == -1 ? string.Empty : proposedFileName.Substring (extensionStartIndex);
      var fileName = extensionStartIndex == -1 ? proposedFileName : proposedFileName.Substring (0, extensionStartIndex);

      int remainingFileNameLength = c_maxPathLength - path.Length - extension.Length;
      if (remainingFileNameLength < 0)
        throw new ArgumentException (FileSystemHelperResource.FileNameLengthErrorMessage);

      fileName = RemoveInvalidChars (fileName);
      return AppendUniqueNumber (path, extension, fileName, remainingFileNameLength);
    }

    private string RemoveInvalidChars (string fileName)
    {
      var builder = new StringBuilder (fileName);
      foreach (var invalidFileNameChar in Path.GetInvalidFileNameChars())
        builder.Replace (invalidFileNameChar.ToString(), "");
      fileName = builder.ToString();
      return fileName;
    }

    private string AppendUniqueNumber (string path, string extension, string fileName, int remainingFileNameLength)
    {
      int fileIndex = 0;
      var resultingFileName = path + fileName.Substring (0, Math.Min (fileName.Length, remainingFileNameLength)) + extension;
      while (File.Exists (resultingFileName))
      {
        fileIndex++;
        var fileIndexString = string.Format (" ({0})", fileIndex);
        resultingFileName =
            path +
            fileName.Substring (0, Math.Min (fileName.Length, remainingFileNameLength - fileIndexString.Length)) +
            fileIndexString +
            extension;
      }
      return resultingFileName;
    }

    public IFileInfo[] GetFilesOfDirectory (string path)
    {
      List<IFileInfo> fileInfos = new List<IFileInfo> ();
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
          GetFilesInSubdirectories (Path.Combine(path, subDirectoryInfo.Name), fileInfos); //Path.Combine
      }
      return fileInfos.ToArray();
    }

    public void DeleteDirectory (string path, bool recursive)
    {
      string rootDirectory = Path.Combine (Environment.GetFolderPath (Environment.SpecialFolder.LocalApplicationData), @"Remotion\Dms");
      var parentDirectory = Directory.GetParent (path);
      if (parentDirectory != null)
      {
        if (parentDirectory.FullName == rootDirectory)
          Directory.Delete (path, recursive);
      }
    }

    public string Combine (string path1, string path2)
    {
      return Path.Combine (path1, path2);
    }

    public DirectoryInfo CreateDirectory (string path)
    {
      return Directory.CreateDirectory (path);
    }

    public string GetFileName (string path)
    {
      return Path.GetFileName (path);
    }

    public string GetDirectoryName (string path)
    {
      return Path.GetDirectoryName (path);
    }

    public string GetPathWithEnvironmentVariable (string path)
    {
      return Environment.ExpandEnvironmentVariables (path);
    }
  }
}