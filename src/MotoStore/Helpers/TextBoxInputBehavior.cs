using Microsoft.Xaml.Behaviors;
using System;
using System.Globalization;
using System.Linq;
using System.Windows.Input;
using System.Windows;
using System.Windows.Controls;
using System.Threading;

namespace MotoStore.Helpers
{
    public class TextBoxInputBehavior : Behavior<TextBox>
    {
        const NumberStyles validDecimalNumberStyles = NumberStyles.AllowDecimalPoint | NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        const NumberStyles validIntegerNumberStyles = NumberStyles.AllowThousands | NumberStyles.AllowLeadingSign;
        public TextBoxInputBehavior()
        {
            InputMode = default;
            JustPositiveDecimalInput = default;
        }

        public TextBoxInputMode InputMode { get; set; }


        public static readonly DependencyProperty JustPositiveDecimalInputProperty =
         DependencyProperty.Register("JustPositiveDecimalInput", typeof(bool),
         typeof(TextBoxInputBehavior), new FrameworkPropertyMetadata(default));

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
                if (!IsValidInput(GetText(" ")))
                {
                    System.Media.SystemSounds.Beep.Play();
                    e.Handled = true;
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
            return InputMode switch
            {
                TextBoxInputMode.None => true,
                TextBoxInputMode.NonSpecialInput => CheckIsNonSpecial(input),
                TextBoxInputMode.DigitInput => CheckIsDigit(input),
                TextBoxInputMode.LetterInput => CheckIsLetter(input),
                TextBoxInputMode.WordsInput => CheckIsWords(input),
                TextBoxInputMode.LetterOrDigitInput => CheckIsLetterOrDigit(input),
                TextBoxInputMode.DecimalInput => CheckIsDecimal(input),
                TextBoxInputMode.IntegerInput => CheckIsInteger(input),
                TextBoxInputMode.DateInput => CheckIsDigitOrSlash(input),
                _ => throw new ArgumentException("Unknown TextBoxInputMode"),
            };
        }

        private static bool CheckIsNonSpecial(string text)
        {
            return text.ToCharArray().All(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c));
        }

        private static bool CheckIsDigit(string text)
        {
            return text.ToCharArray().All(char.IsDigit);
        }

        private static bool CheckIsLetter(string text)
        {
            return text.ToCharArray().All(char.IsLetter);
        }

        private static bool CheckIsWords(string text)
        {
            return text.ToCharArray().All(c => char.IsLetter(c) || char.IsWhiteSpace(c));
        }

        private static bool CheckIsLetterOrDigit(string text)
        {
            return text.ToCharArray().All(char.IsLetterOrDigit);
        }

        private bool CheckIsDecimal(string text)
        {
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

            return string.IsNullOrEmpty(text) || decimal.TryParse(text, validDecimalNumberStyles, CultureInfo.CurrentCulture, out _);
        }

        private bool CheckIsInteger(string text)
        {
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

            return string.IsNullOrEmpty(text) || int.TryParse(text, validIntegerNumberStyles, CultureInfo.CurrentCulture, out _);
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
        NonSpecialInput,
        DecimalInput,
        IntegerInput,
        DigitInput,
        LetterInput,
        WordsInput,
        LetterOrDigitInput,
        DateInput
    }
}
