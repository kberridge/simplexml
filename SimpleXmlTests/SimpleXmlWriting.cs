using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SimpleXmlNs;
using System.Xml;

namespace SimpleXmlTests
{
  [TestFixture]
  public class SimpleXmlWriting
  {
    [Test]
    public void SimpleValue()
    {
      dynamic x = "<root><child>value</child></root>".AsSimpleXml();
      x.root.child = "new value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<child>new value</child>"));
    }

    [Test]
    public void AttributesOnRoot()
    {
      dynamic x = "<root attr=\"test\"><child>value</child></root>".AsSimpleXml();
      x.root.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void AttributesOnLeaf()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      x.root.child.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      x.root.child.text = "new value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<child attr=\"test\">new value</child>"));
    }

    [Test]
    public void SettingValueOnNodeWithChildNodes()
    {
      dynamic x = "<root><child>value</child></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => x.root = "test");
    }

    [Test]
    public void SettingValueOfRoot()
    {
      dynamic x = "<root></root>".AsSimpleXml();
      x.root = "test";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root>test</root>"));
    }

    [Test]
    public void SettingValueOfRootOnlyAttribute()
    {
      dynamic x = "<root attr=\"test\">value</root>".AsSimpleXml();
      x.root.attr = "testvalue";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root attr=\"testvalue\">value</root>"));
    }

    [Test]
    public void MissingNode()
    {
      dynamic x = "<root><child>value</child></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => x.root.dne = "missing");
    }

    [Test]
    public void MissingAttr()
    {
      dynamic x = "<root><child attr=\"test\">value</child></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => x.root.child.noattr = "missing");
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      dynamic x = "<root><node1>test</node1><node2>test2</node2></root>".AsSimpleXml();
      x.root.node2 = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<node2>test3</node2>"));
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      dynamic x = "<root><node>test</node><node>test2</node></root>".AsSimpleXml();
      x.root.node = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<node>test3</node>"));
      Assert.IsTrue(xmlString.Contains("<node>test2</node>"));
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

      x.section.title = "Great Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<title>Great Blogs</title>"));
    }

    [Test]
    public void WithNamedXmlNs()
    {
      dynamic x = "<h:section xmlns:h=\"http://www.w3.org/HTML/1998/html4\"><h:title>Blogs</h:title></h:section>".AsSimpleXml();
      x.section.title = "Awesome Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<h:title>Awesome Blogs</h:title>"));
    }
  }
}