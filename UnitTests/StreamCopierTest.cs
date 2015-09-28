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
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    [TestCase (null)]
    [TestCase (10)]
    [TestCase (20)]
    public void CopyStream_WriteAllAtOnce (int? maxLength)
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();
      inputMock.Stub (s => s.CanSeek).Return (false);

      SetupReadExpectation (inputMock, Enumerable.Range (0, 10).Select (i => (byte) i));
      SetupReadExpectation (inputMock, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      _streamCopier.CopyStream (inputMock, outputStream);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

      inputMock.VerifyAllExpectations();
    }


    [TestCase (null)]
    [TestCase (10)]
    public void CopyStream_WriteInSteps_ReturnsTrue (int? maxLength)
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();

      inputMock.Stub (s => s.CanSeek).Return (false);
      SetupReadExpectation (inputMock, Enumerable.Range (0, 5).Select (i => (byte) i));
      SetupReadExpectation (inputMock, Enumerable.Range (10, 3).Select (i => (byte) i));
      SetupReadExpectation (inputMock, Enumerable.Range (20, 2).Select (i => (byte) i));
      SetupReadExpectation (inputMock, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      var bytesTransferredHistory = new List<long>();
      _streamCopier.TransferProgress += (sender, e) => bytesTransferredHistory.Add (e.CurrentValue);

      var result = _streamCopier.CopyStream (inputMock, outputStream, maxLength);

      Assert.That (result, Is.True);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 10, 11, 12, 20, 21 }));
      Assert.That (bytesTransferredHistory, Is.EqualTo (new object[] { 5L, 8L, 10L, 10L }));

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooShort ()
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();
      const int streamLength = 10;
      const int contentLength = streamLength / 2;

      inputMock.Stub (s => s.Length).Return (streamLength);
      inputMock.Stub (s => s.CanSeek).Return (true);
      SetupReadExpectation (inputMock, Enumerable.Range (0, contentLength).Select (i => (byte) i));
      SetupReadExpectation (inputMock, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      Assert.That (_streamCopier.CopyStream (inputMock, outputStream), Is.False);

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooLong_ThrowsIOException ()
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();
      const int streamLength = 10;
      const int contentLength = streamLength * 2;

      inputMock.Stub (s => s.Length).Return (streamLength);
      inputMock.Stub (s => s.CanSeek).Return (true);
      SetupReadExpectation (inputMock, Enumerable.Range (0, contentLength).Select (i => (byte) i));
      SetupReadExpectation (inputMock, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      Assert.That (
          () => _streamCopier.CopyStream (inputMock, outputStream),
          Throws.TypeOf<IOException>().With.Message.EqualTo ("The stream returned more data than the stream length (10 bytes) specified."));

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_UserCancels_ReturnsFalse ()
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();

      const int numberOfNotificationsToWait = 5;

      inputMock.Expect (s => s.Read (null, 0, 0)).IgnoreArguments().Return (1).Repeat.Times (numberOfNotificationsToWait);
      inputMock.Stub (s => s.Length).Return (10);

      var transferProgressNotificationCount = 0;
      _streamCopier.TransferProgress += (sender, e) =>
      {
        transferProgressNotificationCount++;
        if (transferProgressNotificationCount == numberOfNotificationsToWait)
          e.Cancel = true;
      };

      Assert.That (_streamCopier.CopyStream (inputMock, new MemoryStream()), Is.False);

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_StreamCannotRead_DoesntCallLength ()
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();
      inputMock.Expect (s => s.Read (null, 0, 0)).IgnoreArguments().Return (0);
      inputMock.Stub (s => s.CanSeek).Return (false);
      inputMock.Stub (s => s.Length).Throw (new AssertionException ("Length should not be called, when CanSeek returns false."));

      _streamCopier.CopyStream (inputMock, new MemoryStream());

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_PassesCorrectBufferLength ()
    {
      Stream inputMock = MockRepository.GenerateStrictMock<Stream>();
      inputMock.Expect (s => s.Read (new byte[_streamCopier.BufferSize], 0, _streamCopier.BufferSize)).Return (0);
      inputMock.Stub (s => s.CanSeek).Return (false);

      _streamCopier.CopyStream (inputMock, new MemoryStream());

      inputMock.VerifyAllExpectations();
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanMaximumLength_ThrowsIOException ()
    {
      Stream inputStub = MockRepository.GenerateStub<Stream>();
      inputStub.Stub (_ => _.CanSeek).Return (false);
      inputStub.Expect (_ => _.Read (null, 0, 0)).IgnoreArguments().Return (13);

      Assert.That (
          () => _streamCopier.CopyStream (inputStub, new MemoryStream(), 12),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data (13 bytes) than the specified maximum (12 bytes)."));
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanStreamLength_ThrowsIOException ()
    {
      Stream inputStub = MockRepository.GenerateStub<Stream>();
      inputStub.Stub (_ => _.CanSeek).Return (true);
      inputStub.Stub (_ => _.Length).Return (11);
      inputStub.Stub (_ => _.Read (null, 0, 0)).IgnoreArguments().Return (13).Repeat.Once();
      inputStub.Stub (_ => _.Read (null, 0, 0)).IgnoreArguments().Return (0);

      Assert.That (
          () => _streamCopier.CopyStream (inputStub, new MemoryStream()),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data than the stream length (11 bytes) specified."));
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanMaximumLengthAndStreamLength_ThrowsIOException ()
    {
      Stream inputStub = MockRepository.GenerateStub<Stream>();
      inputStub.Stub (_ => _.CanSeek).Return (true);
      inputStub.Stub (_ => _.Length).Return (11);
      inputStub.Expect (_ => _.Read (null, 0, 0)).IgnoreArguments().Return (13);

      Assert.That (
          () => _streamCopier.CopyStream (inputStub, new MemoryStream(), 12),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data (13 bytes) than the specified maximum (12 bytes)."));
    }

    private void SetupReadExpectation (Stream streamMock, IEnumerable<byte> bytes)
    {
      var byteArray = bytes.ToArray();

      streamMock
          .Expect (s => s.Read (null, 0, 0))
          .IgnoreArguments()
          .WhenCalled (invocation =>
          {
            var buffer = (byte[]) invocation.Arguments[0];
            var offset = (int) invocation.Arguments[1];

            for (int i = 0; i < byteArray.Length; ++i)
              buffer[offset + i] = byteArray[i];
          })
          .Return (byteArray.Length)
          .Repeat.Once();
    }
  }
}