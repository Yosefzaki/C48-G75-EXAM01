using System;

namespace ExaminationSystem
{
    #region Answer Class

    public class Answer : ICloneable
    {
        public int AnswerId { get; set; }
        public string AnswerText { get; set; }

        public Answer(int id, string text)
        {
            AnswerId = id;
            AnswerText = text;
        }

        public object Clone()
        {
            return new Answer(AnswerId, AnswerText);
        }

        public override string ToString()
        {
            return $"{AnswerId}. {AnswerText}";
        }
    }

    #endregion

    #region Base Question Class

    public abstract class Question : ICloneable, IComparable<Question>
    {
        public string Header { get; set; }
        public string Body { get; set; }
        public double Mark { get; set; }
        public Answer[] AnswerList { get; set; }
        public Answer RightAnswer { get; set; }
        public Answer UserAnswer { get; set; }

        protected Question(string header, string body, double mark)
        {
            Header = header;
            Body = body;
            Mark = mark;
        }

        protected Question(string header, string body, double mark, Answer[] answerList, Answer rightAnswer)
            : this(header, body, mark)
        {
            AnswerList = answerList;
            RightAnswer = rightAnswer;
        }

        public abstract void DisplayQuestion();

        public virtual object Clone()
        {
            Answer[] clonedList = new Answer[AnswerList.Length];
            for (int i = 0; i < AnswerList.Length; i++)
            {
                clonedList[i] = (Answer)AnswerList[i].Clone();
            }

            // Creating deep copy
            var clone = (Question)MemberwiseClone();
            clone.AnswerList = clonedList;
            clone.RightAnswer = (Answer)RightAnswer.Clone();
            return clone;
        }

        public int CompareTo(Question other)
        {
            if (other == null) return 1;
            return Mark.CompareTo(other.Mark);
        }

        public override string ToString()
        {
            return $"[{Header}] ({Mark} Marks)\n{Body}";
        }
    }

    #endregion

    #region Derived Question Types

    public class TrueFalseQuestion : Question
    {
        public TrueFalseQuestion(string header, string body, double mark, Answer rightAnswer)
            : base(header, body, mark)
        {
            AnswerList = new Answer[]
            {
                new Answer(1, "True"),
                new Answer(2, "False")
            };
            RightAnswer = rightAnswer;
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine(ToString());
            foreach (var ans in AnswerList)
            {
                Console.WriteLine(ans);
            }
        }
    }

    public class MCQQuestion : Question
    {
        public MCQQuestion(string header, string body, double mark, Answer[] answerList, Answer rightAnswer)
            : base(header, body, mark, answerList, rightAnswer)
        {
        }

        public override void DisplayQuestion()
        {
            Console.WriteLine(ToString());
            foreach (var ans in AnswerList)
            {
                Console.WriteLine(ans);
            }
        }
    }

    #endregion

    #region Base Exam Class

    public abstract class Exam : ICloneable, IComparable<Exam>
    {
        public int TimeInMinutes { get; set; }
        public int NumberOfQuestions { get; set; }
        public Question[] Questions { get; set; }

        protected Exam(int timeInMinutes, int numberOfQuestions)
        {
            TimeInMinutes = timeInMinutes;
            NumberOfQuestions = numberOfQuestions;
            Questions = new Question[numberOfQuestions];
        }

        public abstract void ShowExam();

        public virtual object Clone()
        {
            Exam clonedExam = (Exam)MemberwiseClone();
            clonedExam.Questions = new Question[Questions.Length];
            for (int i = 0; i < Questions.Length; i++)
            {
                clonedExam.Questions[i] = (Question)Questions[i].Clone();
            }
            return clonedExam;
        }

        public int CompareTo(Exam other)
        {
            if (other == null) return 1;
            return TimeInMinutes.CompareTo(other.TimeInMinutes);
        }

        public override string ToString()
        {
            return $"Exam Duration: {TimeInMinutes} mins | Questions Count: {NumberOfQuestions}";
        }
    }

    #endregion

    #region Derived Exam Types

    public class PracticalExam : Exam
    {
        public PracticalExam(int timeInMinutes, int numberOfQuestions)
            : base(timeInMinutes, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("\n--- PRACTICAL EXAM ---");
            for (int i = 0; i < Questions.Length; i++)
            {
                Console.WriteLine($"\nQuestion {i + 1}:");
                Questions[i].DisplayQuestion();

                int userChoice = GetValidUserChoice(Questions[i].AnswerList.Length);
                Questions[i].UserAnswer = Questions[i].AnswerList[userChoice - 1];
            }

            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine(" PRACTICAL EXAM RESULTS & ANSWERS");
            Console.WriteLine("=================================");
            for (int i = 0; i < Questions.Length; i++)
            {
                Console.WriteLine($"\nQ{i + 1}: {Questions[i].Body}");
                Console.WriteLine($"Your Answer : {Questions[i].UserAnswer.AnswerText}");
                Console.WriteLine($"Right Answer: {Questions[i].RightAnswer.AnswerText}");
            }
        }

