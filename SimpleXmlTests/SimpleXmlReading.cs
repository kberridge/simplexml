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
      dynamic x = "<root><child>value</child></root>".AsSimpleXml();
      Assert.AreEqual("value", x.root.child);
    }

    [Test]
    public void AttributesOnRoot()
    {
      dynamic x = "<root attr=\"test\"><child>value</child></root>".AsSimpleXml();
      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void AttributesOnLeaf()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      Assert.AreEqual("test", x.root.child.attr);
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      Assert.AreEqual("value", x.root.child.text);
    }

    [Test]
    public void SimpleXmlIsReusable()
    {
      dynamic x = "<root attr=\"test\"><child>value</child></root>".AsSimpleXml();
      Assert.AreEqual("test", x.root.attr);
      Assert.AreEqual("value", x.root.child);
    }

    [Test]
    public void ReadingRoot()
    {
      dynamic x = "<root>value</root>".AsSimpleXml();
      Assert.AreEqual("value", x.root);
    }

    [Test]
    public void ReadingRootOnlyAttribute()
    {
      dynamic x = "<root attr=\"test\">value</root>".AsSimpleXml();
      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void MissingNode()
    {
      dynamic x = "<root><child>value</child></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => { var s = x.root.dne; });
    }

    [Test]
    public void MissingAttr()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => { var s = x.root.child.noattr; });
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      dynamic x = "<root><node1>test</node1><node2>test2</node2></root>".AsSimpleXml();
      Assert.AreEqual("test2", x.root.node2);
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      dynamic x = "<root><node>test</node><node>test2</node></root>".AsSimpleXml();
      var values = x.root.node;
      Assert.IsInstanceOfType(typeof(IList<string>), values);
      IList<string> list = (IList<string>)values;
      Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void WithXmlNs()
    {
      dynamic x = @"<section xmlns='urn:com:blogs-r-us'>
   <title>Blogs</title>
   <signing>
     <author title='Mr' name='Kevin Berridge' />
     <blog title='kwblog' />
   </signing>
</section>".AsSimpleXml();
      Assert.AreEqual("Mr", x.section.signing.author.title);
    }

    [Test]
    public void WithNamedXmlNs()
    {
      dynamic x = "<h:section xmlns:h=\"http://www.w3.org/HTML/1998/html4\"><h:title>Blogs</h:title></h:section>".AsSimpleXml();
      Assert.AreEqual("Blogs", x.section.title);
    }
  }
}