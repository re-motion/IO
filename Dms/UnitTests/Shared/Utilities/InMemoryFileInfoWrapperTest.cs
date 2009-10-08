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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.IO;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class InMemoryFileInfoWrapperTest
  {
    private TempFile _tempFile;
    private InMemoryFileInfoWrapper _inMemoryFileInfoWrapper;
    private DateTime _lastWriteTime;
    private DateTime _lastAccessTime;
    private DateTime _creationTime;

    [SetUp]
    public void SetUp ()
    {
      _tempFile = new TempFile();
      var stream = new MemoryStream();
      _creationTime = new DateTime (2009, 10, 10);
      _lastAccessTime = new DateTime (2009, 10, 10);
      _lastWriteTime = new DateTime (2009, 10, 10);
      _inMemoryFileInfoWrapper = new InMemoryFileInfoWrapper (_tempFile.FileName, stream, _creationTime, _lastAccessTime, _lastWriteTime);
    }

    [TearDown]
    public void TearDown ()
    {
      _tempFile.Dispose();
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_inMemoryFileInfoWrapper.FullName, Is.EqualTo (_tempFile.FileName));
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_inMemoryFileInfoWrapper.Extension, Is.EqualTo (Path.GetExtension (_tempFile.FileName)));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_inMemoryFileInfoWrapper.Name, Is.EqualTo (Path.GetFileName (_tempFile.FileName)));
    }

    [Test]
    public void Length ()
    {
      Assert.That (_inMemoryFileInfoWrapper.Length, Is.EqualTo (_tempFile.Lenght));
    }

    [Test]
    public void DirectoryName ()
    {
      Assert.That (_inMemoryFileInfoWrapper.DirectoryName, Is.Empty);
    }

    [Test]
    [ExpectedException (typeof (NotImplementedException))]
    public void Directory ()
    {
      var directoryName = _inMemoryFileInfoWrapper.Directory.Name;
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_inMemoryFileInfoWrapper.Exists, Is.True);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_inMemoryFileInfoWrapper.IsReadOnly, Is.EqualTo (true));
    }

    [Test]
    public void CreationTime ()
    {
      Assert.That (_inMemoryFileInfoWrapper.CreationTime, Is.EqualTo (_creationTime));
    }

    [Test]
    public void LastAccessTime ()
    {
      Assert.That (_inMemoryFileInfoWrapper.LastAccessTime, Is.EqualTo (_lastAccessTime));
    }

    [Test]
    public void LastWriteTime ()
    {
      Assert.That (_inMemoryFileInfoWrapper.LastWriteTime, Is.EqualTo (_lastWriteTime));
    }

    [Test]
    public void Open ()
    {
      var actualStream = _inMemoryFileInfoWrapper.Open (FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      var constraintStream = File.Open (_tempFile.FileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
      Assert.That (actualStream, Is.EqualTo (constraintStream));
      actualStream.Dispose ();
      constraintStream.Dispose ();
    }
  }
}