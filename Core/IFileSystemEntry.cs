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
using JetBrains.Annotations;

namespace Remotion.IO
{
  /// <summary>
  /// The <see cref="IFileSystemEntry"/> interface declares members for file system interactions.
  /// </summary>
  public interface IFileSystemEntry
  {
    [CanBeNull]
    string PhysicalPath { get; }
    string FullName { get; }
    string Name { get; }
    string Extension { get; }
    bool Exists { get; }
    bool IsReadOnly { get; }
    DateTime CreationTimeUtc { get; }
    DateTime LastAccessTimeUtc { get; }
    DateTime LastWriteTimeUtc { get; }
    [CanBeNull]
    IDirectoryInfo Parent { get; }
    void Refresh ();
  }
}