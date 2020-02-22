using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LR3
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }
        private void ScanerButton_Clicked(object sender, EventArgs e)
        {
            string text = CodeEditor.Text;
            if (text != null)
            {
                InitializeComponent();
                CodeEditor.Text = text;
                ChepinGrid.IsVisible = true;
                string[] str = text.Split('\r');
                ProgramAnalysis analizedProgram = new ProgramAnalysis(str);
                SpenGrid.Children.Clear();
                SpenGrid.RowDefinitions.Clear();
                int numberOfIdentificators = analizedProgram.Identificators.Count;
                for (int i = 0; i <= numberOfIdentificators + 1; i++) 
                    SpenGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                SpenGrid.Children.Add(new Label { Text = "Идентификатор", FontAttributes = FontAttributes.Bold }, 0, 0);
                SpenGrid.Children.Add(new Label { Text = "Спен", FontAttributes = FontAttributes.Bold }, 1, 0);
                for (int i = 1; i <= numberOfIdentificators; i++) 
                {
                    SpenGrid.Children.Add(new Label { Text = analizedProgram.Identificators.ElementAt(numberOfIdentificators - i).Key }, 0, i);
                    SpenGrid.Children.Add(new Label { Text = analizedProgram.Identificators.ElementAt(numberOfIdentificators - i).Value.ToString() }, 1, i);
                }
                SpenGrid.Children.Add(new Label { Text = "Суммарный спен программы", FontAttributes = FontAttributes.Bold }, 0, numberOfIdentificators + 1);
                SpenGrid.Children.Add(new Label { Text = analizedProgram.SpenSumma.ToString(), FontAttributes = FontAttributes.Bold }, 1, numberOfIdentificators + 1);
                                
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.P), HorizontalOptions = LayoutOptions.Center }, 1, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.M), HorizontalOptions = LayoutOptions.Center }, 2, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.C), HorizontalOptions = LayoutOptions.Center }, 3, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.T), HorizontalOptions = LayoutOptions.Center }, 4, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.P1), HorizontalOptions = LayoutOptions.Center }, 5, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.M1), HorizontalOptions = LayoutOptions.Center }, 6, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.C1), HorizontalOptions = LayoutOptions.Center }, 7, 2);
                ChepinGrid.Children.Add(new Label { Text = Variables(analizedProgram.T1), HorizontalOptions = LayoutOptions.Center }, 8, 2);

                ChepinGrid.Children.Add(new Label { Text = "p=" + analizedProgram.P.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 1, 3);
                ChepinGrid.Children.Add(new Label { Text = "m=" + analizedProgram.M.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 2, 3);
                ChepinGrid.Children.Add(new Label { Text = "c=" + analizedProgram.C.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 3, 3);
                ChepinGrid.Children.Add(new Label { Text = "t=" + analizedProgram.T.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 4, 3);
                ChepinGrid.Children.Add(new Label { Text = "p=" + analizedProgram.P1.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 5, 3);
                ChepinGrid.Children.Add(new Label { Text = "m=" + analizedProgram.M1.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 6, 3);
                ChepinGrid.Children.Add(new Label { Text = "c=" + analizedProgram.C1.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 7, 3);
                ChepinGrid.Children.Add(new Label { Text = "t=" + analizedProgram.T1.Count.ToString(), HorizontalOptions = LayoutOptions.Center }, 8, 3);

                Label CoeffLabel = new Label { Text = "Q=1*" + analizedProgram.P.Count.ToString() + "+2*" + analizedProgram.M.Count.ToString() + "+3*" +
                    analizedProgram.C.Count.ToString() + "+0.5*" + analizedProgram.T.Count.ToString() + "=" + analizedProgram.Coefficient.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                };
                ChepinGrid.Children.Add(CoeffLabel, 1, 4);
                Grid.SetColumnSpan(CoeffLabel, 4);
                Label Coeff1Label = new Label{ Text = "Q=1*" + analizedProgram.P1.Count.ToString() + "+2*" + analizedProgram.M1.Count.ToString() + "+3*" +
                    analizedProgram.C1.Count.ToString() + "+0.5*" + analizedProgram.T1.Count.ToString() + "=" + analizedProgram.Coefficient1.ToString(),
                    HorizontalOptions = LayoutOptions.Center,
                    FontAttributes = FontAttributes.Bold
                };
                ChepinGrid.Children.Add(Coeff1Label, 5, 4);
                Grid.SetColumnSpan(Coeff1Label, 4);
            }
            else
                ChepinGrid.IsVisible = false;
        }

        public string Variables(List<string> list)
        {
            if (list.Count == 0)
                return "--";
            string str = "";
            for (int i = 0; i < list.Count; i++)
            {
                str += list[i];
                if (i != list.Count - 1)
                    str += ", ";
            }
            return str;
        }
    }
}
