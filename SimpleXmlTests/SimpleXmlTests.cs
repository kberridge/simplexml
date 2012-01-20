﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SimpleXmlNs;

namespace SimpleXmlTests
{
  [TestFixture]
  public class SimpleXmlTests
  {
    [Test]
    public void SimpleValues()
    {
      string ex = "<root><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("value", x.root.child);
    }

    [Test]
    public void AttributesOnRoot()
    {
      string ex = "<root attr=\"test\"><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void AttributesOnLeaf()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test", x.root.child.attr);
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("value", x.root.child.text);
    }

    [Test]
    public void SimpleXmlIsReusable()
    {
      string ex = "<root attr=\"test\"><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test", x.root.attr);
      Assert.AreEqual("value", x.root.child);
    }
  }
}