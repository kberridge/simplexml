using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.IO;
using SimpleXmlNs;
using System.Xml;
using System.Text.RegularExpressions;

namespace SimpleXmlTests
{
  [TestFixture]
  public class SimpleXmlWriting
  {
    [Test]
    public void SimpleValue()
    {
      var x = XmlStrings.Simplest;
      x.root.child = "new value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<child>new value</child>"));
    }

    [Test]
    public void AttributesOnRoot()
    {
      var x = XmlStrings.RootAttribute;
      x.root.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void AttributesOnLeaf()
    {
      var x = XmlStrings.LeafAttribute;
      x.root.child.attr = "new attr value";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("attr=\"new attr value\""));
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      var x = XmlStrings.LeafAttribute;
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
      var x = XmlStrings.EmptyRoot;
      x.root = "test";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root>test</root>"));
    }

    [Test]
    public void SettingValueOfRootOnlyAttribute()
    {
      var x = XmlStrings.AttributeRootOnly;
      x.root.attr = "testvalue";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<root attr=\"testvalue\">value</root>"));
    }

    [Test]
    public void MissingNode()
    {
      var x = XmlStrings.Simplest;
      Assert.Catch<InvalidOperationException>(() => x.root.dne = "missing");
    }

    [Test]
    public void MissingAttr()
    {
      var x = XmlStrings.LeafAttribute;
      Assert.Catch<InvalidOperationException>(() => x.root.child.noattr = "missing");
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      var x = XmlStrings.SiblingsWithDifferentNames;
      x.root.node2 = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<node2>test3</node2>"));
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      var x = XmlStrings.SiblingsWithSameNames;
      x.root.node = "test3";

      var xmlString = x.GetXml();
      Assert.IsTrue(Regex.IsMatch(xmlString, @"<node>test3</node>\s*<node>test3</node>"));
    }

    [Test]
    public void WithSiblingNodesWithSameNameAndChildNodes()
    {
      var x = XmlStrings.SiblingsWithSameNameChildNodes;
      x.root.node.node2.node3 = "value3";

      var xmlString = x.GetXml();
      Assert.IsTrue(Regex.IsMatch(xmlString, "<node3>value3</node3>.*<node3>value3</node3>", RegexOptions.Singleline));
    }

    [Test]
    public void WithSiblingNodesWithSameNameAndChildNodesAndAttributes()
    {
      var x = XmlStrings.SiblingsWithSameNameChildNodesWithAttribute;
      x.root.node.node2.node3.attr = "attr3";

      var xmlString = x.GetXml();
      Assert.IsTrue(Regex.IsMatch(xmlString, "<node3 attr=\"attr3\">value1</node3>.*<node3 attr=\"attr3\">value2</node3>", RegexOptions.Singleline));
    }

    [Test]
    public void WithXmlNs()
    {
      var x = XmlStrings.XmlNs;
      x.section.title = "Great Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<title>Great Blogs</title>"));
    }

    [Test]
    public void WithNamedXmlNs()
    {
      dynamic x = XmlStrings.NamedXmlNs;
      x.section.title = "Awesome Blogs";

      var xmlString = x.GetXml();
      Assert.IsTrue(xmlString.Contains("<h:title>Awesome Blogs</h:title>"));
    }
  }
}