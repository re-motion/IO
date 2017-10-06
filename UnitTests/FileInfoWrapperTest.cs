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
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class FileInfoWrapperTest
  {
    private FileInfoWrapper _fileInfoWrapper;
    private TempFile _tempFile;

    [SetUp]
    public void SetUp ()
    {
      _tempFile = new TempFile();
      FileInfo fileInfo = new FileInfo (_tempFile.FileName);
      _fileInfoWrapper = new FileInfoWrapper (fileInfo);
    }

    [TearDown]
    public void TearDown ()
    {
      _tempFile.Dispose();
    }

    [Test]
    public void PhysicalPath ()
    {
      Assert.That (_fileInfoWrapper.PhysicalPath, Is.EqualTo (_tempFile.FileName));
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_fileInfoWrapper.FullName, Is.EqualTo (_tempFile.FileName));
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_fileInfoWrapper.Extension, Is.EqualTo (".tmp"));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_fileInfoWrapper.Name, Is.EqualTo (Path.GetFileName(_tempFile.FileName)));
    }

    [Test]
    public void Length ()
    {
      Assert.That (_fileInfoWrapper.Length, Is.EqualTo(_tempFile.Length));
    }

    [Test]
    public void Parent_WithFileInDirectory_ReturnsDirectory ()
    {
      Assert.That (_fileInfoWrapper.Parent.FullName, Is.EqualTo (new DirectoryInfo (Path.GetDirectoryName (_tempFile.FileName)).FullName));
    }

    [Test]
    public void Parent_WithRootFile_ReturnsRoot ()
    {
      var fileInfoWrapper = new FileInfoWrapper (new FileInfo ("C:\\test.txt"));
      Assert.That (fileInfoWrapper.Parent.FullName, Is.EqualTo (new DirectoryInfo ("C:\\").FullName));
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_fileInfoWrapper.Exists, Is.True);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_fileInfoWrapper.IsReadOnly, Is.EqualTo (false));
    }

    [Test]
    public void CreationTimeUtc ()
    {
      Assert.That (_fileInfoWrapper.CreationTimeUtc, Is.EqualTo (File.GetCreationTimeUtc(_tempFile.FileName)));
    }

    [Test]
    public void SetCreationTimeUtc ()
    {
      _fileInfoWrapper.CreationTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_fileInfoWrapper.CreationTimeUtc, Is.EqualTo (File.GetCreationTimeUtc (_tempFile.FileName)));
    }

    [Test]
    public void LastAccessTimeUtc ()
    {
      Assert.That (_fileInfoWrapper.LastAccessTimeUtc, Is.EqualTo (File.GetLastAccessTimeUtc (_tempFile.FileName)));
    }

    [Test]
    public void SetLastAccessTimeUtc ()
    {
      _fileInfoWrapper.LastAccessTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_fileInfoWrapper.LastAccessTimeUtc, Is.EqualTo (File.GetLastAccessTimeUtc (_tempFile.FileName)));
    }

    [Test]
    public void LastWriteTimeUtc ()
    {
      Assert.That (_fileInfoWrapper.LastWriteTimeUtc, Is.EqualTo (File.GetLastWriteTimeUtc (_tempFile.FileName)));
    }

    [Test]
    public void SetLastWriteTimeUtc ()
    {
      _fileInfoWrapper.LastWriteTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_fileInfoWrapper.LastWriteTimeUtc, Is.EqualTo (File.GetLastWriteTimeUtc (_tempFile.FileName)));
    }

    [Test]
    public void Open ()
    {
      var actualStream = _fileInfoWrapper.Open (FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var constraintStream = File.Open (_tempFile.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      Assert.That (actualStream, Is.EqualTo(constraintStream));
      actualStream.Dispose();
      constraintStream.Dispose();
    }

    [Test]
    public void Refresh ()
    {
      var accessTime1 = new DateTime (2010, 1, 1, 0, 0, 0, DateTimeKind.Utc);
      var accessTime2 = new DateTime (2005, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      File.SetLastAccessTime (_tempFile.FileName, accessTime1);
      _fileInfoWrapper.Refresh();
      Assert.That (_fileInfoWrapper.LastAccessTimeUtc, Is.EqualTo (accessTime1));

      File.SetLastAccessTime (_tempFile.FileName, accessTime2);
      Assert.That (_fileInfoWrapper.LastAccessTimeUtc, Is.EqualTo (accessTime1));
      _fileInfoWrapper.Refresh();
      Assert.That (_fileInfoWrapper.LastAccessTimeUtc, Is.EqualTo (accessTime2));
    }
  }
}