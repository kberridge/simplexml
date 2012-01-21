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
      result = null;

      if (element.Name.LocalName == binder.Name) // root
      {
        if (element.HasElements || element.HasAttributes)
          result = this;
        else
          result = element.Value;
      }

      if (!element.HasElements && element.HasAttributes && binder.Name.ToLower() == "text") // leaf with attributes
        result = element.Value;

      if (result != null) return true;

      var sub = element.Element(binder.Name);
      if (sub != null)
      {
        if (sub.HasElements || sub.HasAttributes)
          result = new SimpleXml(sub);
        else if (sub.HasAttributes)
          result = new SimpleXml(sub);
        else
          result = sub.Value;
      }
      else
      {
        var attr = element.Attribute(binder.Name);
        if (attr != null)
          result = attr.Value;
        else
          throw new InvalidOperationException("No node or attribute found: " + binder.Name);
      }

      return result != null;
    }

    public override bool TrySetMember(SetMemberBinder binder, object value)
    {
      if (element.Name.LocalName == binder.Name) // root
      {
        if (element.HasElements)
          throw new InvalidOperationException("Can't set the value of a node which has child nodes");

        element.Value = value as string;
        return true;
      }

      if (!element.HasElements && element.HasAttributes && binder.Name.ToLower() == "text") // leaf with attributes
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
  }
}