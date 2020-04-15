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
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Moq;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  internal class StreamWithTimeoutDecoratorTest
  {
    private class SlowStream : MemoryStream
    {
      private readonly TimeSpan _delay;

      public SlowStream (TimeSpan delay)
      {
        _delay = delay;
      }

      public override bool CanTimeout
      {
        get { return true; }
      }

      public override int ReadTimeout { get; set; }
      public override int WriteTimeout { get; set; }

      public override int Read (byte[] buffer, int offset, int count)
      {
        Thread.Sleep (_delay);
        return base.Read (buffer, offset, count);
      }

      public override void Write (byte[] buffer, int offset, int count)
      {
        Thread.Sleep (_delay);
        base.Write (buffer, offset, count);
      }
    }

    private Mock<Stream> _innerStreamMock;
    private Stream _decorator;

    [SetUp]
    public void SetUp ()
    {
      _innerStreamMock = new Mock<Stream>(MockBehavior.Strict);
      _decorator = new StreamWithTimeoutDecorator (_innerStreamMock.Object);
    }

    [TestCase (true)]
    [TestCase (false)]
    public void CanRead (bool value)
    {
      _innerStreamMock.SetupGet (s => s.CanRead).Returns (value).Verifiable();
      Assert.That (_decorator.CanRead, Is.EqualTo (value));
      _innerStreamMock.Verify();
    }

    [TestCase (true)]
    [TestCase (false)]
    public void CanSeek (bool value)
    {
      _innerStreamMock.SetupGet (s => s.CanSeek).Returns (value).Verifiable();
      Assert.That (_decorator.CanSeek, Is.EqualTo (value));
      _innerStreamMock.Verify();
    }

    [TestCase (true)]
    [TestCase (false)]
    public void CanTimeout (bool value)
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (value).Verifiable();
      Assert.That (_decorator.CanTimeout, Is.EqualTo (value));
      _innerStreamMock.Verify();
    }

    [TestCase (true)]
    [TestCase (false)]
    public void CanWrite (bool value)
    {
      _innerStreamMock.SetupGet (s => s.CanWrite).Returns (value).Verifiable();
      Assert.That (_decorator.CanWrite, Is.EqualTo (value));
      _innerStreamMock.Verify();
    }

    [Test]
    public void Length ()
    {
      _innerStreamMock.SetupGet (s => s.Length).Returns (0xdeadbeef).Verifiable();
      Assert.That (_decorator.Length, Is.EqualTo (0xdeadbeef));
      _innerStreamMock.Verify();
    }

    [Test]
    public void Position_Get ()
    {
      _innerStreamMock.SetupGet (s => s.Position).Returns (7777).Verifiable();
      Assert.That (_decorator.Position, Is.EqualTo (7777));
      _innerStreamMock.Verify();
    }

    [Test]
    public void Position_Set ()
    {
      _innerStreamMock.SetupSet (s => s.Position = 7777).Verifiable();
      _decorator.Position = 7777;
      _innerStreamMock.Verify();
    }

    [Test]
    public void ReadTimeout_Get ()
    {
      _innerStreamMock.Setup (s => s.ReadTimeout).Returns (7777).Verifiable();
      Assert.That (_decorator.ReadTimeout, Is.EqualTo (7777));
      _innerStreamMock.Verify();
    }

    [Test]
    public void ReadTimeout_Set ()
    {
      _innerStreamMock.SetupSet (s => s.ReadTimeout = 7777).Verifiable();
      _decorator.ReadTimeout = 7777;
      _innerStreamMock.Verify();
    }

    [Test]
    public void WriteTimeout_Get ()
    {
      _innerStreamMock.SetupGet (s => s.WriteTimeout).Returns (7777).Verifiable();
      Assert.That (_decorator.WriteTimeout, Is.EqualTo (7777));
      _innerStreamMock.Verify();
    }

    [Test]
    public void WriteTimeout_Set ()
    {
      _innerStreamMock.SetupSet (s => s.WriteTimeout = 7777).Verifiable();
      _decorator.WriteTimeout = 7777;
      _innerStreamMock.Verify();
    }

    [Test]
    public void Flush ()
    {
      _innerStreamMock.Setup (s => s.Flush()).Verifiable();
      _decorator.Flush();
      _innerStreamMock.Verify();
    }

    [Test]
    public void BeginRead ()
    {
      var buffer = new byte[10];
      AsyncCallback asyncCallback = _ => { };
      var state = new object();

      var result = new Mock<IAsyncResult>();

      _innerStreamMock.Setup (s => s.BeginRead (buffer, 88, 99, asyncCallback, state)).Returns (result.Object).Verifiable();

      Assert.That (_decorator.BeginRead (buffer, 88, 99, asyncCallback, state), Is.SameAs (result.Object));

      _innerStreamMock.Verify();
    }

    [Test]
    public void EndRead ()
    {
      var result = new Mock<IAsyncResult>();

      _innerStreamMock.Setup (s => s.EndRead (result.Object)).Returns (999).Verifiable();

      Assert.That (_decorator.EndRead (result.Object), Is.EqualTo (999));

      _innerStreamMock.Verify();
    }

    [Test]
    public void ReadAsync ()
    {
      var buffer = new byte[10];
      var result = Task.FromResult (666);
      var cancellationTokenSource = new CancellationTokenSource();

      _innerStreamMock.Setup (s => s.ReadAsync (buffer, 88, 99, cancellationTokenSource.Token)).Returns (result).Verifiable();
      Assert.That (_decorator.ReadAsync (buffer, 88, 99, cancellationTokenSource.Token), Is.SameAs (result));
      _innerStreamMock.Verify();
    }

    [Test]
    public void BeginWrite ()
    {
      var buffer = new byte[10];
      AsyncCallback asyncCallback = _ => { };
      var state = new object();
      var result = new Mock<IAsyncResult>();

      _innerStreamMock.Setup (s => s.BeginWrite (buffer, 88, 99, asyncCallback, state)).Returns (result.Object).Verifiable();
      Assert.That (_decorator.BeginWrite (buffer, 88, 99, asyncCallback, state), Is.SameAs (result.Object));
      _innerStreamMock.Verify();
    }

    [Test]
    public void EndWrite ()
    {
      var result = new Mock<IAsyncResult>();

      _innerStreamMock.Setup (s => s.EndWrite (result.Object)).Verifiable();
      _decorator.EndWrite (result.Object);
      _innerStreamMock.Verify();
    }

    [Test]
    public void WriteAsync ()
    {
      var buffer = new byte[10];
      var result = Task.FromResult (666);
      var cancellationTokenSource = new CancellationTokenSource();

      _innerStreamMock.Setup (s => s.WriteAsync (buffer, 88, 99, cancellationTokenSource.Token)).Returns (result).Verifiable();
      Assert.That (_decorator.WriteAsync (buffer, 88, 99, cancellationTokenSource.Token), Is.SameAs (result));
      _innerStreamMock.Verify();
    }

    [Test]
    public void Close ()
    {
      _innerStreamMock.Setup (s => s.Close()).Verifiable();
      _decorator.Close();
      _innerStreamMock.Verify();
    }

    [Test]
    public void CopyToAsync ()
    {
      var cancellationTokenSource = new CancellationTokenSource();

      Task<int> result = Task.FromResult (0);
      var memoryStream = new MemoryStream();

      _innerStreamMock.Setup (s => s.CopyToAsync (memoryStream, 88, cancellationTokenSource.Token)).Returns (result).Verifiable();

      Assert.That (_decorator.CopyToAsync (memoryStream, 88, cancellationTokenSource.Token), Is.SameAs (result));

      _innerStreamMock.Verify();
    }

    [Test]
    public void ReadByte_IsNotOverridden ()
    {
      const string methodName = "ReadByte";
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public;

      Assert.That (_decorator.GetType().GetMethods (flags | BindingFlags.DeclaredOnly).Count (m => m.Name == methodName), Is.EqualTo (0));
      Assert.That (_decorator.GetType().GetMethods (flags | BindingFlags.FlattenHierarchy).Count (m => m.Name == methodName), Is.EqualTo (1));
    }

    [Test]
    public void WriteByte_IsNotOverridden ()
    {
      const string methodName = "WriteByte";
      const BindingFlags flags = BindingFlags.Instance | BindingFlags.InvokeMethod | BindingFlags.Public;

      Assert.That (_decorator.GetType().GetMethods (flags | BindingFlags.DeclaredOnly).Count (m => m.Name == methodName), Is.EqualTo (0));
      Assert.That (_decorator.GetType().GetMethods (flags | BindingFlags.FlattenHierarchy).Count (m => m.Name == methodName), Is.EqualTo (1));
    }

    [Test]
    public void Write_InnerCannotTimeout ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (false).Verifiable();
      byte[] buffer = new byte[10];
      _innerStreamMock.Setup (s => s.Write (buffer, 22, 33)).Verifiable();

      _decorator.Write (buffer, 22, 33);
      _innerStreamMock.Verify();
    }

    [Test]
    public void Write_UnderlyingStreamTimesOut_ThrowsTimeoutException ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (true).Verifiable();
      byte[] buffer = new byte[10];

      var waitHandle = new Mock<WaitHandle> (MockBehavior.Strict);
      waitHandle.Setup (w => w.WaitOne (TimeSpan.FromMilliseconds (123))).Returns (false).Verifiable();

      var asyncResultMock = new Mock<IAsyncResult>();
      asyncResultMock.SetupGet (r => r.AsyncWaitHandle).Returns (waitHandle.Object);

      _innerStreamMock.Setup (s => s.BeginWrite (buffer, 22, 33, null, null)).Returns (asyncResultMock.Object).Verifiable();
      _innerStreamMock.SetupGet (s => s.WriteTimeout).Returns (123).Verifiable();


      Assert.That (
          () => _decorator.Write (buffer, 22, 33),
          Throws.InstanceOf<TimeoutException>().With.Message.EqualTo ("The write operation exceeded the timout of '123' ms."));


      _innerStreamMock.Verify();
    }

    [Test]
    public void Write_UnderlyingStreamDoesnTimeOut ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (true).Verifiable();
      byte[] buffer = new byte[10];

      var waitHandle = new Mock<WaitHandle> (MockBehavior.Strict);
      waitHandle.Setup (w => w.WaitOne (TimeSpan.FromMilliseconds (123))).Returns (true).Verifiable();

      var asyncResultMock = new Mock<IAsyncResult>();
      asyncResultMock.SetupGet (r => r.AsyncWaitHandle).Returns (waitHandle.Object);

      _innerStreamMock.Setup (s => s.BeginWrite (buffer, 22, 33, null, null)).Returns (asyncResultMock.Object).Verifiable();
      _innerStreamMock.SetupGet (s => s.WriteTimeout).Returns (123).Verifiable();
      _innerStreamMock.Setup (s => s.EndWrite (asyncResultMock.Object)).Verifiable();

      _decorator.Write (buffer, 22, 33);

      _innerStreamMock.Verify();
    }

    [Test]
    public void Write_IntegrationTest ()
    {
      var stream = new SlowStream (TimeSpan.FromMilliseconds (200));
      stream.ReadTimeout = 5000;
      stream.WriteTimeout = 50;

      var decoratedStream = new StreamWithTimeoutDecorator (stream);

      Assert.That (
          () => decoratedStream.Write (new byte[10], 22, 33),
          Throws.InstanceOf<TimeoutException>());
    }

    [Test]
    public void Read_UnderlyingStreamTimesOut_ThrowsTimeoutException ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (true);
      byte[] buffer = new byte[10];

      var waitHandle = new Mock<WaitHandle> (MockBehavior.Strict);
      waitHandle.Setup (w => w.WaitOne (TimeSpan.FromMilliseconds (123))).Returns (false).Verifiable();

      var asyncResultMock = new Mock<IAsyncResult>();
      asyncResultMock.SetupGet (r => r.AsyncWaitHandle).Returns (waitHandle.Object);

      _innerStreamMock.Setup (s => s.BeginRead (buffer, 22, 33, null, null)).Returns (asyncResultMock.Object).Verifiable();
      _innerStreamMock.SetupGet (s => s.ReadTimeout).Returns (123).Verifiable();


      Assert.That (
          () => _decorator.Read (buffer, 22, 33),
          Throws.InstanceOf<TimeoutException>().With.Message.EqualTo ("The read operation exceeded the timout of '123' ms."));


      _innerStreamMock.Verify();
    }

    [Test]
    public void Read_UnderlyingStreamDoesnTimeOut ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (true).Verifiable();
      byte[] buffer = new byte[10];

      var waitHandle = new Mock<WaitHandle>(MockBehavior.Strict);
      waitHandle.Setup (w => w.WaitOne (TimeSpan.FromMilliseconds (123))).Returns (true).Verifiable();

      var asyncResultMock = new Mock<IAsyncResult>();
      asyncResultMock.SetupGet (r => r.AsyncWaitHandle).Returns (waitHandle.Object);

      _innerStreamMock.Setup (s => s.BeginRead (buffer, 22, 33, null, null)).Returns (asyncResultMock.Object);
      _innerStreamMock.SetupGet (s => s.ReadTimeout).Returns (123).Verifiable();
      _innerStreamMock.Setup (s => s.EndRead (asyncResultMock.Object)).Returns (999).Verifiable();

      Assert.That (_decorator.Read (buffer, 22, 33), Is.EqualTo (999));

      _innerStreamMock.Verify();
    }

    [Test]
    public void Read_InnerCannotTimeout ()
    {
      _innerStreamMock.SetupGet (s => s.CanTimeout).Returns (false).Verifiable();
      byte[] buffer = new byte[10];
      _innerStreamMock.Setup (s => s.Read (buffer, 22, 33)).Returns (444);

      Assert.That (_decorator.Read (buffer, 22, 33), Is.EqualTo (444));
      _innerStreamMock.Verify();
    }

    [Test]
    public void Read_IntegrationTest ()
    {
      var stream = new SlowStream (TimeSpan.FromMilliseconds (200));
      stream.ReadTimeout = 50;
      stream.WriteTimeout = 5000;

      var decoratedStream = new StreamWithTimeoutDecorator (stream);

      Assert.That (
          () => decoratedStream.Read (new byte[10], 22, 33),
          Throws.InstanceOf<TimeoutException>());
    }
  }
}