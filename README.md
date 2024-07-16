# What is Postman?

**Postman** is a simple library built to solve complex issues surrounding queues and event processing.
Queues are used for any situation where you want to effectively mainain a _first in first out_ order.

A queue is a linear structure that performs operations in a predetermined order. The order in _first in, first out_. A queue is defined as a group of messages waiting for a reso
uce, with the message who arrived first being served first.

The type of queue used in this project is a **simple or linear queue**. This queue is used to solve problems with sequential processing.

## RabbitMq
**Postman** uses **rabbitMq** as the underlying transport agent. When working in your local environment, you have a number of options to connect to an instance of rabbitMq.

### Windows users:
Install rabbitMq as a service by following the steps mentioned in [this article](https://www.rabbitmq.com/install-windows.html).

If using this option, you'll have to manage the service, and nodes manually using various commands such as `rabbitmqctl.bat start/stop`, `rabbitmq-diagnostics`, etc.

### Linux users:
Make sure that you have an instance of `docker` engine installed and running. Clone the repository, and

Copy the **definitions.json.example** file by `cp .init/definitions.json.example .init/definitions.json`. The json file is ignored in git. You can customize all of your rabbitmq config changes here.

Perform `sudo docker compose up -d` to create and start the container. You can access the management-ui at this point.

__This will pull the latest `rabbitmq3` image from docker-hub, install the container, and start it for you.__

rabbitmq uses `rabbit_password_hashing_sha256`. To has a password, use `python` to execute `python .pwd/rabbit_pass_hash.py "mypassword"`. This will provide you with the hash va
lue of your password. You may use the hash value in the `user` object inside of the `definitions.json`.

### Shared instance:
Connect to the shared instance that is currently up &amp; running in the qat environment.

- host: `https://b-4713af5d-bf06-4902-8504-9610fb0e6619.mq.us-east-1.amazonaws.com/`
- user: `nsb-eval-user`
- password: `Me$$ageQR0cks`

Please note that you'll be sharing this resource with other engineers, applications, and resources. This is going to impact the overal performance, and reliability of your work.
 It is **recommended** to use this as the last resort.

### Re-Quening &amp; delivery

If an exception is thrown by the consumer, and if you'd like the broker to requeue your message, make sure to use `throw new QueueMessageProcessingException(isRecoverable: true,
 message)`. This action invokes the `BasicReject(@deliveryTag, true)`.

## Configuration
In your projects' `appsettings.json`, add the following setting under the root element.

You can use the legend to determine the best value for each of the properties below:

- `Username` - This is the username designated in your instance of rabbitMq. It can be the admin user, though it's not recommended, or a user that was created with proper privil
eges.
- `Password` - The password that was designated for the user.
- `Host` - If local, use `localhost`; else, you have the use the shared host name provided above.
- `VirtualHost` - If supplied, it means that your application will be using a [virtual host](https://www.rabbitmq.com/vhosts.html).

```json
{
  "RabbitMQ": {
    "Username": "USER_NAME",
    "Password": "USER_PASSWORD",
    "Host": "localhost",
    "VirtualHost": "/",
    "UseSeparateQueuesPerEvent": true
  }
}
```
