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

namespace Remotion.IO
{
  /// <summary>
  /// Encapsulate FileIO
  /// </summary>
  public interface IFileSystemHelper
  {
    FileStream OpenFile (string path, FileMode mode, FileAccess access, FileShare share);
    bool FileExists (string path);
    void FileDelete (string path);
    void FileMove (string sourceFile, string destinationFile);
    IDirectoryInfo DirectoryCreate (string path);
    bool DirectoryExists (string path);
    void DirectoryDelete (string path, bool recursive);
    string GetTempFileName ();
    string GetTempFolder ();
    string MakeValidFileName (string path, string proposedFileName);
    string MakeUniqueAndValidFileName (string path, string proposedFileName);
    IFileInfo[] GetFilesOfDirectory (string path);
  }
}