# CQRS + MediatR Implementation

Bu proje Clean Architecture prensipleri ile CQRS (Command Query Responsibility Segregation) pattern'ini MediatR kullanarak implement etmektedir.

## Yapı

### 1. Base Interfaces

- `IRequest<TResponse>`: MediatR request interface'i
- `ICommand<TResponse>`: Command marker interface'i
- `IQuery<TResponse>`: Query marker interface'i
- `ISender`: Request gönderme interface'i

### 2. Pipeline Behaviors

#### ValidationBehavior
- FluentValidation kullanarak request'leri validate eder
- Command ve Query'ler için otomatik validation
- Validation hatası durumunda ValidationException fırlatır

#### LoggingBehavior
- Request'lerin başlangıç ve bitiş zamanını loglar
- Performance metrics (elapsed time) tutar
- Hata durumlarını loglar

#### TransactionBehavior
- Command'ler için otomatik transaction yönetimi
- Query'ler için transaction kullanmaz
- UnitOfWork.SaveChangesAsync() çağrısı

#### IdempotencyBehavior
- Command'ler için idempotency sağlar
- Distributed cache kullanarak duplicate request'leri engeller
- 24 saat cache süresi

### 3. Kullanım Örneği

#### Command Oluşturma

```csharp
public class CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public string UserId { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; }
}

public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Business logic implementation
        return response;
    }
}
```

#### Query Oluşturma

```csharp
public class GetOrderQuery : IRequest<GetOrderResponse>
{
    public int OrderId { get; set; }
}

public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, GetOrderResponse>
{
    public async Task<GetOrderResponse> Handle(GetOrderQuery request, CancellationToken cancellationToken)
    {
        // Data retrieval logic
        return response;
    }
}
```

#### Controller'da Kullanım

```csharp
[HttpPost]
public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);
}

[HttpGet("{id}")]
public async Task<ActionResult<GetOrderResponse>> GetOrder(int id)
{
    var query = new GetOrderQuery { OrderId = id };
    var result = await _mediator.Send(query);
    return Ok(result);
}
```

### 4. Validation

FluentValidation kullanarak her Command/Query için validator oluşturulabilir:

```csharp
public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.OrderItems).NotEmpty();
    }
}
```

### 5. Pipeline Behavior Sırası

1. **ValidationBehavior**: Request validation
2. **LoggingBehavior**: Request logging
3. **IdempotencyBehavior**: Idempotency check
4. **TransactionBehavior**: Transaction management
5. **Handler**: Business logic execution

### 6. Avantajlar

- **Separation of Concerns**: Command ve Query'ler ayrı
- **Testability**: Her handler bağımsız test edilebilir
- **Extensibility**: Pipeline behaviors kolayca eklenebilir
- **Maintainability**: Kod organizasyonu net
- **Performance**: Query'ler için transaction yok
- **Idempotency**: Duplicate request koruması

### 7. Yeni Command/Query Ekleme

1. Command/Query class'ı oluştur (IRequest<TResponse> implement et)
2. Handler class'ı oluştur (IRequestHandler<TRequest, TResponse> implement et)
3. Gerekirse Validator class'ı oluştur
4. Controller'da IMediator.Send() ile kullan

### 8. Pipeline Behavior Ekleme

1. IPipelineBehavior<TRequest, TResponse> implement et
2. Program.cs'de DI container'a kaydet
3. Sıralama önemli (validation -> logging -> idempotency -> transaction)

## Notlar

- Tüm Command'ler transaction içinde çalışır
- Query'ler sadece read operation yapar
- Validation otomatik olarak tüm request'lerde çalışır
- Idempotency sadece Command'lerde aktif
- Logging tüm request'leri kapsar 