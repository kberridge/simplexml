using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Dynamic;
using System.Xml;
using System.Collections;

namespace SimpleXmlNs
{
  public class SimpleXml : DynamicObject, IEnumerable
  {
    List<XElement> elements;

    public SimpleXml(Stream stream)
    {
      elements = new List<XElement> { XElement.Load(stream) };
    }

    public SimpleXml(TextReader reader)
    {
      elements = new List<XElement> { XElement.Load(reader) };
    }

    public SimpleXml(XmlTextReader xmlReader)
    {
      elements = new List<XElement> { XElement.Load(xmlReader) };
    }

    public SimpleXml(XElement element)
    {
      this.elements = new List<XElement> { element };
    }

    private SimpleXml(IEnumerable<XElement> elements)
    {
      this.elements = elements.ToList();
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      var elementName = binder.Name;
      result = FindFirstPropertyAccess(elementName);
      if (result != null) return true;

      result = FindSpecialInnerValuePropertyAccess(elementName);
      if (result != null) return true;

      result = FindChildNode(elementName);
      if (result != null) return true;

      result = FindAttribute(elementName);
      if (result != null) return true;

      throw new NodeOrAttributeNotFoundException(elementName);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      var elementName = binder.Name;
      bool wasSet = SetFirstProperty(elementName, value);
      if (wasSet) return true;

      wasSet = SetBySpecialInnerValueProperty(elementName, value);
      if (wasSet) return true;

      wasSet = SetChildNodeValue(elementName, value);
      if (wasSet) return true;

      wasSet = SetAttributeValue(elementName, value);
      if (wasSet) return true;

      throw new NodeOrAttributeNotFoundException(elementName);
    }

    public string GetXml()
    {
      var sw = new StringWriter();
      elements[0].Save(sw);
      return sw.ToString();
    }

    public void Save(Stream stream, SaveOptions options=SaveOptions.None)
    {
      elements[0].Save(stream, options);
    }

    public void Save(TextWriter writer, SaveOptions options=SaveOptions.None)
    {
      elements[0].Save(writer, options);
    }
    
    public dynamic First(string attributeName, string value)
    {
      var match = elements.First(e => e.Attributes().Where(a => a.Name.LocalName == attributeName && a.Value == value).Count() > 0);
      if (match.HasElements) return new SimpleXml(match);
      else return match.Value;
    }

    public dynamic FirstOrDefault(string attributeName, string value)
    {
      var match = elements.FirstOrDefault(e => e.Attributes().Where(a => a.Name.LocalName == attributeName && a.Value == value).Count() > 0);
      if (match == null) return null;
      if (match.HasElements) return new SimpleXml(match);
      else return match.Value;
    }

    public dynamic Where(string attributeName, string value)
    {
      var matches = elements.Where(e => e.Attributes().Where(a => a.Name.LocalName == attributeName && a.Value == value).Count() > 0);
      if (matches.All(n => n.HasElements))
        return new SimpleXml(matches);
      else
        return matches.Select(n => n.Value).ToList();
    }

    /// <summary>
    /// The first property access is a special case because the XElement is already at the root level, 
    /// but SimpleXml requires you to explicitly dot into the root.
    /// </summary>
    object FindFirstPropertyAccess(string propertyName)
    {
      if (elements.Count != 1) return null;
      var element = elements[0];
      if (!IsFirstPropertyAccess(element, propertyName)) return null;

      if (HasChildValues(element))
       return this;
      else
       return element.Value;
    }

    object FindSpecialInnerValuePropertyAccess(string propertyName)
    {
      if (!IsSpecialInnerValueProperty(propertyName)) return null;

      var l = elements.Where(e => IsLeafWithAttributes(e)).Select(e => e.Value).ToList();
      return l.Count == 0 ? null : l.ScalarIfSingle();
    }

    object FindChildNode(string propertyName)
    {
      var nodes = elements.SelectMany(e => e.Elements().Where(n => n.Name.LocalName == propertyName)).ToList();

