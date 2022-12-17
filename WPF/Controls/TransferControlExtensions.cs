using System;
using System.Collections;
using System.Windows;
using System.Windows.Controls;
using HandyControl.Controls;

namespace WPF.Controls;

public class TransferControlExtensions
{
    // Using a DependencyProperty as the backing store for SearchValue.  This enables animation, styling, binding, etc...
    public static readonly DependencyProperty SelectedItemListProperty =
        DependencyProperty.RegisterAttached("SelectedItemList", typeof(IList), typeof(TransferControlExtensions),
            new FrameworkPropertyMetadata(null, OnSelectedItemListChanged));

    public static IList GetSelectedItemList(DependencyObject obj)
    {
        return (IList)obj.GetValue(SelectedItemListProperty);
    }

    public static void SetSelectedItemList(DependencyObject obj, IList value)
    {
        obj.SetValue(SelectedItemListProperty, value);
    }

    private static void OnSelectedItemListChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        if (d is not Transfer transfer) return;
        if (transfer.TransferredItems == null) return;
        transfer.TransferredItems.Clear();
        if (e.NewValue is not IList selectedItems) return;
        foreach (var item in selectedItems)
        {
            transfer.TransferredItems.Add(item);
        }
    }
}