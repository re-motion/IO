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
using System.IO;

namespace Remotion.Dms.Shared.Utilities
{
  public class InMemoryFileInfo : IFileInfo
  {
    private readonly string _fileName;
    private readonly Stream _stream;
    private readonly DateTime _creationTime;
    private readonly DateTime _lastAccessTime;
    private readonly DateTime _lastWriteTime;

    public InMemoryFileInfo (string fileName, Stream stream, DateTime creationTime, DateTime lastAccessTime, DateTime lastWriteTime)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      ArgumentUtility.CheckNotNull ("stream", stream);
      
      _fileName = fileName;
      _stream = stream;
      _creationTime = creationTime;
      _lastAccessTime = lastAccessTime;
      _lastWriteTime = lastWriteTime;
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
      get { return new NullDirectoryInfo(); }
    }

    public bool IsReadOnly
    {
      get { return true; }
    }

    public bool Exists
    {
      get { return true; }
    }

    public DateTime CreationTime 
    {
      get { return _creationTime; }
    }

    public DateTime LastAccessTime 
    {
      get { return _lastAccessTime; }
    }

    public DateTime LastWriteTime
    {
      get { return _lastWriteTime; }
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _stream;
    }
  }
}