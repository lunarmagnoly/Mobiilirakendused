using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp2
{
    public partial class TextPage : ContentPage
    {
        Label lbl;
        Editor editor;
        HorizontalStackLayout hsl;
        List<string> nupud = new List<string>() { "Tagasi", "Avaleht", "Edasi", "Räägi" };
        VerticalStackLayout vsl;

        public TextPage()
        {
            lbl = new Label
            {
                Text = "Pealkiri",
                FontSize = 36,
                FontFamily = "Meie",
                HorizontalOptions = LayoutOptions.Center,
                FontAttributes = FontAttributes.Bold
            };
            editor = new Editor
            {
                Placeholder = "Sisesta tekst...",
                PlaceholderColor = Colors.Red,
                FontSize = 18,
                FontAttributes = FontAttributes.Italic,
                HorizontalOptions = LayoutOptions.Center
            };
            editor.TextChanged += (sender, e) =>
            {
                lbl.Text = editor.Text;
            };
            hsl = new HorizontalStackLayout { Spacing = 8, HorizontalOptions = LayoutOptions.Center };
            for (int j = 0; j < nupud.Count; j++)
            {
                Button nupp = new Button
                {
                    Text = nupud[j],
                    FontSize = 18,
                    FontFamily = "Meie",
                    FontAttributes = FontAttributes.Italic | FontAttributes.Bold,
                    TextColor = Colors.BlueViolet,
                    BackgroundColor = Colors.LightGray,
                    CornerRadius = 10,
                    HeightRequest = 50,
                    ZIndex = j
                };
                hsl.Add(nupp);
                nupp.Clicked += Liikumine;
            }
            vsl = new VerticalStackLayout
            {
                Padding = 20,
                Spacing = 15,
                Children = { lbl, editor, hsl },
                HorizontalOptions = LayoutOptions.Center
            };

            Content = vsl;
        }

        private void Liikumine(object? sender, EventArgs e)
        {
            Button nupp = (Button)sender;
            if (nupp.ZIndex == 0)
            {
                Navigation.PopAsync();
            }
            else if (nupp.ZIndex == 1)
            {
                Navigation.PopToRootAsync();
            }
            else if (nupp.ZIndex == 2)
            {
                Navigation.PushAsync(new FigurePage());
            }
            else if (nupp.ZIndex == 3)
            {
                Raagi(sender, e);
            }
        }

        private async void Raagi(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(editor.Text))
            {
                var locales = await TextToSpeech.Default.GetLocalesAsync();
                var estonian = locales.FirstOrDefault(l => l.Language.StartsWith("et"));

                var options = new SpeechOptions
                {
                    Pitch = 1.0f,
                    Volume = 0.45f,
                    Locale = estonian
                };

                await TextToSpeech.Default.SpeakAsync(editor.Text, options);
            }
            else
            {
                await DisplayAlert("Viga", "Palun sisesta tekst", "OK");
            }
        }
    }
}
