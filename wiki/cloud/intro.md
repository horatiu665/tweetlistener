# Cloud idea

In order to cover more ground and gather larger amounts of data than is currently possible, a cloud solution is sought.

## AWS

Amazon Web Services (AWS) offers scalable cloud computing and storage solutions. The simplest step seems to be creating a [free account which offers some amount of free usage](http://aws.amazon.com/free/), however it can only be used by providing credit card/bank account details, and preparing to be billed unexpectedly by exceeding the free limits of the service. A big nasty [thread states that there is apparently no service for limiting the budget for AWS](https://forums.aws.amazon.com/thread.jspa?threadID=58127) for a few years, and therefore it is possible that without great care, the price of services used can skyrocket (since services are scalable automatically, and even tests/tutorial projects are billed).

It is possible that the price for the AWS is cheaper than the Azure once real deployment is done, but for now it is out of the scope of the project to spend money on this type of test.

## Microsoft Azure

Azure offers the billing limit mentioned above, called [spending limit](http://azure.microsoft.com/en-us/pricing/spending-limits/), and it simply shuts down services when the prices exceed the specified limit. This allows for tests and debug sessions without risk of spending ridiculous amounts by mistake. Therefore this makes it possible to test the deployment of applications on the server.

[Azure offers a free trial](https://azure.microsoft.com/en-us/pricing/free-trial/) which offers $ 200 for testing purposes for the first month. There is a possibility for a free service for students, but it is very limited and does not offer virtual machines (VMs), therefore it is not suitable for this experiment.

### VM listener
The first test using Azure VMs involved the creation of the most basic (and one of the cheapest) VMs, belonging to the [A0 tier](http://azure.microsoft.com/en-us/pricing/details/virtual-machines/) which offers little resources (enough for our purposes) for the estimated price of 90 dkk/month. This allows a remote connection to a machine running Windows Server 2012, to which the TweetListener software can be transferred and run. The software requirements are unknown, thus not suitable for en-masse deployment yet, but a test can be made once the software is capable of directly connecting to a database (for now it only connects via a php script on a (local) server). Another option is to run the tweet gathering software only using .txt backup, but this is not a suitable option for long-term, since all results must then be manually uploaded to a database for further manipulation.

A test was started on 14-09-2015 at 15:38, using one instance of the TweetListener connected to a sample stream (1% of Twitter Firehose), to stress test the server with a minimal task. Results will likely be evaluated after ~18 hours. 
