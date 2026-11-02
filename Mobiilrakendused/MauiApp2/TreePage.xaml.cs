using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Controls.Shapes;
using Microsoft.Maui.Graphics;

namespace MauiApp2;

public partial class TreePage : ContentPage
{
    // Puu võimalikud tegevused
    private enum TreeAction { Kasva, Oitse, Varise, Langeta }

    // Joonistatud puu osad
    private readonly BoxView treeTrunk;
    private readonly BoxView treeBranch;
    private readonly Polygon treeLeaves;
    private readonly AbsoluteLayout treeBody;
    private readonly AbsoluteLayout flowersLayer;

    // Maastik
    private readonly BoxView sky;
    private readonly BoxView soil;
    private readonly BoxView seasonalSurface;
    private readonly Ellipse sun;
    private readonly Ellipse moon;

    // Juhtnupud
    private readonly Picker actionPicker;
    private readonly DatePicker datePicker;
    private readonly TimePicker timePicker;
    private readonly Slider crownSlider;
    private readonly Stepper animationStepper;
    private readonly Label durationLabel;
    private readonly Label statusLabel;
    private readonly Button startButton;

    private bool isGrown;
    private bool isBlooming;
    private bool isFelled;
    private bool isBusy;

    // Puu mõõdud: jalam on (150, 310), täiskasvanud tüve tipp (150, 165).
    private const double TrunkBaseY = 310;
    private const double SmallTrunkHeight = 45;
    private const double AdultTrunkHeight = 145;
    private const double CrownCenterX = 110;
    private const double CrownCenterY = 75;
    private const int CrownPointCount = 48;

    private static readonly Color LeafGreen = Color.FromArgb("#83C879");
    private static readonly Color SakuraPink = Color.FromArgb("#F4A4CF");

    public TreePage()
    {
        Title = "Sakura";
        BackgroundColor = Color.FromArgb("#FFF8F8");

        // Taevas ja maapind asuvad AINULT puu stseeni taga.
        sky = new BoxView { Color = Colors.SkyBlue };
        soil = new BoxView
        {
            Color = Color.FromArgb("#30251C"),
            HeightRequest = 45,
            VerticalOptions = LayoutOptions.End
        };
        seasonalSurface = new BoxView
        {
            Color = Color.FromArgb("#B78F53"),
            HeightRequest = 10,
            VerticalOptions = LayoutOptions.End,
            Margin = new Thickness(0, 0, 0, 35)
        };

        sun = new Ellipse
        {
            Fill = Colors.Gold,
            WidthRequest = 52,
            HeightRequest = 52,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 32, 35, 0)
        };
        moon = new Ellipse
        {
            Fill = Color.FromArgb("#F4F1D9"),
            WidthRequest = 42,
            HeightRequest = 42,
            HorizontalOptions = LayoutOptions.End,
            VerticalOptions = LayoutOptions.Start,
            Margin = new Thickness(0, 35, 42, 0),
            IsVisible = false
        };

        // Kõik puu liikuvad osad on samas paigutuses.
        // Kui puud langetatakse, pööratakse tervet treeBody elementi.
        treeBody = new AbsoluteLayout
        {
            WidthRequest = 300,
            HeightRequest = 360,
            AnchorX = 0.5,
            AnchorY = TrunkBaseY / 360
        };

        // Tüvi kasvab alumisest otsast ülespoole.
        treeTrunk = new BoxView
        {
            Color = Color.FromArgb("#86502E"),
            AnchorX = 0.5,
            AnchorY = 1,
            ScaleY = SmallTrunkHeight / AdultTrunkHeight,
            ScaleX = 8.0 / 14.0
        };
        AddAt(treeBody, treeTrunk, 143, 165, 14, AdultTrunkHeight);

        // Sama hulknurk on alguses leht ja kasvades muutub laiaks võraks.
        treeLeaves = new Polygon
        {
            Fill = LeafGreen,
            Points = MakeCrownPoints(0)
        };
        

