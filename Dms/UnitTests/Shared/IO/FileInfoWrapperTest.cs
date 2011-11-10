// This file is part of re-vision (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;
using Remotion.Dms.Shared.IO;

namespace Remotion.Dms.UnitTests.Shared.IO
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
    public void DirectoryName ()
    {
      Assert.That (_fileInfoWrapper.Directory.FullName, Is.EqualTo (Path.GetDirectoryName(_tempFile.FileName)));
    }

    [Test]
    public void Directory ()
    {
      Assert.That (_fileInfoWrapper.Directory.Name, Is.EqualTo (new DirectoryInfo(Path.GetDirectoryName(_tempFile.FileName)).Name));
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
  }
}