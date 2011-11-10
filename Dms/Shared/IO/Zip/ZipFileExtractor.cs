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
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.Shared.IO.Zip
{
  /// <summary>
  /// Implements <see cref="IArchiveExtractor"/>.
  /// </summary>
  public class ZipFileExtractor : IArchiveExtractor
  {
    private readonly Stream _archiveStream;
    private ZipFile _zipFile;

    public ZipFileExtractor (Stream archiveStream)
    {
      ArgumentUtility.CheckNotNull ("archiveStream", archiveStream);
      _archiveStream = archiveStream;
    }

    public IFileInfo[] GetFiles ()
    {
      _zipFile = new ZipFile (_archiveStream);
      _zipFile.NameTransform = new WindowsNameTransform();

      var directoryTreeResolver = new DirectoryTreeResolver (DateTime.Now);
      var files = new List<IFileInfo> ();

      foreach (ZipEntry zipEntry in _zipFile)
      {
        if (zipEntry.IsFile)
        {
          var localZipEntry = zipEntry;
          var fileInfo = directoryTreeResolver.GetFileInfoWithPath (
              _zipFile.NameTransform.TransformFile (localZipEntry.Name),
              (fileName, directory) => new ExtractedZipEntryAsFileInfo (_zipFile, localZipEntry, directory));
          files.Add (fileInfo);
        }
      }

      return files.ToArray();
    }

    public void Dispose ()
    {
      if (_zipFile != null)
        _zipFile.Close ();
    }
  }
}