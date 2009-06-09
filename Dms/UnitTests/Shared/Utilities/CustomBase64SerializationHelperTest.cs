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
using Remotion.Development.UnitTesting;
using Remotion.Dms.Shared.Utilities;
using Remotion.Dms.UnitTests.Core;

namespace Remotion.Dms.UnitTests.Shared.Utilities
{
  [TestFixture]
  public class CustomBase64SerializationHelperTest
  {
    [Test]
    public void Serialize ()
    {
      var value = new AuthenticationContextStub { Value1 = "test" };
      
      Assert.That (Convert.ToBase64String (Serializer.Serialize (value)).Replace ('=', '.').Replace ('/', '('), Is.EqualTo (CustomBase64SerializationHelper.Serialize (value)));
    }

    [Test]
    public void Deserialize ()
    {
      var value = new AuthenticationContextStub { Value1 = "test" };
      var serialized = Convert.ToBase64String (Serializer.Serialize (value)).Replace ('=', '.').Replace ('/', '(');
      var deserialized = CustomBase64SerializationHelper.Deserialize<AuthenticationContextStub> (serialized);

      Assert.That (value.Value1, Is.EqualTo (deserialized.Value1));
    }

    [Test]
    public void SerializeAndDeserialize ()
    {
      var value = new AuthenticationContextStub { Value1 = "test" };
      var serialized = CustomBase64SerializationHelper.Serialize (value);
      var deserialized = CustomBase64SerializationHelper.Deserialize<AuthenticationContextStub> (serialized);

      Assert.That (value.Value1, Is.EqualTo (deserialized.Value1));
    }
  }
}