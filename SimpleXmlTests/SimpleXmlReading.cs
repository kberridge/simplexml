using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SimpleXmlNs;

namespace SimpleXmlTests
{
  [TestFixture]
  public class SimpleXmlReading
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

    [Test]
    public void ReadingRoot()
    {
      string ex = "<root>value</root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("value", x.root);
    }

    [Test]
    public void ReadingRootOnlyAttribute()
    {
      string ex = "<root attr=\"test\">value</root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void MissingNode()
    {
      string ex = "<root><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.Catch<InvalidOperationException>(() => { var s = x.root.dne; });
    }

    [Test]
    public void MissingAttr()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.Catch<InvalidOperationException>(() => { var s = x.root.child.noattr; });
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      string ex = "<root><node1>test</node1><node2>test2</node2></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test2", x.root.node2);
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      string ex = "<root><node>test</node><node>test2</node></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("test", x.root.node);
    }

    [Test]
    public void WithXmlNs()
    {
      string ex = @"<section xmlns='urn:com:blogs-r-us'>
   <title>Blogs</title>
   <signing>
     <author title='Mr' name='Kevin Berridge' />
     <blog title='kwblog' />
   </signing>
</section>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("Mr", x.section.signing.author.title);
    }

    [Test]
    public void WithNamedXmlNs()
    {
      string ex = "<h:section xmlns:h=\"http://www.w3.org/HTML/1998/html4\"><h:title>Blogs</h:title></h:section>";

      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.AreEqual("Blogs", x.section.title);
    }
  }
}