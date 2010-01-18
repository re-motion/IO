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
  /// <summary>
  /// Wraps a <see cref="Stream"/> and deletes the underlying file on dispose
  /// </summary>
  public class TemporaryFileStream : ReadOnlyStream
  {
    private readonly string _filePath;
    private readonly IFileSystemHelper _fileSystemHelper;

    public TemporaryFileStream (Stream stream, string filePath, IFileSystemHelper fileSystemHelper)
        : base(stream)
    {
      ArgumentUtility.CheckNotNull ("stream", stream);
      ArgumentUtility.CheckNotNull ("filePath", filePath);
      ArgumentUtility.CheckNotNull ("fileSystemHelper", fileSystemHelper);

      _filePath = filePath;
      _fileSystemHelper = fileSystemHelper;
    }

    public override void Close ()
    {
      base.Close ();
      _fileSystemHelper.Delete (_filePath);
    }
  }
}