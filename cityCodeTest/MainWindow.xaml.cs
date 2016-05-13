using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Speech.Synthesis;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

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
        private string currentDir = AppDomain.CurrentDomain.BaseDirectory;
        private SpeechSynthesizer synthesizer = new SpeechSynthesizer();



        public MainWindow()
        {
            InitializeComponent();

            synthesizer.Volume = 100;

            text_Question.IsReadOnly = true;

            buttons[0] = Answer1;
            buttons[1] = Answer2;
            buttons[2] = Answer3;
            buttons[3] = Answer4;

            main();
        }

        private void main()
        {
            OpenFileDialog openFile = new OpenFileDialog();
            openFile.Filter = "Text Files (*.txt)|*.txt|All Files (*.*)|*.*";
            openFile.InitialDirectory = currentDir;

            if(openFile.ShowDialog().Value == false)
            {
                MessageBox.Show("You need to select a file to open!");
                Application.Current.Shutdown();
            }

            if (File.Exists(openFile.FileName) == false)
            {
                MessageBox.Show("Missing the cc text file that contains all the letter codes and cities. Please put the file in the same folder as this application.");
                Environment.Exit(-1);
            }

            System.IO.StreamReader file = new System.IO.StreamReader(openFile.FileName);
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

            Questions = ShuffleList(Questions);
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
                MessageBox.Show("All Done! You got: " + (float)correctAnswers/(float)Questions.Count + "%");
                return;
            }

            if(check_Study.IsChecked == true)
            {
                check_Study_Click(null, null);
            }
            
            setQuestion();
        }

        private void setQuestion()
        {
            synthesizer.SpeakAsyncCancelAll();
            label_QuestionNumber.Content = "Question Number: " + (questionNumber+1);
            label_Total.Content = "Total: " + Questions.Count;
            text_Question.Text = Questions[questionNumber].QuestionText;
            label_Correct.Text = "Correct: " + correctAnswers;


            //Set the answer radio button
            var rand = new Random(DateTime.Now.Millisecond);
            int num = rand.Next(0, 4);
            buttons[num].Content = Questions[questionNumber].CorrectAnswer;

            if(check_Study.IsChecked == false)
            {
                //Set the rest of the answers
                for (int i = 0; i < buttons.Length; i++)
                {
                    if (i != num)
                    {
                        buttons[i].Content = fakeAnswer(Questions[questionNumber].CorrectAnswer);
                    }
                }
            }

            if(check_Audio.IsChecked == true)
            {
                speech();
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
                        MessageBox.Show("Wrong! The correct answer is: " + Questions[questionNumber].CorrectAnswer);
                        button_Next_Click(null, null);
                        return;
                    }
                }
            }

            MessageBox.Show("Nothing is selected!");

        }

        private List<E> ShuffleList<E>(List<E> inputList)
        {
            List<E> randomList = new List<E>();

            Random r = new Random();
            int randomIndex = 0;
            while (inputList.Count > 0)
            {
                randomIndex = r.Next(0, inputList.Count); //Choose a random object in the list
                randomList.Add(inputList[randomIndex]); //add it to the new, random list
                inputList.RemoveAt(randomIndex); //remove to avoid duplicates
            }

            return randomList; //return the new random list
        }


        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/martinoj2009/testTaker/blob/master/cityCodeTest/MainWindow.xaml.cs");
        }

        private void MenuItem_Click_1(object sender, RoutedEventArgs e)
        {
            aboutBox about = new aboutBox();
            about.ShowDialog();
            about.Dispose();
        }

        private void check_Study_Click(object sender, RoutedEventArgs e)
        {
            if(check_Study.IsChecked == true)
            {
                label_StudyAnswer.Visibility = Visibility.Visible;
                label_StudyAnswer.Text = Questions[questionNumber].CorrectAnswer;
                label_StudyAnswer.FontSize = 17;
                button_CheckAnswer.IsEnabled = false;
                button_Next.Content = "Next";
                buttons[0].Visibility = Visibility.Hidden;
                buttons[1].Visibility = Visibility.Hidden;
                buttons[2].Visibility = Visibility.Hidden;
                buttons[3].Visibility = Visibility.Hidden;
                setQuestion();
            }
            else
            {
                label_StudyAnswer.Visibility = Visibility.Hidden;
                label_StudyAnswer.FontSize = 12;
                button_CheckAnswer.IsEnabled = true;
                button_Next.Content = "Skip";
                buttons[0].Visibility = Visibility.Visible;
                buttons[1].Visibility = Visibility.Visible;
                buttons[2].Visibility = Visibility.Visible;
                buttons[3].Visibility = Visibility.Visible;
                setQuestion();
            }
        }

        private void speech()
        {
            if(check_Study.IsChecked == false)
            {
                if (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync(text_Question.Text);

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync("Is. A");

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync(buttons[0].Content.ToString());

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync("B." + buttons[1].Content.ToString());

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync("C." + buttons[2].Content.ToString());

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync("D." + buttons[3].Content.ToString());
            }
            else
            {
                if (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync(text_Question.Text);

                while (synthesizer.State == SynthesizerState.Speaking)
                {
                    System.Threading.Thread.Sleep(50);
                }

                synthesizer.SpeakAsync("The answer is. " + Questions[questionNumber].CorrectAnswer);
            }
            
        }

        private void check_Audio_Click(object sender, RoutedEventArgs e)
        {
            setQuestion();
        }
    }
}
