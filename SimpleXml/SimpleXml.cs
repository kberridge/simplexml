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
    XElement element;

    public SimpleXml(Stream stream)
    {
      element = XElement.Load(stream);
    }

    public SimpleXml(TextReader reader)
    {
      element = XElement.Load(reader);
    }

    public SimpleXml(XElement element)
    {
      this.element = element;
    }

    public override bool TryGetMember(GetMemberBinder binder, out object result)
    {
      result = FindFirstPropertyAccess(element, binder.Name);
      if (result != null) return true;

      result = FindSpecialInnerValuePropertyAccess(element, binder.Name);
      if (result != null) return true;

      result = FindChildNode(element, binder.Name);
      if (result != null) return true;

      result = FindAttribute(element, binder.Name);
      if (result != null) return true;

      throw new InvalidOperationException("No node or attribute found: " + binder.Name);
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      if (IsFirstPropertyAccess(element, binder.Name))
      {
        if (element.HasElements)
          throw new InvalidOperationException("Can't set the value of a node which has child nodes");

        element.Value = value as string;
        return true;
      }

      if (IsLeafWithAttributes(element) && IsSpecialInnerValueProperty(binder.Name))
      {
        element.Value = value as string;
        return true;
      }

      var sub = element.Element(binder.Name);
      if (sub != null)
        sub.Value = value as string;
      else
      {
        var attr = element.Attribute(binder.Name);
        if (attr != null)
          attr.Value = value as string;
        else
          throw new InvalidOperationException("No node or attribute found: " + binder.Name);
      }

      return true;
    }

    public string GetXml()
    {
      var sw = new StringWriter();
      element.Save(sw);
      return sw.ToString();
    }

    public void Save(Stream stream, SaveOptions options=SaveOptions.None)
    {
      element.Save(stream, options);
    }

    public void Save(TextWriter writer, SaveOptions options=SaveOptions.None)
    {
      element.Save(writer, options);
    }

    object FindFirstPropertyAccess(XElement element, string propertyName)
    {
      if (!IsFirstPropertyAccess(element, propertyName)) return null;

      if (HasChildValues(element))
       return this;
      else
       return element.Value;
    }

    object FindSpecialInnerValuePropertyAccess(XElement element, string propertyName)
    {
      if (IsLeafWithAttributes(element) && IsSpecialInnerValueProperty(propertyName))
        return element.Value;

      return null;
    }

    object FindChildNode(XElement element, string propertyName)
    {
      var sub = element.Element(propertyName);
      if (sub == null) return null;

      if (HasChildValues(sub))
        return new SimpleXml(sub);
      else
        return sub.Value;
    }

    object FindAttribute(XElement element, string propertyName)
    {
      var attr = element.Attribute(propertyName);
      if (attr == null) return null;

      return attr.Value;
    }

    bool HasChildValues(XElement e)
    {
      return e.HasElements || e.HasAttributes;
    }

    bool IsLeafWithAttributes(XElement e)
    {
      return !e.HasElements && e.HasAttributes;
    }

    bool IsSpecialInnerValueProperty(string binderName)
    {
      return binderName.ToLower() == "text";
    }

    bool IsFirstPropertyAccess(XElement element, string binderName)
    {
      return element.Name.LocalName == binderName;
    }
  }
}