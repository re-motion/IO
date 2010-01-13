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
using ICSharpCode.SharpZipLib.Zip;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IArchiveBuilder"/>
  /// </summary>
  public class ZipFileBuilder : IArchiveBuilder
  {
    private readonly List<IFileSystemEntry> _files = new List<IFileSystemEntry> ();
    
    public ZipFileBuilder ()
    {
    }

    public void AddDirectory (IDirectoryInfo directoryInfo)
    {
      ArgumentUtility.CheckNotNull ("directoryInfo", directoryInfo);
      _files.Add (directoryInfo);
    }

    public void AddFile (IFileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _files.Add (fileInfo);
    }

    public Stream Build (string archiveFileName, EventHandler<StreamCopyProgressEventArgs> progressHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFileName", archiveFileName);
      ArgumentUtility.CheckNotNull ("progressHandler", progressHandler);

      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        StreamCopier streamCopier = new StreamCopier();
        foreach (var fileInfo in _files)
        {
          if ((fileInfo.GetType ().Equals (typeof (FileInfoWrapper))))
            AddFilesToZipFile (progressHandler, (FileInfoWrapper) fileInfo, ((FileInfoWrapper) fileInfo).Name, zipOutputStream, streamCopier);
          else if ((fileInfo.GetType ().Equals (typeof (DirectoryInfoWrapper))))
            AddDirectoryToZipFile (progressHandler, (DirectoryInfoWrapper) fileInfo, zipOutputStream, streamCopier, string.Empty, new List<string>());
        }
      }
      _files.Clear();
      return File.Open (archiveFileName, FileMode.Open, FileAccess.Read, FileShare.None);
    }

    private void AddFilesToZipFile (EventHandler<StreamCopyProgressEventArgs> progressHandler, FileInfoWrapper fileInfo, string path, ZipOutputStream zipOutputStream, StreamCopier streamCopier)
    {
      using (var fileStream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        ZipEntry zipEntry = new ZipEntry (path); 
        zipOutputStream.PutNextEntry (zipEntry);
        streamCopier.TransferProgress += progressHandler;
        streamCopier.CopyStream (fileStream, zipOutputStream, fileStream.Length);
      }
    }

    private void AddDirectoryToZipFile (EventHandler<StreamCopyProgressEventArgs> progressHandler, DirectoryInfoWrapper directoryInfo, ZipOutputStream zipOutputStream, StreamCopier streamCopier, string parentDirectory, List<string> visitedFiles)
    {
      parentDirectory += directoryInfo.Name + "\\";
      if (directoryInfo.GetDirectories ().Length > 0)
      {
        foreach (var directory in directoryInfo.GetDirectories ())
        {
          AddFilesInDirectoriesToZipFile(parentDirectory, directoryInfo, visitedFiles, progressHandler, zipOutputStream, streamCopier);
          AddDirectoryToZipFile (progressHandler, (DirectoryInfoWrapper) directory, zipOutputStream, streamCopier, parentDirectory, visitedFiles);
        }
      }
      else
      {
         AddFilesInDirectoriesToZipFile (parentDirectory, directoryInfo, visitedFiles, progressHandler, zipOutputStream, streamCopier);
      }
    }

    private void AddFilesInDirectoriesToZipFile (string parentDirectory, DirectoryInfoWrapper directoryInfo, List<string> visitedFiles, EventHandler<StreamCopyProgressEventArgs> progressHandler, ZipOutputStream zipOutputStream, StreamCopier streamCopier)
    {
      foreach (var file in directoryInfo.GetFiles ())
      {
        if (!visitedFiles.Contains (file.FullName))
        {
          visitedFiles.Add (file.FullName);
          if ((file.GetType().Equals (typeof (FileInfoWrapper))))
            AddFilesToZipFile (progressHandler, (FileInfoWrapper) file, Path.Combine (parentDirectory, file.Name), zipOutputStream, streamCopier);
        }
      }
    }

  }
}