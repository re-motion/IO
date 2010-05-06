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
using Remotion.Dms.Shared.IO;
using Rhino.Mocks;

namespace Remotion.Dms.UnitTests.Shared.IO
{
  [TestFixture]
  public class InMemoryFileInfoTest
  {
    private InMemoryFileInfo _inMemoryFileInfo;
    private string _fileName;
    private MemoryStream _stream;
    private IDirectoryInfo _directory;
    private DateTime _lastWriteTime;
    private DateTime _lastAccessTime;
    private DateTime _creationTime;

    [SetUp]
    public void SetUp ()
    {
      _fileName=  "Directory\\File.ext";
      _stream = new MemoryStream(new byte[10]);
      _directory = MockRepository.GenerateStub<IDirectoryInfo>();
      _creationTime = new DateTime (2009, 10, 1);
      _lastAccessTime = new DateTime (2009, 10, 2);
      _lastWriteTime = new DateTime (2009, 10, 3);
      _inMemoryFileInfo = new InMemoryFileInfo (_fileName, _stream, null, _creationTime, _lastAccessTime, _lastWriteTime);
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_inMemoryFileInfo.FullName, Is.EqualTo (_fileName));
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_inMemoryFileInfo.Extension, Is.EqualTo (Path.GetExtension (_fileName)));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_inMemoryFileInfo.Name, Is.EqualTo (Path.GetFileName (_fileName)));
    }

    [Test]
    public void Length ()
    {
      Assert.That (_inMemoryFileInfo.Length, Is.EqualTo (_stream.Length));
    }

    [Test]
    public void Directory ()
    {
      Assert.That (_inMemoryFileInfo.Directory, Is.Null);
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_inMemoryFileInfo.Exists, Is.True);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_inMemoryFileInfo.IsReadOnly, Is.EqualTo (true));
    }

    [Test]
    public void CreationTime ()
    {
      Assert.That (_inMemoryFileInfo.CreationTimeUtc, Is.EqualTo (_creationTime));
    }

    [Test]
    public void LastAccessTime ()
    {
      Assert.That (_inMemoryFileInfo.LastAccessTimeUtc, Is.EqualTo (_lastAccessTime));
    }

    [Test]
    public void LastWriteTime ()
    {
      Assert.That (_inMemoryFileInfo.LastWriteTimeUtc, Is.EqualTo (_lastWriteTime));
    }

    [Test]
    public void Open ()
    {
      Assert.That (_inMemoryFileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.ReadWrite), Is.SameAs (_stream));
    }
  }
}