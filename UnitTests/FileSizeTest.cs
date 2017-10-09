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
using NUnit.Framework;
using Remotion.Utilities;

namespace Remotion.IO.UnitTests
{
  [TestFixture]
  public class FileSizeTest
  {
    [Test]
    [ExpectedException (typeof (ArgumentOutOfRangeException))]
    public void ValueSmallerZero ()
    {
      new FileSize (-1);
    }

    [Test]
    [SetCulture ("")]
    [SetUICulture ("")]
    public void Output ()
    {
      Assert.That (new FileSize (0).ToString(), Is.EqualTo ("0 bytes"));

      Assert.That (new FileSize (10).ToString(), Is.EqualTo ("10 bytes"));
      Assert.That (new FileSize (100).ToString(), Is.EqualTo ("100 bytes"));
      Assert.That (new FileSize (1000).ToString(), Is.EqualTo ("1000 bytes"));

      Assert.That (new FileSize (10000).ToString(), Is.EqualTo ("9.77 KB"));
      Assert.That (new FileSize (100000).ToString(), Is.EqualTo ("97.66 KB"));
      Assert.That (new FileSize (1000000).ToString(), Is.EqualTo ("976.56 KB"));
      Assert.That (new FileSize (1023000).ToString(), Is.EqualTo ("999.02 KB")); //TODO: possible refactor output to 1000 

      Assert.That (new FileSize (10000000).ToString(), Is.EqualTo ("9.54 MB"));
      Assert.That (new FileSize (100000000).ToString(), Is.EqualTo ("95.37 MB"));
      Assert.That (new FileSize (1000000000).ToString(), Is.EqualTo ("953.67 MB"));

      Assert.That (new FileSize (10000000000).ToString(), Is.EqualTo ("9.31 GB"));
      Assert.That (new FileSize (100000000000).ToString(), Is.EqualTo ("93.13 GB"));
      Assert.That (new FileSize (1000000000000).ToString(), Is.EqualTo ("931.32 GB"));

      Assert.That (new FileSize (10000000000000).ToString(), Is.EqualTo ("9.09 TB"));
    }

