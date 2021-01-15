using System;
using System.Collections.Generic;

namespace DoFactory.GangOfFour.Composite.RealWorld
{
    class MainApp
    {
        static void Main()
        {
            /////////////////////////////////////////////////*COMPOSITE*////////////////////////////////////////////////
            // Create a tree structure 
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("///////////////*COMPOSITE*/////////////\n");

            Console.ForegroundColor = ConsoleColor.Yellow;

            CompositeElement root =new CompositeElement("All workers:");
            root.Add(new PrimitiveElement("Boss"));  //leaf
            root.Add(new PrimitiveElement("Manager"));
            root.Add(new PrimitiveElement("Master"));

            // Create a branch  
            
            CompositeElement comp = new CompositeElement("Two masters:");  
            comp.Add(new PrimitiveElement("Master for manicure"));
            comp.Add(new PrimitiveElement("Master for pedicure"));
            root.Add(comp);

            // Add and remove a PrimitiveElement

            PrimitiveElement pe =
              new PrimitiveElement("Yellow Line");
            root.Add(pe);
            root.Remove(pe);

            // Recursively display nodes

            root.Display(1);


            /////////////////////////////////////////////////*FACADE*////////////////////////////////////////////////
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("///////////////*FACADE*/////////////\n");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("I am manager ! \n");
            Console.ForegroundColor = ConsoleColor.Yellow;
          

            Time time = new Time();

            Password password = new Password();

            ReportSystemFacade ide = new ReportSystemFacade(time, password);

            Manager manager = new Manager();
            manager.CreateApplication(ide);


            /////////////////////////////////////////////////*DECORATOR*////////////////////////////////////////////////
            
            Sale sale1 = new PromocodeSale();
            sale1 = new BlackPromokode(sale1);
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("///////////////*DECORATOR*/////////////\n");
           
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Name sale: {0}", sale1.Name);
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine(" Sale: {0} %", sale1.GetCost());

            Sale sale2 = new PromocodeSale();
            sale2 = new InstagramPromocode(sale2);

            Console.WriteLine(" Name sale:{0}", sale2.Name);
            Console.WriteLine(" Sale:  {0} %", sale2.GetCost());

            Sale sale3 = new AprilSale();
            sale3 = new BlackPromokode(sale3);
            sale3 = new InstagramPromocode(sale3);

            Console.WriteLine(" Name sale: {0}", sale3.Name);
            Console.WriteLine(" Sale:  {0} %", sale3.GetCost());



            // Wait for user

            Console.Read();
        }
    }

    /////////////////////////////////////////////////*COMPOSITE*////////////////////////////////////////////////
    /// The 'Component' Treenode

    abstract class DrawingElement   //Component визначає інтерфейс для всіх компонентів в структурі дерева

    {
        protected string _name;

        // Constructor

        public DrawingElement(string name)
        {
            this._name = name;
        }

        public abstract void Add(DrawingElement d);
        public abstract void Remove(DrawingElement d);
        public abstract void Display(int indent);
    }


    /// The 'Leaf' class 

    class PrimitiveElement : DrawingElement   //представляє окремий компонент, який не може містити інші компоненти(leaf has not child)

    {
        // Constructor

        public PrimitiveElement(string name)
          : base(name)
        {
        }

        public override void Add(DrawingElement c)
        {
            Console.WriteLine(
              "Cannot add to a PrimitiveElement");
        }

        public override void Remove(DrawingElement c)
        {
            Console.WriteLine(
              "Cannot remove from a PrimitiveElement");
        }

        public override void Display(int indent)
        {
            Console.WriteLine(
              new String('-', indent) + " " + _name);
        }
    }


    /// The 'Composite' class(have child)


    class CompositeElement : DrawingElement  //представляє компонент, який може зберігати дочірні компоненти. і реалізує механізм для їх додавання і видалення 

    {
        private List<DrawingElement> elements =
          new List<DrawingElement>();

        // Constructor

        public CompositeElement(string name)
          : base(name)
        {
        }

        public override void Add(DrawingElement d)
        {
            elements.Add(d);
        }

        public override void Remove(DrawingElement d)
        {
            elements.Remove(d);
        }

        public override void Display(int indent)
        {
            Console.WriteLine(new String('-', indent) +
              "+ " + _name);

            // Display each child element on this node

            foreach (DrawingElement d in elements)
            {
                d.Display(indent + 2);
            }
        }
    }

    /////////////////////////////////////////////////*FACADE*////////////////////////////////////////////////

    class Time   // TIME
    {
        public void Timer()    //Time()
        {
            Console.WriteLine("Start session");   //START AND END SESSOIN
        }

    }


    class Password         //PASWORD
    {
        public void Execute()
        {
            Console.WriteLine("System password");    //System password
        }
        public void Finish()
        {
            Console.WriteLine("Pasword is success");   //Pasword is success
        }
    }


    class ReportSystemFacade  //
    {
      
        Time time; 
        Password password;    


        public ReportSystemFacade(Time ti, Password pass)
        {

            this.time = ti;
            this.password = pass;
        }

        public void Start()
        {

            time.Timer();
            password.Execute();
        }

        public void Stop()
        {
            password.Finish();
        }

        public void CreateReport()
        {
            Console.WriteLine("Create a report ");// CREATE REPORT 
        }
        public void Save()
        {
            Console.WriteLine("Save report");//SAVE REPORT 
        }

    }


    class Manager
    {
        public void CreateApplication(ReportSystemFacade facade)
        {
            facade.CreateReport();
            facade.Start();
            // facade.End();
            facade.Stop();
            facade.Save();

        }
    }

    /////////////////////////////////////////////////*DECORATOR*////////////////////////////////////////////////

    abstract class Sale  //Sale 
    {
        public Sale(string n)
        {
            this.Name = n;
        }
        public string Name { get; protected set; }
        public abstract int GetCost();
    }

    class PromocodeSale : Sale    //PromocodeSale:Sale
    {
        public PromocodeSale() : base("PromocodeSale")  //PromocodeSale():base 
        { }

        public override int GetCost()
        {
            return 20;
        }
    }
    class AprilSale : Sale  //AprilSale:Sale
    {
        public AprilSale() //AprilSale()
            : base("AprilSale")
        { }
        public override int GetCost()
        {
            return 8;
        }
    }

    abstract class SaleDecorator : Sale    
    {
        protected Sale sale; 
        public SaleDecorator(string n, Sale sale) : base(n)  
        {
            this.sale = sale;
        }
    }


    class BlackPromokode : SaleDecorator  
    {
        public BlackPromokode(Sale s)
            : base(s.Name + ", with Black promocode", s)
        { }

        public override int GetCost()
        {
            return sale.GetCost() + 3;
        }
    }

    class InstagramPromocode : SaleDecorator   //InstagramPromocode
    {
        public InstagramPromocode(Sale s)             //InstagramPromocode(Sale s)
            : base(s.Name + ", with Instagram promocode", s)
        { }

        public override int GetCost()
        {
            return sale.GetCost() + 5;
        }
    }
}