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
      dynamic x = XmlStrings.Simplest;
      Assert.AreEqual("value", x.root.child);
    }

    [Test]
    public void AttributesOnRoot()
    {
      dynamic x = XmlStrings.RootAttribute;
      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void AttributesOnLeaf()
    {
      dynamic x = XmlStrings.LeafAttribute;
      Assert.AreEqual("test", x.root.child.attr);
    }

    [Test]
    public void ValueOnLeafWithAttributes()
    {
      dynamic x = XmlStrings.LeafAttribute;
      Assert.AreEqual("value", x.root.child.text);
    }

    [Test]
    public void SimpleXmlIsReusable()
    {
      dynamic x = XmlStrings.RootAttribute;
      Assert.AreEqual("test", x.root.attr);
      Assert.AreEqual("value", x.root.child);
    }

    [Test]
    public void ReadingRoot()
    {
      dynamic x = XmlStrings.RootValueOnly;
      Assert.AreEqual("value", x.root);
    }

    [Test]
    public void ReadingRootOnlyAttribute()
    {
      dynamic x = XmlStrings.AttributeRootOnly;
      Assert.AreEqual("test", x.root.attr);
    }

    [Test]
    public void MissingNode()
    {
      dynamic x = XmlStrings.Simplest;
      Assert.Catch<InvalidOperationException>(() => { var s = x.root.dne; });
    }

    [Test]
    public void MissingAttr()
    {
      dynamic x = XmlStrings.LeafAttribute;
      Assert.Catch<InvalidOperationException>(() => { var s = x.root.child.noattr; });
    }

    [Test]
    public void WithSiblingNodesWithDiffNames()
    {
      dynamic x = XmlStrings.SiblingsWithDifferentNames;
      Assert.AreEqual("test2", x.root.node2);
    }

    [Test]
    public void WithSiblingNodesWithSameNames()
    {
      dynamic x = XmlStrings.SiblingsWithSameNames;
      var values = x.root.node;
      Assert.IsInstanceOf<IList<string>>(values);
      IList<string> list = (IList<string>)values;
      Assert.AreEqual(2, list.Count);
    }

    [Test]
    public void WithSiblingNodesWithSameNameAndChildNodes()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameChildNodes;
      var values = x.root.node.node2.node3;
      Assert.IsInstanceOf<IList<string>>(values);
      IList<string> list = (IList<string>)values;
      Assert.AreEqual(2, list.Count);
      Assert.AreEqual("value1", list[0]);
      Assert.AreEqual("value2", list[1]);
    }

    [Test]
    public void WithSiblingNodesWithSameNameAndChildNodesAndAttributes()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameChildNodesWithAttribute;
      var values = x.root.node.node2.node3.attr;
      Assert.IsInstanceOf<IList<string>>(values);
      IList<string> list = (IList<string>)values;
      Assert.AreEqual(2, list.Count);
      Assert.AreEqual("attr1", list[0]);
      Assert.AreEqual("attr2", list[1]);
    }

    [Test]
    public void WithXmlNs()
    {
      dynamic x = XmlStrings.XmlNs;
      Assert.AreEqual("Mr", x.section.signing.author.title);
    }

    [Test]
    public void WithNamedXmlNs()
    {
      dynamic x = XmlStrings.NamedXmlNs;
      Assert.AreEqual("Blogs", x.section.title);
    }
  }
}