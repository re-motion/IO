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

namespace Remotion.Dms.Shared.Utilities
{
  /// <summary>
  /// Represents a custom file size type tp print the size in an human readable way.
  /// </summary>
  public class FileSize
  {
    private const long terabyte = 1099511627776;
    private const long gigabyte = 1073741824;
    private const long megabyte = 1048576;
    private const long kilobyte = 1024;

    private readonly long _maximumFileSize;

    public FileSize (long maximumFileSize)
    {
      _maximumFileSize = maximumFileSize;
    }

    public long MaximumFileSize
    {
      get { return _maximumFileSize; }
    }

    public override string ToString ()
    {
      return GetFileSizeFormatted();
    }

    private string GetFileSizeFormatted ()
    {
      if (_maximumFileSize >= terabyte)
      {
        return ((decimal) _maximumFileSize / terabyte).ToString ("0.00 TB");
      }
      else if ((decimal) _maximumFileSize >= gigabyte)
      {
        return ((decimal) _maximumFileSize / gigabyte).ToString ("0.00 GB");
      }
      else if (_maximumFileSize >= megabyte)
      {
        return((decimal)_maximumFileSize / megabyte).ToString ("0.00 MB");
      }
      else if (_maximumFileSize >= kilobyte)
      {
        return (_maximumFileSize / kilobyte).ToString ("0 KB");
      }
      else
        return _maximumFileSize + " _maximumFileSize";
    }
  }
}