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
using Remotion.Utilities;

namespace Remotion.IO
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