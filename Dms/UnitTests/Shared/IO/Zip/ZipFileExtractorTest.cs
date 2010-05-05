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
using System.Collections.Generic;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Dms.DesktopConnector.Utilities;
using Remotion.Dms.Shared.IO;
using Remotion.Dms.Shared.IO.Zip;

namespace Remotion.Dms.UnitTests.Shared.IO.Zip
{
  [TestFixture]
  public class ZipFileExtractorTest
  {
    private FileSystemHelperExtended _helperExtended;
    private string _file1;
    private string _file2;
    private string _destinationPath;
    private string _sourcePath;

    [SetUp]
    public void SetUp ()
    {
      _helperExtended = new FileSystemHelperExtended();

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
      if (Directory.Exists (_destinationPath))
        Directory.Delete (_destinationPath);

      if (Directory.Exists (_sourcePath))
        Directory.Delete (_sourcePath, true);
    }

    [Test]
    public void ExtractZipFile ()
    {
      var zipBuilder = _helperExtended.CreateArchiveBuilder();
      zipBuilder.Progress += ((sender, e) => { });
      zipBuilder.AddFile (new FileInfoWrapper (new FileInfo (_file1)));
      zipBuilder.AddFile (new FileInfoWrapper (new FileInfo (_file2)));

      var zipFileName = _helperExtended.MakeUniqueAndValidFileName (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid() + ".zip");

      using (zipBuilder.Build (zipFileName))
      {
      }

      var zipUnpacker = new ZipFileExtractor (new MemoryStream());
      _destinationPath = Path.Combine (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid().ToString());
      zipUnpacker.Extract (zipFileName, _destinationPath);

      List<string> files = new List<string>();
      foreach (var file in Directory.GetFiles (_destinationPath))
        files.Add (file);

      Assert.That (Path.GetFileName (_file1), Is.EqualTo (Path.GetFileName (files[0])));
      Assert.That (Path.GetFileName (_file2), Is.EqualTo (Path.GetFileName (files[1])));

      _helperExtended.Delete (files[0]);
      _helperExtended.Delete (files[1]);
      _helperExtended.Delete (zipFileName);
    }

    [Test]
    public void ExtractZipFileWithFiles ()
    {
      byte[] zipArchive = CreateZipArchive (_sourcePath);

      using (var zipArchiveStream = new MemoryStream (zipArchive))
      {
        var zipExtractor = new ZipFileExtractor (zipArchiveStream);

        var files = zipExtractor.GetFiles();

        Assert.That (files.Length, Is.EqualTo (2));

        Assert.That (files[0].FullName, Is.EqualTo (Path.GetFileName (_file1)));
        Assert.That (GetBytesFromFile (files[0]), Is.EqualTo (File.ReadAllBytes (_file1)));

        Assert.That (files[1].FullName, Is.EqualTo (Path.GetFileName (_file2)));
        Assert.That (GetBytesFromFile (files[1]), Is.EqualTo (File.ReadAllBytes (_file2)));
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
        var zipExtractor = new ZipFileExtractor (zipArchiveStream);

        var files = zipExtractor.GetFiles();

        Assert.That (files[0].FullName, Is.EqualTo (Path.Combine ("SubFolder", Path.GetFileName (_file1))));
        Assert.That (files[1].FullName, Is.EqualTo (Path.Combine ("SubFolder", Path.GetFileName (_file2))));

        Assert.That (files[0].Directory, Is.Not.Null);
        Assert.That (files[0].Directory.Name, Is.EqualTo ("SubFolder"));
        Assert.That (files[0].Directory, Is.SameAs (files[1].Directory));
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