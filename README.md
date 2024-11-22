
# **Order Processing Pipeline**

---

## **O que é o Pipeline Pattern?**

O **Pipeline Pattern** é um padrão de design que oferece uma abordagem clara e extensível para construir e organizar fluxos de processamento, permitindo que cada etapa (step) seja desacoplada e focada em uma tarefa específica. 

Esse padrão organiza o processamento de dados em uma série de etapas, onde:

1. Cada etapa (step) recebe os dados e realiza uma transformação ou validação.
2. Steps são desacoplados entre si e podem ser reutilizados em outros pipelines.

A POC construída nesse projeto tem o objetivo de mostrar uma abordagem utilizando o Pipeline Pattern com **Middlewares** para lidar com funcionalidades genéricas, como **tratamento de exceções**, sem modificar diretamente a lógica dos steps. E também foram acrescentados testes unitários para cobrir alguns cenários.

Para as próximas atualizações, foi pensado em criar um PipelineContext, com um Dictionary, sendo esta uma abordagem flexível, especialmente quando o pipeline precisa lidar com múltiplos tipos de dados sem amarrar-se a um objeto de domínio específico(como é o caso da versão atual do código). Com isso, cada step do pipeline pode adicionar, modificar ou remover dados do contexto conforme necessário.

---

## **Principais Componentes**

### **1. Steps**
Steps são os "blocos de construção" do pipeline. Cada step executa uma ação específica no objeto que está sendo processado. Exemplos de steps no nosso pipeline incluem:
- **`ValidateOrderStep`**: Valida se o pedido está correto.
- **`CalculateShippingStep`**: Calcula os custos de envio.
- **`ReserveStockStep`**: Garante que os itens estão reservados no estoque.
- **`ProcessPaymentStep`**: Processa o pagamento do pedido.
- **`GenerateInvoiceStep`**: Gera uma nota fiscal para o pedido.
- **`NotifyCustomerStep`**: Envia uma notificação ao cliente.

### **2. Middlewares**
Middlewares adicionam comportamentos transversais ao pipeline. No exemplo, usamos o **`ExceptionHandlingMiddleware`** para capturar exceções de forma centralizada e garantir que falhas sejam tratadas sem interromper o pipeline.

---

## **Exemplo: Pipeline para Processamento de Pedidos**

O pipeline a seguir ilustra um fluxo de processamento completo para um pedido:

```csharp
var processedOrder = await PipelineBuilder<Order>
    .Create()
    .UseMiddleware(ExceptionHandlingMiddleware<Order>.Apply) // Middleware para tratamento de exceções
    .AddStep(new ValidateOrderStep())                        // Valida o pedido
    .AddStep(new CalculateShippingStep())                    // Calcula frete
    .AddStep(new ReserveStockStep())                         // Reserva itens no estoque
    .AddStep(new ProcessPaymentStep(paymentService))         // Processa pagamento
    .AddStep(new GenerateInvoiceStep(invoiceService))        // Gera nota fiscal
    .AddStep(new NotifyCustomerStep(notificationService))    // Notifica cliente
    .ProcessAsync(order);                                    // Executa o pipeline
```

---

## **Como Funciona?**

### **1. Middleware**
O middleware **`ExceptionHandlingMiddleware`** envolve cada step para capturar e tratar exceções de forma centralizada. Isso garante que erros em um step não impactem diretamente os demais, permitindo retries ou logs de falha controlados.

```csharp
public static Func<TContext, Func<TContext, Task>, Task> Apply<TContext>() =>
    async (context, next) =>
    {
        try
        {
            await next(context); // Executa o próximo step no pipeline
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Exception caught: {ex.Message}");
            throw; // Re-encaminha a exceção, se necessário
        }
    };
```

### **2. Steps**
Cada step opera de forma isolada. Aqui está um exemplo simplificado de um step que valida o pedido:

```csharp
public class ValidateOrderStep : BaseStep
{
    protected override Task ProcessAsync(Order context)
    {
        if (context.Items == null || !context.Items.Any())
        {
            throw new InvalidOperationException("The order must have at least one item.");
        }
        Console.WriteLine("Order validated successfully.");
        return Task.CompletedTask;
    }
}
```

---

## **Benefícios do Pipeline Pattern**

- **Separação de Responsabilidades**: Cada step tem uma responsabilidade única.
- **Flexibilidade**: É fácil adicionar, remover ou reorganizar steps no pipeline.
- **Reutilização**: Steps podem ser reaproveitados em diferentes pipelines.
- **Escalabilidade**: Funcionalidades genéricas podem ser adicionadas por meio de middlewares.
- **Manutenção Simplificada**: Alterações em um step não afetam diretamente os outros.

---

## **Requisitos**

- **.NET 7 ou superior**
- Familiaridade com padrões de design como *Chain of Responsibility* e *Decorator*.

---

## **Estrutura do Projeto**

```plaintext
PipelineApp/                     # Diretório raiz do projeto
├── PipelineApp.sln              # Arquivo de solução
├── src/                         # Código-fonte da aplicação principal
│   ├── PipelineApp/             # Projeto Console principal
│   │   ├── Program.cs           # Ponto de entrada da aplicação
│   │   ├── Domain/              # Entidade do domínio da aplicação
│   │   │   ├── Invoice.cs
│   │   │   ├── Order.cs
│   │   │   ├── OrderItem.cs           
│   │   ├── Services/            # Serviços e interfaces
│   │   │   ├── IPaymentService.cs
│   │   │   ├── IInvoiceService.cs
│   │   │   ├── INotificationService.cs
│   │   │   ├── PaymentService.cs
│   │   │   ├── InvoiceService.cs
│   │   │   ├── NotificationService.cs
│   │   ├── Steps/               # Implementação dos Steps do Pipeline
│   │   │   ├── BaseStep.cs
│   │   │   ├── ValidateOrderStep.cs
│   │   │   ├── CalculateShippingStep.cs
│   │   │   ├── ReserveStockStep.cs
│   │   │   ├── ProcessPaymentStep.cs
│   │   │   ├── GenerateInvoiceStep.cs
│   │   │   ├── NotifyCustomerStep.cs
│   │   ├── Middleware/          # Middleware e infraestrutura do pipeline
│   │   │   ├── PipelineMiddleware.cs
│   │   │   ├── ExceptionHandlingMiddleware.cs
│   │   ├── Pipeline/            # Core do Pipeline Pattern
│   │       ├── IAsyncPipelineStep.cs
│   │       ├── PipelineBuilder.cs
│   └── Tests/                   # Projeto de testes unitários
│       ├── PipelineApp.Tests/   # Projeto de testes
│       │   ├── Steps/           # Testes dos Steps
│       │   │   ├── OrderPipelineTests.cs
├── .gitignore                   # Configurações do Git
├── README.md                    # Documentação do projeto

```

---

## **Como Executar o Projeto**

1. Clone o repositório:
   ```bash
   git clone https://github.com/seu-usuario/pipeline-pattern-orders.git
   cd pipeline-pattern-order/OrderPipelineMiddleware
   ```

2. Restaure as dependências:
   ```bash
   dotnet restore
   ```

3. Compile o projeto:
   ```bash
   dotnet build
   ```

4. Execute o programa:
   ```bash
   dotnet run
   ```
