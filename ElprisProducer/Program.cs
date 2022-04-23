using Confluent.Kafka;
using System.Net;
using ElprisProducer;
using Newtonsoft.Json;
//ElprisProducer:
//Is waiting for requests on requestelpris.
//Reply is send bask on ReplyTopic.

var produceConfig = new ProducerConfig
{
    //BootstrapServers = "192.168.126.128",
    BootstrapServers = "Ubuntu2004",
    ClientId = Dns.GetHostName(),
};

var consumeConfig = new ConsumerConfig
{
    //BootstrapServers = "192.168.126.128:9092",
    BootstrapServers = "Ubuntu2004:9092",
    GroupId = "Gruppe",
    AutoOffsetReset = AutoOffsetReset.Latest,
};


var logit = new SimpleLogging();

//????
//var tempVal = await RequestElPris();
//var tempReplydata = tempVal.ToString();
//????

using (var consumer = new ConsumerBuilder<Ignore, string>(consumeConfig).Build())
{
    consumer.Subscribe("requestelpris");

    Console.WriteLine("Elpris Producer klar til at modtage beskeder");
    logit.Log("Elpris Producer klar til at modtage beskeder");
    while (true)
    {
        try
        {
            var consumeResult = consumer.Consume();
            string message = consumeResult.Message.Value;

            var request = new MessageRequest();
            request = JsonConvert.DeserializeObject<MessageRequest>(message);

            Console.WriteLine($"I producer er besked request: {request.MessageId} modtaget.");
            logit.Log($"Request: {request.MessageId} modtaget.");
            if (request.RequestType == "ElprisRequest")
            {
                Console.WriteLine($"I producer - Vil hente ElData.");
                logit.Log($"Vil hente ElData.");
                //????
                var tempVal = await RequestElPris();
                var tempReplydata = tempVal.ToString();
                //????
                MessageReply replyMessage = new MessageReply()
                {
                    MessageId = Guid.NewGuid(),
                    ReplyOnRequestId = request.MessageId,
                    CreationDateTime = DateTime.Now, 
                    Replydata = tempReplydata // "Her skal elprisdata være"
                    //Replydata = tempVal.ToString()
                };

                Console.WriteLine($"Producer svarer på request: {replyMessage.ReplyOnRequestId} med {replyMessage.MessageId}");
                logit.Log($"Svarer på request: {replyMessage.ReplyOnRequestId} med {replyMessage.MessageId}");
                var jsonMessageReply = JsonConvert.SerializeObject(replyMessage);

                using (var producer = new ProducerBuilder<Null, string>(produceConfig).Build())
                {
                    await producer.ProduceAsync(request.ReplyTopic, new Message<Null, string> { Value = jsonMessageReply });
                }
                Console.WriteLine("Producer - Ended on reply");
                logit.Log($"Ended on reply {replyMessage.ReplyOnRequestId} med {replyMessage.MessageId}");
            }
        }
        catch (Exception)
        {
            Console.WriteLine("Producer har bøvl");
            logit.Log($"Producer har bøvl!!");
        }
    }
    consumer.Close();
}

async Task <double> RequestElPris()
{
    MessageRequest request = new MessageRequest()
    {
        MessageId = Guid.NewGuid(),
        ReplyTopic = "publisheldata",
        RequestType = "ElprisDataRequest",
        CreationDateTime = DateTime.Now
    };
    var jsonRequest = JsonConvert.SerializeObject(request);

    var messageReply = new MessageReply();

    Console.WriteLine($"Elpris Producer - Started on ElprisDataRequest. Sender message {request.MessageId}");
    logit.Log($"Started on ElprisDataRequest. Sender message {request.MessageId}");
    using (var producer = new ProducerBuilder<Null, string>(produceConfig).Build())
    {
        await producer.ProduceAsync("requesteldata", new Message<Null, string> { Value = jsonRequest });
    }
    Console.WriteLine($"Ended requesting ElprisData");
    logit.Log("Ended requesting ElprisData");

    Console.WriteLine($"Started waiting on reply of ElprisData on {request.MessageId}");
    logit.Log($"Started waiting on reply of ElprisData on {request.MessageId}");
    using (var consumer = new ConsumerBuilder<Ignore, string>(consumeConfig).Build())
    {
        consumer.Subscribe("publisheldata");

        Console.WriteLine($"ElprisData Requester venter på reply på {request.MessageId}");
        logit.Log($"ElprisData Requester waiting for reply on {request.MessageId}");
        bool gotARelpy = false;
        while (!gotARelpy)
        {
            try
            {
                var consumeResult = consumer.Consume();
                string message = consumeResult.Message.Value;
                //Console.WriteLine($"ElprisData Requester har modtaget dette reply: {message}");
                logit.Log($"ElprisData Requester has received a reply.");
                //var messageReply = new MessageReply();
                messageReply = JsonConvert.DeserializeObject<MessageReply>(message);
                //Console.WriteLine($"Selve reply data: {messageReply.Replydata} som svar på {messageReply.ReplyOnRequestID}");
                Console.WriteLine($"ElprisData Requester: Selve reply som svar på {messageReply.ReplyOnRequestId}");
                logit.Log($"ElprisData Requester: The reply on {messageReply.ReplyOnRequestId}");
                if (messageReply.ReplyOnRequestId == request.MessageId)
                {
                    gotARelpy = true;
                    Console.WriteLine($"ElprisData Requester: ReplyOnRequestId: {messageReply.ReplyOnRequestId} & MessageId: {request.MessageId}");
                    logit.Log($"ElprisData Requester: ReplyOnRequestId: {messageReply.ReplyOnRequestId} & MessageId: {request.MessageId}");
                }
            }
            catch (Exception)
            {
                Console.WriteLine($"ElprisData Requester har bøvl");
                logit.Log($"ElprisData Requester: bøvl!!");
            }
        }
        consumer.Close();
    }

    try
    {
        logit.Log("Will find the actual elpris.");
        //PriceParser parser = new(json);
        PriceParser parser = new(messageReply.Replydata);
        logit.Log("Will find the actual elpris - Done the parser");

        DateTime now = DateTime.Now;
        TimeOnly time = new(now.Hour, 00);
        DateTime thisHour = DateOnly.FromDateTime(now).ToDateTime(time);

        var price = parser.GetWestPrice(thisHour);
        Console.WriteLine($"Den aktuelle pris er {price} per kWh");
        logit.Log($"Den aktuelle pris er {price} per kWh");
        return (double)price;
    }
    
    catch (Exception)
    {
        logit.Log($"ElprisData Requester: !! -- Standard price used -- !!");
        return 1.23; //Sometimes the pricefield of the recieved pricedata is null.
    }
}