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
using Moq;
using NUnit.Framework;

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
      var inputMock = new Mock<Stream> (MockBehavior.Strict);
      inputMock.SetupGet (s => s.CanSeek).Returns (false);

      var sequence = new MockSequence();
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (0, 10).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      _streamCopier.CopyStream (inputMock.Object, outputStream);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 }));

      inputMock.Verify();
    }

    [TestCase (null)]
    [TestCase (10)]
    public void CopyStream_WriteInSteps_ReturnsTrue (int? maxLength)
    {
      var inputMock = new Mock<Stream> (MockBehavior.Strict);

      inputMock.SetupGet (s => s.CanSeek).Returns (false);
      var sequence = new MockSequence();
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (0, 5).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (10, 3).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (20, 2).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      var bytesTransferredHistory = new List<long>();
      _streamCopier.TransferProgress += (sender, e) => bytesTransferredHistory.Add (e.CurrentValue);

      var result = _streamCopier.CopyStream (inputMock.Object, outputStream, maxLength);

      Assert.That (result, Is.True);
      Assert.That (outputStream.ToArray(), Is.EqualTo (new byte[] { 0, 1, 2, 3, 4, 10, 11, 12, 20, 21 }));
      Assert.That (bytesTransferredHistory, Is.EqualTo (new object[] { 5L, 8L, 10L, 10L }));

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooShort ()
    {
      var inputMock = new Mock<Stream>();
      const int streamLength = 10;
      const int contentLength = streamLength / 2;

      var sequence = new MockSequence();
      inputMock.SetupGet (s => s.Length).Returns (streamLength);
      inputMock.SetupGet (s => s.CanSeek).Returns (true);
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (0, contentLength).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      Assert.That (_streamCopier.CopyStream (inputMock.Object, outputStream), Is.False);

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_LengthDiffersWithContentTooLong_ThrowsIOException ()
    {
      var inputMock = new Mock<Stream>();
      const int streamLength = 10;
      const int contentLength = streamLength * 2;

      var sequence = new MockSequence();
      inputMock.SetupGet (s => s.Length).Returns (streamLength);
      inputMock.SetupGet (s => s.CanSeek).Returns (true);
      SetupReadExpectation (inputMock, sequence, Enumerable.Range (0, contentLength).Select (i => (byte) i));
      SetupReadExpectation (inputMock, sequence, new byte[] { });

      MemoryStream outputStream = new MemoryStream();

      Assert.That (
          () => _streamCopier.CopyStream (inputMock.Object, outputStream),
          Throws.TypeOf<IOException>().With.Message.EqualTo ("The stream returned more data than the stream length (10 bytes) specified."));

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_UserCancels_ReturnsFalse ()
    {
      var inputMock = new Mock<Stream>();

      const int numberOfNotificationsToWait = 5;

      var sequence = new MockSequence();
      for (int i = 0; i < numberOfNotificationsToWait; i++)
        inputMock.InSequence (sequence).Setup (s => s.Read (It.IsAny<Byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns (1).Verifiable();

      inputMock.SetupGet (s => s.Length).Returns (10);

      var transferProgressNotificationCount = 0;
      _streamCopier.TransferProgress += (sender, e) =>
                                        {
                                          transferProgressNotificationCount++;
                                          if (transferProgressNotificationCount == numberOfNotificationsToWait)
                                            e.Cancel = true;
                                        };

      Assert.That (_streamCopier.CopyStream (inputMock.Object, new MemoryStream()), Is.False);

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_StreamCannotRead_DoesntCallLength ()
    {
      var inputMock = new Mock<Stream>();
      inputMock.Setup (s => s.Read (It.IsAny<Byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns (0).Verifiable();
      inputMock.SetupGet (s => s.CanSeek).Returns (false);
      inputMock.SetupGet (s => s.Length).Throws (new AssertionException ("Length should not be called, when CanSeek returns false."));

      _streamCopier.CopyStream (inputMock.Object, new MemoryStream());

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_PassesCorrectBufferLength ()
    {
      var inputMock = new Mock<Stream>();
      inputMock.Setup (s => s.Read (new byte[_streamCopier.BufferSize], 0, _streamCopier.BufferSize)).Returns (0).Verifiable();
      inputMock.SetupGet (s => s.CanSeek).Returns (false);

      _streamCopier.CopyStream (inputMock.Object, new MemoryStream());

      inputMock.Verify();
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanMaximumLength_ThrowsIOException ()
    {
      var inputStub = new Mock<Stream>();
      inputStub.SetupGet (_ => _.CanSeek).Returns (false);
      inputStub.Setup (_ => _.Read (It.IsAny<Byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns (13).Verifiable();

      Assert.That (
          () => _streamCopier.CopyStream (inputStub.Object, new MemoryStream(), 12),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data (13 bytes) than the specified maximum (12 bytes)."));
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanStreamLength_ThrowsIOException ()
    {
      var inputStub = new Mock<Stream>();
      inputStub.SetupGet (_ => _.CanSeek).Returns (true);
      inputStub.SetupGet (_ => _.Length).Returns (11);
      inputStub.SetupSequence (_ => _.Read (It.IsAny<Byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
               .Returns (13)
               .Returns (0);

      Assert.That (
          () => _streamCopier.CopyStream (inputStub.Object, new MemoryStream()),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data than the stream length (11 bytes) specified."));
    }

    [Test]
    public void CopyStream_StreamReturnsMoreDataThanMaximumLengthAndStreamLength_ThrowsIOException ()
    {
      var inputStub = new Mock<Stream>();
      inputStub.SetupGet (_ => _.CanSeek).Returns (true);
      inputStub.SetupGet (_ => _.Length).Returns (11);
      inputStub.Setup (_ => _.Read (It.IsAny<Byte[]>(), It.IsAny<int>(), It.IsAny<int>())).Returns (13).Verifiable();

      Assert.That (
          () => _streamCopier.CopyStream (inputStub.Object, new MemoryStream(), 12),
          Throws.InstanceOf<IOException>().With.Message.EqualTo ("The stream returned more data (13 bytes) than the specified maximum (12 bytes)."));
    }

    private void SetupReadExpectation (Mock<Stream> streamMock, MockSequence mockSequence, IEnumerable<byte> bytes)
    {
      var byteArray = bytes.ToArray();

      streamMock
         .InSequence (mockSequence)
         .Setup (s => s.Read (It.IsAny<byte[]>(), It.IsAny<int>(), It.IsAny<int>()))
         .Callback (
              (byte[] buffer, int offset, int _) =>
              {
                for (int i = 0; i < byteArray.Length; ++i)
                  buffer[offset + i] = byteArray[i];
              })
         .Returns (byteArray.Length)
         .Verifiable();
    }
  }
}