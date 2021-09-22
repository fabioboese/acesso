# Preparação do ambiente

Para iniciar a infraestrutura necessária à execução do projeto, execute os seguintes comandos para carregar os containers da API de conta, do MongoDB e do RabbitMQ.

```
	docker run -d -p 5000:80 baldini/testacesso
	
	docker run -d -p 27017:27017 mongo
	
	docker run -d -p 15671:15671 -p 5672:5672 -p 25676:25676 -p 15672:15672 rabbitmq:3-management
```

# Bugs Conhecidos
Ao tentar executar um teste de stress realizando as chamadas para a API de conta, o seguinte erro ocorreu
    
    An error occurred while sending the request
    The response ended prematurely

Não foi possível identificar a causa do erro pois a falta do código fonte impediu uma análise mais aprofundada.
Ao substituir a chamada para a API por resultados aleatórios de true/false, a aplicação conseguiu responder 500 requisições em pouco menos de 2s. 
Esse cenário foi realizado com webserver, rabbitmq, mongodb e console de teste rodando localmente em uma máquina com 16GB de RAM.


# Resposta à questões da Fase 2

1.	Qual a arquitetura de solução você criaria para suportar esta nova funcionalidade?
	Pela necessidade de escala e visando simplificar algumas questões de gerenciamento da infraestrutura, sugeriria uma arquitetura Cloud Serverless.
	Seriam desenvolvidos 3 Lambdas sendo o primeiro para retirada dos recursos financeiros da conta origem, o segundo para realizar a comunicação com a API do BACEN e o terceiro para devolução dos recursos caso ocorra algum erro na comunicação com o BACEN.
	Todos os logs seriam registrados no Cloudwatch e todo o processo de retirada dos recursos financeiros minunciosamente registrado a cada estágio no DynamoDb
	A verificação dos agendamentos seria realizado pelo EventBridge a cada minuto e a comunicação com os Lambdas seria executada pelo SNS.
   
2. 	Quais tecnologias seriam utilizadas?
	EventBridge, Lambda, SNS, DynamoDb, API Gateway, Cloudwatch
   
3. 	Como a solução é escalável?
	Como a solução proposta é totalmente Serverless a infraestrutura proporcionada e os SLAs oferecidos podem dar conta de um bom crescimento da demanda.
	Uma forma de escalar é particionar as operações programadas por um hash que as divida, por exemplo, em 10 grupos e realizar o processamento desses 10 grupos em paralelo, aumentando a quantidade de lambdas instanciados para dar conta do volume de notificações do SQS.
   
4. 	As operações serão síncronas ou assíncronas?
	Após a identificação das transações que devem ser enviadas em um determinado minuto, todas as solicitações são agrupadas por origem e enviadas para um SNS que inicia uma Lambda e, neste cenário, uma quantidade de instâncias paralelas serão iniciadas para processar o workload que está sendo recebido pelo SNS.
	Cada lambda deve executar o seu trabalho de forma síncrona, pois precisa manter uma transação no momento da retirada dos recursos da conta.
	Todo o trabalho realizado pelo Lambda deve ser registrado no banco, com o seu estágio de execução de modo que seja possível recuperar-se de uma eventual falha.
	Portanto, os seguintes estágios, ao menos, devem estar presentes:
	1 - Início do processamento 
	2 - Verificação da disponibilidade de recursos
	3 - Retirada de recursos da conta origem iniciada
	4 - Retirada de recursos da conta origem finalizada
	5 - Lançamento de novo evento no SNS indicando RETIRADA CONCLUIDA
	6 - Fim do processamento do lambda
	Então, o Lambda registra um evento de acordo com o tipo de transferência solicitada (PIX ou TED)
	Nesse momento o SNS acionará um novo lambda responsável, no caso do PIX, por comunicar-se com a API do BACEN, realizando o PIX conforme os dados contidos no evento que o disparou. 
	Esse lambda também registraria ao final no DynamoDb a falha ou sucesso da operação. No caso de falha poderia disparar um outro Lambda para realizar a devolução dos recursos financeiros
   
5. 	Este for utilizar algum banco de dados, qual será utilizado?
	Para o registro das transações, sugiro a utilização de um banco NoSql com capacidade de atender um alto throughput como o DynamoDb.
	Caso haja necessidade de utilizar tais informações para algum tipo de análise, adicionaria uma integração que poderia ser near-realtime plugando Lambdas aos eventos gerados por cada inserção ou alteração de documento no DynamoDb ou jobs no AwsBatch responsáveis por enviar as informações para o Redshift, por exemplo.
	
6. 	Qual é a sua proposta de entrega para cumprir os prazos?
	Entregar a solução do PIX que engloba uma boa parte de todos os requisitos necessários para atender ao TED, pensando em utilizar a mesma estrutura para o TED ao término da entrega do PIX.
	Assim colocarímos o PIX em uma Sprint menor e o colocaríamos em produção a tempo de atender o calendário do BACEN.
	No desenvolvimento do TED o SNS acionado ao final da retirada de recursos seria um outro e, portanto, acionaria outro lambda que faria a comunicação necessária para o registro do TED. Como o desenvolvimento dessa novo lambda não impactaria no que já foi entregue do PIX, a mudança poderia ser realizada com uma boa segurança e sem a necessidade de manutenção em uma estrutura que já está em produção.