    [Test]
    [TestCase ("", "0 bytes")]
    [TestCase ("de", "0 Bytes")]
    [TestCase ("DE", "0 Bytes")]
    [TestCase ("de-AT", "0 Bytes")]
    [TestCase ("de-DE", "0 Bytes")]
    [TestCase ("en", "0 bytes")]
    [TestCase ("en-GB", "0 bytes")]
    [TestCase ("en-US", "0 bytes")]
    [TestCase ("fr", "0 octets")]
    [TestCase ("FR", "0 octets")]
    [TestCase ("fr-FR", "0 octets")]
    [TestCase ("it", "0 byte")]
    [TestCase ("IT", "0 byte")]
    [TestCase ("it-IT", "0 byte")]
    public void OutputWithZeroBytes (string culture, string formattedValue)
    {
      var fileSize = new FileSize (0);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "1 byte")]
    [TestCase ("de", "1 Byte")]
    [TestCase ("DE", "1 Byte")]
    [TestCase ("de-AT", "1 Byte")]
    [TestCase ("de-DE", "1 Byte")]
    [TestCase ("en", "1 byte")]
    [TestCase ("en-GB", "1 byte")]
    [TestCase ("en-US", "1 byte")]
    [TestCase ("fr", "1 octet")]
    [TestCase ("FR", "1 octet")]
    [TestCase ("fr-FR", "1 octet")]
    [TestCase ("it", "1 byte")]
    [TestCase ("IT", "1 byte")]
    [TestCase ("it-IT", "1 byte")]
    public void OutputWithOneByte (string culture, string formattedValue)
    {
      var fileSize = new FileSize (1);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "128 bytes")]
    [TestCase ("de", "128 Bytes")]
    [TestCase ("DE", "128 Bytes")]
    [TestCase ("de-AT", "128 Bytes")]
    [TestCase ("de-DE", "128 Bytes")]
    [TestCase ("en", "128 bytes")]
    [TestCase ("en-GB", "128 bytes")]
    [TestCase ("en-US", "128 bytes")]
    [TestCase ("fr", "128 octets")]
    [TestCase ("FR", "128 octets")]
    [TestCase ("fr-CH", "128 octets")]
    [TestCase ("it", "128 byte")]
    [TestCase ("IT", "128 byte")]
    [TestCase ("it-IT", "128 byte")]
    public void OutputWithBytes (string culture, string formattedValue)
    {
      var fileSize = new FileSize (128);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "1000 bytes")]
    [TestCase ("de", "1000 Bytes")]
    [TestCase ("DE", "1000 Bytes")]
    [TestCase ("de-AT", "1000 Bytes")]
    [TestCase ("de-DE", "1000 Bytes")]
    [TestCase ("en", "1000 bytes")]
    [TestCase ("en-GB", "1000 bytes")]
    [TestCase ("en-US", "1000 bytes")]
    [TestCase ("fr", "1000 octets")]
    [TestCase ("FR", "1000 octets")]
    [TestCase ("fr-CH", "1000 octets")]
    [TestCase ("it", "1000 byte")]
    [TestCase ("IT", "1000 byte")]
    [TestCase ("it-IT", "1000 byte")]
    public void OutputWith1000Bytes (string culture, string formattedValue)
    {
      var fileSize = new FileSize (1000);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "125.00 KB")]
    [TestCase ("de", "125.00 KB")]
    [TestCase ("de-AT", "125.00 KB")]
    [TestCase ("de-DE", "125.00 KB")]
    [TestCase ("en", "125.00 KB")]
    [TestCase ("en-GB", "125.00 KB")]
    [TestCase ("en-US", "125.00 KB")]
    [TestCase ("fr", "125.00 Ko")]
    [TestCase ("FR", "125.00 Ko")]
    [TestCase ("fr-CH", "125.00 Ko")]
    [TestCase ("it", "125.00 KB")]
    [TestCase ("it-IT", "125.00 KB")]
    public void OutputWithKiloByte (string culture, string formattedValue)
    {
      var fileSize = new FileSize (128000);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "1.25 MB")]
    [TestCase ("de", "1.25 MB")]
    [TestCase ("de-AT", "1.25 MB")]
    [TestCase ("de-DE", "1.25 MB")]
    [TestCase ("en", "1.25 MB")]
    [TestCase ("en-GB", "1.25 MB")]
    [TestCase ("en-US", "1.25 MB")]
    [TestCase ("fr", "1.25 Mo")]
    [TestCase ("FR", "1.25 Mo")]
    [TestCase ("fr-CH", "1.25 Mo")]
    [TestCase ("it", "1.25 MB")]
    [TestCase ("it-IT", "1.25 MB")]
    public void OutputWithMB1 (string culture, string formattedValue)
    {
      var fileSize = new FileSize (1312000);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [SetCulture ("")]
    [SetUICulture ("")]
    public void OutputWithMB2 ()
    {
      var fileSize = new FileSize (13107200);
      Assert.That (fileSize.ToString(), Is.EqualTo ("12.50 MB"));
    }

    [Test]
    [TestCase ("", "12.50 GB")]
    [TestCase ("de", "12.50 GB")]
    [TestCase ("de-AT", "12.50 GB")]
    [TestCase ("de-DE", "12.50 GB")]
    [TestCase ("en", "12.50 GB")]
    [TestCase ("en-GB", "12.50 GB")]
    [TestCase ("en-US", "12.50 GB")]
    [TestCase ("fr", "12.50 Go")]
    [TestCase ("FR", "12.50 Go")]
    [TestCase ("fr-CH", "12.50 Go")]
    [TestCase ("it", "12.50 GB")]
    [TestCase ("it-IT", "12.50 GB")]
    public void OutputWithGB (string culture, string formattedValue)
    {
      var fileSize = new FileSize (13421772800);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }

    [Test]
    [TestCase ("", "12.50 TB")]
    [TestCase ("de", "12.50 TB")]
    [TestCase ("de-AT", "12.50 TB")]
    [TestCase ("de-DE", "12.50 TB")]
    [TestCase ("en", "12.50 TB")]
    [TestCase ("en-GB", "12.50 TB")]
    [TestCase ("en-US", "12.50 TB")]
    [TestCase ("fr", "12.50 To")]
    [TestCase ("FR", "12.50 To")]
    [TestCase ("fr-CH", "12.50 To")]
    [TestCase ("it", "12.50 TB")]
    [TestCase ("it-IT", "12.50 TB")]
    public void OutputWithTB (string culture, string formattedValue)
    {
      var fileSize = new FileSize (13743895347200);
      using (new CultureScope ("", culture))
      {
        Assert.That (fileSize.ToString(), Is.EqualTo (formattedValue));
      }
    }
  }
}