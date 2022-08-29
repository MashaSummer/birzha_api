# **ADR**

# **Demo-счёт**

</br>

## **Статус**

Предложение


---

## **Контекст**

Отправка инвестором запроса на получение информации по своему счёту.

Пополнение Счета Инвестора.

---

## **Архитектура**
Создание микросервиса баланса с использованием шаблонов. </br>
[Identity и простой шаблоны](https://github.com/Calabonga/Microservice-Template/tree/master/AspNetCore%20v6.0/MinimalAPI) </br>
[Требования по просмотру инвестором общей информации об аккаунте](https://docs.google.com/document/d/1pgPy82yXRVCNzg4vHvQDOScUsdKRvdsGnhQccSYnDl0/edit#heading=h.tikuacsydbsn) </br>
[Требования по пополнению счета инвестора](https://docs.google.com/document/d/1x8DXZh9CsJeGgLQX6pNTtrdc-CaqekNFoPI4aqc9nr4/edit)

---
## **Добавление пользователя в базу данных.** </br>
При регистрации нового пользователя, сервис регистрации отправляет об этом сообщение в кафку, которое содержит *id* пользователя, сервис баланса получает его и добавляет новую запись в базу данных с нулевым балансом, если запись уже существует то сообщение игнорируется.

---

## **Общение фасада и сервиса.** </br>
*Сервис баланса* будет принимать запросы на получение и пополнение по *grpc* протоколу.
Общение с другими сервисам происходит через **Kafka**

---

## **Отправка инвестором запроса на получение информации по своему счёту.** </br>
 Фасад достаёт из токена id и по grpc отправляет запрос с ним на микросервис. Микросервис баланса получает запрос и возвращает баланс по полученному id-пользователя.

Если пользователь отсутствует в базе данных, то сервис отправляет *Ошибку.*

---

## **Пополнение Счета Инвестора.** </br>
Фасад отправляет сообщение *{id, сумма_пополнения}* на микросервис баланса.
Микросервис баланса получает запрос и ищет *id* инвестора в бд:

 - если нет в таблице информации о балансе, то вносится новая запись с данной суммой;
 - если же есть информация, то происходит пополнение баланса *(active_balance)*; </br>

Далее отправляется информация об общей сумме инвестора на счету по grpc в фасад. После чего возвращается ответ на клиент.

---

## **Списание со счёта инвестора** </br>
Фасад отправляет сообщение *{id, сумма_списания}* на микросервис баланса.
Сервис проверяет достаточность денег на счёте:

- Если достаточно, то сервис отправляет обратно сообщение об успешность операции, в которое входит и итоговый баланс.
- Если не достаточно, то сервис отправляет сообщение об ошибке.


---
## **Идемпотентность**

Проблема *идемпотентности* будет решена на фасаде ключом идемпотентности.
Клиент будет передавать некий ключ уникальности запроса. Таким образом, серверу по этому ключу нужно определить, обрабатывался ли этот запрос ранее. Если нет (новый запрос), обрабатываем и сохраняем результат обработки, да (повторный запрос) - загружаем результат и возвращаем его без обработки.


---

## **Кафка**

- **Заморозка баланса** </br>
  (Сервис баланса имеет два поля - **active_balance and frozen_balance**. </br>
  *active_balance* - баланс, доступный для формирования заявок, </br>
  *frozen_balance* - баланс, который уже зафиксирован какими-то заявками) 

  Сервис заявок создает новую заявку и записывает данные по ней в топик *Orders_Create*, после чего сервис баланса получает *сумму* и *id_покупателя* созданной заявки, записывает сумму в *frozen_balance* и вычитает это значение из *active_balance* данного покупателя.

- **Списывание из замороженных активов** </br>
  После того как заявка исполнена, сервис заявок записывает в топик *Orders_Executed* все данные по совершенной транзакции, сервис баланса считывает *Id_продавца*, *Id_покупателя*, *сумму*.
  По Id_покупателя списывается сумма с frozen_balance.
  По Id_продавца добавляется сумма в active_balance.
  
---

## **Случай повторного запроса на пополнение**

**Решить данную проблему можно:**
- **с помощью ключа уникальности данного запроса**, новый будет генерироваться на фасаде (или же клиенте) только тогда, когда прошлая операция прошла успешно, то есть был получен ответ с сервиса, что пополнение успешно. </br>
На сервисе этот ключ сравнивается с уже ранее записанным: 
  - если они не равны, то операция производится.
  - если равны, то отправляется запрос, что данная операция уже совершена.


---

## **Предлагаемые proto**

```protobuf

service CommandBalanceService {
  rpc GetBalance(GetBalanceRequest) returns (GetBalanceResponse);
	rpc AddBalance(ChangeBalanceRequest) returns (GetBalanceResponse);
  rpc ReduceBalance(ChangeBalanceRequest) returns (GetBalanceResponse)
}


message GetBalanceRequest {
	string id = 1;
}

message ChangeBalanceRequest {
	string id = 1;
	int value = 2;
}
message GetBalanceResponse {
  int active_balance = 1;
  int frozen_balance = 2;
  enum Status {
    NONE = 0;
    SUCCESS = 1;
    FAILED = 2;
  }
  Status status = 2;
}

```

## Последствия

- Получение данных баланса будет включать **active_balance** и **frozen_balance**, для доступного и замороженного баланса соотсветственно.
- Сервис общается с фасадом по **grpc**, с другими микросеврвисами только через **Kafka**.
