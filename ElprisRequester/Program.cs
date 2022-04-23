using Confluent.Kafka;
using System.Net;
using ElprisRequester;
using Newtonsoft.Json;

//ElprisRequester:
//Requests for a elprice is send on requestelpris.
//Reply is waited for on publicelpris.

var consumeConfig = new ConsumerConfig
{
    //BootstrapServers = "192.168.126.128:9092",
    BootstrapServers = "Ubuntu2004:9092",
    GroupId = "Gruppe",
    AutoOffsetReset = AutoOffsetReset.Latest,
};

var produceConfig = new ProducerConfig
{
    //BootstrapServers = "192.168.126.128",
    BootstrapServers = "Ubuntu2004",
    ClientId = Dns.GetHostName(),
};

MessageRequest request = new MessageRequest()
{
    MessageId = Guid.NewGuid(),
    ReplyTopic = "publicelpris",
    RequestType = "ElprisRequest",
    CreationDateTime = DateTime.Now,
};
var jsonRequest = JsonConvert.SerializeObject(request);

Console.WriteLine($"Started on requesting with the MedssageId: {request.MessageId}");
using (var producer = new ProducerBuilder<Null, string>(produceConfig).Build())
{
    await producer.ProduceAsync("requestelpris", new Message<Null, string> { Value = jsonRequest });
}
Console.WriteLine("Ended requesting");


using (var consumer = new ConsumerBuilder<Ignore, string>(consumeConfig).Build())
{
    consumer.Subscribe("publicelpris");

    Console.WriteLine("Requester venter på reply.");

    while (true)
    {
        try
        {
            var consumeResult = consumer.Consume();
            string message = consumeResult.Message.Value;
            //Console.WriteLine($"Requester har modtaget dette reply: {message}");

            var messageReply = new MessageReply();
            messageReply = JsonConvert.DeserializeObject<MessageReply>(message);
            Console.WriteLine($"messageReply har messageid: {messageReply.MessageId} der svarer på {messageReply.ReplyOnRequestID}");
            Console.WriteLine($"Selve reply data: {messageReply.Replydata}");
        }
        catch (Exception)
        {
            Console.WriteLine($"Requester har bøvl");
        }
    }
    consumer.Close();
}

