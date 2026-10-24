# GodLeaks

Bot do Discord para consultas de dados por meio da API LeakSight.

O projeto foi feito em C# com .NET 10 e oferece comandos para consultar CPF, e-mail, nome, username, placa, senha e URL. O acesso dos usuários é controlado por assinaturas armazenadas em um banco SQLite local.

## Requisitos

- [.NET 10 SDK](https://dotnet.microsoft.com/download/dotnet/10.0)
- Um bot criado no [Discord Developer Portal](https://discord.com/developers/applications)
- Uma chave de acesso da API LeakSight

## Configuração

Na raiz do projeto, configure os segredos do ambiente de desenvolvimento:

```bash
dotnet user-secrets set "Discord:Token" "SEU_TOKEN_DO_DISCORD" --project GodLeaks
dotnet user-secrets set "APIKEY" "SUA_CHAVE_DA_LEAKSIGHT" --project GodLeaks
```

O prefixo padrão dos comandos é `!` e já está definido em `GodLeaks/appsettings.json`.

Não compartilhe o token do Discord nem a chave da API. Eles são credenciais privadas.

## Executando

```bash
dotnet restore
dotnet run --project GodLeaks
```

Ao iniciar, o bot se conecta ao Discord e cria/usa o arquivo `godleaks.db` para armazenar administradores e assinaturas.

## Comandos disponíveis

| Comando | Função |
|---|---|
| `!comandos` | Mostra a lista de comandos |
| `!cpf` | Consulta um CPF |
| `!email` | Consulta um e-mail |
| `!username` | Consulta um username |
| `!nome` | Consulta um nome completo |
| `!placa` | Consulta uma placa de veículo |
| `!senha` | Consulta uma senha |
| `!url` | Consulta logins relacionados a uma URL |

Os comandos de consulta exigem uma assinatura ativa. A administração de usuários é feita pelos comandos internos de administrador, como `!add_user`, `!remove_user`, `!list_users` e `!list_admins`.

## Estrutura do projeto

- `Commands/`: comandos do Discord.
- `Application/`: serviços e interfaces da aplicação.
- `Domain/`: entidades e modelos de domínio.
- `Infrastructure/`: integração com a API, banco SQLite e injeção de dependências.
- `Presentation/`: mensagens exibidas no console.

## Uso responsável

Use o bot somente com autorização e de acordo com as leis e regras do Discord. Consultas envolvendo dados pessoais devem ter finalidade legítima, respeitar a privacidade das pessoas e seguir a legislação aplicável. O projeto não deve ser usado para doxxing, perseguição, fraude ou qualquer atividade ilegal.

## Licença

Este projeto ainda não possui uma licença definida.
