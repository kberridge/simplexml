using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace SimpleXmlNs
{
  public static class Extensions
  {
    public static dynamic AsSimpleXml(this string str)
    {
      return new SimpleXml(new StringReader(str));
    }
  }
}