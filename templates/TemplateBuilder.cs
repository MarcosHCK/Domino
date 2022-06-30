/* Copyright 2021-2025 MarcosHCK
 * This file is part of Moogle!.
 *
 * Moogle! is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 *
 * Moogle! is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with Moogle!. If not, see <http://www.gnu.org/licenses/>.
 *
 */
using System.Reflection;
using System.Text;

namespace Gtk
{
  public sealed class TemplateBuilder
  {
    public static void InitTemplate (Gtk.Widget widget)
    {
      var type = widget.GetType ();
      var g_type = ((GLib.Object) widget).NativeType;
      var attrtype = typeof (TemplateAttribute);
      var attr = type.GetCustomAttribute (attrtype);
      if (attr != null)
      {
        var template = (TemplateAttribute) attr;
        if (template.ResourceName != TemplateAttribute.INVALID)
        {
          var assembly = type.Assembly;
          var builder = new Gtk.Builder ();
          using (var stream = assembly.GetManifestResourceStream (template.ResourceName))
          {
            if(stream != null)
            {
              var bytes = new byte [stream.Length];
              stream.Read (bytes, 0, bytes.Length);
              var code = Encoding.UTF8.GetString (bytes);
              builder.ExtendWithTemplate (widget, g_type, code);
              builder.Autoconnect (widget);
            }
            else
            {
              throw new TemplateBuilderException ("Non-accessible template resource");
            }
          }
        }
        else
        {
          throw new TemplateBuilderException ("Invalid template resource name");
        }
      }
      else
      {
        throw new TemplateBuilderException ("Type doesn't have '" + attrtype.Name + "' attribute");
      }
    }
  }
}
