# E-Trade Core

Clean Architecture prensipleri ile geliştirilmiş, CQRS pattern'i kullanan e-ticaret core projesi.

## Proje Yapısı

```
etrade-core/
├── etrade-core.api/           # Web API katmanı
├── etrade-core.application/   # Application katmanı (CQRS + MediatR)
├── etrade-core.domain/        # Domain katmanı (Entities, Enums)
├── etrade-core.infrastructure/# Infrastructure katmanı
└── etrade-core.persistence/   # Persistence katmanı (DbContext, Repositories)
```

## Özellikler

### 🏗️ Clean Architecture
- Katmanlı mimari yapısı
- Dependency Inversion prensibi
- Separation of Concerns

### 📝 CQRS + MediatR
- Command Query Responsibility Segregation
- MediatR ile request/response pattern
- Pipeline behaviors (Validation, Logging, Transaction, Idempotency)

### 🔒 Identity & Authorization
- ASP.NET Core Identity
- JWT token authentication
- Role-based authorization

### 🗄️ Database
- Entity Framework Core
- PostgreSQL
- Repository pattern
- Unit of Work pattern

### 📊 Generic Product System
- Esnek ürün yapısı
- Dinamik özellikler
- Ürün şablonları

## CQRS + MediatR Implementasyonu

### Pipeline Behaviors

1. **ValidationBehavior**: FluentValidation ile otomatik validation
2. **LoggingBehavior**: Request/response logging ve performance metrics
3. **TransactionBehavior**: Command'ler için otomatik transaction yönetimi
4. **IdempotencyBehavior**: Duplicate request koruması

### Kullanım Örneği

```csharp
// Command
public class CreateOrderCommand : IRequest<CreateOrderResponse>
{
    public string UserId { get; set; }
    public List<CreateOrderItemDto> OrderItems { get; set; }
}

// Handler
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, CreateOrderResponse>
{
    public async Task<CreateOrderResponse> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        // Business logic
        return response;
    }
}

// Controller
[HttpPost]
public async Task<ActionResult<CreateOrderResponse>> CreateOrder([FromBody] CreateOrderCommand command)
{
    var result = await _mediator.Send(command);
    return CreatedAtAction(nameof(GetOrder), new { id = result.OrderId }, result);
}
```

## Kurulum

### Gereksinimler
- .NET 9.0
- PostgreSQL
- Visual Studio 2022 / VS Code

### Adımlar

1. **Repository'yi klonlayın**
   ```bash
   git clone <repository-url>
   cd etrade-core
   ```

2. **Connection string'leri ayarlayın**
   - `etrade-core.api/appsettings.json` dosyasında database connection string'lerini güncelleyin

3. **Database migration'ları çalıştırın**
   ```bash
   cd etrade-core.persistence
   dotnet ef database update --context DomainDbContext
   dotnet ef database update --context ApplicationDbContext
   ```

4. **Projeyi çalıştırın**
   ```bash
   cd etrade-core.api
   dotnet run
   ```

5. **Swagger UI'a erişin**
   - `https://localhost:5001/swagger`

## API Endpoints

### Orders
- `POST /api/order` - Yeni sipariş oluştur
- `GET /api/order/{id}` - Sipariş detayı getir
- `GET /api/order/user/{userProfileId}` - Kullanıcı siparişleri
- `PUT /api/order/{id}/status` - Sipariş durumu güncelle
- `DELETE /api/order/{id}` - Sipariş iptal et

## Geliştirme

### Yeni Command/Query Ekleme

1. Command/Query class'ı oluşturun
2. Handler class'ı oluşturun
3. Gerekirse Validator class'ı oluşturun
4. Controller'da kullanın

### Pipeline Behavior Ekleme

1. `IPipelineBehavior<TRequest, TResponse>` implement edin
2. `Program.cs`'de DI container'a kaydedin
3. Sıralamaya dikkat edin

## Test

```bash
dotnet test
```

## Katkıda Bulunma

1. Fork edin
2. Feature branch oluşturun (`git checkout -b feature/amazing-feature`)
3. Commit edin (`git commit -m 'Add amazing feature'`)
4. Push edin (`git push origin feature/amazing-feature`)
5. Pull Request oluşturun

## Lisans

Bu proje MIT lisansı altında lisanslanmıştır.

## İletişim

- Proje Linki: [https://github.com/username/etrade-core](https://github.com/username/etrade-core)
- E-posta: support@etrade.com 