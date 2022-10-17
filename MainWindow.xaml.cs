using GTranslate;
using GTranslate.Translators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Translator.Keys;
using static System.Net.Mime.MediaTypeNames;

namespace Translator
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            HotkeysManager.SetupSystemHook();

            HotkeysManager.AddHotkey(new Translater.Keys.GlobalHotkey(ModifierKeys.Alt, Key.T, () => { Translate(); }));
        }

        private void Start_Translation(object sender, RoutedEventArgs e)
        {
            Translate();
        }


        private async void Translate()
        {
            bool use_clipboard_input = false;
            bool use_clipboard_output = false;
            if ((bool)clipboard_output.IsChecked == true) { use_clipboard_output = true; };
            if ((bool)clipboard_input.IsChecked == true) { use_clipboard_input = true;  };

            string ToTranslate = "error";
            string TranslatedComplete = "error";

            if (use_clipboard_input == true)
            {
                ToTranslate = ClipboardManager.GetText();
                input.Text = ToTranslate;
            }else
            {
                ToTranslate = input.Text;
            }

            var translator = new AggregateTranslator();
            string language = "English";

            switch (translate_mode.Text)
            {
                case "English":
                    {
                        language = "English";
                        break;
                    }
                case "Korean":
                    {
                        language = "Korean";
                        break;
                    }
                case "Japanese":
                    {
                        language = "Japanese";
                        break;
                    }
            }
            
            try
            {
                var result = await translator.TranslateAsync(ToTranslate, language);
                TranslatedComplete = result.Translation;
            }
            catch(Exception Ex)
            {
                Console.WriteLine(Ex);
                TranslatedComplete = "Error";
            }

            if (use_clipboard_output == true)
            {
                ClipboardManager.SetText(TranslatedComplete);
                output.Text = TranslatedComplete;
            }else
            {
                output.Text = TranslatedComplete;
            }
        }

        private void KeyDownHandler(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.RightShift)
            {
                Console.WriteLine("heyo");
            }
        }
    }
}

public static class ClipboardManager
{
    public static void SetText(string p_Text)
    {
        Thread STAThread = new Thread(
            delegate ()
            {
                // Use a fully qualified name for Clipboard otherwise it
                // will end up calling itself.
                Clipboard.SetText(p_Text);
            });
        STAThread.SetApartmentState(ApartmentState.STA);
        STAThread.Start();
        STAThread.Join();
    }
    public static string GetText()
    {
        string ReturnValue = string.Empty;
        Thread STAThread = new Thread(
            delegate ()
            {
                // Use a fully qualified name for Clipboard otherwise it
                // will end up calling itself.
                ReturnValue = Clipboard.GetText();
            });
        STAThread.SetApartmentState(ApartmentState.STA);
        STAThread.Start();
        STAThread.Join();

        return ReturnValue;
    }
}
