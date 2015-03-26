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
using System.IO;
using NUnit.Framework;
using Rhino.Mocks;
using MockIs = Rhino.Mocks.Constraints.Is;

namespace Remotion.IO.UnitTests
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
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeAllAtOnce);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

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
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeStep1);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeStep2);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeStep3);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      var result = _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length);

      Assert.That (result, Is.True);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 0, 1, 2, 0, 1 }));

      mockRepository.VerifyAll();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooShort ()
    {
      MockRepository mockRepository = new MockRepository();
      Stream inputMock = mockRepository.StrictMock<Stream>();
      SetupResult.For (inputMock.Length).Return (10L);
      Func<byte[], int, int, int> writeStep1 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 10; ++i)
          buffer[offset + i] = (byte) i;
        return 10;
      };

      using (mockRepository.Ordered())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeStep1);
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Return (0);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      var result = _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length * 2);
      Assert.That (result, Is.False);

      mockRepository.VerifyAll();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooLong ()
    {
      MockRepository mockRepository = new MockRepository();
      Stream inputMock = mockRepository.StrictMock<Stream>();
      SetupResult.For (inputMock.Length).Return (10L);
      Func<byte[], int, int, int> writeStep1 = delegate (byte[] buffer, int offset, int count)
      {
        for (int i = 0; i < 10; ++i)
          buffer[offset + i] = (byte) i;
        return 10;
      };

      using (mockRepository.Ordered())
      {
        Expect.Call (inputMock.Read (null, 0, 0))
            .IgnoreArguments()
            .Constraints (MockIs.Anything(), MockIs.Equal (0), MockIs.Equal (_streamCopier.BufferSize))
            .Do (writeStep1);
      }

      MemoryStream outputStream = new MemoryStream();
      mockRepository.ReplayAll();

      var result = _streamCopier.CopyStream (inputMock, outputStream, inputMock.Length / 2);
      Assert.That (result, Is.False);

      mockRepository.VerifyAll();
    }
  }
}