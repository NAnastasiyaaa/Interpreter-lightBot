using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Serialization
{
    [Serializable]
    class Person
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public Person(string name, int age)
        {
            Name = name;
            Age = age;
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            //////////////////////**************Binary Serializable**************////////////////////
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("//////////////////////Binary Serializable////////////////////");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Person person1 = new Person("Ilona", 29);
            Person person2 = new Person("Kate", 25);
            // массив для сериализации
            Person[] people = new Person[] { person1, person2 };

            BinaryFormatter formatter = new BinaryFormatter();

            using (FileStream fs = new FileStream("people.dat", FileMode.OpenOrCreate))
            {
                // сериализуем весь массив people
                formatter.Serialize(fs, people);

                Console.WriteLine("Object is serializabled");
            }

            // десериализация
            using (FileStream fs = new FileStream("people.dat", FileMode.OpenOrCreate))
            {
                Person[] deserilizePeople = (Person[])formatter.Deserialize(fs);

                foreach (Person p in deserilizePeople)
                {
                    Console.WriteLine($"Master: {p.Name}\n  +-Sale: {p.Age} %");
                }
            }

            /////////////////////////////************PATERN MEMENTO**************///////////////////////////////////
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("///////////PATERN MEMENTO//////////");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Hello, I am manager!");
            Client client = new Client();
            client.Note(); // 9 seats
            GameHistory game = new GameHistory();

            game.History.Push(client.SaveState()); // save program

            client.Note(); //write client 8 seat

            client.RestoreState(game.History.Pop());

            client.Note(); //write client; 8 seat


            //////////**************PATERN_STATE************///////////
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("//////////PATERN_STATE///////////");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Report report = new Report(new DeleteReportState());
            report.Save();
            report.Delete();
            report.Delete();

            ///////////*************PATERN ITERATOR*******************/////////////
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("///////////PATERN ITERATOR/////////////");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Our masters:");
            Salon salon = new Salon();
            Client1 client1 = new Client1();
            client1.SeeMasters(salon);

            Console.ReadLine();
        }
    }


    ///////////**************PATERN MEMENTO**************//////////
    // Originator ств обєект збереження для збереження стану
    class Client   //Client
    {
        private int seats = 10; 
        private int masters = 9;

        public void Note()    
        {
            if (seats > 0)  
            {
                seats--;  
                Console.WriteLine("Write Client. There are {0} seats", seats);   //write clients
            }
            else
                Console.WriteLine("there are no free places");    //there are no free places
        }
        //save state
        public ClientMemento SaveState()        //ClientMomento
        {
            Console.WriteLine("Save program. Now: {0} seats, {1} masters", seats, masters);
            return new ClientMemento(seats, masters);
        }

        //restore state
        public void RestoreState(ClientMemento memento)
        {
            this.seats = memento.Seats;
            this.masters = memento.Masters;
            Console.WriteLine("Recovery program . Now: {0} seats, {1} masters", seats, masters);
        }
    }
    // Memento  зберігає стан обєкту 
    class ClientMemento  //ClientMemento
    {
        public int Seats { get; private set; }
        public int Masters { get; private set; }

        public ClientMemento(int seats, int masters)
        {
            this.Seats = seats;
            this.Masters = masters;
        }
    }

    // Caretaker  зберігє обєкт мементо
    class GameHistory
    {
        public Stack<ClientMemento> History { get; private set; }
        public GameHistory()
        {
            History = new Stack<ClientMemento>();
        }
    }


    //////////**************PATERN_STATE************///////////


    class Report  //представляє об'єкт, поведінка якого має динамічно змінюватися відповідно до стану.
    {
        public IReportState State { get; set; }  

        public Report(IReportState rs)      
        {
            State = rs;
        }

        public void Save()      
        {
            State.Save(this);
        }
        public void Delete()     
        {
            State.Delete(this);
        }
    }

    interface IReportState  //interface state
    {
        void Save(Report report);    
        void Delete(Report report);   
    }

    class SaveReportState : IReportState
    {
        public void Save(Report report)
        {
            Console.WriteLine("Сonvert report to draft");
            report.State = new DeleteReportState();
        }

        public void Delete(Report report)
        {
            Console.WriteLine("Moderating by admin");
        }
    }
    class DeleteReportState : IReportState
    {
        public void Save(Report report)
        {
            Console.WriteLine("Сonvert report to draft ");
            report.State = new RecoveryReportState();
        }

        public void Delete(Report report)
        {
            Console.WriteLine("Report is published");
            report.State = new SaveReportState();
        }
    }
    class RecoveryReportState : IReportState
    {
        public void Save(Report report)
        {
            Console.WriteLine("Renew report ");
        }

        public void Delete(Report report)
        {
            Console.WriteLine("Moderating by admin ");
            report.State = new DeleteReportState();
        }
    }

   ///////////////////////////////////**********PATERN ITERATOR********////////
   
    class Client1 //Client use concrete objects
    {
        public void SeeMasters(Salon salon)    
        {
            IMasterIterator iterator = salon.CreateNumerator();    
            while (iterator.HasNext())
            {
                Master master = iterator.Next();
                Console.WriteLine(master.Name);
            }
        }
    }

    interface IMasterIterator //визначає інтерфейс для обходу складових об'єктів
    {
        bool HasNext();
        Master Next();
    }
    interface IMasterNumerable // Агрегатор, визначає інтерфейс для створення об'єкта-ітератора
    {
        IMasterIterator CreateNumerator();
        int Count { get; }
        Master this[int index] { get; }
    }

    class Master
    {
        public string Name { get; set; }
    }

    class Salon : IMasterNumerable //конкретна реалізація Aggregate, Зберігає елементи, які треба буде перебирати
    {
        private Master[] masters;
        public Salon()
        {
            masters = new Master[]
            {
            new Master{Name="Kate"},
            new Master{Name="Joly"},
            new Master{Name="Freny"}
            };
        }
        public int Count
        {
            get { return masters.Length; }
        }

        public Master this[int index]
        {
            get { return masters[index]; }
        }
        public IMasterIterator CreateNumerator()
        {
            return new SalonNumerator(this);
        }
    }
    class SalonNumerator : IMasterIterator
    {
        IMasterNumerable aggregate;
        int index = 0;
        public SalonNumerator(IMasterNumerable a)
        {
            aggregate = a;
        }
        public bool HasNext()
        {
            return index < aggregate.Count;
        }

        public Master Next()
        {
            return aggregate[index++];
        }
    }
}