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
using System.Security.AccessControl;

namespace Remotion.Dms.Shared.Utilities
{
  public class FileInfoWrapper : IFileInfo
  {
    private readonly FileInfo _wrappedInstance;

    public FileInfoWrapper (FileInfo fileInfo)
    {
      ArgumentUtility.CheckNotNull ("fileInfo", fileInfo);
      _wrappedInstance = fileInfo;
    }

    public string FullName
    {
      get { return _wrappedInstance.FullName; }
    }

    public string Extension
    {
      get { return _wrappedInstance.Extension; }
    }

    public DateTime CreationTime
    {
      get { return _wrappedInstance.CreationTime; }
      set { _wrappedInstance.CreationTime = value; }
    }

    public DateTime CreationTimeUtc
    {
      get { return _wrappedInstance.CreationTimeUtc; }
      set { _wrappedInstance.CreationTimeUtc = value; }
    }

    public DateTime LastAccessTime
    {
      get { return _wrappedInstance.LastAccessTime; }
      set { _wrappedInstance.LastAccessTime = value; }
    }

    public DateTime LastAccessTimeUtc
    {
      get { return _wrappedInstance.LastAccessTimeUtc; }
      set { _wrappedInstance.LastAccessTimeUtc = value; }
    }

    public DateTime LastWriteTime
    {
      get { return _wrappedInstance.LastWriteTime; }
      set { _wrappedInstance.LastWriteTime = value; }
    }

    public DateTime LastWriteTimeUtc
    {
      get { return _wrappedInstance.LastWriteTimeUtc; }
      set { _wrappedInstance.LastWriteTimeUtc = value; }
    }

    public FileAttributes Attributes
    {
      get { return _wrappedInstance.Attributes; }
      set { _wrappedInstance.Attributes = value; }
    }

    public FileSecurity GetAccessControl ()
    {
      return _wrappedInstance.GetAccessControl();
    }

    public FileSecurity GetAccessControl (AccessControlSections includeSections)
    {
      return _wrappedInstance.GetAccessControl (includeSections);
    }

    public void SetAccessControl (FileSecurity fileSecurity)
    {
      _wrappedInstance.SetAccessControl (fileSecurity);
    }

    public StreamReader OpenText ()
    {
      return _wrappedInstance.OpenText();
    }

    public StreamWriter CreateText ()
    {
      return _wrappedInstance.CreateText();
    }

    public StreamWriter AppendText ()
    {
      return _wrappedInstance.AppendText();
    }

    public FileInfo CopyTo (string destFileName)
    {
      return _wrappedInstance.CopyTo (destFileName);
    }

    public FileInfo CopyTo (string destFileName, bool overwrite)
    {
      return _wrappedInstance.CopyTo (destFileName, overwrite);
    }

    public FileStream Create ()
    {
      return _wrappedInstance.Create();
    }

    public void Delete ()
    {
      _wrappedInstance.Delete();
    }

    public void Decrypt ()
    {
      _wrappedInstance.Decrypt();
    }

    public void Encrypt ()
    {
      _wrappedInstance.Encrypt();
    }

    public Stream Open (FileMode mode)
    {
      return _wrappedInstance.Open (mode);
    }

    public Stream Open (FileMode mode, FileAccess access)
    {
      return _wrappedInstance.Open (mode, access);
    }

    public Stream Open (FileMode mode, FileAccess access, FileShare share)
    {
      return _wrappedInstance.Open (mode, access, share);
    }

    public Stream OpenRead ()
    {
      return _wrappedInstance.OpenRead();
    }

    public Stream OpenWrite ()
    {
      return _wrappedInstance.OpenWrite();
    }

    public void MoveTo (string destFileName)
    {
      _wrappedInstance.MoveTo (destFileName);
    }

    public FileInfo Replace (string destinationFileName, string destinationBackupFileName)
    {
      return _wrappedInstance.Replace (destinationFileName, destinationBackupFileName);
    }

    public FileInfo Replace (string destinationFileName, string destinationBackupFileName, bool ignoreMetadataErrors)
    {
      return _wrappedInstance.Replace (destinationFileName, destinationBackupFileName, ignoreMetadataErrors);
    }

    public string Name
    {
      get { return _wrappedInstance.Name; }
    }

    public long Length
    {
      get { return _wrappedInstance.Length; }
    }

    public string DirectoryName
    {
      get { return _wrappedInstance.DirectoryName; }
    }

    public DirectoryInfo Directory
    {
      get { return _wrappedInstance.Directory; }
    }

    public bool IsReadOnly
    {
      get { return _wrappedInstance.IsReadOnly; }
      set { _wrappedInstance.IsReadOnly = value; }
    }

    public bool Exists
    {
      get { return _wrappedInstance.Exists; }
    }

    public void Refresh ()
    {
      _wrappedInstance.Refresh();
    }
  }
}