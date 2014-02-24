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
using System.IO;
using Remotion.Utilities;

namespace Remotion.Dms.Shared.IO
{
  /// <summary>
  /// In-memory implementation of the <see cref="IFileInfo"/> interface.
  /// </summary>
  public class InMemoryFileInfo : IFileInfo
  {
    private readonly string _fileName;
    private readonly Stream _stream;
    private readonly DateTime _creationTimeUtc;
    private readonly DateTime _lastAccessTimeUtc;
    private readonly DateTime _lastWriteTimeUtc;
    private readonly IDirectoryInfo _directory;

    public InMemoryFileInfo (string fileName, Stream stream, IDirectoryInfo directory, DateTime creationTimeUtc, DateTime lastAccessTimeUtc, DateTime lastWriteTimeUtc)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      ArgumentUtility.CheckNotNull ("stream", stream);

      _fileName = fileName;
      _stream = stream;
      _creationTimeUtc = creationTimeUtc;
      _lastAccessTimeUtc = lastAccessTimeUtc;
      _lastWriteTimeUtc = lastWriteTimeUtc;
      _directory = directory;
    }

    public string FullName
    {
      get { return _fileName; }
    }

    public string Extension
    {
      get { return Path.GetExtension (_fileName); }
    }

    public string Name
    {
      get { return Path.GetFileName (_fileName); }
    }

    public long Length
    {
      get { return _stream.Length; }
    }

    public IDirectoryInfo Directory
    {
      get { return _directory; }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool Exists
    {
      get { return true; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _creationTimeUtc; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _lastAccessTimeUtc; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _lastWriteTimeUtc; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _stream;
    }

    public override string ToString ()
    {
      return _fileName;
    }
  }
}