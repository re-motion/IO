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
using System.Runtime.Serialization;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// This exception is thrown if an argument is empty although it must have a content.
  /// </summary>
  [Serializable]
  public class ArgumentEmptyException : ArgumentException
  {
    public ArgumentEmptyException (string argumentName)
        : base (FormatMessage (argumentName))
    {
    }

    public ArgumentEmptyException (SerializationInfo info, StreamingContext context)
        : base (info, context)
    {
    }

    private static string FormatMessage (string argumentName)
    {
      return string.Format ("Argument {0} is empty.", argumentName);
    }
  }
}