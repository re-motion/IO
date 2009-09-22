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
    private readonly List<IFileInfo> _files = new List<IFileInfo>();

    public ZipFileBuilder ()
    {
    }

    public void AddFile (IFileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _files.Add (fileInfo);
    }

    public void Build (string archiveFileName, EventHandler<StreamCopyProgressEventArgs> progressHandler)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFileName", archiveFileName);
      ArgumentUtility.CheckNotNull ("progressHandler", progressHandler);

      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        StreamCopier streamCopier = new StreamCopier();
        foreach (var fileInfo in _files)
        {
          using (var fileStream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.Read))
          {
            ZipEntry zipEntry = new ZipEntry (fileInfo.Name);
            zipOutputStream.PutNextEntry (zipEntry);
            streamCopier.TransferProgress += progressHandler;
            streamCopier.CopyStream (
                fileStream,
                zipOutputStream,
                fileStream.Length,
                () => false);
          }
        }
      }
      _files.Clear();
    }
  }
}