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
using Remotion.Development.UnitTesting.IO;
using Remotion.Dms.DesktopConnector.Utilities;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class ZipFileBuilderTest
  {
    private FileSystemHelperExtended _helperExtended;
    private TempFile _file1;
    private TempFile _file2;
    private string _folder;
    private string _path;
    private string _destinationPath;

    [SetUp]
    public void SetUp ()
    {
      _helperExtended = new FileSystemHelperExtended();

      _file1 = new TempFile();
      _file2 = new TempFile();
      var bytes = new byte[8191];
      for (int i = 0; i < 8191; i++)
        bytes[i] = (byte) i;

      _file1.WriteAllBytes (bytes);
      _file2.WriteAllBytes (bytes);

      _folder = Path.GetRandomFileName();
      _path = Path.Combine (Path.GetTempPath(), _folder);
      var directory = Directory.CreateDirectory (_path);

      _helperExtended.CopyFile (_file1.FileName, Path.Combine (directory.FullName, Path.GetFileName (_file1.FileName)), true);
      _helperExtended.CopyFile (_file2.FileName, Path.Combine (directory.FullName, Path.GetFileName (_file2.FileName)), true);
    }

    [TearDown]
    public void TearDown ()
    {
      _file1.Dispose();
      _file2.Dispose();
      Directory.Delete (_path, true);
      if (Directory.Exists (_destinationPath))
        Directory.Delete (_destinationPath, true);
    }

    [Test]
    public void BuildReturnsZipFileWithFiles ()
    {
      var zipBuilder = _helperExtended.CreateArchiveFileBuilder();
      zipBuilder.ArchiveProgress += ((sender, e) => { });
      zipBuilder.AddFile (new FileInfoWrapper (new FileInfo (_file1.FileName)));
      zipBuilder.AddFile (new FileInfoWrapper (new FileInfo (_file2.FileName)));

      var zipFileName = _helperExtended.MakeUniqueAndValidFileName (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid() + ".zip");

      List<string> expectedFiles = null;
      try
      {
        using (zipBuilder.Build (zipFileName))
        {
        }

        expectedFiles = UnZipFile (zipFileName);

        Assert.That (Path.GetFileName (_file1.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[0])));
        Assert.That (Path.GetFileName (_file2.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[1])));
      }
      finally
      {
        if (expectedFiles != null)
        {
          if (_helperExtended.FileExists (expectedFiles[0]))
            _helperExtended.Delete (expectedFiles[0]);
          if (_helperExtended.FileExists (expectedFiles[1]))
            _helperExtended.Delete (expectedFiles[1]);
        }
        _helperExtended.Delete (zipFileName);
      }
    }

    [Test]
    public void BuildReturnsZipFileWithFolder ()
    {
      var zipBuilder = _helperExtended.CreateArchiveFileBuilder();
      zipBuilder.ArchiveProgress += ((sender, e) => { });
      zipBuilder.AddDirectory (new DirectoryInfoWrapper (new DirectoryInfo (_path)));

      var zipFileName = _helperExtended.MakeUniqueAndValidFileName (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid() + ".zip");

      List<string> expectedFiles = null;
      try
      {
        using (zipBuilder.Build (zipFileName))
        {
        }

        expectedFiles = UnZipFile (zipFileName);

        Assert.That (Path.GetFileName (_file1.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[0])));
        Assert.That (Path.GetFileName (_file2.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[1])));
      }
      finally
      {
        if (expectedFiles != null)
        {
          if (_helperExtended.FileExists (expectedFiles[0]))
            _helperExtended.Delete (expectedFiles[0]);
          if (_helperExtended.FileExists (expectedFiles[1]))
            _helperExtended.Delete (expectedFiles[1]);
        }
        _helperExtended.Delete (zipFileName);
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
      //--Directory3
      //---file4
      //---file5
      //--file6

      var file1 = new TempFile();
      var file2 = new TempFile();
      var file3 = new TempFile();
      var file4 = new TempFile();
      var file5 = new TempFile();
      var file6 = new TempFile();

      var bytes = new byte[8191];
      for (int i = 0; i < 8191; i++)
        bytes[i] = (byte) i;

      file1.WriteAllBytes (bytes);
      file2.WriteAllBytes (bytes);
      file3.WriteAllBytes (bytes);
      file4.WriteAllBytes (bytes);
      file5.WriteAllBytes (bytes);
      file6.WriteAllBytes (bytes);

      var rootPath = Path.Combine (_helperExtended.GetOrCreateAppDataPath(), "complex");

      var directory1 = Directory.CreateDirectory (Path.Combine (rootPath, "Directory1"));
      var directory2 = Directory.CreateDirectory (Path.Combine (rootPath, "Directory2"));
      var directory3 = Directory.CreateDirectory (Path.Combine (directory2.FullName, "Directory3"));

      File.Copy (file1.FileName, Path.Combine (rootPath, Path.GetFileName (file1.FileName)));
      File.Copy (file2.FileName, Path.Combine (directory1.FullName, Path.GetFileName (file2.FileName)));
      File.Copy (file3.FileName, Path.Combine (directory1.FullName, Path.GetFileName (file3.FileName)));
      File.Copy (file4.FileName, Path.Combine (directory3.FullName, Path.GetFileName (file4.FileName)));
      File.Copy (file5.FileName, Path.Combine (directory3.FullName, Path.GetFileName (file5.FileName)));
      File.Copy (file6.FileName, Path.Combine (directory2.FullName, Path.GetFileName (file6.FileName)));

      var zipFileName = _helperExtended.MakeUniqueAndValidFileName (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid() + ".zip");

      var zipBuilder = _helperExtended.CreateArchiveFileBuilder();
      zipBuilder.ArchiveProgress += ((sender, e) => { });
      zipBuilder.AddDirectory (new DirectoryInfoWrapper (new DirectoryInfo (rootPath)));

      try
      {
        using (zipBuilder.Build (zipFileName))
        {
        }

        var expectedFiles = UnZipFile (zipFileName);

        Assert.That (Path.GetFileName (file1.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[0])));
        Assert.That (Path.GetFileName (file2.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[4])));
        Assert.That (Path.GetFileName (file3.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[5])));
        Assert.That (Path.GetFileName (file4.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[2])));
        Assert.That (Path.GetFileName (file5.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[3])));
        Assert.That (Path.GetFileName (file6.FileName), Is.EqualTo (Path.GetFileName (expectedFiles[1])));
      }
      finally
      {
        Directory.Delete (rootPath, true);
        file1.Dispose();
        file2.Dispose();
        file3.Dispose();
        file4.Dispose();
        file5.Dispose();
        file6.Dispose();
        File.Delete (zipFileName);
      }
    }

    private List<string> UnZipFile (string zipFile)
    {
      FastZip fastZip = new FastZip();
      _destinationPath = Path.Combine (_helperExtended.GetOrCreateAppDataPath(), "tmp");
      fastZip.ExtractZip (zipFile, _destinationPath, FastZip.Overwrite.Always, null, null, null, false);
      List<string> files = new List<string>();
      files.AddRange (Directory.GetFiles (_destinationPath, "*", SearchOption.AllDirectories));
      return files;
    }
  }
}