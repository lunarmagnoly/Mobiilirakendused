using Microsoft.Maui.Controls.Shapes;
using System;

namespace MauiApp2;

public class ValgusfoorPage : ContentPage
{
    // Valgusfoori osad
    BoxView punane;
    BoxView kollane;
    BoxView roheline;
    BoxView post;

    // Lehe taust
    Image taust;

    //Valgusfoori staatus
    bool valgusfoorOn = false;
    bool day = true;

    // Nupud
    Button sisse;
    Button valja;

    //Tekst
    Label lbl;

    // Lehe paigutus
    VerticalStackLayout vst;
    VerticalStackLayout foor;
    Grid hrs;



    public ValgusfoorPage()
    {
        //Taust
        taust = new Image
        {
            Source = "street.png",
            Aspect = Aspect.AspectFill,

        };

        // Valgusfoori tuled
        punane = new BoxView
        {
            Color = Colors.Gray,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 150,
            HeightRequest = 150,
            CornerRadius = 50
        };

        TapGestureRecognizer tapPunane = new TapGestureRecognizer();

        tapPunane.Tapped += (sender, e) =>
        {
            if (valgusfoorOn && day)
                lbl.Text = "Seisa";
        };

        punane.GestureRecognizers.Add(tapPunane);

        kollane = new BoxView
        {
            Color = Colors.Gray,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 150,
            HeightRequest = 150,
            CornerRadius = 50
        };

        TapGestureRecognizer tapKollane = new TapGestureRecognizer();

        tapKollane.Tapped += (sender, e) =>
        {
            if (valgusfoorOn && day)
                lbl.Text = "Valmista";
        };

        kollane.GestureRecognizers.Add(tapKollane);

        roheline = new BoxView
        {
            Color = Colors.Gray,
            BackgroundColor = Colors.Transparent,
            WidthRequest = 150,
            HeightRequest = 150,
            CornerRadius = 50
        };

        TapGestureRecognizer tapRoheline = new TapGestureRecognizer();

        tapRoheline.Tapped += (sender, e) =>
        {
            if (valgusfoorOn && day)
                lbl.Text = "Sõida";
        };

        roheline.GestureRecognizers.Add(tapRoheline);

        // Valgusfoori post
        post = new BoxView
        {
            Color = Colors.DarkGray,
            Margin = new Thickness(0, -12, 0, 0),
            WidthRequest = 50,
            HeightRequest = 100,
            HorizontalOptions = LayoutOptions.Center
        };

        // Nupud
        sisse = new Button
        {
            Text = "Sisse",
            FontSize = 22,
            FontFamily = "SimpleReminder",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.BlueViolet,
            BackgroundColor = Colors.LightGray,
            CornerRadius = 10,
            HeightRequest = 50
        };

        valja = new Button
        {
            Text = "Välja",
            FontSize = 22,
            FontFamily = "SimpleReminder",
            FontAttributes = FontAttributes.Bold,
            TextColor = Colors.BlueViolet,
            BackgroundColor = Colors.LightGray,
            CornerRadius = 10,
            HeightRequest = 50
        };

        sisse.Clicked += Sisse_Clicked;
        valja.Clicked += Valja_Clicked;

        // Tekst
        lbl = new Label
        {
            Text = "Vali valgus",
            FontSize = 22,
            FontFamily = "SimpleReminder",
            FontAttributes = FontAttributes.Bold,
            HorizontalOptions = LayoutOptions.Center
        };

        // Teksti taust
        Border lblBorder = new Border
        {
            BackgroundColor = Color.FromRgba(255, 255, 255, 0.75),
            StrokeThickness = 0,
            Padding = new Thickness(15, 5),
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 15
            },
            Content = lbl,
            HorizontalOptions = LayoutOptions.Center
        };

        // Nuppude paigutus
        hrs = new Grid
        {
            WidthRequest = 370,
            ColumnDefinitions =
            {
                new ColumnDefinition { Width = GridLength.Star },
                new ColumnDefinition { Width = 180 },
                new ColumnDefinition { Width = GridLength.Star }
            }
        };

        hrs.Add(sisse, 0, 0);
        hrs.Add(post, 1, 0);
        hrs.Add(valja, 2, 0);

        // Valgusfoori korpus
        foor = new VerticalStackLayout
        {
            BackgroundColor = Colors.Black,
            Padding = 10,
            Spacing = 10,
            HorizontalOptions = LayoutOptions.Center,
            Children = { punane, kollane, roheline }
        };

        Border foorBorder = new Border
        {
            Margin = new Thickness(0, 10, 0, 0),
            BackgroundColor = Colors.Black,
            HorizontalOptions = LayoutOptions.Center,
            WidthRequest = 170,
            StrokeShape = new RoundRectangle
            {
                CornerRadius = 15
            },
            Content = foor
        };




        // Lehe paigutus
        vst = new VerticalStackLayout
        {
            Spacing = 12,
            Padding = 20,
            HorizontalOptions = LayoutOptions.Center,
            Children = { lblBorder, foorBorder, hrs }
        };

        Grid mainGrid = new Grid();

        // Taust on all, elemendid on tausta peal
        mainGrid.Add(taust);
        mainGrid.Add(vst);

        Content = mainGrid;

        // Käivitab päeva ja öö vahetuse
        _ = DayChange();

    }

    private async Task Day()
    {
        while (valgusfoorOn && day)
        {
            punane.Color = Colors.Red;
            kollane.Color = Colors.Gray;
            roheline.Color = Colors.Gray;

            await Task.Delay(2000);

            if (!valgusfoorOn || !day)
                break;

            punane.Color = Colors.Gray;
            kollane.Color = Colors.Yellow;

            await Task.Delay(2000);

            if (!valgusfoorOn || !day)
                break;

            kollane.Color = Colors.Gray;
            roheline.Color = Colors.Green;

            await Task.Delay(2000);

            if (!valgusfoorOn || !day)
                break;

            roheline.Color = Colors.Gray;
            kollane.Color = Colors.Yellow;

            await Task.Delay(2000);
        }
    }

    private async Task Night()
    {
        while (valgusfoorOn && !day)
        {
            punane.Color = Colors.Gray;
            kollane.Color = Colors.Yellow;
            roheline.Color = Colors.Gray;

            await Task.Delay(2000);

            if (!valgusfoorOn || day)
                break;

            kollane.Color = Colors.Gray;

            await Task.Delay(2000);
        }
    }



    // Lülitab valgusfoori sisse
    private async void Sisse_Clicked(object sender, EventArgs e)
    {
        if (!valgusfoorOn)
        {
            valgusfoorOn = true;

            while (valgusfoorOn)
            {
                // Päev
                if (day)
                {

                    await Day();

                }
                // Öö
                else
                {
                    await Night();
                }              

            }



        }
    }




    // Lülitab valgusfoori välja
    private void Valja_Clicked(object sender, EventArgs e)
    {

        valgusfoorOn = false;

        punane.Color = Colors.Gray;
        kollane.Color = Colors.Gray;
        roheline.Color = Colors.Gray;

        lbl.Text = "Lülita esmalt foor sisse";
    }

    // Vahetab päeva ja öö
    private async Task DayChange()
    {
        while (true)
        {
            day = true;
            taust.Source = "street.png";

            if (valgusfoorOn)
                lbl.Text = "Vali valgus";

            await Task.Delay(30000);

            day = false;
            taust.Source = "night_street.png";

            if (valgusfoorOn)
                lbl.Text = "Öörežiim";

            await Task.Delay(20000);
        }
    }



}