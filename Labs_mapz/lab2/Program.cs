using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Project_for_abstract_factory
{
    class Program
    {
        class Singleton
        {
            private Singleton() { }
            private static Singleton _instance;
            //обєкт блокування для синхронізації потоків для першого доступу до сінгл.
            private static readonly object _lock = new object();

            public static Singleton GetInstance(string value)
            {
                if (_instance == null)
                {
                    lock (_lock)
                    {  
                        if (_instance == null)
                        {
                            _instance = new Singleton();
                            _instance.Value = value;
                        }
                    }
                }
                return _instance;
            }
            public string Value { get; set; }
        }

        class Password  //Password
        {
            private Password() { }          //private Password()    завжди повинен бути прайвет

            private static Password _instance;      //private static Password _instance;   обєкт зберігається в статичному полі класу. 

            public static Password GetInstance()         //альтернативний конструктор, який дозволяє отримувати один і той самий екземпляр класу при кожному виклику
            {
                if (_instance == null)
                {
                    _instance = new Password();          //new Password();
                }
                return _instance;
            }


            public void Print()
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("*******Congratulation, you logged in!*******\n");
            }
        }


        static void Main(string[] args)
        {
            //--------------------------------/*SINGLTON_1*/--------------------------------
            Thread process1 = new Thread(() =>
            {
                TestSingleton("DataBase is connected! ");
            });
            process1.Start();
            process1.Join();
            //--------------------------------/*SINGLTON_2*/--------------------------------

            Password p1 = Password.GetInstance();
            p1.Print();


            //--------------------------------/*BUILDER*/-------------------------------
            Console.ForegroundColor = ConsoleColor.DarkYellow ;
            Console.WriteLine(" ################################################## BUILDER ###########################################################");
            Master master = new Master();
            //створюю білдера для педикюрю
            ProcedureBuilder builder = new PedicureProcedureBuilder();
            Console.ForegroundColor = ConsoleColor.Yellow;
            
            Procedure pedProcedure = master.Make(builder);//майстер працює
            Console.WriteLine(pedProcedure.ToString());

            Console.ForegroundColor = ConsoleColor.Yellow;
            //створюю білдера для манюкюру
               
            builder = new ManicureProcedureBuilder();

            Procedure manProcedure = master.Make(builder);

            Console.WriteLine(manProcedure.ToString());


            //--------------------------------/*ABSTRACT_FACTORY*/--------------------------------

            Console.ForegroundColor = ConsoleColor.DarkMagenta;
            Console.WriteLine("##############################################  ABSTRACT FACTORY ###################################################### ");

            Console.ForegroundColor = ConsoleColor.Magenta;
            Client client1 = new Client(new SpringFactory());
            client1.GetRegSale();

          
            Client client2 = new Client(new WinterFactory());
            client2.GetRegSale();


            Client client3 = new Client(new AutumnFactory());
            client3.GetRegSale();

            Client client4 = new Client(new SummerFactory());
            client4.GetRegSale();





            Console.ReadLine();
        }
        public static void TestSingleton(string value)
        {
            Singleton singleton = Singleton.GetInstance(value);
            Console.WriteLine(singleton.Value);
        }
    }
}

//##############################################  ABSTRACT FACTORY ###################################################### 

abstract class SeasonSale     //абстрактний клас знижки (визначає методи для ств обєктів)
{
    public abstract void GetRegSale();
}


class ClientWithSpringSale: SeasonSale     //клас клієнт з весняною знижкою 
{
    public override void GetRegSale()
    {
        Console.WriteLine(" Client with SpringSale get spring sale!");
    }
}

class ClientWithWinterSale : SeasonSale      //клас клієнт із зимовою знижкою
{
    public override void GetRegSale()
    {
        Console.WriteLine(" Client with WinterSale get winter sale!");
    }
}




class ClientWithAutumnSale : SeasonSale      //клас клієнт із осінньою знижкою
{
    public override void GetRegSale()
    {
        Console.WriteLine(" Client with AutumnSale get autumn sale!");
    }
}

