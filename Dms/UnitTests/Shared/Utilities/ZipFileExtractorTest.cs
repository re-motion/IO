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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Development.UnitTesting.IO;
using Remotion.Dms.DesktopConnector.Utilities;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class ZipFileExtractorTest
  {
    private FileSystemHelperExtended _helperExtended;
    private TempFile _file1;
    private TempFile _file2;
    private string _destinationPath;

    [SetUp]
    public void SetUp ()
    {
      _helperExtended = new FileSystemHelperExtended ();

      _file1 = new TempFile ();
      _file2 = new TempFile ();
      var bytes = new byte[8191];
      for (int i = 0; i < 8191; i++)
        bytes[i] = (byte) i;

      _file1.WriteAllBytes (bytes);
      _file2.WriteAllBytes (bytes);
    }

    [TearDown]
    public void TearDown ()
    {
      _file1.Dispose ();
      _file2.Dispose ();
      Directory.Delete (_destinationPath);
    }

    [Test]
    public void ExtractZipFile ()
    {
      var zipBuilder = _helperExtended.CreateArchiveBuilder();
      zipBuilder.Progress += ((sender, e) => { });
      zipBuilder.AddFile (new FileInfoWrapper(new FileInfo(_file1.FileName)));
      zipBuilder.AddFile (new FileInfoWrapper(new FileInfo(_file2.FileName)));

      var zipFileName = _helperExtended.MakeUniqueAndValidFileName (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid() + ".zip");

      using (zipBuilder.Build (zipFileName))
      {
      }

      var zipUnpacker = new ZipFileExtractor();
      _destinationPath = Path.Combine (_helperExtended.GetOrCreateAppDataPath(), Guid.NewGuid().ToString());
      zipUnpacker.Extract (zipFileName, _destinationPath);

      List<string> files = new List<string>();
      foreach (var file in Directory.GetFiles (_destinationPath))
      {
        files.Add (file);
      }

      Assert.That (Path.GetFileName (_file1.FileName), Is.EqualTo (Path.GetFileName (files[0])));
      Assert.That (Path.GetFileName (_file2.FileName), Is.EqualTo (Path.GetFileName (files[1])));
      
      _helperExtended.Delete (files[0]);
      _helperExtended.Delete (files[1]);
      _helperExtended.Delete (zipFileName);
    }
  }
}