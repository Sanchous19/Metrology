using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace LR2
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

                NumberOfConditionalStatementsLabel.Text = "Количество условных операторов: " + analizedProgram.NumberOfConditionalStatementsLabel.ToString();
                SaturationOfConditionalStatementsLabel.Text = "Насыщенность программы условными операторами: " + analizedProgram.SaturationOfConditionalStatements.ToString();
                MaximumNestingLevelOfConditionalOperatorLable.Text = "Максимальный уровень вложенности условного оператора: " + analizedProgram.MaximumNestingLevelOfConditionalOperator.ToString();
            }
        }
    }
}
