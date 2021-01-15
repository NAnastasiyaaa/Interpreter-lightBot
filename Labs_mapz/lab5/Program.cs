using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp12
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("############################################## -BEAUTY SALON- #########################################################");
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("///////////////////PATERN COMMAND///////////////////////////");
             Console.ForegroundColor = ConsoleColor.Yellow;
            Manager manager = new Manager();
            Master ms = new Master();
            manager.SetCommand(new MasterAddCommand(ms));
            manager.PressButton();
            manager.PressUndo();
            Console.ForegroundColor = ConsoleColor.Red;

            Console.WriteLine("///////////////////PATERN MEDIATOR//////////////////////////");
            Console.ForegroundColor = ConsoleColor.Yellow;
            ManagerMediator mediator = new ManagerMediator();
            Colleague customer = new CustomerColleague(mediator);
            Colleague programmer = new ProgrammerColleague(mediator);
            Colleague tester = new TesterColleague(mediator);
            mediator.Customer = customer;
            mediator.Programmer = programmer;
            mediator.Tester = tester;
            customer.Send("You have an order! You must do a procedure!"); // є замовлення від майстра, потрібно зробити процедуру
            programmer.Send("Master accepted the order! You need to make a record!"); // завка прийнята, потрібно зробити запис 
            tester.Send("Your record is accepted! You are accepted for a procedure!"); // Запис зроблено! Надіслати повідомлення клієнту!

            Console.Read();
        }
    }
    /////////////*************************PATERN COMMAND******************************//////////
    interface ICommand //interface
    {
        void Execute(); //виконання дії
        void Undo();//скасування дії
    }

    // Receiver                                       (отримувач )  
    class Master    //class Master визначає дії які мають викон в результаті запиту
    {
        public void TakeOnMaster()   
        {
            Console.WriteLine("Take on a master!");
        }

        public void TakeOffMaster()  
        {
            Console.WriteLine("Fire a master!");
        }
    }


    class MasterAddCommand : ICommand  
    {
        Master ms;    
        public MasterAddCommand(Master msSet)  
        {
            ms = msSet;
        }
        public void Execute()
        {
            ms.TakeOnMaster();   
        }
        public void Undo()
        {
            ms.TakeOffMaster();  
        }
    }

    // Invoker - инициатор
    class Manager   //викликає команду для виконання запиту
    {
        ICommand command;

        public Manager() { }

        public void SetCommand(ICommand com)
        {
            command = com;
        }

        public void PressButton()
        {
            command.Execute();
        }
        public void PressUndo()
        {
            command.Undo();
        }
    }

    /////////////***********************PATERN MEDIATOR*************************//////////
    abstract class Mediator  //представляє інтерфейс для взаємодії з об'єктами Colleague
    {
        public abstract void Send(string msg, Colleague colleague);
    }
    abstract class Colleague  //представляє інтерфейс для взаємодії з об'єктом Mediator
    {
        protected Mediator mediator;

        public Colleague(Mediator mediator)
        {
            this.mediator = mediator;
        }

        public virtual void Send(string message)
        {
            mediator.Send(message, this);
        }
        public abstract void Notify(string message);
    }

    // класс client  // конкретні класи колег, які обмінюються між собою через об'єкт Mediator
    class CustomerColleague : Colleague              
    {
        public CustomerColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Message for Client: " + message);
        }
    }


    // класс master
    class ProgrammerColleague : Colleague
    {
        public ProgrammerColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Message for Master: " + message);
        }
    }
    // класс тестера
    class TesterColleague : Colleague
    {
        public TesterColleague(Mediator mediator)
            : base(mediator)
        { }

        public override void Notify(string message)
        {
            Console.WriteLine("Message for Manager: " + message);
        }
    }

    class ManagerMediator : Mediator  //конкретний посередник, який реалізує інтерфейс типу Mediator
    {
        public Colleague Customer { get; set; }
        public Colleague Programmer { get; set; }
        public Colleague Tester { get; set; }
        public override void Send(string msg, Colleague colleague)
        {
            // якщо відправник - client, значить є нове замовлення
            // відправляємо майстру  що є новий заказ
            if (Customer == colleague)
                Programmer.Notify(msg);
            // якщо відправник- менеджер 
            // відправляємо повідомлення тестеру
            else if (Programmer == colleague)
                Tester.Notify(msg);
            // якщо виконалось тестування, то замовлення готове
            else if (Tester == colleague)
                Customer.Notify(msg);
        }
    }
}



