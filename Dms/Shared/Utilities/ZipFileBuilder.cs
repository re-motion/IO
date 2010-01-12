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
            AddFilesToZipFile (progressHandler, (FileInfoWrapper) fileInfo, zipOutputStream, streamCopier);
        }
      }
      _files.Clear();
      return File.Open (archiveFileName, FileMode.Open, FileAccess.Read, FileShare.None);
    }

    private void AddFilesToZipFile (EventHandler<StreamCopyProgressEventArgs> progressHandler, FileInfoWrapper fileInfo, ZipOutputStream zipOutputStream, StreamCopier streamCopier)
    {
      using (var fileStream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.Read))
      {
        ZipEntry zipEntry = new ZipEntry (fileInfo.Name);
        zipOutputStream.PutNextEntry (zipEntry);
        streamCopier.TransferProgress += progressHandler;
        streamCopier.CopyStream (fileStream, zipOutputStream, fileStream.Length);
      }
    }

    //private void AddDirectoryToZipFile (EventHandler<StreamCopyProgressEventArgs> progressHandler, DirectoryInfoWrapper directoryInfo, ZipOutputStream zipOutputStream, StreamCopier streamCopier)
    //{
    //  //FastZip fastZip = new FastZip ();
    //  //fastZip.CreateZip (zipFileName, directoryInfo.FullName, true, string.Empty, string.Empty);
      
    //  //create directory in zip file
    //  foreach (var file in directoryInfo.GetFiles())
    //  {
    //    AddFilesToZipFile (progressHandler, file, zipOutputStream, streamCopier);
    //  }

      

    //}

  }
}