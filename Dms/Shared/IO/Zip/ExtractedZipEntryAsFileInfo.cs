// This file is part of re-vision (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.Shared.IO.Zip
{
  /// <summary>
  /// Wrapper type to represent a <see cref="ZipEntry"/> as an <see cref="IFileInfo"/>.
  /// </summary>
  public class ExtractedZipEntryAsFileInfo : IFileInfo
  {
    private readonly ZipFile _zipFile;
    private readonly ZipEntry _zipEntry;
    private readonly IDirectoryInfo _directory;

    public ExtractedZipEntryAsFileInfo (ZipFile zipFile, ZipEntry zipEntry, IDirectoryInfo directory)
    {
      ArgumentUtility.CheckNotNull ("zipFile", zipFile);
      ArgumentUtility.CheckNotNull ("zipEntry", zipEntry);

      _zipFile = zipFile;
      _zipEntry = zipEntry;
      _directory = directory;
    }

    public long Length
    {
      get { return _zipEntry.Size; }
    }

    public string FullName
    {
      get { return _zipFile.NameTransform.TransformFile (_zipEntry.Name); }
    }

    public string Name
    {
      get { return Path.GetFileName (FullName); }
    }

    public string Extension
    {
      get { return Path.GetExtension (FullName); }
    }

    public bool Exists
    {
      get { return true; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _zipEntry.DateTime; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _zipEntry.DateTime; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _zipEntry.DateTime; }
    }

    public IDirectoryInfo Directory
    {
      get { return _directory; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _zipFile.GetInputStream (_zipEntry);
    }
  }
}