        private int GetValidUserChoice(int maxOption)
        {
            int choice;
            do
            {
                Console.Write("Enter your choice ID: ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > maxOption);

            return choice;
        }
    }

    public class FinalExam : Exam
    {
        public FinalExam(int timeInMinutes, int numberOfQuestions)
            : base(timeInMinutes, numberOfQuestions) { }

        public override void ShowExam()
        {
            Console.WriteLine("\n--- FINAL EXAM ---");
            double totalGrade = 0;
            double userGrade = 0;

            for (int i = 0; i < Questions.Length; i++)
            {
                totalGrade += Questions[i].Mark;
                Console.WriteLine($"\nQuestion {i + 1}:");
                Questions[i].DisplayQuestion();

                int userChoice = GetValidUserChoice(Questions[i].AnswerList.Length);
                Questions[i].UserAnswer = Questions[i].AnswerList[userChoice - 1];

                if (Questions[i].UserAnswer.AnswerId == Questions[i].RightAnswer.AnswerId)
                {
                    userGrade += Questions[i].Mark;
                }
            }

            Console.Clear();
            Console.WriteLine("=================================");
            Console.WriteLine("     FINAL EXAM RESULTS");
            Console.WriteLine("=================================");
            for (int i = 0; i < Questions.Length; i++)
            {
                Console.WriteLine($"\nQ{i + 1}: {Questions[i].Body}");
                Console.WriteLine($"Your Answer : {Questions[i].UserAnswer.AnswerText}");
                Console.WriteLine($"Right Answer: {Questions[i].RightAnswer.AnswerText}");
                Console.WriteLine($"Mark        : {Questions[i].Mark}");
            }

            Console.WriteLine("---------------------------------");
            Console.WriteLine($"Your Final Grade: {userGrade} / {totalGrade}");
            Console.WriteLine("=================================");
        }

        private int GetValidUserChoice(int maxOption)
        {
            int choice;
            do
            {
                Console.Write("Enter your choice ID: ");
            } while (!int.TryParse(Console.ReadLine(), out choice) || choice < 1 || choice > maxOption);

            return choice;
        }
    }

    #endregion

    #region Subject Class

    public class Subject
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public Exam SubjectExam { get; set; }

        public Subject(int id, string name)
        {
            SubjectId = id;
            SubjectName = name;
        }

        public void CreateExam()
        {
            Console.WriteLine($"--- Designing Exam for Subject: {SubjectName} ---");
            Console.Write("Enter Exam Type (1 for Practical, 2 for Final): ");
            int examType = int.Parse(Console.ReadLine());

            Console.Write("Enter Exam Time in Minutes: ");
            int time = int.Parse(Console.ReadLine());

            Console.Write("Enter Number of Questions: ");
            int numQuestions = int.Parse(Console.ReadLine());

            if (examType == 1)
            {
                SubjectExam = new PracticalExam(time, numQuestions);
                for (int i = 0; i < numQuestions; i++)
                {
                    Console.WriteLine($"\nSetting up Question {i + 1} (MCQ):");
                    SubjectExam.Questions[i] = CreateMCQQuestion();
                }
            }
            else
            {
                SubjectExam = new FinalExam(time, numQuestions);
                for (int i = 0; i < numQuestions; i++)
                {
                    Console.Write($"\nSelect Question Type for Q{i + 1} (1 for True/False, 2 for MCQ): ");
                    int qType = int.Parse(Console.ReadLine());

                    if (qType == 1)
                    {
                        SubjectExam.Questions[i] = CreateTrueFalseQuestion();
                    }
                    else
                    {
                        SubjectExam.Questions[i] = CreateMCQQuestion();
                    }
                }
            }
        }

        private Question CreateTrueFalseQuestion()
        {
            Console.Write("Enter Question Header: ");
            string header = Console.ReadLine();

            Console.Write("Enter Question Body: ");
            string body = Console.ReadLine();

            Console.Write("Enter Question Mark: ");
            double mark = double.Parse(Console.ReadLine());

            Console.Write("Enter Right Answer ID (1 for True, 2 for False): ");
            int rightId = int.Parse(Console.ReadLine());

            Answer rightAnswer = (rightId == 1) ? new Answer(1, "True") : new Answer(2, "False");
            return new TrueFalseQuestion(header, body, mark, rightAnswer);
        }

        private Question CreateMCQQuestion()
        {
            Console.Write("Enter Question Header: ");
            string header = Console.ReadLine();

            Console.Write("Enter Question Body: ");
            string body = Console.ReadLine();

            Console.Write("Enter Question Mark: ");
            double mark = double.Parse(Console.ReadLine());

            Console.Write("Enter Number of Options: ");
            int optionCount = int.Parse(Console.ReadLine());

            Answer[] answerList = new Answer[optionCount];
            for (int i = 0; i < optionCount; i++)
            {
                Console.Write($"Enter Option {i + 1} Text: ");
                string text = Console.ReadLine();
                answerList[i] = new Answer(i + 1, text);
            }

            Console.Write("Enter Right Answer Option ID: ");
            int rightId = int.Parse(Console.ReadLine());

            Answer rightAnswer = answerList[rightId - 1];
            return new MCQQuestion(header, body, mark, answerList, rightAnswer);
        }

        public override string ToString()
        {
            return $"Subject ID: {SubjectId} | Name: {SubjectName}";
        }
    }

    #endregion

    #region Program Execution

    class Program
    {
        static void Main(string[] args)
        {
            Subject csharpSubject = new Subject(101, "C# Programming");

            csharpSubject.CreateExam();

            Console.Clear();
            Console.Write("Do you want to start the exam? (Y/N): ");
            char choice = char.Parse(Console.ReadLine());

            if (choice == 'Y' || choice == 'y')
            {
                csharpSubject.SubjectExam.ShowExam();
            }
            else
            {
                Console.WriteLine("Exam postponed.");
            }
        }
    }

    #endregion
}