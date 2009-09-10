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
  /// Implements <see cref="IArchiveExtractor"/>.
  /// </summary>
  public class ZipFileExtractor : IArchiveExtractor
  {
    public List<string> Extract (string archiveFile, string dstPath)
    {
      var files = new List<string> ();
      using (FileStream fileStreamIn = new FileStream (archiveFile, FileMode.Open, FileAccess.Read))
      {
        using (ZipInputStream zipInStream = new ZipInputStream (fileStreamIn))
        {
          ZipEntry entry;
          while ((entry = zipInStream.GetNextEntry ()) != null)
          {
            var filePath = dstPath + @"\" + entry.Name;
            using (FileStream fileStreamOut = new FileStream (filePath, FileMode.Create, FileAccess.Write))
            {
              files.Add (filePath);
              int size;
              byte[] buffer = new byte[4096];
              do
              {
                size = zipInStream.Read (buffer, 0, buffer.Length);
                fileStreamOut.Write (buffer, 0, size);
              } while (size > 0);
            }
          }
        }
      }
      return files;
    }
  }
}