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

namespace Remotion.Dms.Shared.Utilities.Zip
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

    public void Extract (string archiveFile, string destinationPath)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("archiveFile", archiveFile);
      ArgumentUtility.CheckNotNullOrEmpty ("destinationPath", destinationPath);

      FastZip fastZip = new FastZip ();
      fastZip.ExtractZip (archiveFile, destinationPath, FastZip.Overwrite.Always, null, null, null, false);
    }

    public IFileInfo[] GetFiles ()
    {
      _zipFile = new ZipFile (_archiveStream);
      _zipFile.NameTransform = new WindowsNameTransform();

      _rootDirectories = new Dictionary<string, InMemoryDirectoryInfo> ();
      _directoryDateTime = DateTime.Now;
      var files = new List<IFileInfo> ();
      foreach (ZipEntry zipEntry in _zipFile)
      {
        if (zipEntry.IsFile)
          files.Add (AddFileToDirectoryTree (zipEntry));
      }

      return files.ToArray();
    }

    public void Dispose ()
    {
      if (_zipFile != null)
        _zipFile.Close ();
    }

    private Dictionary<string, InMemoryDirectoryInfo> _rootDirectories;
    private DateTime _directoryDateTime;

    private IFileInfo AddFileToDirectoryTree (ZipEntry zipEntry)
    {
      var nameParts = _zipFile.NameTransform.TransformFile (zipEntry.Name).Split (Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

      if (nameParts.Length == 1)
      {

        return new ExtractedZipEntryAsFileInfo (_zipFile, zipEntry, null);
      }
      else
      {
        InMemoryDirectoryInfo directoryInfo = GetOrCreateDirectoryInfo (GetElementsExceptLast (nameParts));

        var fileInfo = new ExtractedZipEntryAsFileInfo (_zipFile, zipEntry, directoryInfo);

        directoryInfo.Files.Add (fileInfo);

        return fileInfo;
      }
    }

    private InMemoryDirectoryInfo GetOrCreateDirectoryInfo (string[] nameParts)
    {
      if (nameParts.Length == 0)
      {
        throw new ArgumentEmptyException ("nameParts");
      }
      else if (nameParts.Length == 1)
      {
        var name = nameParts[0];
        if (!_rootDirectories.ContainsKey (name))
          _rootDirectories.Add (name, CreateDirectoryInfo (name, null));

        return _rootDirectories[name];
      }
      else
      {
        var parent = GetOrCreateDirectoryInfo (GetElementsExceptLast (nameParts));
        var name = string.Join (Path.DirectorySeparatorChar.ToString(), nameParts);
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

    private string[] GetElementsExceptLast (string[] nameParts)
    {
      var directoryNames = new string[nameParts.Length - 1];
      Array.Copy (nameParts, directoryNames, directoryNames.Length);
      return directoryNames;
    }
  }
}