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
using System.Text;
using NUnit.Framework;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class FileNameFactoryTest
  {
    private string _tempFolder;
    private IFileNameFactory _fileNameFactory;

    [SetUp]
    public void SetUp ()
    {
      _tempFolder = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory (_tempFolder);

      _fileNameFactory = new FileNameFactory();
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists (_tempFolder))
        Directory.Delete (_tempFolder, true);
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsUnique ()
    {
      Assert.That (
          _fileNameFactory.MakeUniqueAndValidFilePath (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile.txt")));
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsNotUnique_OneFileExists ()
    {
      using (File.Create (Path.Combine (_tempFolder, "MyFile.txt")))
      {
        //NOP
      }
      Assert.That (
          _fileNameFactory.MakeUniqueAndValidFilePath (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile (1).txt")));
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsNotUnique_MoreThanOneFileExist ()
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
          _fileNameFactory.MakeUniqueAndValidFilePath (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile (2).txt")));
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsUnique_FileNameIsTooLong ()
    {
      string actual = _fileNameFactory.MakeUniqueAndValidFilePath (
          _tempFolder,
          "00abcdefg_01abcdefg_02abcdefg_03abcdefg_04abcdefg_05abcdefg_06abcdefg_07abcdefg_08abcdefg_09abcdefg_10abcdefg_11abcdefg_12abcdefg_13abcdefg_14abcdefg_15abcdefg_16abcdefg_17abcdefg_18abcdefg_19abcdefg_20abcdefg_21abcdefg_22abcdefg_23abcdefg_24abcdefg_25abcdefg.txt");
      Assert.That (actual.Length, Is.LessThan (260));
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsTooLong_FileNameIsNotUnique ()
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
      string actual = _fileNameFactory.MakeUniqueAndValidFilePath (_tempFolder, Path.GetFileName (shortFileName));
      Assert.That (actual.Length, Is.LessThan (260));
    }

    [Test]
    public void MakeUniqueAndValidFilePath_FileNameIsTooLong_FileNameIsNotUniqueAfterShorting ()
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
      string actual = _fileNameFactory.MakeUniqueAndValidFilePath (_tempFolder, Path.GetFileNameWithoutExtension (shortFileName) + "MoreText.txt");
      Assert.That (actual.Length, Is.LessThan (260));
      Assert.That (actual, Does.EndWith (" (1).txt"));
    }

    [Test]
    public void MakeValidFilePath_FileNameIsTooLong_FileNameIsTruncatedAtTheEnd ()
    {
      string actual = _fileNameFactory.MakeValidFilePath (
          _tempFolder,
          "00abcdefg_01abcdefg_02abcdefg_03abcdefg_04abcdefg_05abcdefg_06abcdefg_07abcdefg_08abcdefg_09abcdefg_10abcdefg_11abcdefg_12abcdefg_13abcdefg_14abcdefg_15abcdefg_16abcdefg_17abcdefg_18abcdefg_19abcdefg_20abcdefg_21abcdefg_22abcdefg_23abcdefg_24abcdefg_25abcdefg.txt");
      Assert.That (actual.Length, Is.LessThan (260));
      Assert.That (Path.GetFileName (actual).StartsWith ("00abcdefg_"));
    }

    [Test]
    public void MakeValidFilePath_FileNameIsNotUnique_IsNotTakenIntoAccount ()
    {
      using (File.Create (Path.Combine (_tempFolder, "MyFile.txt")))
      {
        //NOP
      }
      Assert.That (
          _fileNameFactory.MakeValidFilePath (_tempFolder, "MyFile.txt"),
          Is.EqualTo (Path.Combine (_tempFolder, "MyFile.txt")));
    }

    [Test]
    public void MakeValidFilePath_FileNameContainsInvalidChars_InvalidCharsAreRemoved ()
    {
      string actual = _fileNameFactory.MakeValidFilePath (
          _tempFolder,
          @"\ab!c:d<e>f�.txt");
      Assert.That (Path.GetFileName (actual), Is.EqualTo ("ab!cdef�.txt"));
    }
  }
}