      if (nodes.Count == 0)
        return null;
      if (nodes.Any(n => HasChildValues(n)) && nodes.Any(n => !HasChildValues(n)))
        throw new InconsistentXmlStructureException(InconsistentMessages.Nodes);

      if (HasChildValues(nodes[0]))
        return new SimpleXml(nodes);
      else
        return nodes.Select(n => n.Value).ToList().ScalarIfSingle();
    }

    object FindAttribute(string propertyName)
    {
      var attrs = elements.Select(e => e.Attribute(propertyName)).ToList();

      if (attrs.All(a => a == null))
        return null;
      if (attrs.Any(a => a == null))
        throw new InconsistentXmlStructureException(InconsistentMessages.Attributes);

      return attrs.Select(a => a.Value).ToList().ScalarIfSingle();
    }

    /// <summary>
    /// The first property access is a special case because the XElement is already at the root level, 
    /// but SimpleXml requires you to explicitly dot into the root.
    /// </summary>
    bool SetFirstProperty(string propertyName, object value)
    {
      if (elements.Count != 1) return false;
      var element = elements[0];
      if (!IsFirstPropertyAccess(element, propertyName)) return false;

      if (element.HasElements)
        throw new InvalidOperationException("Can't set the value of a node which has child nodes");

      element.Value = value as string;
      return true;
    }

    bool SetBySpecialInnerValueProperty(string propertyName, object value)
    {
      if (!IsSpecialInnerValueProperty(propertyName)) return false;

      var l = elements.Where(e => IsLeafWithAttributes(e)).ToList();
      if (l.Count == 0) return false;
      l.ForEach(e => e.Value = value as string);
      return true;
    }

    bool SetChildNodeValue(string propertyName, object value)
    {
      var nodes = elements.SelectMany(e => e.Elements().Where(sub => sub.Name.LocalName == propertyName)).ToList();

      if (nodes.Count == 0)
        return false;
      if (nodes.Any(n => HasChildValues(n)) && nodes.Any(n => !HasChildValues(n)))
        throw new InconsistentXmlStructureException(InconsistentMessages.Nodes);

      nodes.ForEach(e => e.Value = value as string);
      return true;
    }

    bool SetAttributeValue(string propertyName, object value)
    {
      var attrs = elements.Select(e => e.Attribute(propertyName)).ToList();

      if (attrs.All(a => a == null))
        return false;
      if (attrs.Any(a => a == null))
        throw new InconsistentXmlStructureException(InconsistentMessages.Attributes);

      attrs.ForEach(a => a.Value = value as string);
      return true;
    }

    bool HasChildValues(XElement e)
    {
      return e.HasElements || e.HasAttributes;
    }

    bool IsLeafWithAttributes(XElement e)
    {
      return !e.HasElements && e.HasAttributes;
    }

    bool IsSpecialInnerValueProperty(string propertyName)
    {
      return propertyName.ToLower() == "text";
    }

    bool IsFirstPropertyAccess(XElement element, string propertyName)
    {
      return element.Name.LocalName == propertyName;
    }

    public IEnumerator GetEnumerator()
    {
      return elements.Select(e => new SimpleXml(e)).GetEnumerator();
    }
  }

  static class EnumerableExtensions
  {
    public static object ScalarIfSingle<T>(this IEnumerable<T> list)
    {
      int count = list.Count();
      if (count == 0)
        return null;
      else if (count == 1)
        return list.ElementAt(0);
      else
        return list;
    }
  }

  static class InconsistentMessages
  {
    public static string Nodes 
    { 
      get 
      { 
        return "The structure of your xml was inconsistent, some matching nodes had child values but others did not"; 
      } 
    }
    public static string Attributes 
    { 
      get 
      { 
        return "The structure of your xml was inconsistent, some nodes had matching attributes but others did not"; 
      }
    }
  }

  public class InconsistentXmlStructureException : Exception
  {
    public InconsistentXmlStructureException(string message) : base(message) { }
  }

  public class NodeOrAttributeNotFoundException : Exception
  {
    public NodeOrAttributeNotFoundException(string elementName) : base("No node or attribute found: " + elementName) { }
  }
}