class ClientWithSummerSale : SeasonSale      //клас клієнт із літньою знижкою
{
    public override void GetRegSale()
    {
        Console.WriteLine(" Client with SummerSale get summer sale!");
    }
}


//class abstr factory ( визначає методи для створення об'єктів)
abstract class ClientFactory
{
    public abstract SeasonSale CreateSale();
}

//фабрика створює клієнта зі знижками((конкретні продукти)

class SpringFactory : ClientFactory
{
    public override SeasonSale CreateSale()
    {
        return new ClientWithSpringSale();
    }
}

class WinterFactory : ClientFactory
{
    public override SeasonSale CreateSale()
    {
        return new ClientWithWinterSale();
    }
}


class AutumnFactory : ClientFactory
{
    public override SeasonSale CreateSale()
    {
        return new ClientWithAutumnSale();
    }
}

class SummerFactory : ClientFactory
{
    public override SeasonSale CreateSale()
    {
        return new ClientWithSummerSale();
    }
}

//use factory for create object
class Client 
{
    private SeasonSale sale;
    public Client(ClientFactory factory)
    {
        sale = factory.CreateSale();
    }


    public void GetRegSale()
    {
        sale.GetRegSale();
    }
}

//---------------------------------/*BUILDER*/------------------------
// абстрактный класс будівельника
abstract class ProcedureBuilder
{
    public Procedure Procedure { get; private set; }

    public void CreateProcedure()
    {
        Procedure = new Procedure();
    }

    public abstract void SetClean();
    public abstract void SetGel();
    public abstract void SetAdditives();
}


class Master
{
    public Procedure Make(ProcedureBuilder procedureBuilder)
    {
        procedureBuilder.CreateProcedure();
        procedureBuilder.SetClean();
        procedureBuilder.SetGel();
        procedureBuilder.SetAdditives();
        return procedureBuilder.Procedure;
    }
}


// ! будівельник для педикюру !

class PedicureProcedureBuilder : ProcedureBuilder
{
    public override void SetClean() //чистка  SetClean()
    {
        this.Procedure.Clean = new Clean { Sort = "*Pedicure clean*" };
    }

    public override void SetGel()
    {
        this.Procedure.Gel = new Gel();
    }

    public override void SetAdditives()
    {
        this.Procedure.Additives = new Additives { Name = "-Additional design for pedicure:french or tinsel" };
    }
}

//  !будівельник  для процедури !

class ManicureProcedureBuilder : ProcedureBuilder
{
    public override void SetClean()
    {
        this.Procedure.Clean = new Clean { Sort = "*Manicure clean*" };   //процедура.чистка (вид: фрезером) //this.Procedure.Clean=new Clean{Sort=...};
    }

    public override void SetGel()
    {
        this.Procedure.Gel = new Gel();
    }

    public override void SetAdditives()   //   блискітки або олійка для нігтів 
    {
        this.Procedure.Additives = new Additives { Name = "-Additional design for manicure: tinsel or french" };   //процедура.декор=декор (додатковий декор)
    }
}


class Clean  //чистка   
{

    public string Sort { get; set; }    //type вид 
}



class Gel  // гель лак  class Gel
{ }



class Additives
{
    public string Name { get; set; }
}


class Procedure//процедура 
{

    public Clean Clean { get; set; }    //чистка  public Clean Clean

    public Gel Gel { get; set; }   //гель лак   public Gel Gel


    //                 блискітки або олійка для нігтів
    public Additives Additives { get; set; }


    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        if (Clean != null)    //чистка       
            sb.Append(Clean.Sort + "\n");  //чистка     



        if (Gel != null)    //гель лак    Gel
            sb.Append(" *Gelpolish for nails*\n");    //гель лак    



        if (Additives != null)       // блискітки або олійка для нігтів
            sb.Append("-Additional design for nails: " + Additives.Name + " \n");
        return sb.ToString();
    }
}
