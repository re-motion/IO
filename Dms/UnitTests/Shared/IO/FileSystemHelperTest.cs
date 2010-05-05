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
using System.Text;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Dms.Shared.IO;

namespace Remotion.Dms.UnitTests.Shared.IO
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
    public void MakeUniqueAndValidFileName_FileNameIsUnique ()
    {
      Assert.That (
          _fileSystemHelper.MakeUniqueAndValidFileName (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile.txt")));
    }

    [Test]
    public void MakeUniqueAndValidFileName_FileNameIsNotUnique_OneFileExists ()
    {
      using (File.Create (Path.Combine (_tempFolder, "MyFile.txt")))
      {
        //NOP
      }
      Assert.That (
          _fileSystemHelper.MakeUniqueAndValidFileName (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile (1).txt")));
    }

    [Test]
    public void MakeUniqueAndValidFileName_FileNameIsNotUnique_MoreThanOneFileExist ()
    {
      using (File.Create (Path.Combine (_tempFolder, "MyFile.txt")))
      {
        //NOP
      }
      using (File.Create (Path.Combine (_tempFolder, "MyFile (1).txt")))
      {
        //NOP
      }
      Assert.That (
          _fileSystemHelper.MakeUniqueAndValidFileName (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile (2).txt")));
    }

    [Test]
    public void MakeUniqueAndValidFileName_FileNameIsUnique_FileNameIsTooLong ()
    {
      string actual = _fileSystemHelper.MakeUniqueAndValidFileName (
          _tempFolder,
          "00abcdefg_01abcdefg_02abcdefg_03abcdefg_04abcdefg_05abcdefg_06abcdefg_07abcdefg_08abcdefg_09abcdefg_10abcdefg_11abcdefg_12abcdefg_13abcdefg_14abcdefg_15abcdefg_16abcdefg_17abcdefg_18abcdefg_19abcdefg_20abcdefg_21abcdefg_22abcdefg_23abcdefg_24abcdefg_25abcdefg.txt");
      Assert.That (actual.Length, Is.LessThan (260));
    }

    [Test]
    public void MakeUniqueAndValidFileName_FileNameIsTooLong_FileNameIsNotUnique ()
    {
      StringBuilder shortFileNameBuilder = new StringBuilder (260);
      shortFileNameBuilder.Append (Path.Combine (_tempFolder, "Start"));
      for (int i = shortFileNameBuilder.Length; i < 259 - 4; i++)
        shortFileNameBuilder.Append (i % 10);
      shortFileNameBuilder.Append (".txt");

      string shortFileName = shortFileNameBuilder.ToString();
      using (File.Create (shortFileName))
      {
        //NOP
      }
      string actual = _fileSystemHelper.MakeUniqueAndValidFileName (_tempFolder, Path.GetFileName (shortFileName));
      Assert.That (actual.Length, Is.LessThan (260));
    }

    [Test]
    public void MakeUniqueAndValidFileName_FileNameIsTooLong_FileNameIsNotUniqueAfterShorting ()
    {
      StringBuilder shortFileNameBuilder = new StringBuilder (260);
      shortFileNameBuilder.Append (Path.Combine (_tempFolder, "Start"));
      for (int i = shortFileNameBuilder.Length; i < 259 - 4; i++)
        shortFileNameBuilder.Append (i % 10);
      shortFileNameBuilder.Append (".txt");

      string shortFileName = shortFileNameBuilder.ToString();
      using (File.Create (shortFileName))
      {
        //NOP
      }
      string actual = _fileSystemHelper.MakeUniqueAndValidFileName (_tempFolder, Path.GetFileNameWithoutExtension (shortFileName) + "MoreText.txt");
      Assert.That (actual.Length, Is.LessThan (260));
      Assert.That (actual, NUnit.Framework.SyntaxHelpers.Text.EndsWith (" (1).txt"));
    }
  }
}