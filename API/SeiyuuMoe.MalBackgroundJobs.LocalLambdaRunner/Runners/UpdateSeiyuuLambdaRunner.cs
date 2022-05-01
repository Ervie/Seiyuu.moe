using Amazon.Lambda.SQSEvents;
using SeiyuuMoe.Domain.SqsMessages;
using SeiyuuMoe.MalBackgroundJobs.Lambda.Function;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace SeiyuuMoe.MalBackgroundJobs.LocalLambdaRunner.Runners;

public class UpdateSeiyuuLambdaRunner
{
    public async Task RunAsync()
    {
        var lambda = new UpdateSeiyuuLambda();

        var message = new UpdateSeiyuuMessage
        {
            Id = Guid.Parse("9738c977-6eab-447b-8f6a-3f0bc48b111e"),
            MalId = 212
        };

        var sqsMessage = new SQSEvent
        {
            Records = new List<SQSEvent.SQSMessage>
            {
                new SQSEvent.SQSMessage
                {
                    Body = JsonSerializer.Serialize(message)
                }
            }
        };

        await lambda.InvokeAsync(sqsMessage);
    }
}