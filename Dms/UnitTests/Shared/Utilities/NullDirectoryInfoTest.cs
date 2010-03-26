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
  public class NullDirectoryInfoTest
  {
    private NullDirectoryInfo _directoryInfo;

    [SetUp]
    public void SetUp ()
    {
      _directoryInfo = new NullDirectoryInfo();
    }

    [Test]
    public void FullName ()
    {
      Assert.That (_directoryInfo.FullName, Is.Empty);
    }

    [Test]
    public void Extension ()
    {
      Assert.That (_directoryInfo.Extension, Is.Empty);
    }

    [Test]
    public void Name ()
    {
      Assert.That (_directoryInfo.Name, Is.Empty);
    }

    [Test]
    public void Directory ()
    {
      Assert.That (_directoryInfo.Directory, Is.Null);
    }

    [Test]
    public void Exists ()
    {
      Assert.That (_directoryInfo.Exists, Is.False);
    }

    [Test]
    public void IsReadOnly ()
    {
      Assert.That (_directoryInfo.IsReadOnly, Is.False);
    }

    [Test]
    public void CreationTime ()
    {
      Assert.That (_directoryInfo.CreationTime, Is.EqualTo (DateTime.MinValue));
    }

    [Test]
    public void LastAccessTime ()
    {
      Assert.That (_directoryInfo.LastAccessTime, Is.EqualTo (DateTime.MinValue));
    }

    [Test]
    public void LastWriteTime ()
    {
      Assert.That (_directoryInfo.LastWriteTime, Is.EqualTo (DateTime.MinValue));
    }

    [Test]
    public void GetFiles ()
    {
      Assert.That (_directoryInfo.GetFiles(), Is.Empty);
    }

    [Test]
    public void GetDirectories ()
    {
      Assert.That (_directoryInfo.GetDirectories(), Is.Empty);
    }
  }
}