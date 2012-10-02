// This file is part of re-vision (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
  [Explicit]
  [TestFixture]
  public class DirectoryInfoWrapperTest
  {
    private DirectoryInfoWrapper _directoryInfoWrapper;
    private TempFile _tempFile1;
    private TempFile _tempFile2;
    private string _folder;
    private string _path;
    private long _folderSize;

    [SetUp]
    public void SetUp ()
    {
      _tempFile1 = new TempFile();
      _tempFile2 = new TempFile ();

      _folderSize = 0;
      _folderSize += _tempFile1.Length + _tempFile2.Length;
      
      _folder = Path.GetRandomFileName ();
      _path = Path.Combine (Path.GetTempPath(), _folder);
      Directory.CreateDirectory (_path);
      
      DirectoryInfo directoryInfo = new DirectoryInfo (_path);
      _directoryInfoWrapper = new DirectoryInfoWrapper (directoryInfo);
    }

    [TearDown]
    public void TearDown ()
    {
      _tempFile1.Dispose ();
      _tempFile2.Dispose ();
      Directory.Delete (_path, true);
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_directoryInfoWrapper.FullName, Is.EqualTo (_path));
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_directoryInfoWrapper.Extension, Is.EqualTo (Path.GetExtension(_path)));
    }

    [Test]
    public void Name ()
    {
      Assert.That (_directoryInfoWrapper.Name, Is.EqualTo (_folder));
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_directoryInfoWrapper.Exists, Is.True);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_directoryInfoWrapper.IsReadOnly, Is.False);
    }

    [Test]
    public void CreationTimeUtc ()
    {
      Assert.That (_directoryInfoWrapper.CreationTimeUtc, Is.EqualTo (Directory.GetCreationTime (_path)));
    }

    [Test]
    public void SetCreationTimeUtc ()
    {
      _directoryInfoWrapper.CreationTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.CreationTimeUtc, Is.EqualTo (Directory.GetCreationTime (_path)));
    }

    [Test]
    public void LastAccessTimUtce ()
    {
      _directoryInfoWrapper.LastAccessTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.LastAccessTimeUtc, Is.EqualTo (Directory.GetLastAccessTime (_path))); 
    }

    [Test]
    public void LastWriteTimeUtc ()
    {
      _directoryInfoWrapper.LastWriteTimeUtc = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.LastWriteTimeUtc, Is.EqualTo (Directory.GetLastWriteTime (_path)));
    }

    [Test]
    public void GetFiles ()
    {
      File.Copy (_tempFile1.FileName, Path.Combine (_path, Path.GetFileName (_tempFile1.FileName)), true);
      File.Copy (_tempFile2.FileName, Path.Combine (_path, Path.GetFileName (_tempFile2.FileName)), true);

      var files = _directoryInfoWrapper.GetFiles();

      Assert.That (files[0].Name, Is.EqualTo (Path.GetFileName(_tempFile1.FileName)));
      Assert.That (files[1].Name, Is.EqualTo (Path.GetFileName(_tempFile2.FileName)));
    }

    [Test]
    public void GetDirectories ()
    {
      var directories = _directoryInfoWrapper.GetDirectories();
      Assert.That (directories.Length, Is.EqualTo (0));
    }

    [Test]
    public void DirectoryMember ()
    {
      Assert.That (_directoryInfoWrapper.Directory, Is.InstanceOf (typeof(DirectoryInfoWrapper)));
      Assert.That (_directoryInfoWrapper.Directory.Name, Is.EqualTo (_folder));
    }
  }
}