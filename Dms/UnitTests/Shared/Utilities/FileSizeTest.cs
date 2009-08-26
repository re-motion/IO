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
using NUnit.Framework;
using NUnit.Framework.SyntaxHelpers;
using Remotion.Dms.Shared.Utilities;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class FileSizeTest
  {
    [Test]
    public void OutputWithKiloByte ()
    {
      var maxFileSize = 128000;
      var fileSize = new FileSize (maxFileSize);
      Assert.That (fileSize.ToString (), Is.EqualTo ("125 KB"));
    }

    [Test]
    public void OutputWithMB1 ()
    {
      var maxFileSize = 1312000;
      var fileSize = new FileSize (maxFileSize);
      Assert.That (fileSize.ToString (), Is.EqualTo ("1,25 MB"));
    }

    [Test]
    public void OutputWithMB2 ()
    {
      var maxFileSize = 13107200;
      var fileSize = new FileSize (maxFileSize);
      Assert.That (fileSize.ToString(), Is.EqualTo ("12,50 MB"));
    }

    [Test]
    public void OutputWithGB ()
    {
      var maxFileSize = 13421772800;
      var fileSize = new FileSize (maxFileSize);
      Assert.That (fileSize.ToString (), Is.EqualTo ("12,50 GB"));
    }

    [Test]
    public void OutputWithTB ()
    {
      var maxFileSize = 13743895347200;
      var fileSize = new FileSize (maxFileSize);
      Assert.That (fileSize.ToString (), Is.EqualTo ("12,50 TB"));
    }
    
  }
}