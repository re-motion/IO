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

namespace Remotion.Dms.Shared.IO
{
  public class FileOpenExceptionEventArgs : EventArgs
  {
    private readonly string _fileName;
    private readonly Exception _exception;
    
    public FileOpenExceptionEventArgs (string fileName, Exception exception)
    {
      _fileName = fileName;
      _exception = exception;
    }

    public string FileName
    {
      get { return _fileName; }
    }

    public Exception Exception
    {
      get { return _exception; }
    }
  }
}