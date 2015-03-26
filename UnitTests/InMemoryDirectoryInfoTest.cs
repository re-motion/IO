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
using Rhino.Mocks;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class InMemoryDirectoryInfoTest
  {
    private InMemoryDirectoryInfo _inMemoryDirectoryInfo;
    private string _directoryName;
    private IDirectoryInfo _parentDirectory;
    private DateTime _lastWriteTime;
    private DateTime _lastAccessTime;
    private DateTime _creationTime;

    [SetUp]
    public void SetUp ()
    {
      _directoryName = "Parent\\Directory";
      _parentDirectory = MockRepository.GenerateStub<IDirectoryInfo>();
      _creationTime = new DateTime (2009, 10, 1);
      _lastAccessTime = new DateTime (2009, 10, 2);
      _lastWriteTime = new DateTime (2009, 10, 3);
      _inMemoryDirectoryInfo = new InMemoryDirectoryInfo (_directoryName, _parentDirectory, _creationTime, _lastAccessTime, _lastWriteTime);
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_inMemoryDirectoryInfo.FullName, Is.EqualTo (_directoryName));
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_inMemoryDirectoryInfo.Extension, Is.Empty);
    }

    [Test]
    public void Name ()
    {
      Assert.That (_inMemoryDirectoryInfo.Name, Is.EqualTo (Path.GetFileName (_directoryName)));
    }

    [Test]
    public void Directory ()
    {
      Assert.That (_inMemoryDirectoryInfo.Directory, Is.SameAs (_parentDirectory));
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_inMemoryDirectoryInfo.Exists, Is.True);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_inMemoryDirectoryInfo.IsReadOnly, Is.EqualTo (true));
    }

    [Test]
    public void CreationTime ()
    {
      Assert.That (_inMemoryDirectoryInfo.CreationTimeUtc, Is.EqualTo (_creationTime));
    }

    [Test]
    public void LastAccessTime ()
    {
      Assert.That (_inMemoryDirectoryInfo.LastAccessTimeUtc, Is.EqualTo (_lastAccessTime));
    }

    [Test]
    public void LastWriteTime ()
    {
      Assert.That (_inMemoryDirectoryInfo.LastWriteTimeUtc, Is.EqualTo (_lastWriteTime));
    }

    [Test]
    public void ToStringTest ()
    {
      Assert.That (_inMemoryDirectoryInfo.ToString (), Is.EqualTo (_directoryName));
    }

    [Test]
    public void GetFiles ()
    {
      var files = new[] { MockRepository.GenerateStub<IFileInfo>(), MockRepository.GenerateStub<IFileInfo>() };
      _inMemoryDirectoryInfo.Files.AddRange (files);

      Assert.That (_inMemoryDirectoryInfo.GetFiles(), Is.EquivalentTo (files));
    }

    [Test]
    public void GetDirectories ()
    {
      var directory = new[] { MockRepository.GenerateStub<IDirectoryInfo> (), MockRepository.GenerateStub<IDirectoryInfo> () };
      _inMemoryDirectoryInfo.Directories.AddRange (directory);

      Assert.That (_inMemoryDirectoryInfo.GetDirectories(), Is.EquivalentTo (directory));
    }
  }
}