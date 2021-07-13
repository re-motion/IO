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
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using JetBrains.Annotations;
using Remotion.Utilities;

namespace Remotion.IO.Archive.Zip
{
  /// <summary>
  /// Wrapper type to represent a <see cref="ZipEntry"/> as an <see cref="IFileInfo"/>.
  /// </summary>
  public class ExtractedZipEntryAsFileInfo : IFileInfo
  {
    [NotNull]
    private readonly ZipFile _zipFile;

    [NotNull]
    private readonly ZipEntry _zipEntry;

    [CanBeNull]
    private readonly IDirectoryInfo _directory;

    [CLSCompliant (false)]
    public ExtractedZipEntryAsFileInfo (
        [NotNull] ZipFile zipFile,
        [NotNull] ZipEntry zipEntry,
        [CanBeNull] IDirectoryInfo directory)
    {
      ArgumentUtility.CheckNotNull ("zipFile", zipFile);
      ArgumentUtility.CheckNotNull ("zipEntry", zipEntry);

      _zipFile = zipFile;
      _zipEntry = zipEntry;
      _directory = directory;
    }

    public string PhysicalPath
    {
      get { return null; }
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

    public IDirectoryInfo Parent
    {
      get { return _directory; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _zipFile.GetInputStream (_zipEntry);
    }

    public void Refresh ()
    {
      //
    }
  }
}