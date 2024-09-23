# 123Vendas

## Tecnologias Utilizadas

- **ASP.NET Core 8.0**: Framework utilizado para construção da API.
- **Entity Framework Core**: ORM para interação com o banco de dados SQL Server.
- **SQL Server**: Banco de dados relacional utilizado pelo projeto.
- **AutoMapper**: Para o mapeamento entre entidades de domínio e DTOs.
- **Serilog**: Para logging estruturado.
- **Bogus**: Gerador de dados falsos utilizado nos testes unitários.
- **xUnit**: Framework de testes utilizado para os testes unitários e de integração.
- **NSubstitute**: Mocking framework utilizado nos testes para substituir dependências.
- **Docker**: Utilizado para criar containers e simular um ambiente real de banco de dados em testes de integração.

## Funcionalidades

- **Gestão de Vendas**: Criação, atualização, cancelamento de vendas e itens de venda.
- **Mensageria**: Publicação de eventos de criação, atualização e cancelamento de vendas e itens. (Apenas Logs)
- **Cálculo de Valor Total**: O valor total de uma venda é calculado automaticamente, levando em consideração apenas os itens ativos (não cancelados).
- **Autenticação**: Implementação de autenticação básica via Bearer tokens. (Não Implementado)
- **Swagger**: Documentação automática da API.

## Estrutura do Projeto

O projeto é dividido em várias camadas principais:

- **API**: Contém os controladores e as definições dos endpoints.
- **Application**: Camada de negócios onde a lógica das vendas e o processamento de eventos ocorrem.
- **Core**: Definições de entidades e interfaces de repositório.
- **Infrastructure**: Implementações concretas de repositórios e do MessageBus.
- **IntegrationTests**: Testes de integração para garantir o funcionamento completo do sistema.
- **UnitTests**: Testes unitários focados na lógica de negócios e validações.

## Configurações Importantes

### Conexão com Banco de Dados

A conexão com o banco de dados está configurada no arquivo `appsettings.json` da API:

```json
"ConnectionStrings": {
  "123Vendas": "Server=localhost\\SQLSERVER;Database=123Vendas;TrustServerCertificate=True;Trusted_Connection=True"
}
```

### Logs de Eventos

O sistema utiliza o **Serilog** para gerenciar logs. Todos os eventos relevantes, como a criação, atualização e cancelamento de vendas e itens, são registrados. O nível de log pode ser ajustado no `appsettings.json`:

```json
"Logging": {
  "LogLevel": {
    "Default": "Information",
    "Microsoft.AspNetCore": "Warning"
  }
}
```

Os eventos de mensageria são logados explicitamente com o nível Information.


### Migrations

Para aplicar as migrações e configurar o banco de dados corretamente, utilize o seguinte comandos:

```bash
dotnet ef database update
```

Isso gerará as tabelas e relacionamentos conforme definidos no contexto do Entity Framework.

## Testes

### Testes Unitários

Os testes unitários cobrem a lógica de negócios principal, como criação de vendas, publicação de eventos e cálculo de valores. Eles utilizam o **xUnit** e o **NSubstitute** para mocks. A execução dos testes pode ser feita com o comando:

```bash
dotnet test
```

### Testes de Integração

Os testes de integração utilizam o Docker para subir um ambiente com banco de dados real e testar o funcionamento do sistema em um cenário mais próximo do ambiente de produção.

Os testes podem ser executados também com o comando:

```bash
dotnet test
```
Certifique-se de que o Docker esteja em execução e configurado corretamente.
