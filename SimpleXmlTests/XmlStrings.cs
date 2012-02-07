using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SimpleXmlNs;

namespace SimpleXmlTests
{
  public static class XmlStrings
  {
    public static dynamic Simplest
    {
      get { return "<root><child>value</child></root>".AsSimpleXml(); }
    }

    public static dynamic RootAttribute
    {
      get { return "<root attr=\"test\"><child>value</child></root>".AsSimpleXml(); }
    }

    public static dynamic LeafAttribute
    {
      get { return "<root><child attr=\"test\">value</child></root>".AsSimpleXml(); }
    }

    public static dynamic EmptyRoot
    {
      get { return "<root></root>".AsSimpleXml(); }
    }

    public static dynamic RootValueOnly
    {
      get { return "<root>value</root>".AsSimpleXml(); }
    }

    public static dynamic AttributeRootOnly
    {
      get { return "<root attr=\"test\">value</root>".AsSimpleXml(); }
    }

    public static dynamic SiblingsWithDifferentNames
    {
      get { return "<root><node1>test</node1><node2>test2</node2></root>".AsSimpleXml(); }
    }

    public static dynamic SiblingsWithSameNames
    {
      get { return "<root><node>test</node><node>test2</node></root>".AsSimpleXml(); }
    }

    public static dynamic SiblingsWithSameNameChildNodes
    {
      get
      {
        return 
          @"<root>
              <node>
                <node2>
                  <node3>value1</node3>
                </node2>
              </node>
              <node>
                <node2>
                  <node3>value2</node3>
                </node2>
              </node>
            </root>".AsSimpleXml();
      }
    }

    public static dynamic SiblingsWithSameNameChildNodesWithAttribute
    {
      get
      {
        return
          @"<root>
            <node>
              <node2>
                <node3 attr='attr1'>value1</node3>
              </node2>
            </node>
            <node>
              <node2>
                <node3 attr='attr2'>value2</node3>
              </node2>
            </node>
          </root>".AsSimpleXml();
      }
    }

    public static dynamic SiblingsWithSameNameButOneWithoutChildNode
    {
      get
      {
        return 
          @"<root>
              <node>
                <node1>
                  <node2>a value</node2>
                </node1>
              </node>
              <node>
                <node1>
                  <node2>a value2</node2>
                </node1>
              </node>
              <node>
                <node1>
                  a different value
                </node1>
              </node>
            </root>".AsSimpleXml();
      }
    }

    public static dynamic SiblingsWithSameNameButOneWithExtraNode
    {
      get
      {
        return
          @"<root>
              <node>
                <node1>
                  <node2>a value</node2>
                </node1>
              </node>
              <node>
                <node1>
                  <node2>a value2</node2>
                </node1>
              </node>
              <node>
                <node1>
                  <node2>
                    <node3>AWESOME</node3>
                  </node2>
                </node1>
              </node>
            </root>".AsSimpleXml();
      }
    }

    public static dynamic SiblingsWithSameNameButOneMissingAttributeOnChildNode
    {
      get
      {
        return
          @"<root>
              <node>
                <node1>
                  <node2 attr='v1'>a value</node2>
                </node1>
              </node>
              <node>
                <node1>
                  <node2 attr='v2'>a value2</node2>
                </node1>
              </node>
              <node>
                <node1>
                  <node2>
                    <node3>AWESOME</node3>
                  </node2>
                </node1>
              </node>
            </root>".AsSimpleXml();
      }
    }
    public static dynamic XmlNs
    {
      get
      {
        return @"<section xmlns='urn:com:blogs-r-us'>
   <title>Blogs</title>
   <signing>
     <author title='Mr' name='Kevin Berridge' />
     <blog title='kwblog' />
   </signing>
</section>".AsSimpleXml();
      }
    }

    public static dynamic NamedXmlNs
    {
      get
      {
        return "<h:section xmlns:h=\"http://www.w3.org/HTML/1998/html4\"><h:title>Blogs</h:title></h:section>".AsSimpleXml();
      }
    }
  }
}
