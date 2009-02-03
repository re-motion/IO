// This file is part of re-vision (www.re-motion.org)
// Copyright (C) 2005-2008 rubicon informationstechnologie gmbh, www.rubicon.eu
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
  public class ArgumentUtilityTest
  {
    [Test]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Null ()
    {
      ArgumentUtility.CheckNotNull ("test", (object) null);
    }

    [Test]
    public void NotNUll ()
    {
      int result = ArgumentUtility.CheckNotNull ("test", 5);
      Assert.That (result, Is.EqualTo (5));
    }
  }
}