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
using NUnit.Framework;
using Remotion.Dms.Shared.IO;
using Rhino.Mocks;
using Rhino.Mocks.Constraints;

namespace Remotion.Dms.UnitTests.Shared.IO
{
  [TestFixture]
  public class StreamCopierTest
  {
    private StreamCopier _streamCopier;

    [SetUp]
    public void SetUp ()
    {
      _streamCopier = new StreamCopier();
    }

    [Test]
    [Ignore ("TODO: Implement raise event")]
    public void CopyStream_WriteAllAtOnce ()
    {
      MockRepository mockRepository = new MockRepository();
      Stream inputMock = mockRepository.StrictMock<Stream>();
      
      Func<byte[], int, int, int> writeAllAtOnce = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 10; ++i)
          buffer[offset + i] = (byte) i;
        return 10;
      };

      using (mockRepository.Ordered())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Do (writeAllAtOnce);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length);
      Assert.That (outputStream.ToArray(), NUnit.Framework.SyntaxHelpers.Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

      mockRepository.VerifyAll();
    }

    [Test]
    public void CopyStream_WriteInSteps ()
    {
      MockRepository mockRepository = new MockRepository();
      Stream inputMock = mockRepository.StrictMock<Stream>();
      SetupResult.For (inputMock.Length).Return (10L);
      Func<byte[], int, int, int> writeStep1 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 5; ++i)
          buffer[offset + i] = (byte) i;
        return 5;
      };

      Func<byte[], int, int, int> writeStep2 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 3; ++i)
          buffer[offset + i] = (byte) i;
        return 3;
      };

      Func<byte[], int, int, int> writeStep3 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 2; ++i)
          buffer[offset + i] = (byte) i;
        return 2;
      };

      using (mockRepository.Ordered())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Do (writeStep1);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Do (writeStep2);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Do (writeStep3);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (Is.Anything(), Is.Equal (0), Is.Equal (_streamCopier.BufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length);
      Assert.That (outputStream.ToArray(), NUnit.Framework.SyntaxHelpers.Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 0, 1, 2, 0, 1 }));

      mockRepository.VerifyAll();
    }
  }
}