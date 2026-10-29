# GodLeaks

Bot do Discord escrito em C#/.NET para consultar a API LeakSight. O projeto possui controle de acesso por assinatura, administração de usuários e persistência local em SQLite.

> **Aviso importante:** este projeto pode processar dados pessoais e credenciais. Use-o somente com autorização, finalidade legítima e em conformidade com a legislação aplicável, os termos do Discord e os contratos da API utilizada. Não use o bot para doxxing, perseguição, fraude, invasão ou compartilhamento indevido de dados.

## Sumário

- [Visão geral](#visão-geral)
- [Requisitos](#requisitos)
- [Configuração](#configuração)
- [Primeira execução](#primeira-execução)
- [Comandos do bot](#comandos-do-bot)
- [Arquitetura](#arquitetura)
- [Desenvolvimento](#desenvolvimento)
- [Banco de dados](#banco-de-dados)
- [Solução de problemas](#solução-de-problemas)
- [Contribuindo](#contribuindo)
- [Segurança e privacidade](#segurança-e-privacidade)
- [Licença](#licença)

## Visão geral

O bot recebe comandos com o prefixo configurado, valida a assinatura do usuário e encaminha a consulta para a API LeakSight. Os resultados são enviados ao Discord, normalmente como arquivo `.txt`, para evitar mensagens muito extensas.

O fluxo principal é:

```text
Discord → módulo de comando → validação da assinatura → LeakSight API
                                      ↓
                              SQLite local (assinaturas/admins)
```

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0).
- Um aplicativo/bot criado no [Discord Developer Portal](https://discord.com/developers/applications).
- Token do bot com as permissões necessárias no servidor.
- Chave válida da API LeakSight.
- Opcionalmente, `dotnet-ef` para criar e aplicar migrations durante o desenvolvimento.

Clone o repositório e entre na pasta do projeto:

```bash
git clone <URL-DO-REPOSITORIO>
cd GodLeaks
```

## Configuração

### Segredos

Não coloque tokens ou chaves no Git. Em desenvolvimento, use User Secrets:

```bash
dotnet user-secrets set "Discord:Token" "SEU_TOKEN_DO_DISCORD" --project GodLeaks
dotnet user-secrets set "APIKEY" "SUA_CHAVE_DA_LEAKSIGHT" --project GodLeaks
```

O projeto possui `UserSecretsId` configurado no `.csproj`, portanto não é necessário criar esse identificador manualmente.

### Configurações não secretas

O arquivo [`GodLeaks/appsettings.json`](GodLeaks/appsettings.json) contém o prefixo do bot:

```json
{
  "Discord": {
    "Prefix": "!"
  }
}
```

O token vazio no `appsettings.json` é intencional. Em produção, prefira variáveis de ambiente ou um gerenciador de segredos. As chaves equivalentes são `Discord__Token` e `APIKEY`.

## Primeira execução

Restaure as dependências e compile:

```bash
dotnet restore
dotnet build
```

Execute o bot:

```bash
dotnet run --project GodLeaks
```

Na inicialização, o programa:

1. registra os serviços de aplicação e o cliente HTTP da LeakSight;
2. configura a conexão do Discord;
3. executa `Database.MigrateAsync()` para aplicar migrations disponíveis;
4. carrega os módulos de comando;
5. conecta ao Discord.

### Atenção às migrations

O repositório contém a migration inicial em `GodLeaks/Migrations/`. Em um ambiente novo, aplique-a antes de depender do banco:

```bash
dotnet tool restore
dotnet ef database update --project GodLeaks --startup-project GodLeaks
```

Se o modelo mudar, gere uma nova migration, revise o arquivo e inclua-o no commit. Em produção, migrations devem ser aplicadas de forma controlada, com backup do arquivo `godleaks.db`.

### Criar o primeiro administrador

Os comandos administrativos exigem que o Discord User ID esteja na tabela `Admins` com `IsActive = true`. O código não cria um administrador automaticamente. Depois de criar o banco, faça o bootstrap por um procedimento controlado de administração do banco e substitua `SEU_DISCORD_USER_ID` pelo ID numérico correto:

```sql
INSERT INTO Admins (DiscordUserId, IsActive)
VALUES ('SEU_DISCORD_USER_ID', 1);
```

Ative o Developer Mode no Discord para copiar o User ID. Nunca publique esse valor junto com tokens ou outros segredos.

## Comandos do bot

O prefixo padrão é `!`. Os parâmetros abaixo são exemplos de formato; use apenas dados para os quais você tenha autorização.

| Comando | Parâmetro | Acesso | Descrição |
|---|---|---|---|
| `!comandos` | nenhum | público | Mostra a central de comandos e uma imagem. |
| `!cpf` | CPF | assinatura | Consulta um CPF e envia os resultados em arquivo. |
| `!email` | e-mail | assinatura | Consulta ocorrências associadas a um e-mail. |
| `!username` | username | assinatura | Consulta ocorrências associadas a um username. |
| `!nome` | nome completo | assinatura | Consulta dados relacionados a um nome. |
| `!placa` | placa | assinatura | Consulta dados de veículo. |
| `!senha` | senha | assinatura | Consulta ocorrências associadas à senha. |
| `!url` | URL | assinatura | Consulta logins relacionados a uma URL. |
| `!add_user` | Discord User ID, dias | admin | Cria uma assinatura ativa. Ex.: `!add_user 123456789012345678 30`. |
| `!update_user` | Discord User ID, ativo, dias | admin | Atualiza o estado e a validade. Ex.: `!update_user 123456789012345678 true 30`. |
| `!remove_user` | ID interno | admin | Remove uma assinatura pelo `Id` do banco. |
| `!list_users` | nenhum | admin | Lista assinaturas e datas de expiração. |
| `!list_admins` | nenhum | admin | Lista administradores e o estado de cada um. |

Assinaturas são consideradas válidas quando `IsActive = true` e `ExpiresAt` ainda não passou. Datas são gravadas e comparadas em UTC.

## Arquitetura

```text
GodLeaks/
├── Commands/                         # Módulos de comandos NetCord
├── Application/
│   ├── Interfaces/                   # Contratos de serviços
│   └── Services/                     # Assinaturas e autorização de admins
├── Domain/Entities/                  # Entidades e modelos da API
├── Infrastructure/
│   ├── DependencyInjection/          # Registro de dependências
│   ├── LeakSightAPI/                 # Cliente HTTP e desserialização
│   └── Persistence/                  # DbContext e SQLite
├── Logging/                          # Logger do bot
├── Presentation/Console/             # Mensagens no terminal
├── Assets/                           # Arquivos enviados pelo bot
├── Program.cs                        # Composition root e inicialização
└── appsettings.json                  # Configurações não secretas
```

Principais pontos de extensão:

- Para adicionar um comando, crie uma classe em `Commands/`, herde de `CommandModule<CommandContext>` e use o atributo `[Command("nome")]`.
- Para adicionar uma consulta à LeakSight, atualize `ILeakSight`, implemente o método em `LeakSight` e crie/ajuste o modelo correspondente em `Domain/Entities/LeakSightAPI/`.
- Para alterar persistência, ajuste `GodLeaksDbContext`, crie uma migration e atualize os serviços que usam o contexto.
- O registro de serviços fica em `AddServerCtlServices()`; mantenha dependências externas fora dos módulos de comando quando possível.

## Desenvolvimento

Comandos úteis:

```bash
# Restaurar ferramentas locais e pacotes
dotnet tool restore
dotnet restore

# Compilar
dotnet build

# Executar em Development
dotnet run --project GodLeaks

# Ver o estado do repositório
git status
```

O perfil `GodLeaks` em `GodLeaks/Properties/launchSettings.json` define `DOTNET_ENVIRONMENT=Development`. O projeto não possui testes automatizados no momento; novas funcionalidades devem incluir testes sempre que a lógica puder ser separada da infraestrutura do Discord/API.

## Banco de dados

O banco é SQLite e, por padrão, fica na raiz de execução com o nome `godleaks.db`. As entidades principais são:

- `UserSubscriptions`: `DiscordUserId`, estado, data de início, expiração e `PlanId`.
- `Admins`: `DiscordUserId` e estado ativo.

Há índices únicos para evitar mais de um registro por Discord User ID em cada tabela. O banco e seus arquivos auxiliares estão no `.gitignore`; não os force para o repositório.

## Solução de problemas

**`Token do Discord não configurado`**

Confirme o User Secret ou a variável `Discord__Token` no mesmo ambiente em que o processo é executado.

**`APIKEY não definida` ou `LeakSight API key is not configured`**

Configure `APIKEY` via User Secrets ou ambiente e reinicie o processo.

**`no such table` no SQLite**

Verifique se existe uma migration versionada e execute `dotnet ef database update`. Faça backup antes de operar sobre um banco existente.

**Comando administrativo informa que não há permissão**

Confirme se o ID do autor está na tabela `Admins` e se `IsActive` está como `1`.

**Consulta retorna erro HTTP**

Valide a chave, a conectividade, o endpoint e os limites/contrato atuais da LeakSight. Não registre a chave nem o conteúdo completo da resposta nos logs.

**O bot não responde**

Confirme que o bot está no servidor, que o token pertence ao aplicativo correto e que as intents/permissões necessárias foram habilitadas no Discord Developer Portal.

## Contribuindo

1. Crie uma branch descritiva a partir da branch principal.
2. Faça uma alteração pequena e focada.
3. Execute `dotnet restore` e `dotnet build` antes de abrir o PR.
4. Se alterar o modelo, gere e revise uma migration.
5. Atualize este README quando mudar comandos, configuração ou operação.
6. Abra um Pull Request descrevendo o problema, a solução, como validar e eventuais riscos de migração.

Não inclua no commit tokens, User Secrets, bancos SQLite, dumps da LeakSight, anexos de consultas ou dados pessoais reais. Para testes, use dados fictícios e substitua credenciais por mocks.

## Segurança e privacidade

- Trate respostas da API e anexos enviados ao Discord como dados potencialmente sensíveis.
- Restrinja o bot a servidores e canais autorizados.
- Conceda acesso administrativo somente a pessoas necessárias.
- Evite logar parâmetros de consulta, tokens e respostas completas.
- Defina retenção e descarte para o SQLite e para os arquivos/exports gerados.
- Revise permissões do bot e a política da LeakSight antes de colocar o sistema em produção.
- Considere adicionar auditoria, rate limiting, mascaramento de dados e testes de autorização antes de uso em escala.

## Licença

Este projeto é distribuído sob a licença MIT. Consulte o arquivo `LICENSE` quando ele estiver presente no repositório.
