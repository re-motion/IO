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
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using Remotion.Development.UnitTesting.IO;
using Remotion.IO.Zip;

namespace Remotion.IO.UnitTests.Zip
{
  [TestFixture]
  public class ZipFileExtractorTest
  {
    private string _file1;
    private string _file2;
    private string _sourcePath;

    [SetUp]
    public void SetUp ()
    {
      _sourcePath = Path.Combine (Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory (_sourcePath);

      _file1 = Path.Combine (_sourcePath, "File1.bin");
      _file2 = Path.Combine (_sourcePath, "File2.bin");

      File.WriteAllText (_file1, "File1");
      File.WriteAllText (_file2, "File2");
    }

    [TearDown]
    public void TearDown ()
    {
      if (Directory.Exists (_sourcePath))
        Directory.Delete (_sourcePath, true);
    }

    [Test]
    public void ExtractZipFileWithFiles ()
    {
      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        using (var zipExtractor = new ZipFileExtractor (zipArchiveStream))
        {
          var files = zipExtractor.GetFiles();

          Assert.That (files.Length, Is.EqualTo (2));

          Assert.That (files[0].PhysicalPath, Is.Null);
          Assert.That (files[0].FullName, Is.EqualTo (Path.GetFileName (_file1)));
          Assert.That (GetBytesFromFile (files[0]), Is.EqualTo (File.ReadAllBytes (_file1)));

          Assert.That (files[1].PhysicalPath, Is.Null);
          Assert.That (files[1].FullName, Is.EqualTo (Path.GetFileName (_file2)));
          Assert.That (GetBytesFromFile (files[1]), Is.EqualTo (File.ReadAllBytes (_file2)));
        }
      }
    }

    [Test]
    public void ExtractZipFileWithFolder ()
    {
      Directory.CreateDirectory (Path.Combine (_sourcePath, "SubFolderEmpty"));
      string subFolder1 = Path.Combine (_sourcePath, "SubFolder");
      Directory.CreateDirectory (subFolder1);
      File.Move (_file1, Path.Combine (subFolder1, "File1.bin"));
      File.Move (_file2, Path.Combine (subFolder1, "File2.bin"));

      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        using (var zipExtractor = new ZipFileExtractor (zipArchiveStream))
        {
          var files = zipExtractor.GetFiles();

          Assert.That (files[0].PhysicalPath, Is.Null);
          Assert.That (files[0].FullName, Is.EqualTo (Path.Combine ("SubFolder", Path.GetFileName (_file1))));
          Assert.That (files[1].PhysicalPath, Is.Null);
          Assert.That (files[1].FullName, Is.EqualTo (Path.Combine ("SubFolder", Path.GetFileName (_file2))));

          Assert.That (files[0].Parent.PhysicalPath, Is.Null);
          Assert.That (files[0].Parent, Is.Not.Null);
          Assert.That (files[0].Parent.Name, Is.EqualTo ("SubFolder"));
          Assert.That (files[0].Parent, Is.SameAs (files[1].Parent));
        }
      }
    }

