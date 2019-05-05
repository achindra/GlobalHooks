using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceBus.Messaging;

namespace StressLogger
{
    class ServiceBus
    {
        //RootManageSharedAccessKey
        //Endpoint=sb://stresslogger.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iXdKnQpKjzuvIMiQf7RIzejZuZORaatfyHUcQWiuysM=
        //Sender:Endpoint=sb://stresslogger.servicebus.windows.net/;SharedAccessKeyName=SDQSender;SharedAccessKey=PPp316dthZsmum5+iQNGV6B4MTioFE/l1/6rWdlnaoM=;EntityPath=stressdataqueue
        //Receiver:Endpoint=sb://stresslogger.servicebus.windows.net/;SharedAccessKeyName=SDQReceiver;SharedAccessKey=Wqcebxbx0MXuwRLp9HrqqkwJPrVEAuyUrWEF+fN47c8=;EntityPath=stressdataqueue
        public static void Demo()
        {
            var connectionString = "sb://stresslogger.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=iXdKnQpKjzuvIMiQf7RIzejZuZORaatfyHUcQWiuysM=";
            var queueName = "stressdataqueue";

            var client = QueueClient.CreateFromConnectionString(connectionString, queueName);
            var message = new BrokeredMessage("TestMessage");
            client.Send(message);
        }
    }
}
