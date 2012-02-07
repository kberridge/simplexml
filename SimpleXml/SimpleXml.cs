using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using System.IO;
using System.Dynamic;

namespace SimpleXmlNs
{
  public class SimpleXml : DynamicObject
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
      result = FindFirstPropertyAccess(binder.Name);
      if (result != null) return true;

      result = FindSpecialInnerValuePropertyAccess(binder.Name);
      if (result != null) return true;

      result = FindChildNode(binder.Name);
      if (result != null) return true;

      result = FindAttribute(binder.Name);
      if (result != null) return true;

      throw new NodeOrAttributeNotFoundException(binder.Name);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      bool wasSet = SetFirstProperty(binder.Name, value);
      if (wasSet) return true;

      wasSet = SetBySpecialInnerValueProperty(binder.Name, value);
      if (wasSet) return true;

      wasSet = SetElementValue(binder.Name, value);
      if (wasSet) return true;

      wasSet = SetAttributeValue(binder.Name, value);
      if (wasSet) return true;

      throw new NodeOrAttributeNotFoundException(binder.Name);
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
      var nodes = new List<XElement>();
      foreach (var e in elements)
        nodes.AddRange(e.Elements().Where(n => n.Name.LocalName == propertyName));

      if (nodes.Count == 0)
        return null;
      else if (nodes.All(n => HasChildValues(n)))
        return new SimpleXml(nodes.ToList());
      else if (nodes.All(n => !HasChildValues(n)))
        return nodes.Select(n => n.Value).ToList().ScalarIfSingle();
      else
        throw new InconsistentXmlStructureException(InconsistentMessages.Nodes);
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

    bool SetElementValue(string propertyName, object value)
    {
      var nodes = elements.SelectMany(e => e.Elements().Where(sub => sub.Name.LocalName == propertyName)).ToList();
      if (nodes.Count == 0)
        return false;
      else if (nodes.All(n => HasChildValues(n)) || nodes.All(n => !HasChildValues(n)))
      {
        nodes.ForEach(e => e.Value = value as string);
        return true;
      }
      else
        throw new InconsistentXmlStructureException(InconsistentMessages.Nodes);
    }

    bool SetAttributeValue(string propertyName, object value)
    {
      var attrs = elements.Select(e => e.Attribute(propertyName)).ToList();
      if (attrs.All(a => a == null))
        return false;
      else if (attrs.Any(a => a == null))
        throw new InconsistentXmlStructureException(InconsistentMessages.Attributes);
      else
      {
        attrs.ForEach(a => a.Value = value as string);
        return true;
      }
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