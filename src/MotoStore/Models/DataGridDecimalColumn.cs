using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace MotoStore.Models
{
    internal class DataGridDecimalColumn : DataGridTextColumn
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        TextBox? edittingCell;

        protected override object PrepareCellForEdit(FrameworkElement editingElement, RoutedEventArgs editingEventArgs)
        {
            edittingCell = editingElement as TextBox;
            edittingCell.PreviewTextInput += AssociatedObjectPreviewTextInput;
            DataObject.AddPastingHandler(edittingCell, Pasting);
            return base.PrepareCellForEdit(editingElement, editingEventArgs);
        }

        private void Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                var pastedText = (string)e.DataObject.GetData(typeof(string));

                if (!IsValidInput(GetText(pastedText)))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.CancelCommand();
                }
            }
            else
            {
                System.Media.SystemSounds.Beep.Play();
                e.CancelCommand();
            }
        }

        private void AssociatedObjectPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                if (!IsValidInput(GetText(" ")))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
                }
            }
        }

        private void AssociatedObjectPreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (!IsValidInput(GetText(e.Text)))
            {
                System.Media.SystemSounds.Beep.Play();
                e.Handled = true;
            }
        }

        private string GetText(string input)
        {
            var txt = edittingCell;

            int selectionStart = txt.SelectionStart;
            if (txt.Text.Length < selectionStart)
                selectionStart = txt.Text.Length;

            int selectionLength = txt.SelectionLength;
            if (txt.Text.Length < selectionStart + selectionLength)
                selectionLength = txt.Text.Length - selectionStart;

            var realtext = txt.Text.Remove(selectionStart, selectionLength);

            int caretIndex = txt.CaretIndex;
            if (realtext.Length < caretIndex)
                caretIndex = realtext.Length;

            var newtext = realtext.Insert(caretIndex, input);

            return newtext;
        }

        private static bool IsValidInput(string input)
        {
            // Số thập phân chỉ có 1 dấu chấm
            if (input.ToCharArray().Where(x => x == '.').Count() > 1)
                return false;
            return decimal.TryParse(input, validNumberStyles, CultureInfo.CurrentCulture, out _);
        }
    }
}
