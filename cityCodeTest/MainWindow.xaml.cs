﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace cityCodeTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private int questionNumber = 0;
        private List<question> Questions = new List<question>();
        private List<string> possibleAnswers = new List<string>();
        private RadioButton[] buttons = new RadioButton[4];
        private int correctAnswers = 0;



        public MainWindow()
        {
            InitializeComponent();

            buttons[0] = Answer1;
            buttons[1] = Answer2;
            buttons[2] = Answer3;
            buttons[3] = Answer4;

            main();
        }

        private void main()
        {
            System.IO.StreamReader file = new System.IO.StreamReader("cc.txt");
            string line;
            while ((line = file.ReadLine()) != null)
            {
                if(line.Contains(",") == true)
                {
                    string answer = line.Split(',')[0].Trim();
                    possibleAnswers.Add(answer);
                    string city = line.Substring(line.IndexOf(',') + 1).Trim();
                    Questions.Add(new question(city, answer));
                }
            }

            setQuestion();


        }

        private void button_Next_Click(object sender, RoutedEventArgs e)
        {
            //Reset the buttons
            for (int i = 0; i < buttons.Length; i++)
            {
                buttons[i].IsChecked = false;
            }

            questionNumber++;

            if (questionNumber >= Questions.Count)
            {
                MessageBox.Show("All Done! You got: " + (double)correctAnswers/(double)Questions.Count + "%");
                return;
            }

            setQuestion();
        }

        private void setQuestion()
        {
            label_QuestionNumber.Content = "Question Number: " + (questionNumber+1);
            label_Total.Content = "Total: " + Questions.Count;
            label_city.Content = Questions[questionNumber].QuestionText;
            label_Correct.Text = "Correct: " + correctAnswers;


            //Set the answer radio button
            var rand = new Random(DateTime.Now.Millisecond);
            int num = rand.Next(0, 4);
            buttons[num].Content = Questions[questionNumber].CorrectAnswer;

            //Set the rest of the answers
            for(int i = 0; i < buttons.Length; i++)
            {
                if(i != num)
                {
                    buttons[i].Content = fakeAnswer(Questions[questionNumber].CorrectAnswer);
                }
            }
        }


        private string fakeAnswer(string realAnswer)
        {
            string fakeAnswer;
            int max = possibleAnswers.Count;

            var seed = Convert.ToInt32(Regex.Match(Guid.NewGuid().ToString(), @"\d+").Value);

            var rnd = new Random(seed);

            int ticks = rnd.Next(0, max);
            fakeAnswer = possibleAnswers[ticks];
            

            return fakeAnswer;
        }

        private void button_CheckAnswer_Click(object sender, RoutedEventArgs e)
        {
            for(int i = 0; i < buttons.Length; i++)
            {
                if(buttons[i].IsChecked == true)
                {
                    if(buttons[i].Content.Equals(Questions[questionNumber].CorrectAnswer) == true)
                    {
                        MessageBox.Show("Correct!");
                        correctAnswers++;
                        button_Next_Click(null, null);
                        return;
                    }
                    else
                    {
                        MessageBox.Show("Wrong!");
                        button_Next_Click(null, null);
                        return;
                    }
                }
            }

            MessageBox.Show("Nothing is selected!");

        }
    }
}