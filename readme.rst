SimpleXml is simple dynamic xml parsing.

`Install with NuGet <https://nuget.org/packages/SimpleXml>`_: install-package SimpleXml

**Read and Write Xml**::

  var x = 
    @"<root attr="attrvalue">
        <child>nodevalue</child>
      </root>".AsSimpleXml();

  x.root.child == "nodevalue"
  x.root.attr == "attrvalue"

  x.root.child = "newnodevalue";
  x.root.attr = "newattrvalue";

**Read and Write Multiple Nodes**::

  var x =
    @"<root>
        <node>value1</node>
        <node>value2</node>
      </root>".AsSimpleXml();

  x.root.node == IEnumerable containing "value1" and "value2"
  x.root.node = "new value"

**Read and Write Multiple Nodes With Child Nodes**::

  var x =
    @"<root>
        <node>
          <child>value1</child>
        </node>
        <node>
          <child>value2</child>
        </node>
      </root>".AsSimpleXml();

  x.root.node.child == IEnumerable containing "value1" and "value2"
  x.root.node.child = "new value"

**Loop Over Child Nodes**::

  var x = 
    @"<root>
        <node>
          <child>value1</child>
        </node>
        <node>
          <child>value2</child>
        </node>
      </root>".AsSimpleXml();

  var nodes = x.root.node;
  foreach(var node in nodes)
  {
    node.child == "value1" on first loop and "value2" on second loop
  }   

**Selecting Nodes By Attribute Value**::

  var x = 
    @"<root>
        <node name="a">value1</node>
        <node name="b">value2</node>
        <node name="c">value3</node>
      </root>".AsSimpleXml();

  x.root.node.First("name", "b") == "value2"
  x.root.node.FirstOrDefault("name", "b") == "value2"
  x.root.node.Where("name", "b") == IEnumerable containing "value2"

**Selecting Nodes By Attribute Value With Child Nodes**::

  var x = 
    @"<root>
        <node name="a"><child>value1</child></node>
        <node name="b"><child>value2</child></node>
        <node name="c"><child>value3</child></node>
      </root>".AsSimpleXml();

  x.root.node.First("name", "b").child == "value2"
  x.root.node.FirstOrDefault("name", "b").child == "value2"
  x.root.node.Where("name", "b").child == "value2"

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


Written by `Kevin Berridge <http://www.kevinberridge.com>`_

Licensed under the `MIT License <http://www.opensource.org/licenses/mit-license.php>`_
