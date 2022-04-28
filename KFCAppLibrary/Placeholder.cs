using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace KFCAppLibrary
{
	static class Placeholder
	{
		private static Dictionary<string, string> defaultPlaceholder = new Dictionary<string, string>();
		private static Brush placeholderColor = Brushes.Gray;
		private static Brush textColor = Brushes.Black;
		private static List<UIElement> uIElements = new List<UIElement>();

		public static void SetElement(TextBox element, string uid, string placeholder)
		{
			element.Uid = uid;
			element.Text = placeholder;
			element.Padding = new Thickness(10, 0, 0, 0);
			element.VerticalContentAlignment = VerticalAlignment.Center;
			element.HorizontalContentAlignment = HorizontalAlignment.Left;
			element.FontFamily = new FontFamily("Verdana");
			element.FontSize = 14;
			element.Foreground = placeholderColor;
			element.GotFocus += textBoxFocus;
			element.LostFocus += textBoxLostFocus;

			defaultPlaceholder[uid] = placeholder;
		}

		private static void setTextBoxPlaceholder()
		{
			foreach (var element in uIElements)
			{
				if (element.GetType() == typeof(TextBox))
				{
					TextBox textBox = (TextBox)element;
					textBox.Foreground = placeholderColor;
					textBox.Text = defaultPlaceholder[textBox.Uid];
					textBox.GotFocus += textBoxFocus;
					textBox.LostFocus += textBoxLostFocus;
				}
			}
		}

		private static void textBoxLostFocus(object sender, RoutedEventArgs e)
		{

			TextBox textBox = (TextBox)sender;
			if (textBox != null)
			{

				if (textBox.Text.Length == 0)
				{
					textBox.Text = defaultPlaceholder[textBox.Uid];
				}

				if (textBox.Text == defaultPlaceholder[textBox.Uid])
				{
					textBox.Foreground = placeholderColor;
				}

			}

		}

		private static void textBoxFocus(object sender, RoutedEventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			if (textBox != null)
			{

				if (textBox.Text == defaultPlaceholder[textBox.Uid])
				{
					textBox.Text = "";
					textBox.Foreground = textColor;
				}

			}

		}

	}
}
