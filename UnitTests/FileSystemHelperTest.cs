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
using System.Linq;
using NUnit.Framework;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class FileSystemHelperTest
  {
    private string _tempFolder;
    private IFileSystemHelper _fileSystemHelper;

    [SetUp]
    public void SetUp ()
    {
      _tempFolder = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory (_tempFolder);

      _fileSystemHelper = new FileSystemHelper();
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists (_tempFolder))
        Directory.Delete (_tempFolder, true);
    }

    [Test]
    public void GetFilesOfDirectory_NotEmpty_ReturnsAllFilesInHierarchy ()
    {
      var expectedFilePath1 = Path.Combine (_tempFolder, "MyFile1.txt");
      using (File.Create (expectedFilePath1))
      {
        //NOP
      }

      var directoryPath1 = Path.Combine (_tempFolder, "MyDirectory1");
      Directory.CreateDirectory (directoryPath1);

      var expectedFilePath2 = Path.Combine (directoryPath1, "MyFile2.txt");
      using (File.Create (expectedFilePath2))
      {
        //NOP
      }

      var expectedFilePath3 = Path.Combine (directoryPath1, "MyFile3.txt");
      using (File.Create (expectedFilePath3))
      {
        //NOP
      }

      var directoryPath2 = Path.Combine (_tempFolder, "MyDirectory2");
      Directory.CreateDirectory (directoryPath2);

      var expectedFilePath4 = Path.Combine (directoryPath2, "MyFile4.txt");
      using (File.Create (expectedFilePath4))
      {
        //NOP
      }

      var result = _fileSystemHelper.GetFilesOfDirectory (_tempFolder);

      Assert.That (
          result.Select (f => f.FullName),
          Is.EquivalentTo (new[] { expectedFilePath1, expectedFilePath2, expectedFilePath3, expectedFilePath4 }));
    }

    [Test]
    public void GetFilesOfDirectory_Empty_ReturnsEmptyList ()
    {
      var result = _fileSystemHelper.GetFilesOfDirectory (_tempFolder);

      Assert.That (result, Is.Empty);
    }

    [Test]
    public void GetFilesOfDirectory_NonExistent_ReturnsEmptyList ()
    {
      var result = _fileSystemHelper.GetFilesOfDirectory ("C:\\" + Guid.NewGuid());

      Assert.That (result, Is.Empty);
    }
  }
}