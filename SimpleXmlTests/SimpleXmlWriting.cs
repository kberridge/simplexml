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
      string ex = "<root><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.child = "new value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<child>new value</child>"));
    }

    [Test]
    public void AttributesOnRoot()
    {
      string ex = "<root attr=\"test\"><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void AttributesOnLeaf()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.child.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.child.text = "new value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<child attr=\"test\">new value</child>"));
    }

    [Test]
    public void SettingValueOnNodeWithChildNodes()
    {
      string ex = "<root><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.Catch<InvalidOperationException>(() => x.root = "test");
    }

    [Test]
    public void SettingValueOfRoot()
    {
      string ex = "<root></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root = "test";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root>test</root>"));
    }

    [Test]
    public void SettingValueOfRootOnlyAttribute()
    {
      string ex = "<root attr=\"test\">value</root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.attr = "testvalue";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root attr=\"testvalue\">value</root>"));
    }

    [Test]
    public void MissingNode()
    {
      string ex = "<root><child>value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.Catch<InvalidOperationException>(() => x.root.dne = "missing");
    }

    [Test]
    public void MissingAttr()
    {
      string ex = "<root><child attr=\"test\">value</child></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      Assert.Catch<InvalidOperationException>(() => x.root.child.noattr = "missing");
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      string ex = "<root><node1>test</node1><node2>test2</node2></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.node2 = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<node2>test3</node2>"));
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      string ex = "<root><node>test</node><node>test2</node></root>";
      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.root.node = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<node>test3</node>"));
      Assert.IsTrue(xmlString.Contains("<node>test2</node>"));
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

      x.section.title = "Great Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<title>Great Blogs</title>"));
    }

    [Test]
    public void WithNamedXmlNs()
    {
      string ex = "<h:section xmlns:h=\"http://www.w3.org/HTML/1998/html4\"><h:title>Blogs</h:title></h:section>";

      var r = new StringReader(ex);
      dynamic x = new SimpleXml(r);

      x.section.title = "Awesome Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<h:title>Awesome Blogs</h:title>"));
    }
  }
}