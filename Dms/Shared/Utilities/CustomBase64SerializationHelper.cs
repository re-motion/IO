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
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Utility class used to serialize objects to strings.
  /// </summary>
  /// <remarks>Uses the <see cref="BinaryFormatter"/> for serialization. Serialization format is not version-dependent.</remarks>
  public static class CustomBase64SerializationHelper
  {
    public static string Serialize (object value)
    {
      ArgumentUtility.CheckNotNull ("value", value);

      var formatter = new BinaryFormatter ();
      using (var tempStream = new MemoryStream ())
      {
        formatter.Serialize (tempStream, value);
        string base64 = Convert.ToBase64String (tempStream.ToArray ());
        return base64.Replace ('=', '.').Replace ('/', '(');
      }
    }

    public static T Deserialize<T> (string serialized)
    {
      ArgumentUtility.CheckNotNull ("serialized", serialized);

      var formatter = new BinaryFormatter ();
      using (var tempStream = new MemoryStream (Convert.FromBase64String (serialized.Replace ('.', '=').Replace ('(', '/'))))

      {
        return (T) formatter.Deserialize (tempStream);
      }
    }
  }
}