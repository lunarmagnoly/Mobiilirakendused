using System;
using System.Collections.Generic;
using System.Text;

namespace MauiApp2;

public partial class StartPage : ContentPage
{
    VerticalStackLayout vst;
    ScrollView sv;
    public List<ContentPage> Lehed = new List<ContentPage>() { new TextPage(), new FigurePage(), new ValgusfoorPage() };
    public List<string> LeheNimed = new List<string>() { "Tekst", "Kujund", "Valgusfoor" };
    public StartPage()
    {
        //Title = "Avaleht";
        vst = new VerticalStackLayout { Padding = 20, Spacing = 15 };
        for (int i = 0; i < Lehed.Count; i++)
        {
            Button nupp = new Button
            {
                Text = LeheNimed[i],
                FontSize = 36,
                FontFamily = "StylishCalligraphy",
                FontAttributes = FontAttributes.Bold,
                BackgroundColor = Colors.LightGray,
                TextColor = Colors.Black,
                CornerRadius = 10,
                HeightRequest = 80,
                ZIndex = i
            };
            vst.Add(nupp);
            nupp.Clicked += (sender, e) =>
            {
                var valik = Lehed[nupp.ZIndex];
                Navigation.PushAsync(valik);
            };
        }
        sv = new ScrollView { Content = vst };
        Content = sv;

    }
}
