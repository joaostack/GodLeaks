# GodLeaks

<img width="1983" height="793" alt="GodLeaks: Shadow Network Intelligence" src="https://github.com/user-attachments/assets/dc971b98-eedd-4259-88d1-e85dcc5e0a21" />

Bot para Discord escrito em C# e .NET 10. Recebe comandos com prefixo, verifica assinaturas armazenadas em SQLite e encaminha consultas autorizadas à API LeakSight.

> **Uso responsável:** as consultas podem envolver dados pessoais ou credenciais. Use o bot somente com autorização e finalidade legítima, respeitando a legislação aplicável, os termos do Discord e os contratos da API. Não use resultados para doxxing, perseguição, fraude ou compartilhamento indevido.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Aplicação e bot configurados no [Discord Developer Portal](https://discord.com/developers/applications)
- Token de bot com acesso ao servidor e permissões/intents necessárias
- Chave válida da API LeakSight

## Configuração

Não armazene tokens ou chaves no repositório. Para desenvolvimento, configure User Secrets:

```bash
dotnet user-secrets set "Discord:Token" "SEU_TOKEN_DO_DISCORD" --project GodLeaks
dotnet user-secrets set "APIKEY" "SUA_CHAVE_DA_LEAKSIGHT" --project GodLeaks
```

O projeto já define um `UserSecretsId`. O prefixo padrão `!` está em [`GodLeaks/appsettings.json`](GodLeaks/appsettings.json). Também é possível configurar valores por variáveis de ambiente: `Discord__Token`, `Discord__Prefix` e `APIKEY`.

## Executar

Na raiz do repositório:

```bash
dotnet restore
dotnet build
dotnet run --project GodLeaks
```

Na inicialização, o programa aplica as migrations pendentes antes de validar as configurações e conectar ao Discord. O SQLite usa o caminho relativo `godleaks.db` no diretório de trabalho do processo.

### Banco de dados e primeiro administrador

As migrations do Entity Framework Core ficam em `GodLeaks/Migrations` e devem ser mantidas no controle de versão. O `.gitignore` permite essa pasta. Para gerar uma migration após alterar entidades ou o modelo:

```bash
dotnet tool restore
dotnet ef migrations add NomeDaMigration --project GodLeaks --startup-project GodLeaks
```

Revise e versione os arquivos gerados. O programa chama `Database.MigrateAsync()` na inicialização; não é necessário executar `database update` separadamente para iniciar o bot. Faça backup de `godleaks.db` antes de aplicar mudanças de esquema em ambientes com dados.

Não há criação automática de administradores. Depois da primeira inicialização (que cria o banco e aplica as migrations), insira o ID do Discord autorizado na tabela `Admins`:

```sql
INSERT INTO Admins (DiscordUserId, IsActive)
VALUES ('SEU_DISCORD_USER_ID', 1);
```

Use o ID numérico do usuário (ative o Developer Mode no Discord para copiá-lo). O comando `!list_admins` lista administradores, mas só pode ser usado por um administrador já cadastrado.

## Comandos

O prefixo padrão é `!`. Comandos de consulta exigem assinatura ativa e não expirada. As datas de validade são comparadas em UTC.

| Comando | Argumento | Acesso | Ação |
|---|---|---|---|
| `!comandos` | — | Público | Exibe o menu de consultas. |
| `!cpf` | CPF | Assinatura | Consulta por CPF. |
| `!email` | e-mail | Assinatura | Consulta por e-mail. |
| `!username` | username | Assinatura | Consulta por username. |
| `!nome` | nome completo | Assinatura | Consulta por nome. |
| `!placa` | placa | Assinatura | Consulta por placa. |
| `!senha` | senha | Assinatura | Consulta por senha. |
| `!telefone` | telefone | Assinatura | Consulta por telefone. |
| `!url` | URL | Assinatura | Consulta logins relacionados a uma URL. |
| `!add_user` | ID do Discord, dias | Admin | Cria ou renova uma assinatura. Ex.: `!add_user 123456789012345678 30`. |
| `!update_user` | ID do Discord, ativo, dias | Admin | Atualiza estado e validade. Ex.: `!update_user 123456789012345678 true 30`. |
| `!remove_user` | ID do Discord | Admin | Remove a assinatura do usuário. |
| `!ban_user` | ID do Discord | Admin | Desativa a assinatura. |
| `!unban_user` | ID do Discord | Admin | Reativa uma assinatura ainda válida. |
| `!list_users` | — | Admin | Lista assinaturas e validade. |
| `!list_admins` | — | Admin | Lista administradores e estado. |

As respostas de consulta são enviadas pelo bot, normalmente como arquivo de texto. Use somente dados cuja consulta esteja autorizada.

## Estrutura do projeto

```text
GodLeaks/
├── Commands/                 # Comandos Discord e verificação de assinatura
├── Application/              # Interfaces e serviços de assinatura/admin
├── Domain/Entities/           # Entidades locais e modelos LeakSight
├── Infrastructure/
│   ├── DependencyInjection/   # Registro de serviços
│   ├── LeakSightAPI/          # Cliente HTTP da API
│   └── Persistence/           # DbContext SQLite
├── Migrations/                # Migrations do Entity Framework Core
├── Handlers/                  # Tratamento de resultados de comandos
├── Logging/                   # Logger
├── Presentation/Console/      # Saída do terminal
├── Assets/                    # Arquivos usados pelo bot
├── Program.cs                 # Inicialização e composição
└── appsettings.json           # Configurações não secretas
```

O contexto SQLite contém as tabelas `UserSubscriptions` e `Admins`. O banco e arquivos auxiliares SQLite estão no `.gitignore`.

## Desenvolvimento

```bash
dotnet tool restore
dotnet restore
dotnet build
dotnet run --project GodLeaks
```

Comandos com assinatura herdam de `SubscriptionCommandModule`. A integração LeakSight é definida por `ILeakSight` e implementada em `Infrastructure/LeakSightAPI/LeakSight.cs`. Ao alterar entidades, gere, revise e versione uma migration.

O repositório não contém testes automatizados no momento.

## Solução de problemas

- **Token não configurado:** confira `Discord:Token` nos User Secrets ou `Discord__Token` no ambiente do processo.
- **APIKEY não configurada:** configure `APIKEY` e reinicie o bot.
- **Erro de tabela inexistente:** verifique se os arquivos de migration estão presentes e versionados; confira também o diretório de trabalho e o caminho do banco.
- **Comando administrativo sem permissão:** confirme se o ID está em `Admins` com `IsActive = 1`.
- **Bot não responde:** confira token, presença do bot no servidor, intents e permissões no Discord Developer Portal.
- **Erro HTTP da LeakSight:** valide a chave, a conectividade e as condições atuais da API. Não registre tokens, parâmetros de consulta ou respostas completas nos logs.

## Segurança e privacidade

- Restrinja o bot a servidores e canais autorizados e limite o acesso administrativo.
- Trate respostas e anexos como dados potencialmente sensíveis; defina retenção e descarte para banco e arquivos gerados.
- Não registre tokens, chaves, consultas ou respostas completas.
- Não versione User Secrets, banco SQLite, dumps, anexos de consultas ou dados pessoais reais.
- Antes de usar em produção, revise permissões, política da LeakSight e controles de acesso, auditoria e limitação de uso.

## Licença
MIT
