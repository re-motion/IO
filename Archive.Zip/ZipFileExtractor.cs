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
using ICSharpCode.SharpZipLib.Zip;
using Remotion.Utilities;

namespace Remotion.IO.Archive.Zip
{
  /// <summary>
  /// Implements <see cref="IArchiveExtractor"/>.
  /// </summary>
  public class ZipFileExtractor : IArchiveExtractor
  {
    private readonly Stream _archiveStream;
    private readonly ZipFile _zipFile;
    private readonly IFileInfo[] _files;
    private readonly InMemoryDirectoryInfo _root;

    public ZipFileExtractor (Stream archiveStream)
    {
      ArgumentUtility.CheckNotNull ("archiveStream", archiveStream);
      _archiveStream = archiveStream;

      _zipFile = new ZipFile (_archiveStream);
      _zipFile.NameTransform = new WindowsNameTransform();
      var directoryTreeResolver = new DirectoryTreeResolver (DateTime.Now);
      _files = GetFilesInternal(directoryTreeResolver);
      _root = new InMemoryDirectoryInfo ("\\\\", null, DateTime.UtcNow, DateTime.UtcNow, DateTime.UtcNow);
      _root.Directories.AddRange (directoryTreeResolver.GetRootDirectories());
    }

    public IFileInfo[] GetFiles ()
    {
      return _files;
    }

    public IDirectoryInfo GetZipArchiveAsDirectoryInfo ()
    {
      return _root;
    }

    public void Dispose ()
    {
      if (_zipFile != null)
        _zipFile.Close();
    }

    private IFileInfo[] GetFilesInternal (DirectoryTreeResolver treeResolver)
    {
      var files = new List<IFileInfo>();

      foreach (ZipEntry zipEntry in _zipFile)
      {
        if (zipEntry.IsFile)
        {
          var localZipEntry = zipEntry;
          var fileInfo = treeResolver.GetFileInfoWithPath (
              _zipFile.NameTransform.TransformFile (localZipEntry.Name),
              (fileName, directory) => new ExtractedZipEntryAsFileInfo (_zipFile, localZipEntry, directory));
          files.Add (fileInfo);
        }
      }

      return files.ToArray();
    }
  }
}