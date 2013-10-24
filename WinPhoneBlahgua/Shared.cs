﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Media;


namespace WinPhoneBlahgua
{
    public static class VisualTreeEnumeration
    {
        public static IEnumerable<DependencyObject> Descendents(this DependencyObject root)
        {
            int count = VisualTreeHelper.GetChildrenCount(root);
            for (int i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(root, i);
                yield return child;
                foreach (var descendent in Descendents(child))
                    yield return descendent;
            }
        }
    }

}
