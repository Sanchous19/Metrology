using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LR1
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private void ScanerButton_Clicked(object sender, EventArgs e)
        {
            if (CodeEditor.Text != null)
            {
                string[] str = CodeEditor.Text.Split('\r');
                int a = CodeEditor.Text.Length;
                ProgramAnalysis analizedProgram = new ProgramAnalysis(str);
                MainGrid.Children.Clear();
                MainGrid.RowDefinitions.Clear();
                int count = analizedProgram.NumberOfUniqueOperands > analizedProgram.NumberOfUniqueOperators ? analizedProgram.NumberOfUniqueOperands : analizedProgram.NumberOfUniqueOperators;
                for (int i = 0; i < count + 2; i++)
                    MainGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Star) });
                MainGrid.Children.Add(new Label { Text = "j" }, 0, 0);
                MainGrid.Children.Add(new Label { Text = "Оператор" }, 1, 0);
                MainGrid.Children.Add(new Label { Text = "f(1,j)" }, 2, 0);
                MainGrid.Children.Add(new Label { Text = "i" }, 3, 0);
                MainGrid.Children.Add(new Label { Text = "Операнд" }, 4, 0);
                MainGrid.Children.Add(new Label { Text = "f(2,i)" }, 5, 0);
                for (int i = 0, num = analizedProgram.NumberOfUniqueOperators; i < num; i++)
                {
                    MainGrid.Children.Add(new Label { Text = (i + 1).ToString() + "." }, 0, i + 1);
                    MainGrid.Children.Add(new Label { Text = analizedProgram.OperatorsInCode.ElementAt(num - i - 1).Key }, 1, i + 1);
                    MainGrid.Children.Add(new Label { Text = analizedProgram.OperatorsInCode.ElementAt(num - i - 1).Value.ToString() }, 2, i + 1);
                }
                for (int i = 0, num = analizedProgram.NumberOfUniqueOperands; i < num; i++)
                {
                    MainGrid.Children.Add(new Label { Text = (i + 1).ToString() + "." }, 3, i + 1);
                    MainGrid.Children.Add(new Label { Text = analizedProgram.OperandsInCode.ElementAt(num - i - 1).Key }, 4, i + 1);
                    MainGrid.Children.Add(new Label { Text = analizedProgram.OperandsInCode.ElementAt(num - i - 1).Value.ToString() }, 5, i + 1);
                }
                MainGrid.Children.Add(new Label { Text = "n1=" + analizedProgram.NumberOfUniqueOperators.ToString(), FontAttributes = FontAttributes.Bold }, 0, count + 1);
                MainGrid.Children.Add(new Label { Text = "N1=" + analizedProgram.NumberOfAllOperators.ToString(), FontAttributes = FontAttributes.Bold }, 2, count + 1);
                MainGrid.Children.Add(new Label { Text = "n2=" + analizedProgram.NumberOfUniqueOperands.ToString(), FontAttributes = FontAttributes.Bold }, 3, count + 1);
                MainGrid.Children.Add(new Label { Text = "N2=" + analizedProgram.NumberOfAllOperands.ToString(), FontAttributes = FontAttributes.Bold }, 5, count + 1);
                DictionaryOfTheProgramLabel.Text = "Словарь программы: n=" + analizedProgram.NumberOfUniqueOperators.ToString() + "+" + analizedProgram.NumberOfUniqueOperands.ToString() + "=" + analizedProgram.DictionaryOfTheProgram.ToString();
                ProgramLengthLabel.Text = "Длина программы: N=" + analizedProgram.NumberOfAllOperators.ToString() + "+" + analizedProgram.NumberOfAllOperands.ToString() + "=" + analizedProgram.ProgramLength.ToString();
                ProgramScopeLable.Text = "Объем программы: V=" + analizedProgram.ProgramLength.ToString() + "*log2(" + analizedProgram.DictionaryOfTheProgram.ToString() + ")=" + analizedProgram.ProgramScope.ToString();
            }
        }
    }
}
