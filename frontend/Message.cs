/* Copyright 2021-2025 MarcosHCK
 * This file is part of Domino/frontend.
 *
 */

namespace frontend
{
  public enum MessageType
  {
    Question,
    Message,
    Warning,
    Error,
  }

  [GLib.TypeName ("DominoMessage")]
  [Gtk.Template (ResourceName = "message.ui")]
  public sealed class Message : Gtk.Dialog
  {
    [Gtk.Builder.Object]
    private Gtk.Image? image1;
    [Gtk.Builder.Object]
    private Gtk.Label? label1;

    private void Question (string text)
    {
      AddButton ("No", Gtk.ResponseType.No);
      AddButton ("Yes", Gtk.ResponseType.Yes);
      label1!.Text = text;
      image1!.SetFromIconName ("dialog-question-symbolic", Gtk.IconSize.Dialog);
    }

    private void Notification (string text)
    {
      AddButton ("OK", Gtk.ResponseType.Ok);
      label1!.Text = text;
      image1!.SetFromIconName ("dialog-information-symbolic", Gtk.IconSize.Dialog);
    }

    private void Warning (string text)
    {
      AddButton ("OK", Gtk.ResponseType.Ok);
      label1!.Text = text;
      image1!.SetFromIconName ("dialog-warning-symbolic", Gtk.IconSize.Dialog);
    }

    private void Error (string text)
    {
      AddButton ("OK", Gtk.ResponseType.Ok);
      label1!.Text = text;
      image1!.SetFromIconName ("dialog-error-symbolic", Gtk.IconSize.Dialog);
    }

    private void Exception (Exception e)
    {
      AddButton ("OK", Gtk.ResponseType.Ok);
      label1!.Text = e.ToString ();
      image1!.SetFromIconName ("dialog-error-symbolic", Gtk.IconSize.Dialog);
    }

    public Message (string text)
      : this (MessageType.Message, text) { }
    public Message (Exception e)
      : this () => Exception (e);
    public Message (MessageType type, string text)
      : this ()
    {
      switch (type)
      {
        case MessageType.Question:
          Question (text);
          break;
        case MessageType.Message:
          Notification (text);
          break;
        case MessageType.Warning:
          Warning (text);
          break;
        case MessageType.Error:
          Error (text);
          break;
      }
    }

    public Message ()
    {
      Gtk.TemplateBuilder.InitTemplate (this);
    }
  }
}
