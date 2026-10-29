using System.Linq.Expressions;
using System.Runtime.InteropServices.Marshalling;

namespace ThreadDemo.Domain;

public class ThreadExperiments
{




    public void RunSequential()
    {
        Work("A");
        Work("B");
    }

    public void RunThreads()
    {
        // lambda function to assign a certain work to a thread in his constructor
        Thread thread1= new(()=>Work("A"));
        Thread thread2= new(()=>Work("B"));
        Console.WriteLine("Starting threads...");
        
        //this is a non deterministic run
        thread1.Start();
        thread2.Start();

        Thread.Sleep(1);

        Console.WriteLine("this line is written by main thread");


        //this is a deterministic run
        thread1.Join();//waiting for thread to finish
        thread2.Join();
    }

    public void ShowThreadProperties()
    {

        Thread thread = Thread.CurrentThread;
        Console.WriteLine($"Thread ID: {thread.ManagedThreadId}");
        Console.WriteLine($"Thread Name: {thread.Name}");
        Console.WriteLine($"Thread Priority: {thread.Priority}");
        Console.WriteLine($"Is Background Thread: {thread. IsBackground}");
        Console.WriteLine($"Is Thread Pool Thread: {thread.IsThreadPoolThread}");
    }


//avere una risorsa condivisa su cui più thread può essere 
//pericoloso in quanto potrebbero esserci problemi 
// sull'aggiornamento della variabile condivisa

//1°test: i thread contano sulla stessa varabile condivisa senza sincronizzazione
//2° test: i thread contano sulla stessa varabile condivisa con sincronizzazione
//il metodo ritorna 2 valori(una tupla) -> (Expected, Actual)
    public (int Expected, int Actual) RunCounter(bool useLock, int iterations=100_000)
    {
        int counter =0;
        object gateLock= new();
        

        //function used by our thread 
        void Increment()
        {
            for(int i=0; i<=iterations; i++)
            {
                //mi serve un accesso in mutua esclusione per ottenere una soluzione deterministica
                if (useLock)
                {
                    //sezione critica
                    lock (gateLock)
                    {
                        counter+=1;
                    }
                }
                else
                {
                    counter+=1;
                }
                
                
            }
        }

        Thread t1 = new(Increment);
        Thread t2 = new(Increment);

        t1.Start();
        t2.Start();

        t1.Join();
        t2.Join();

        return (iterations*2, counter);
    }

    





    //funzione per simulare del lavoro
    private static void Work(string name)
    {
        for (int i=0; i<=5; i++)
        {
            Console.WriteLine($"{name}: Thread: {Thread.CurrentThread.ManagedThreadId}; Index {i}");
            Thread.Sleep(100);
        }
    }

}
