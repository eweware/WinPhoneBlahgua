using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.ComponentModel;


namespace BibleBlahgua
{
    public class UrlTextBlock : RichTextBox
    {
        // Copied from http://geekswithblogs.net/casualjim/archive/2005/12/01/61722.aspx
        private static readonly Regex UrlRegex = new Regex(@"(?#Protocol)(?:(?:ht|f)tp(?:s?)\:\/\/|~/|/)?(?#Username:Password)(?:\w+:\w+@)?(?#Subdomains)(?:(?:[-\w]+\.)+(?#TopLevel Domains)(?:com|org|net|gov|mil|biz|info|mobi|name|aero|jobs|museum|travel|[a-z]{2}))(?#Port)(?::[\d]{1,5})?(?#Directories)(?:(?:(?:/(?:[-\w~!$+|.,=]|%[a-f\d]{2})+)+|/)+|\?|#)?(?#Query)(?:(?:\?(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)(?:&(?:[-\w~!$+|.,*:]|%[a-f\d{2}])+=(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)*)*(?#Anchor)(?:#(?:[-\w~!$+|.,*:=]|%[a-f\d]{2})*)?");
 
        public static readonly DependencyProperty ContentProperty = DependencyProperty.RegisterAttached(
            "Content",
            typeof(string),
            typeof(UrlTextBlock),
            new PropertyMetadata(null, OnContentChanged)
        );
 
        public string Content
        {
            get
            {
                return (string)GetValue(ContentProperty);
            }
            set
            {
                SetValue(ContentProperty, value);
            }
        }
 
        private static void OnContentChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UrlTextBlock textBox = d as UrlTextBlock;
            if (textBox == null)
                return;
 
            textBox.Blocks.Clear();
 
            string newText = (string)e.NewValue;
            if (String.IsNullOrEmpty(newText))
                return;
 
            Paragraph paragraph = new Paragraph();
            try
            {
                int lastPos = 0;
                foreach (Match match in UrlRegex.Matches(newText))
                {
                    // Copy raw string from the last position up to the match
                    if (match.Index != lastPos)
                    {
                        string rawText = newText.Substring(lastPos, match.Index - lastPos);
                        paragraph.Inlines.Add(rawText);
                    }
 
                    // Add matched url
                    string rawUrl = match.Value;
                    Uri uri = null;
                    if (!Uri.TryCreate(rawUrl, UriKind.Absolute, out uri))
                    {
                        // Attempt to craft a valid url
                        if (!rawUrl.StartsWith("http://"))
                        {
                            Uri.TryCreate("http://" + rawUrl, UriKind.Absolute, out uri);
                        }
                    }
                    if (uri != null)
                    {
                        Hyperlink link = new Hyperlink()
                        {
                            NavigateUri = uri,
                            TargetName = "_blank",
                        };
                        link.Inlines.Add(rawUrl);
                        paragraph.Inlines.Add(link);
                    }
                    else
                    {
                        paragraph.Inlines.Add(rawUrl);
                    }
 
                    // Update the last matched position
                    lastPos = match.Index + match.Length;
                }
 
                // Finally, copy the remainder of the string
                if (lastPos < newText.Length)
                    paragraph.Inlines.Add(newText.Substring(lastPos));
            }
            catch (Exception)
            {
                paragraph.Inlines.Clear();
                paragraph.Inlines.Add(newText);
            }
 
            // Add the paragraph to the RichTextBox.
            textBox.Blocks.Add(paragraph);
        }
    }
}