        // Oks jääb tüve tipu lähedale ja joonistatakse võra peale.
        // Oks jääb võra taha
        treeBranch = new BoxView
        {
            Color = Color.FromArgb("#86502E"),
            AnchorY = 1,
            Rotation = 35,
            ScaleY = 0.8
        };

        // Oks algab tüve ülemise osa juurest
        AddAt(treeBody, treeBranch, 147, 230, 5, 35);

        // Võra katab oksa
        AddAt(treeBody, treeLeaves, 40, 190, 220, 150);

        // Õied on omaette kihis, kuid liiguvad koos terve puuga.
        flowersLayer = new AbsoluteLayout
        {
            IsVisible = false,
            Opacity = 0,
            InputTransparent = true
        };
        AddAt(treeBody, flowersLayer, 0, 0, 300, 360);
        CreateFlowers();

        // Kasvupinnas katab tüve alumist otsa, kuid ei pöördu koos puuga.
        Polygon mound = new Polygon
        {
            Fill = Color.FromArgb("#995735"),
            Points = new PointCollection
            {
                new Point(0, 30), new Point(20, 19),
                new Point(45, 9), new Point(70, 3),
                new Point(95, 9), new Point(120, 19),
                new Point(140, 30)
            }
        };
        AbsoluteLayout moundLayer = new AbsoluteLayout
        {
            WidthRequest = 300,
            HeightRequest = 360,
            InputTransparent = true
        };
        AddAt(moundLayer, mound, 80, 290, 140, 30);

        Grid treeStage = new Grid
        {
            WidthRequest = 300,
            HeightRequest = 360,
            HorizontalOptions = LayoutOptions.Center
        };
        treeStage.Children.Add(treeBody);
        treeStage.Children.Add(moundLayer);

        Grid scene = new Grid { HeightRequest = 360 };
        scene.Children.Add(sky);
        scene.Children.Add(soil);
        scene.Children.Add(seasonalSurface);
        scene.Children.Add(sun);
        scene.Children.Add(moon);
        scene.Children.Add(treeStage);

        // Tegevuste ja animatsiooni valimine
        actionPicker = new Picker
        {
            Title = "Vali tegevus",
            ItemsSource = new List<string> { "Kasva", "Õitse", "Värise", "Langeta" },
            SelectedIndex = 0,
            TextColor = Color.FromArgb("#24232C"),
            TitleColor = Color.FromArgb("#65616A")
        };

        startButton = new Button
        {
            Text = "Käivita",
            FontSize = 20,
            BackgroundColor = Color.FromArgb("#F6B7CD"),
            TextColor = Colors.Black,
            CornerRadius = 12
        };
        startButton.Clicked += StartButton_Clicked;

        // Puu algoleku taastamise nupp
        Button resetButton = new Button
        {
            Text = "Lähtesta puu",
            FontSize = 18,
            BackgroundColor = Color.FromArgb("#E2DFE4"),
            TextColor = Colors.Black,
            CornerRadius = 12
        };

        // Taastab puu algse oleku
        resetButton.Clicked += (_, _) =>
        {
            if (isBusy)
                return;

            ResetTree();
            statusLabel.Text = "Puu on lähtestatud!";
        };
        

        statusLabel = new Label
        {
            Text = "Vali tegevus",
            FontSize = 17,
            TextColor = Color.FromArgb("#35323A"),
            HorizontalTextAlignment = TextAlignment.Center,
            HorizontalOptions = LayoutOptions.Fill
        };

        datePicker = new DatePicker
        {
            Date = DateTime.Today,
            Format = "dd.MM.yyyy",
            TextColor = Color.FromArgb("#24232C")
        };
        timePicker = new TimePicker
        {
            Time = DateTime.Now.TimeOfDay,
            Format = "HH:mm",
            TextColor = Color.FromArgb("#24232C")
        };
        datePicker.DateSelected += (_, _) => UpdateLandscape();
        timePicker.TimeSelected += (_, _) => UpdateLandscape();

        crownSlider = new Slider { Minimum = 0, Maximum = 1, Value = 1 };
        crownSlider.ValueChanged += (_, e) =>
        {
            treeLeaves.Opacity = e.NewValue;
            if (isBlooming)
                flowersLayer.Opacity = e.NewValue;
        };

