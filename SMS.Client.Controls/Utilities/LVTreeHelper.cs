using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace SMS.Client.Controls
{
    public class LVTreeHelper
    {
        public static T GetVisualChild<T>(object parent) where T : Visual
        {
            DependencyObject dependencyObject = parent as DependencyObject;
            return InternalGetVisualChild<T>(dependencyObject);
        }

        private static T InternalGetVisualChild<T>(DependencyObject parent) where T : Visual
        {
            T child = default(T);

            int numVisuals = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < numVisuals; i++)
            {
                Visual v = (Visual)VisualTreeHelper.GetChild(parent, i);
                child = v as T;
                if (child == null)
                {
                    child = GetVisualChild<T>(v);
                }
                if (child != null)
                {
                    break;
                }
            }
            return child;
        }

        public static T FindFirstVisualChild<T>(DependencyObject parent, string childName) where T : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);

                if (child != null && child is T && child.GetValue(FrameworkElement.NameProperty).ToString() == childName)
                {
                    return (T)child;
                }
                else
                {
                    T childOfChild = FindFirstVisualChild<T>(child, childName);
                    if (childOfChild != null)
                    {
                        return childOfChild;
                    }
                }
            }
            return null;
        }

        public static List<T> FindVisualChildren<T>(DependencyObject parent, string elementName) where T : DependencyObject
        {
            List<T> elementList = new List<T>();
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(parent); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(parent, i);
                if (child == null)
                {
                    continue;
                }

                if (child is T && child.GetValue(FrameworkElement.NameProperty).ToString() == elementName)
                {
                    T element = child as T;
                    elementList.Add(element);
                }

                elementList.AddRange(FindVisualChildren<T>(child, elementName));
            }

            return elementList;
        }

        public static T GetVisualParent<T>(DependencyObject obj, string name = "") where T : FrameworkElement
        {
            DependencyObject parent = VisualTreeHelper.GetParent(obj);

            while (parent != null)
            {
                if (parent is T && (((T)parent).Name == name | string.IsNullOrEmpty(name)))
                {
                    return (T)parent;
                }

                // 在上一级父控件中没有找到指定名字的控件，就再往上一级找
                parent = VisualTreeHelper.GetParent(parent);
            }

            return null;
        }

        public static T GetAncestor<T>(DependencyObject source) where T : DependencyObject
        {
            if (source == null)
                return null;
            do
            {
                source = VisualTreeHelper.GetParent(source);
            } while (source != null && !(source is T));

            return source as T;
        }

        /// <summary>
        /// 返回鼠标点击中的元素的父级元素
        /// </summary>
        /// <typeparam name="T">要获取的祖先的类型。</typeparam>
        /// <param name="source">获取的祖先，如果不存在则为 <c>null</c>。</param>
        /// <returns>获取的祖先对象。</returns>
        public static T GetAncestor<T>(MouseEventArgs e, IInputElement relativeTo) where T : DependencyObject
        {
            Point pos = e.GetPosition(relativeTo);
            HitTestResult hitResult = VisualTreeHelper.HitTest(relativeTo as Visual, pos);
            if (hitResult != null)
            {
                return GetAncestor<T>(hitResult.VisualHit);
            }

            return null;
        }

        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                    return obj as T;

                obj = VisualTreeHelper.GetParent(obj);
            }

            return null;
        }

        /// <summary>
        /// 获取控件的嵌套深度
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="item"></param>
        /// <returns></returns>
        public static int GetDepth<T>(T item) where T : DependencyObject
        {
            int depth = 0;
            while ((item = GetAncestor<T>(item)) != null)
            {
                depth++;
            }
            return depth;
        }

        private static DependencyObject GetParent(DependencyObject element)
        {
            Visual visual = element as Visual;
            DependencyObject parent = (visual == null) ? null : VisualTreeHelper.GetParent(visual);

            if (parent == null)
            {
                // No Visual parent. Check in the logical tree.
                FrameworkElement fe = element as FrameworkElement;

                if (fe != null)
                {
                    parent = fe.Parent;

                    if (parent == null)
                    {
                        parent = fe.TemplatedParent;
                    }
                }
                else
                {
                    FrameworkContentElement fce = element as FrameworkContentElement;

                    if (fce != null)
                    {
                        parent = fce.Parent;

                        if (parent == null)
                        {
                            parent = fce.TemplatedParent;
                        }
                    }
                }
            }

            return parent;
        }

        public static bool IsDescendantOf(DependencyObject element, DependencyObject parent)
        {
            while (element != null)
            {
                if (element == parent)
                    return true;

                element = LVTreeHelper.GetParent(element);
            }

            return false;
        }
    }
}
