// This file is part of the re-motion Framework (www.re-motion.org)
// Copyright (c) rubicon IT GmbH, www.rubicon.eu
// 
// re-motion is free software; you can redistribute it and/or modify it under 
// the terms of the GNU Lesser General Public License as published by the 
// Free Software Foundation; either version 2.1 of the License, 
// or (at your option) any later version.
// 
// re-motion is distributed in the hope that it will be useful, 
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the 
// GNU Lesser General Public License for more details.
// 
// You should have received a copy of the GNU Lesser General Public License
// along with re-motion; if not, see http://www.gnu.org/licenses.
// 

using System;
using System.Globalization;

namespace Remotion.IO
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
        return ((decimal) _value / c_terabyte).ToString (GetLocalizedFormattingStringForTeraBytes());
      }
      else if ((decimal) _value >= c_gigabyte)
      {
        return ((decimal) _value / c_gigabyte).ToString (GetLocalizedFormattingStringForGigaBytes());
      }
      else if (_value >= c_megabyte)
      {
        return((decimal)_value / c_megabyte).ToString (GetLocalizedFormattingStringForMegaBytes());
      }
      else if (_value >= c_kilobyte)
      {
        return ((decimal) _value / c_kilobyte).ToString (GetLocalizedFormattingStringForKiloBytes());
      }
      else if (_value > 1)
      {
        return _value.ToString (GetLocalizedFormattingStringForBytes());
      }
      else if (_value == 1)
      {
        return GetLocalizedStringForOneByte();
      }
      else
      {
        return GetLocalizedStringForZeroBytes();
      }
    }

    private string GetLocalizedFormattingStringForTeraBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0.00 To";
        default:
          return "0.00 TB";
      }
    }

    private string GetLocalizedFormattingStringForGigaBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0.00 Go";
        default:
          return "0.00 GB";
      }
    }

    private string GetLocalizedFormattingStringForMegaBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0.00 Mo";
        default:
          return "0.00 MB";
      }
    }

    private string GetLocalizedFormattingStringForKiloBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0.00 Ko";
        default:
          return "0.00 KB";
      }
    }

    private string GetLocalizedFormattingStringForBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0 octets";
        case "de":
          return "0 Bytes";
        case "it":
          return "0 byte";
        default:
          return "0 bytes";
      }
    }

    private string GetLocalizedStringForOneByte ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "1 octet";
        case "de":
          return "1 Byte";
        case "it":
          return "1 byte";
        default:
          return "1 byte";
      }
    }

    private string GetLocalizedStringForZeroBytes ()
    {
      switch (CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
      {
        case "fr":
          return "0 octets";
        case "de":
          return "0 Bytes";
        case "it":
          return "0 byte";
        default:
          return "0 bytes";
      }
    }
  }
}