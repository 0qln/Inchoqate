using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Inchoqate.GUI
{
    public static class Utils
    {
        public static double Lerp(double min, double max, double t)
        {
            return min + (max - min) * t;
        }

        public static TTarget FindVisualChildOfType<TTarget>(DependencyObject reference)
            where TTarget : DependencyObject
        {
            DependencyObject current = reference;
            while (current is not TTarget)
            {
                current = VisualTreeHelper.GetChild(current, 0);
            }
            return (TTarget)current;
        }

        public static T? FirstOrDefault<T>(this IEnumerable enumerable, Predicate<T> predicate)
        {
            foreach (var item in enumerable)
            {
                if (item is T t && predicate(t))
                {
                    return t;
                }
            }

            return default;
        }
    }
}
