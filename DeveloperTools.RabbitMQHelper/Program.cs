// See https://aka.ms/new-console-template for more information

using DeveloperTools.RabbitMQHelper;

Console.WriteLine("RabbitMQ HandyTools!");


Console.WriteLine("Select Process: ");
Console.WriteLine("1- Purged All Queues");
Console.WriteLine("2- Purged All Queues except Error Queues");

var key = Console.ReadLine();
switch (key)
{
    case "1":
        Console.WriteLine("Start Purge All Queues");
        await Helper.PurgeAllQueue();
        break;
    case "2":
        Console.WriteLine("Start Purge All Queues Without Error Queues");
        await Helper.PurgeAllQueue(true);
        break;
    default:
        Console.WriteLine("Invalid Key");
        break;
}







