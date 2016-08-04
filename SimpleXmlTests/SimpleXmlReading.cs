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
      Assert.Catch<NodeOrAttributeNotFoundException>(() => { var s = x.root.dne; });
    }

    [Test]
    public void MissingAttr()
    {
      dynamic x = XmlStrings.LeafAttribute;
      Assert.Catch<NodeOrAttributeNotFoundException>(() => { var s = x.root.child.noattr; });
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
    public void WithSiblingNodesWithSameNameAndChildNodes_Each()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameChildNodes;
      var nodes = x.root.node;
      Assert.IsInstanceOf<SimpleXml>(nodes);
      int i = 1;
      foreach (var n in nodes)
      {
        Assert.AreEqual("value" + i, n.node2.node3);
        i++;
      }
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
    public void WithSiblingNodesWithSameNameButOneMissingChildNode()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameButOneWithoutChildNode;
      Assert.Catch<InconsistentXmlStructureException>(() => { var s = x.root.node.node1.node2; });
    }

    [Test]
    public void WithSiblingsWithSameNameButOneWithExtraNode()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameButOneWithExtraNode;
      Assert.Catch<InconsistentXmlStructureException>(() => { var s = x.root.node.node1.node2; });
    }

    [Test]
    public void WithSiblingsWithSameNameButOneWithMissingAttribute()
    {
      dynamic x = XmlStrings.SiblingsWithSameNameButOneMissingAttributeOnChildNode;
      Assert.Catch<InconsistentXmlStructureException>(() => { var s = x.root.node.node1.node2.attr; });
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

    [Test]
    public void FirstWithValue()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.AreEqual("value2", x.root.node.First("name", "b"));
    }

    [Test]
    public void FirstWithChildElements()
    {
      dynamic x = "<root><node name='a'><child>value</child></node><node name='b'><child>value2</child></node><node name='c'><child>value3</child></node></root>".AsSimpleXml();
      Assert.AreEqual("value2", x.root.node.First("name", "b").child);
    }

    [Test]
    public void FirstWithNoMatchingValue()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => { x.root.node.First("name", "d"); });
    }

    [Test]
    public void FirstWithNoMatchingAttribute()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.Catch<InvalidOperationException>(() => { x.root.node.First("missing", "d"); });
    }

    [Test]
    public void FirstOrDefaultWithValue()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.AreEqual("value2", x.root.node.FirstOrDefault("name", "b"));
    }

    [Test]
    public void FirstOrDefaultWithChildElements()
    {
      dynamic x = "<root><node name='a'><child>value</child></node><node name='b'><child>value2</child></node><node name='c'><child>value3</child></node></root>".AsSimpleXml();
      Assert.AreEqual("value2", x.root.node.FirstOrDefault("name", "b").child);
    }

    [Test]
    public void FirstOrDefaultWithNoMatchingValue()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.IsNull(x.root.node.FirstOrDefault("name", "d"));
    }

    [Test]
    public void FirstOrDefaultWithNoMatchingAttribute()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='c'>value3></node></root>".AsSimpleXml();
      Assert.IsNull(x.root.node.FirstOrDefault("missing", "d"));
    }
    [Test]
    public void WhereWithValue()
    {
      dynamic x = "<root><node name='a'>value</node><node name='b'>value2</node><node name='b'>value3</node></root>".AsSimpleXml();
      var matches = x.root.node.Where("name", "b");
      Assert.Contains("value2", matches);
      Assert.Contains("value2", matches);
    }

    [Test]
    public void WhereWithChildElements()
    {
      dynamic x = "<root><node name='a'><child>value</child></node><node name='b'><child>value2</child></node><node name='b'><child>value3</child></node></root>".AsSimpleXml();
      var matches = x.root.node.Where("name", "b").child;
      Assert.Contains("value2", matches);
      Assert.Contains("value2", matches);
    }

    [Test]
    public void WithPrefixesWithNoXmlns()
    {
      var nameTable = new NameTable();
      var nameSpaceManager = new XmlNamespaceManager(nameTable);
      nameSpaceManager.AddNamespace("log4j", "urn:ignore");
      var parserContext = new XmlParserContext(null, nameSpaceManager, null, XmlSpace.None);
      var txtReader = new XmlTextReader(XmlStrings.PrefixesWithNoXmlns, XmlNodeType.Element, parserContext);

      dynamic x = new SimpleXml(txtReader);

      Assert.AreEqual("myMachine", x.@event.properties.data.value[0]);
    }
  }
}