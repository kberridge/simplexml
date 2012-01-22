SimpleXml is simple dynamic xml parsing.

**Read Xml**::

  var x = 
  @"<root attr="attrvalue">
    <child>nodevalue</child>
  </root>".AsSimpleXml();

  x.root.child == "nodevalue"
  x.root.attr == "attrvalue"

**Write Xml**::

  x.root.child = "newnodevalue";
  x.root.attr = "newattrvalue";

**Load From File**::

  var stream = File.Open("path\to\file", FileMode.Open);
  dynamic x = new SimpleXml(stream);

**Save To File**::

  var stream = File.Open("path\to\file", FileMode.Create);
  x.Save(stream);

**Ignores Namespaces**::

  var x =
  @"<root xmlns="http://kevinberridge.com/namespacessuck" xmlns:t="http://kevinberridge.com/namespacesreallysuck">
    <t:child>value</child>
  </root>".AsSimpleXml();

  x.root.child == "value"

**Special Case For Leaf Nodes With Attributes**::

  var x =
  @"<root>
    <child attr="attrvalue">childvalue</child>
  </root>".AsSimpleXml();

  x.root.child.text == "childvalue"
  x.root.child.attr == "attrvalue"