    [Test]
    public void ExtractZipFile_WithFileWithUmlaut ()
    {
      File.Move (_file1, Path.Combine (_sourcePath, "Umlaut_Ä.bin"));

      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        using (var zipExtractor = new ZipFileExtractor (zipArchiveStream))
        {
          var files = zipExtractor.GetFiles ();

          Assert.That (files[1].FullName, Is.EqualTo (Path.GetFileName ( "Umlaut_Ä.bin")));
        }
      }
    }

    [Test]
    public void ExtractZipFile_WithEmtpyFile ()
    {
      File.WriteAllBytes (Path.Combine (_sourcePath, "EmtpyFile.bin"), new byte[0]);

      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        using (var zipExtractor = new ZipFileExtractor (zipArchiveStream))
        {
          var files = zipExtractor.GetFiles ();

          Assert.That (files[0].FullName, Is.EqualTo (Path.GetFileName ("EmtpyFile.bin")));
          Assert.That (GetBytesFromFile (files[0]), Is.EqualTo (new byte[0]));
        }
      }
    }

    [Test]
    public void BuildReturnsZipFilesWithFoldersAndFiles ()
    {
      //complex
      //-file1
      //-Directory1
      //--file2
      //--file3
      //-Directory2
      //--Directory3 ü
      //---file4
      //---file5
      //--file6

      var file1 = new TempFile ();
      var file2 = new TempFile ();
      var file3 = new TempFile ();
      var file4 = new TempFile ();
      var file5 = new TempFile ();
      var file6 = new TempFile ();

      var bytes = new byte[8191];
      for (int i = 0; i < 8191; i++)
        bytes[i] = (byte) i;

      file1.WriteAllBytes (bytes);
      file2.WriteAllBytes (bytes);
      file3.WriteAllBytes (bytes);
      file4.WriteAllBytes (bytes);
      file5.WriteAllBytes (bytes);
      file6.WriteAllBytes (bytes);

      var rootPath = Path.Combine (Path.GetTempPath (), Guid.NewGuid ().ToString ());

      var directory1 = Directory.CreateDirectory (Path.Combine (rootPath, "Directory1"));
      var directory2 = Directory.CreateDirectory (Path.Combine (rootPath, "Directory2"));
      var directory3 = Directory.CreateDirectory (Path.Combine (directory2.FullName, "Directory3 ü"));

      File.Move (file1.FileName, Path.Combine (rootPath, Path.GetFileName (file1.FileName)));
      File.Move (file2.FileName, Path.Combine (directory1.FullName, Path.GetFileName (file2.FileName)));
      File.Move (file3.FileName, Path.Combine (directory1.FullName, Path.GetFileName (file3.FileName)));
      File.Move (file4.FileName, Path.Combine (directory3.FullName, Path.GetFileName (file4.FileName)));
      File.Move (file5.FileName, Path.Combine (directory3.FullName, Path.GetFileName (file5.FileName)));
      File.Move (file6.FileName, Path.Combine (directory2.FullName, Path.GetFileName (file6.FileName)));

      byte[] zipArchive = CreateZipArchive (rootPath);

      Directory.Delete (rootPath, true);

      using (var archiveStream = new MemoryStream (zipArchive))
      {
        using (var zipFileExtractor = new ZipFileExtractor (archiveStream))
        {
          var files = zipFileExtractor.GetFiles();
          var actualFileNames = files.Select (file => file.FullName).ToArray();
          var expectedFileNames = new[]
                                  {
                                      Path.GetFileName (file1.FileName),
                                      Path.Combine (directory1.Name, Path.GetFileName (file2.FileName)),
                                      Path.Combine (directory1.Name, Path.GetFileName (file3.FileName)),
// ReSharper disable PossibleNullReferenceException
                                      Path.Combine (Path.Combine (directory3.Parent.Name, directory3.Name), Path.GetFileName (file4.FileName)),
// ReSharper restore PossibleNullReferenceException
                                      Path.Combine (Path.Combine (directory3.Parent.Name, directory3.Name), Path.GetFileName (file5.FileName)),
                                      Path.Combine (directory2.Name, Path.GetFileName (file6.FileName))
                                  };

          Assert.That (actualFileNames, Is.EquivalentTo (expectedFileNames));
        }
      }
    }

    [Test]
    public void GetZipArchiveAsDirectoryInfo ()
    {
      Directory.CreateDirectory (Path.Combine (_sourcePath, "SubFolderEmpty"));
      string subFolder1 = Path.Combine (_sourcePath, "SubFolder1");
      Directory.CreateDirectory (subFolder1);
      File.Move (_file1, Path.Combine (subFolder1, "File1.bin"));
      
      string subFolder2 = Path.Combine (_sourcePath, "SubFolder2");
      Directory.CreateDirectory (subFolder2);
      File.Move (_file2, Path.Combine (subFolder2, "File2.bin"));

      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        using (var zipExtractor = new ZipFileExtractor (zipArchiveStream))
        {
          var directory = zipExtractor.GetZipArchiveAsDirectoryInfo();

          Assert.That (directory.Exists);
          Assert.That (directory.GetDirectories().Select (d => d.Name), Is.EqualTo (new[] { "SubFolder1", "SubFolder2" }));

          var subDirectory1 = directory.GetDirectories().Single (d => d.Name == "SubFolder1");
          Assert.That (subDirectory1.Exists);
          Assert.That (subDirectory1.FullName, Is.EqualTo ("SubFolder1"));
          Assert.That (subDirectory1.GetDirectories(), Is.Empty);
          Assert.That (subDirectory1.GetFiles().Select (f => f.Name), Is.EqualTo (new[] { "File1.bin" }));
          Assert.That (subDirectory1.GetFiles().Select (f => f.FullName), Is.EqualTo (new[] { "SubFolder1\\File1.bin" }));
        
          var subDirectory2 = directory.GetDirectories().Single (d => d.Name == "SubFolder2");
          Assert.That (subDirectory2.Exists);
          Assert.That (subDirectory2.FullName, Is.EqualTo ("SubFolder2"));
          Assert.That (subDirectory2.GetDirectories(), Is.Empty);
          Assert.That (subDirectory2.GetFiles().Select (f => f.Name), Is.EqualTo (new[] { "File2.bin" }));
          Assert.That (subDirectory2.GetFiles().Select (f => f.FullName), Is.EqualTo (new[] { "SubFolder2\\File2.bin" }));
        }
      }
    }

    private byte[] GetBytesFromFile (IFileInfo fileInfo)
    {
      using (var stream = fileInfo.Open (FileMode.Open, FileAccess.Read, FileShare.None))
      {
        using (BinaryReader binaryReader = new BinaryReader (stream))
        {
          return binaryReader.ReadBytes ((int) fileInfo.Length);
        }
      }
    }

    private byte[] CreateZipArchive (string sourceDirectory)
    {
      using (var stream = new MemoryStream())
      {
        var fastZip = new FastZip();
        fastZip.CreateEmptyDirectories = true;
        fastZip.CreateZip (stream, sourceDirectory, true, null, null);
        return stream.ToArray();
      }
    }
  }
}