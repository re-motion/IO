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
    public void Length ()
    {
      Assert.That (_directoryInfoWrapper.Length, Is.EqualTo (_folderSize));
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
    public void CreationTime ()
    {
      Assert.That (_directoryInfoWrapper.CreationTime, Is.EqualTo (Directory.GetCreationTime (_path)));
    }

    [Test]
    public void SetCreationTime ()
    {
      _directoryInfoWrapper.CreationTime = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.CreationTime, Is.EqualTo (Directory.GetCreationTime (_path)));
    }

    [Test]
    public void LastAccessTime ()
    {
      _directoryInfoWrapper.LastAccessTime = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.LastAccessTime, Is.EqualTo (Directory.GetLastAccessTime (_path))); 
    }

    [Test]
    public void LastWriteTime ()
    {
      _directoryInfoWrapper.LastWriteTime = new DateTime (2009, 10, 10);
      Assert.That (_directoryInfoWrapper.LastWriteTime, Is.EqualTo (Directory.GetLastWriteTime (_path)));
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
      Assert.That (_directoryInfoWrapper.Directory, Is.InstanceOfType (typeof(DirectoryInfoWrapper)));
      Assert.That (_directoryInfoWrapper.Directory.Name, Is.EqualTo (_folder));
    }
  }
}