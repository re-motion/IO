using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;

namespace Remotion.Dms.UnitTests
{
  public abstract class GlobalizationIndependentTestBase
  {
    [TestFixtureSetUp]
    public void TestFixtureSetUp()
    {
      var culture = new CultureInfo("en-US");
      var currentThread = Thread.CurrentThread;
      currentThread.CurrentUICulture = culture;
      currentThread.CurrentCulture = culture;
    }
  }
}