        animationStepper = new Stepper
        {
            Minimum = 500,
            Maximum = 2000,
            Increment = 100,
            Value = 1000
        };
        durationLabel = new Label
        {
            Text = "Animatsiooni kestus: 1000 ms",
            FontSize = 15,
            TextColor = Color.FromArgb("#35323A")
        };
        animationStepper.ValueChanged += (_, e) =>
            durationLabel.Text = $"Animatsiooni kestus: {e.NewValue:0} ms";

        // Juhtnupud ei ole enam tumeda mullapinna peal.
        VerticalStackLayout controls = new VerticalStackLayout
        {
            Padding = new Thickness(18, 16),
            Spacing = 10,
            BackgroundColor = Color.FromArgb("#FFF8F8")
        };
        controls.Children.Add(actionPicker);
        controls.Children.Add(startButton);
        controls.Children.Add(resetButton);
        controls.Children.Add(statusLabel);
        controls.Children.Add(new Label { Text = "Kuupäev ja kellaaeg", FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#35323A") });
        controls.Children.Add(datePicker);
        controls.Children.Add(timePicker);
        controls.Children.Add(new Label { Text = "Võra läbipaistvus", FontAttributes = FontAttributes.Bold,
            TextColor = Color.FromArgb("#35323A") });
        controls.Children.Add(crownSlider);
        controls.Children.Add(durationLabel);
        controls.Children.Add(animationStepper);

        VerticalStackLayout pageLayout = new VerticalStackLayout { Spacing = 0 };
        pageLayout.Children.Add(scene);
        pageLayout.Children.Add(controls);

        Content = new ScrollView { Content = pageLayout };
        UpdateLandscape();
    }

    // Paigutab elemendi fikseeritud koordinaatidega.
    private static void AddAt(AbsoluteLayout parent, View view,
        double x, double y, double width, double height)
    {
        AbsoluteLayout.SetLayoutBounds(view, new Rect(x, y, width, height));
        parent.Children.Add(view);
    }

    // Alguses väike leht, lõpus lai ja lainelise servaga võra.
    // Mõlema kuju keskpunkt on sama, seega ei teki "pitsalõiku".
    private static PointCollection MakeCrownPoints(double progress)
    {
        PointCollection result = new PointCollection();

        for (int i = 0; i < CrownPointCount; i++)
        {
            double angle = 2 * Math.PI * i / CrownPointCount;
            double cosine = Math.Cos(angle);
            double sine = Math.Sin(angle);

            // Väikese lehe veidi viltune ovaalne kuju.
            double leafX = CrownCenterX + 10 * cosine + 3 * sine;
            double leafY = CrownCenterY + 16 * sine;

            // Täiskasvanud puu lai võra, pehmed ebakorrapärased sopistused.
            double waves = 1.0
                + 0.065 * Math.Sin(7 * angle + 0.6)
                + 0.035 * Math.Cos(11 * angle - 0.3)
                + 0.015 * Math.Sin(17 * angle);
            double crownX = CrownCenterX + 101 * cosine * waves;
            double crownY = CrownCenterY + 59 * sine * waves;

            result.Add(new Point(
                leafX + (crownX - leafX) * progress,
                leafY + (crownY - leafY) * progress));
        }

        return result;
    }

    // Üks õis koosneb viiest õielehest ja kollasest südamikust.
    private static AbsoluteLayout CreateFlower()
    {
        AbsoluteLayout flower = new AbsoluteLayout { WidthRequest = 22, HeightRequest = 22 };
        (double x, double y)[] positions =
        {
            (7, 0), (13, 6), (11, 13), (3, 13), (1, 6)
        };

        foreach (var (x, y) in positions)
        {
            Ellipse petal = new Ellipse { Fill = Color.FromArgb("#FFE0EE") };
            AddAt(flower, petal, x, y, 9, 10);
        }

        Ellipse center = new Ellipse { Fill = Color.FromArgb("#EFC95B") };
        AddAt(flower, center, 8.5, 8, 5, 5);
        return flower;
    }

    private void CreateFlowers()
    {
        // Kõik õied mahuvad täiskasvanud võra piiridesse.
        (double x, double y)[] positions =
        {
            (-72, -7), (-52, -33), (-22, -44), (11, -45), (45, -32),
            (72, -8), (-79, 17), (-49, 27), (-17, 9), (22, 23),
            (52, 21), (0, -20), (34, -6), (-39, -8), (0, 38)
        };

        foreach (var (x, y) in positions)
        {
            AbsoluteLayout flower = CreateFlower();
            AddAt(flowersLayer, flower, 150 + x - 11, 165 + y - 11, 22, 22);
        }
    }

    private void UpdateLandscape()
    {
        // .NET 10 lubab kuupäeva ja kellaaja tühjaks jätta.
        int month = (datePicker.Date ?? DateTime.Today).Month;
        TimeSpan chosenTime = timePicker.Time ?? DateTime.Now.TimeOfDay;
        double hour = chosenTime.TotalHours;

        bool winter = month == 12 || month == 1 || month == 2;
        bool spring = month >= 3 && month <= 5;
        bool summer = month >= 6 && month <= 8;
        bool night = hour < 6 || hour >= 21;
        bool sunrise = hour >= 6 && hour < 9;
        bool sunset = hour >= 18 && hour < 21;

        if (night)
            sky.Color = Color.FromArgb("#192947");
        else if (sunrise)
            sky.Color = Color.FromArgb("#F4B7C7");
        else if (sunset)
            sky.Color = Color.FromArgb("#ED9AB7");
        else
            sky.Color = winter ? Color.FromArgb("#BBD8ED") : Color.FromArgb("#8BCFED");

        // Must muld jääb alles, muutub ainult pealmine kiht.
        seasonalSurface.Color = winter ? Color.FromArgb("#F4F6FA")
            : spring ? Color.FromArgb("#83C979")
            : summer ? Color.FromArgb("#3D9A58")
            : Color.FromArgb("#C4A05A");

        sun.IsVisible = !night;
        moon.IsVisible = night;
    }

    private void ResetTree()
    {
        treeBody.Rotation = 0;
        treeBody.TranslationX = 0;
        treeTrunk.ScaleY = SmallTrunkHeight / AdultTrunkHeight;
        treeTrunk.ScaleX = 8.0 / 14.0;
        treeLeaves.Points = MakeCrownPoints(0);
        treeLeaves.TranslationY = 0;
        treeLeaves.Opacity = crownSlider.Value;
        treeLeaves.Fill = LeafGreen;
        treeBranch.TranslationY = 0;
        treeBranch.ScaleY = 0.8;
        flowersLayer.IsVisible = false;
        flowersLayer.Opacity = 0;
        isGrown = false;
        isBlooming = false;
        isFelled = false;
    }

    // Animation.Commit ei tagasta Task-i; teeme ootamise ise.
    private Task RunAnimationAsync(string name, Action<double> update, Easing easing)
    {
        TaskCompletionSource<bool> completion = new TaskCompletionSource<bool>(
            TaskCreationOptions.RunContinuationsAsynchronously);

        Animation animation = new Animation(update, 0, 1);
        animation.Commit(
            this,
            name,
            16,
            (uint)animationStepper.Value,
            easing,
            (_, cancelled) => completion.TrySetResult(!cancelled));

        return completion.Task;
    }

    private async Task GrowAsync()
    {
        if (isGrown && !isFelled)
        {
            statusLabel.Text = "Puu on juba kasvanud!";
            return;
        }

        ResetTree();
        statusLabel.Text = "Puu kasvab!";

        await RunAnimationAsync("TreeGrow", progress =>
        {
            // Tüve alumine ots püsib mullas, ülemine kasvab ülespoole.
            double height = SmallTrunkHeight
                + (AdultTrunkHeight - SmallTrunkHeight) * progress;
            treeTrunk.ScaleY = height / AdultTrunkHeight;
            treeTrunk.ScaleX = (8 + 6 * progress) / 14;

            // Lehe keskpunkt liigub täpselt koos tüve tipuga.
            treeLeaves.TranslationY = -(height - SmallTrunkHeight);
            treeLeaves.Points = MakeCrownPoints(progress);

            // Oks liigub koos tüve tipuga.
            treeBranch.TranslationY = -(height - SmallTrunkHeight);
            treeBranch.ScaleY = 0.8 + 0.45 * progress;
        }, Easing.CubicInOut);

        isGrown = true;
        statusLabel.Text = "Puu on kasvanud!";
    }

    private async Task BloomAsync()
    {
        if (isFelled)
        {
            statusLabel.Text = "Langetatud puu ei saa õitseda.";
            return;
        }
        if (!isGrown)
        {
            statusLabel.Text = "Kõigepealt kasvata puu!";
            return;
        }
        if (isBlooming)
        {
            statusLabel.Text = "Sakura juba õitseb!";
            return;
        }

        statusLabel.Text = "Sakura õitseb!";
        flowersLayer.IsVisible = true;
        flowersLayer.Opacity = 0;

        await RunAnimationAsync("TreeBloom", progress =>
        {
            // Rohelisest võrast saab õrnroosa õitsev sakura.
            byte r = (byte)(131 + (244 - 131) * progress);
            byte g = (byte)(200 + (164 - 200) * progress);
            byte b = (byte)(121 + (207 - 121) * progress);
            treeLeaves.Fill = Color.FromRgb(r, g, b);
            flowersLayer.Opacity = progress * crownSlider.Value;
        }, Easing.CubicInOut);

        isBlooming = true;
        treeLeaves.Fill = SakuraPink;
        statusLabel.Text = "Sakura õitseb!";
    }

    private async Task ShakeAsync()
    {
        if (isFelled)
        {
            statusLabel.Text = "Langetatud puud ei saa raputada.";
            return;
        }

        statusLabel.Text = "Puu väriseb!";
        await RunAnimationAsync("TreeShake", progress =>
        {
            double wave = Math.Sin(progress * Math.PI * 10) * (1 - progress);
            treeBody.TranslationX = 7 * wave;
            treeBody.Rotation = 3 * wave;
        }, Easing.Linear);

        treeBody.TranslationX = 0;
        treeBody.Rotation = 0;
        statusLabel.Text = "Puu lõpetas värisemise.";
    }

    private async Task FellAsync()
    {
        if (isFelled)
        {
            statusLabel.Text = "Puu on juba langetatud!";
            return;
        }

        int month = (datePicker.Date ?? DateTime.Today).Month;
        TimeSpan time = timePicker.Time ?? DateTime.Now.TimeOfDay;
        bool isWinter = month == 12 || month == 1 || month == 2;
        bool allowedTime = time >= TimeSpan.FromHours(8)
            && time <= TimeSpan.FromHours(17);

        if (!isWinter || !allowedTime)
        {
            statusLabel.Text = "Puud võib langetada ainult talvel kell 08:00–17:00!";
            return;
        }

        statusLabel.Text = "Puu langeb!";
        // Puu pöördub oma maasse kinnitatud jalami ümber.
        await treeBody.RotateToAsync(90, (uint)animationStepper.Value, Easing.CubicIn);
        isFelled = true;
        statusLabel.Text = "Puu on langetatud!";
    }

    private async void StartButton_Clicked(object sender, EventArgs e)
    {
        if (isBusy || actionPicker.SelectedIndex < 0)
            return;

        isBusy = true;
        startButton.IsEnabled = false;
        try
        {
            TreeAction action = (TreeAction)actionPicker.SelectedIndex;
            switch (action)
            {
                case TreeAction.Kasva:
                    await GrowAsync();
                    break;
                case TreeAction.Oitse:
                    await BloomAsync();
                    break;
                case TreeAction.Varise:
                    await ShakeAsync();
                    break;
                case TreeAction.Langeta:
                    await FellAsync();
                    break;
            }
        }
        finally
        {
            isBusy = false;
            startButton.IsEnabled = true;
        }
    }
}