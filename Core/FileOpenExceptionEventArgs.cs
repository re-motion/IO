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

namespace Remotion.IO
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