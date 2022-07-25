
# Товар

## Status
---
Предложение

## Context
---
Необходимо реализовать функционал получения списка товаров на бирже.
[Требования](https://docs.google.com/document/d/1LMghw4bXezuPiETa0or1F-m5znHtXrK0DNNMGikpgv0/edit?usp=sharing)

## Decision
---

### Новый микросервис
---
Создание нового микросервиса, отвечающего за работу с товарами, в частности получение списка товаров.  
Сервис будет объединять в себе Commands и Queries.  

### Реализация нового сервиса
---
На данном этапе будет реализован метод чтения всех товаров на бирже (без пагинации).  
В требованиях указано не показвать товары без best_ask'а, но т.к. на данный момент не реализована функциональность транзакций/стакана, то
вместо этого будут применяться заглушки (best_ask = 1) и выводится все товары на бирже.  


### Фасад
---
Также будет реализован метод в фасаде для отправки запроса на получения списка всех товаров на бирже

### Общение фасада и product-service'а
---
Общение между фасадом и микросервисом будет осуществляться по протоколу grpc


### Предлагаемые proto-файлы
---
```protobuf
service ProductService {
    rpc GetAllProducts (GetAllProductsRequest) returns (GetAllProductsResponse);
}

message GetAllProductsRequest {}

enum Status {
    NONE = 0;
    SUCCESS = 1;
    FAILED = 2;
} 

message GetAllProductsResponse {
    message Product {
        int32 id = 1;
        string name = 2;
        double best_ask = 3;
        double best_bid = 4;
    }

    repeated Product products = 1;
    Status status = 2;
}
```



## Consequences
---
1. Повторение запросов на фасад и на сервис
2. Для получения товаров имеется отдельный сервис
3. Тесная связь фасада с сервисом
