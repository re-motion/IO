// This file is part of re-vision (www.re-motion.org)
// Copyright (C) rubicon IT GmbH, www.rubicon.eu
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

namespace Remotion.Dms.Shared.IO
{
  /// <summary>
  /// Represents a custom file size type to print the size in an human readable way.
  /// </summary>
  [Serializable]
  public struct FileSize
  {
    private const long c_terabyte = c_gigabyte * c_kilobyte;
    private const long c_gigabyte = c_megabyte * c_kilobyte;
    private const long c_megabyte = c_kilobyte * c_kilobyte;
    private const long c_kilobyte = 1024;

    private readonly long _value;
    
    public FileSize (long value)
    {
      if (value < 0)
        throw new ArgumentOutOfRangeException ("value", "Value is smaller than 0.");
      _value = value;
    }

    public long Value
    {
      get { return _value; }
    }

    public override string ToString ()
    {
      return GetFileSizeFormatted();
    }

    private string GetFileSizeFormatted ()
    {
      if (_value >= c_terabyte)
      {
        return ((decimal) _value / c_terabyte).ToString ("0.00 TB");
      }
      else if ((decimal) _value >= c_gigabyte)
      {
        return ((decimal) _value / c_gigabyte).ToString ("0.00 GB");
      }
      else if (_value >= c_megabyte)
      {
        return((decimal)_value / c_megabyte).ToString ("0.00 MB");
      }
      else if (_value >= c_kilobyte)
      {
        return ((decimal) _value / c_kilobyte).ToString ("0.00 kB");
      }
      else
        return _value + " B";
    }
  }
}