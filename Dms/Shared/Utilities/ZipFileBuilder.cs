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

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Implements <see cref="IArchiveBuilder"/>
  /// </summary>
  public class ZipFileBuilder : IArchiveBuilder
  {
    private readonly List<string> _files = new List<string>();

    public List<string> Files
    {
      get { return _files; }
    }

    public void AddFile (string fileName)
    {
      ArgumentUtility.CheckNotNullOrEmpty ("fileName", fileName);
      _files.Add (fileName);
    }

    public void Build (string archiveFileName)
    {
      using (var zipOutputStream = new ZipOutputStream (File.Create (archiveFileName)))
      {
        StreamCopier streamCopier = new StreamCopier (); 
        foreach (var file in _files)
        {
          using (var fileStreamIn = new FileStream (file, FileMode.Open, FileAccess.Read))
          {
            ZipEntry zipEntry = new ZipEntry (Path.GetFileName (file));
            zipOutputStream.PutNextEntry (zipEntry);
            streamCopier.CopyStream (fileStreamIn, zipOutputStream, fileStreamIn.Length, ()=>false);
          }
        }
      }
    }
  }
}