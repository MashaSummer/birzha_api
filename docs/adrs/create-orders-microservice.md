# Микросервис заявок

## Status

Предложение

## Context

Необходимо реализовать микросервис лимитных заказов на продажу (аск) и покупку (бид) в соотвествии со  следующими требованиями:
+ [Требования по созданию лимитной заяки аск (продажу)](https://docs.google.com/document/d/1e60-ou9c1p_JRWwZ613XFqYKoPQC-Yh2iyGsChzqrhU/edit?pli=1#)
+ [Требования по созданию лимитной заявки бид (покупку)](https://docs.google.com/document/d/1ZkI4wA1G_JldqxuCtSxJlWan9AYKG8fxNPMfGt1NUOU/edit#heading=h.erirw953pvj1)

Данный сервис не будет выполнять функции, связанные с исполнением завки бид, прописанные в [требованиях](https://docs.google.com/document/d/1ZkI4wA1G_JldqxuCtSxJlWan9AYKG8fxNPMfGt1NUOU/edit#heading=h.erirw953pvj1), т.к. это выходит за его зону ответственности. За исполнение будет отвечать микросервис транзакций

## Decision

### <b>Создание нового микросервиса</b>
---
Будет создан один новый микросервис (физически), который будет разделять комманды записи и чтения

### <b>Создание новой заявки</b>
---
Сервис заявок будет принимать запросы на создания заявки определенного типа, проверять возможность создания заявки (проверка портфеля пользователя или баланса), в случае положительной проверки будет создана в базе новая заявка, в базе событий (EventStore) появится запись о создании новой заявки, а в Kafka пойдет сообщение о том, что была создана новая заявка. Сервисы баланса и портфеля будут получать это сообщение и замораживать активы необходимые для данной заявки

### <b>Как проходит валидация заявки</b>
---
Сервис заявок отправляет сообщение в Kafka о запросе портфеля (или баланса, в зависимости от типа заявки) пользователя, сохраняет данные о заявки в базу со статусом `validation` и продолжает работу. После получения ответа сервис комманд проверяет возможно ли создать такую заявку


### <b>Общение между сервисами</b>
---
+ <b>Сервис заявок</b> будет принимать запросы на создания заявки по <b>grpc</b> протоколу
+ <b>Сервис заявок</b> будет принимать запросы на чтение по <b>grpc</b>
+ <b>Сервис заявок</b> общается с другими сервисами при помощи <b>Kafka</b>

### <b>Какие методы будут реализованы?</b>
---
На фасаде:  
+ Метод создания новой заявки опредленного типа  

На сервисе заявок:
+ Метод создания новой завяки определенного типа
+ Метод получения всех заявок по определенному <b>пользователю</b>
+ Метод получения всех заявок по определенному <b>товару</b>

### <b>Идемпотентность</b>
---
Для обеспечения идемпотентности запросов на создание новой заявки мы будем использовать ключ идемпотентности. Клиент прежде чем отправить запрос на создание новой заявки должен сделать запрос на фасад для получения своего ключа идемпотентности и добавить его в запрос

### <b>Предлагаемые proto</b>
---
Для запроса на фасад
```protobuf
service OrdersService {
    rpc CreateOrder(CreateOrderRequest) returns (CreateOrderResponse);
    rpc GetOrderKey(KeyRequest) returns (KeyResponse)
}

KeyRequest {}

KeyResponse {
    string key = 1;
}

enum OrderType {
    ASC = 0;
    BID = 1;
}
message DateTime {
    
    int64 sec = 0;
    int32 minute = 1;
    int32 hour = 2;
    int32 day = 3;
    int32 month = 4;
    int32 year = 5;
}
message CreateOrderRequest {
    
    OrdertType type = 1;
    
    string product_id = 2;
    
    double volume = 3;
    
    double price = 4;
    
    bool only_full_execution = 5;

    DateTime deadline = 6;

    string guid = 7;
}
message Success {
    string success_text = 1;
}
message Error {
    
    string error_text = 1;
    string stack_trace = 2;
}
message CreateOrderResponse {
    oneof result {
        Success success = 0;
        Error error = 1;
    }
}
```

Для запроса на сервис заявок
```protobuf
service OrdersService {
    rpc CreateOrder(CreateOrderRequest) returns (CreateOrderResponse);
}
enum OrderType {
    // look above
}
message DateTime {
    // look above
}
message CreateOrderRequest {
    
    OrdertType type = 1;
    
    string product_id = 2;
    
    double volume = 3;
    
    double price = 4;
    
    bool only_full_execution = 5;

    DateTime deadline = 6;

    string order_guid = 7;

    string investor_id = 8;
}
message CreateOrderResponse {
    // look above
}
```

### <b>Предлагаемый формат хранения заявок в базе</b>
---
```json
"order" : {
    "_id" : "1111-2222",
    "type" : "bid" | "ask",
    "product_id" : "3232-2323",
    "volume" : 1000,
    "price" : 100,
    "full_execution" : true | false,
    "deadline" : "1990-12-1 12:00" | "",
    "key" : "some idemponent key",
    "investor_id" : "2323-2222",
    "status" : "VALIDATING" | "VALIDATED" | "ACTIVE" | "INACTIVE"
}
```

### <b>Предлагаемый формат хранения событий в EventStore</b>
```json
"event" :  {
    "_id" : "some id",
    "order_id" : "order id",
    "type" : "VALIDATING" | "VALIDATED" | "CREATED" | "ABORTED",
    "time" : "time of event"
}
```

## Consequences
---
1. В случаи полного сбоя Kafka мы можем восстановить все активные заявки благодаря EventStore на сервисе заявок
2. За счет заморозки активов мы гарантируем, что на момент исполнения заявки у пользователя будет достаточное кол-во продукта (или денег) для ее совершения
3. Сервисы мало связаны между собой, общение происходит только через Kafka