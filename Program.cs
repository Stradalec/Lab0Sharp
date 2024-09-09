using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Xml.Linq;

namespace Lab0
{
    //Интерфейс, связывающий форму с кодом, выполняющим задачу
    interface IView
    {
        string PathToFile();
        string NameOfPerson();
        string SurnameOfPerson();
        Gender GenderOfPerson();
        double HeightOfPerson();
        double Density();
        double Radius();
        double Mass();

        void ReturnResultOfWidth(double width);
        void ReturnResultOfHeight(double[] height);
        void ReturnTallestHeight(Person[] humans);


        event EventHandler<EventArgs> AddPersonInList;
        event EventHandler<EventArgs> CreateDocument;
        event EventHandler<EventArgs> OpenDocument;
        event EventHandler<EventArgs> FindHeight;
        event EventHandler<EventArgs> ReturnWidth;
    }


    //Тип перечислений (сугубо для упрощения визуального понимания, да и лень было через true/false делать) 
    public enum Gender
    {
        Male,
        Female
    }

    //Класс "Персона" со всеми необходимыми данными.
    public class Person
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public double Height { get; set; }


        //Пустой конструктор нужен для лямбды, не убирать
        public Person()
        {

        }

        //Этот конструктор тоже нужен, тоже не убирать
        public Person(string inputName, string inputSurname, Gender inputGender, double inputHeight)
        {
            Name = inputName;
            Surname = inputSurname;
            Gender = inputGender;
            Height = inputHeight;
        }
    }

    // Модель. Основная часть работы программы происходит здесь
    class Model
    {
        private List<Person> humansList = new List<Person>();

        //Здесь мы находим толщину диска
        public double FindWidth(double inputDensity, double inputRadius, double inputMass)
        {
            double Width = inputMass / (Math.PI * (inputRadius * inputRadius) * inputDensity);
            return Width;
        }

        //Добавляем человека в список на добавление в файл
        public void AddPerson(Person inputPerson)
        {
            humansList.Add(inputPerson);
        }

        //Добавляем записи о людях из списка в файл
        public void CreateDocument(string path)
        {
            var newDocument = new XDocument(
                new XElement("people",
                    humansList.Select(human => new XElement("person",
                    new XAttribute("name", human.Name),
                    new XAttribute("surname", human.Surname),
                    new XAttribute("gender", human.Gender.ToString()),
                    new XAttribute("height", human.Height)
                    ))
                )
            );

            newDocument.Save(path + "\\people.xml");

            humansList.Clear();
        }

        //Находим средний рост
        public double[] AverageHeight(string path)
        {
            double[] averageHeight = new double[2];

            //Читаем данные из XML
            XDocument document = XDocument.Load(path);
            var peopleInList = document.Root.Elements("person")
                .Select(human => new Person { //Именно для этого фокуса с лямбдой нам и понадобился пустой конструктор
                    Name = human.Attribute("name").Value,
                    Surname = human.Attribute("surname").Value,
                    Gender = (Gender)Enum.Parse(typeof(Gender), human.Attribute("gender").Value),
                    Height = double.Parse(human.Attribute("height").Value)
                });

            double averageMaleHeight = 0;
            double averageFemaleHeight = 0;
            int maleCount = 0;
            int femaleCount = 0;

            foreach (var person in peopleInList)
            {
                if (person.Gender == Gender.Male)
                {
                    averageMaleHeight += person.Height;
                    ++maleCount;
                }

                if (person.Gender == Gender.Female)
                {
                    averageFemaleHeight += person.Height;
                    ++femaleCount;
                }
            }

            averageHeight[0] = averageMaleHeight / maleCount;
            averageHeight[1] = averageFemaleHeight / femaleCount;
            return averageHeight;
        }

        //Ищем самых высоких
        public Person[] TallestPersons(string path)
        {
            Person[] tallestHeight = new Person[2];
            XDocument document = XDocument.Load(path);

            var peopleInList = document.Root.Elements("person")
                .Select(human => new Person { //фокус с лямбдой, аналогичный предыдущему
                    Name = human.Attribute("name").Value,
                    Surname = human.Attribute("surname").Value,
                    Gender = (Gender)Enum.Parse(typeof(Gender), human.Attribute("gender").Value),
                    Height = double.Parse(human.Attribute("height").Value)
                });

            Person tallestMale = null;
            Person tallestFemale = null;

            foreach (var person in peopleInList)
            {
                if (person.Gender == Gender.Male && (tallestMale == null || person.Height > tallestMale.Height))
                {
                    tallestMale = person;
                }
                if (person.Gender == Gender.Female && (tallestFemale == null || person.Height > tallestFemale.Height))
                {
                    tallestFemale = person;
                }
            }

            tallestHeight[0] = tallestMale;
            tallestHeight[1] = tallestFemale;
            return tallestHeight;
        }
    }

    // Презентер. Извлекает данные из модели, передает в вид. Обрабатывает события
    class Presenter
    {
        private IView mainView;
        private Model model;

        public Presenter(IView inputView)
        {
            mainView = inputView;
            model = new Model();

            mainView.ReturnWidth += new EventHandler<EventArgs>(FindWidth);
            mainView.AddPersonInList += new EventHandler<EventArgs>(AddPersonInModel);
            mainView.CreateDocument += new EventHandler<EventArgs>(CreateNewDocument);
            mainView.FindHeight += new EventHandler<EventArgs>(FindHeight);
        }

        // Обработка события "Возврат толщины диска"
        private void FindWidth(object sender, EventArgs inputEvent)
        {
            double outputWidth = model.FindWidth(mainView.Density(), mainView.Radius(), mainView.Mass());
            mainView.ReturnResultOfWidth(outputWidth);
        }

        // Обработка события "Добавление человека в список"
        private void AddPersonInModel(object sender, EventArgs inputEvent)
        {

            model.AddPerson(new Person(mainView.NameOfPerson(), mainView.SurnameOfPerson(), mainView.GenderOfPerson(), mainView.HeightOfPerson()));
        }

        // Обработка события "Создание документа"
        private void CreateNewDocument(object sender, EventArgs inputEvent)
        {
            model.CreateDocument(mainView.PathToFile());
        }

        // Обработка события "Открытие документа и извлечение необходимых данных"
        private void FindHeight(object sender, EventArgs inputEvent)
        {
            double[] outputAverageHeight = model.AverageHeight(mainView.PathToFile());
            mainView.ReturnResultOfHeight(outputAverageHeight);
            Person[] outputTallest = model.TallestPersons(mainView.PathToFile());
            mainView.ReturnTallestHeight(outputTallest);
        }

    }
    internal static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }
    }
}
