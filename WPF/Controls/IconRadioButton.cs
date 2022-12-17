using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FontAwesome.WPF;

namespace WPF.Controls;

public class IconRadioButton : RadioButton
{
    private static readonly FontFamily FontAwesomeFontFamily =
        new(new Uri("pack://application:,,,/FontAwesome.WPF;component/"), "./#FontAwesome");

    public static readonly DependencyProperty IconProperty = DependencyProperty.Register(nameof(Icon),
        typeof(FontAwesomeIcon), typeof(IconRadioButton),
        new PropertyMetadata(FontAwesomeIcon.None,
            OnIconPropertyChanged));

    static IconRadioButton()
    {
        DefaultStyleKeyProperty.OverrideMetadata(typeof(IconRadioButton),
            new FrameworkPropertyMetadata(typeof(IconRadioButton)));
    }

    public FontAwesomeIcon Icon
    {
        get => (FontAwesomeIcon)GetValue(IconProperty);
        set => SetValue(ContentProperty, value);
    }

    private static void OnIconPropertyChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e)
    {
        d.SetValue(TextOptions.TextRenderingModeProperty, TextRenderingMode.ClearType);
        // d.SetValue(TextBlock.FontFamilyProperty, FontAwesomeFontFamily);
        // d.SetValue(TextBlock.TextAlignmentProperty, Center);
        d.SetValue(TextBlock.TextProperty, char.ConvertFromUtf32((int)e.NewValue));
    }
}