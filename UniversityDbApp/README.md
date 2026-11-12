# NEXUS UNIVERSITY — Week 3

Учебное приложение ASP.NET Core MVC для управления кафедрами и преподавателями, а также демонстрации ORM на примере Category → Product.

## Стек

- ASP.NET Core MVC на .NET 10 LTS
- Entity Framework Core 10 и Microsoft SQL Server
- Razor Views, CSS, JavaScript, Three.js и Chart.js
- Docker Compose для локального SQL Server

## Структура данных

| Таблица | Основные поля | Связь |
|---|---|---|
| Departments | Id (PK), Name, Code (unique), Faculty, Description | Один ко многим с Teachers |
| Teachers | Id (PK), имя, фамилия, Email (unique), DepartmentId (FK) | У каждой записи одна кафедра; удаление кафедры с преподавателями запрещено |
| Categories | Id (PK), Name, Description | Один ко многим с Products |
| Products | Id (PK), Name, Price decimal(12,2), CategoryId (FK) | У товара одна категория; удаление связанной категории запрещено |

## ORM и EF Core

ORM сопоставляет объекты C# с реляционными таблицами. `Department`, `Teacher`, `Category` и `Product` — сущности. `AppDbContext` задаёт наборы таблиц, ограничения, индексы и связи. Подход Code First хранит структуру в моделях и применяет её к SQL Server через миграции. Первичный ключ (PK) идентифицирует запись, внешний ключ (FK) поддерживает связь и целостность ссылок.

## Запуск в Windows PowerShell

Требуются .NET 10 SDK, Docker Desktop с запущенным движком и PowerShell. Windows-профиль `win-x64` собирается self-contained, поэтому для запуска собранного приложения отдельный системный .NET 10 runtime не нужен; для сборки исходников нужен .NET 10 SDK.

```powershell
cd .\UniversityDbApp
Copy-Item .env.example .env
# Откройте .env и задайте собственный сложный пароль SQL Server.
docker compose up -d
dotnet restore
dotnet tool restore
$env:MSSQL_SA_PASSWORD = "тот-же-пароль-что-указан-в-.env"
$env:ConnectionStrings__DefaultConnection = "Server=localhost,1433;Database=NexusUniversity;User Id=sa;Password=$env:MSSQL_SA_PASSWORD;Encrypt=True;TrustServerCertificate=True"
dotnet ef database update
dotnet run
```

Docker Compose читает `.env`, но PowerShell не импортирует значения из него в переменные текущего сеанса. Поэтому после изменения `.env` присвойте тот же пароль `$env:MSSQL_SA_PASSWORD`, как показано выше.

```powershell
$env:MSSQL_SA_PASSWORD = "ваш-сложный-пароль"
```

При старте в Development приложение также применяет миграции и добавляет идемпотентные демонстрационные данные, если соответствующие таблицы пусты. Данные seed — вымышленные; в Production они не добавляются.

Откройте адрес, показанный командой `dotnet run`. Для локального SQL Server без Docker используйте Windows Authentication либо задайте строку подключения через `ConnectionStrings__DefaultConnection` или User Secrets:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:DefaultConnection" "Server=localhost,1433;Database=NexusUniversity;User Id=sa;Password=ваш-пароль;Encrypt=True;TrustServerCertificate=True"
```

## Миграции

CLI:

```powershell
dotnet ef migrations add InitialDb
dotnet ef database update
```

Visual Studio Package Manager Console:

```powershell
Add-Migration InitialDb
Update-Database
```

EF CLI хранится в локальном tool manifest. `dotnet tool restore` устанавливает версию 10.0.15 из манифеста. Не добавляйте повторно `InitialDb`, если папка `Migrations` уже содержит эту миграцию.

## Страницы приложения

- `/` — русскоязычная главная с WebGL-визуализацией сети знаний и резервным оформлением при отключённом WebGL.
- `/Dashboard` — статистика из SQL Server, распределение преподавателей и последние записи.
- `/Departments` и `/Teachers` — просмотр, фильтрация, создание, изменение, детали и безопасное удаление.
- `/OrmLab` — учебные Category и Product с реальными SQL-записями и формами добавления.
- `/Database` — диаграммы PK/FK и объяснение терминов на русском.

Предпочтение светлой/тёмной темы сохраняется в браузере. Формы используют серверную валидацию и antiforgery-токены.

## Проверка сборки и SQL Server

```powershell
dotnet restore
dotnet build
dotnet test ..\UniversityDbApp.sln
dotnet ef database update
```

Если Docker или SQL Server недоступен, сборка приложения остаётся проверяемой, но подключение к реальной базе — нет. При отсутствии БД приложение пишет предупреждение в лог, а страницы с SQL-данными потребуют запущенного SQL Server.

## Скриншоты для сдачи

1. Запустите приложение и выполните `dotnet ef database update`.
2. Откройте SQL Server Management Studio и подключитесь к `localhost,1433` с SQL Server Authentication (`sa` и пароль из `.env`).
3. В Object Explorer раскройте **Databases → NexusUniversity → Tables**. Обновите список правой кнопкой по `Tables` → **Refresh**.
4. Сделайте скриншот видимых таблиц `dbo.Departments`, `dbo.Teachers`, `dbo.Categories`, `dbo.Products` и панели подключения. Не включайте пароль или окно с секретами.
5. Для подтверждения внешних ключей раскройте **dbo.Teachers → Keys** и **dbo.Products → Keys**, либо выполните запрос:

```sql
SELECT TABLE_NAME, COLUMN_NAME, CONSTRAINT_NAME
FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
WHERE TABLE_NAME IN ('Teachers', 'Products');
```

Скриншоты нужно сделать в своей среде после подключения. Этот репозиторий их не подменяет и не содержит вымышленных снимков.

## GitHub

Создайте пустой репозиторий в GitHub, затем из каталога проекта:

```powershell
git init
git add .
git commit -m "Build Nexus University Week 3"
git branch -M main
git remote add origin https://github.com/<username>/<repository>.git
git push -u origin main
```

`.env`, `bin` и `obj` исключены из Git. Перед отправкой проверьте `git status` и убедитесь, что секреты не добавлены.
