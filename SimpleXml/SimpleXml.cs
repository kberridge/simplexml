﻿using System;
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
        result = this;

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
        result = element.Attribute(binder.Name).Value;

      return result != null;
    }
  }
}