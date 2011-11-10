// This file is part of re-vision (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
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
using JetBrains.Annotations;

namespace Remotion.Dms.Shared.Utilities
{
  public static class ArgumentUtility
  {
    [AssertionMethod]
    public static T CheckNotNull<T> ([InvokerParameterName] string argumentName, [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] T actualValue)
    {
// ReSharper disable CompareNonConstrainedGenericWithNull
      if (actualValue == null)
// ReSharper restore CompareNonConstrainedGenericWithNull
        throw new ArgumentNullException (argumentName);

      return actualValue;
    }

    [AssertionMethod]
    public static string CheckNotNullOrEmpty ([InvokerParameterName] string argumentName, [AssertionCondition (AssertionConditionType.IS_NOT_NULL)] string actualValue)
    {
      CheckNotNull (argumentName, actualValue);
      if (actualValue.Length == 0)
        throw new ArgumentEmptyException (argumentName);

      return actualValue;
    }
  }
}