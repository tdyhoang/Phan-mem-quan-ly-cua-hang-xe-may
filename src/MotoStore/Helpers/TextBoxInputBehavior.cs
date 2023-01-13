using Microsoft.Xaml.Behaviors;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;

namespace MotoStore.Helpers
{
    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        public TextBoxInputBehavior()
        {
            InputMode = TextBoxInputMode.None;
            JustPositiveDecimalInput = false;
        }

        public TextBoxInputMode InputMode { get; set; }


        public static readonly DependencyProperty JustPositiveDecimalInputProperty =
         DependencyProperty.Register("JustPositiveDecimalInput", typeof(bool),
         typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(false));

        public bool JustPositiveDecimalInput
        {
            get { return (bool)GetValue(JustPositiveDecimalInputProperty); }
            set { SetValue(JustPositiveDecimalInputProperty, value); }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.PreviewTextInput += AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown += AssociatedObjectPreviewKeyDown;

            DataObject.AddPastingHandler(AssociatedObject, Pasting);

        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            AssociatedObject.PreviewTextInput -= AssociatedObjectPreviewTextInput;
            AssociatedObject.PreviewKeyDown -= AssociatedObjectPreviewKeyDown;

            DataObject.RemovePastingHandler(AssociatedObject, Pasting);
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
            var txt = AssociatedObject;

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

        private bool IsValidInput(string input)
        {
            switch (InputMode)
            {
                case TextBoxInputMode.None:
                    return true;

                case TextBoxInputMode.DigitInput:
                    return CheckIsDigit(input);

                case TextBoxInputMode.LetterInput:
                    return CheckIsLetter(input);

                case TextBoxInputMode.LetterOrDigitInput:
                    return CheckIsLetterOrDigit(input);

                case TextBoxInputMode.DecimalInput:
                    return CheckIsDecimal(input);

                case TextBoxInputMode.DateInput:
                    return CheckIsDigitOrSlash(input);

                default: throw new ArgumentException("Unknown TextBoxInputMode");

            }
        }

        private static bool CheckIsDigit(string text)
        {
            return text.ToCharArray().All(Char.IsDigit);
        }

        private static bool CheckIsLetter(string text)
        {
            return text.ToCharArray().All(Char.IsLetter);
        }

        private static bool CheckIsLetterOrDigit(string text)
        {
            return text.ToCharArray().All(Char.IsLetterOrDigit);
        }

        private bool CheckIsDecimal(string text)
        {
            // Số thập phân chỉ có 1 dấu chấm
            if (text.ToCharArray().Where(x => x == '.').Count() > 1)
                return false;


            if (text.Contains('-'))
            {
                if (JustPositiveDecimalInput)
                    return false;


                if (text.IndexOf("-", StringComparison.Ordinal) > 0)
                    return false;

                if (text.ToCharArray().Count(x => x == '-') > 1)
                    return false;

                // Ban đầu được phép nhập 1 dấu trừ
                if (text.Length == 1)
                    return true;
            }

            return decimal.TryParse(text, validNumberStyles, CultureInfo.CurrentCulture, out _);
        }

        private static bool CheckIsDigitOrSlash(string text)
        {
            // Định dạng ngày d/M/yyyy, do đó chỉ chấp nhận 2 dấu gạch chéo
            if (text.ToCharArray().Where(x => x == '/').Count() > 2)
                return false;
            return !System.Text.RegularExpressions.Regex.IsMatch(text, "[^0-9/]+");
        }
    }

    public enum TextBoxInputMode
    {
        None,
        DecimalInput,
        DigitInput,
        LetterInput,
        LetterOrDigitInput,
        DateInput
    }
}
