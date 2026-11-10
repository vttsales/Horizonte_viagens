-- ALTERAÇÃO PIM: migrations consolidadas; selecione o banco no SSMS antes de executar.
-- Gerado por EF Core 9.0.22; sem execução em SQL Server neste PC.
IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    CREATE TABLE [Clientes] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [Cpf] nchar(11) NOT NULL,
        [DataCadastro] datetime2 NOT NULL DEFAULT (GETDATE()),
        [DataAtualizacao] datetime2 NULL,
        [Ativo] bit NOT NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Cpf] ON [Clientes] ([Cpf]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Clientes_Email] ON [Clientes] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    EXEC(N'
    CREATE TRIGGER TR_Clientes_DataAtualizacao
    ON Clientes
    AFTER UPDATE
    AS
    BEGIN
        SET NOCOUNT ON;
        UPDATE c
           SET DataAtualizacao = GETDATE()
          FROM Clientes c
          JOIN inserted i ON i.Id = c.Id;
    END');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    EXEC(N'
    CREATE PROCEDURE sp_Clientes_BuscarPorNome
        @Trecho NVARCHAR(100)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT Id, Nome, Email, Cpf, DataCadastro, DataAtualizacao, Ativo
          FROM Clientes
         WHERE Nome LIKE ''%'' + @Trecho + ''%''
         ORDER BY Nome;
    END');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261001130158_Inicial'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261001130158_Inicial', N'9.0.22');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    ALTER TABLE [Clientes] ADD [Celular] nchar(11) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    ALTER TABLE [Clientes] ADD [SenhaHash] nvarchar(200) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    CREATE TABLE [Administradores] (
        [Id] int NOT NULL IDENTITY,
        [UltimoAcesso] datetime2 NULL,
        [Nome] nvarchar(100) NOT NULL,
        [Email] nvarchar(150) NOT NULL,
        [SenhaHash] nvarchar(200) NOT NULL,
        [Ativo] bit NOT NULL,
        CONSTRAINT [PK_Administradores] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'Email', N'Nome', N'SenhaHash', N'UltimoAcesso') AND [object_id] = OBJECT_ID(N'[Administradores]'))
        SET IDENTITY_INSERT [Administradores] ON;
    EXEC(N'INSERT INTO [Administradores] ([Id], [Ativo], [Email], [Nome], [SenhaHash], [UltimoAcesso])
    VALUES (1, CAST(1 AS bit), N''admin@horizonteviagens.com'', N''Administrador'', N''100000.R03eHwaH3+lUVmH+AEHXwg==.sPZ3h/VnYltKhLwXGC43dpu9Ck/HZ65KF4i87G7148g='', NULL)');
    IF EXISTS (SELECT * FROM [sys].[identity_columns] WHERE [name] IN (N'Id', N'Ativo', N'Email', N'Nome', N'SenhaHash', N'UltimoAcesso') AND [object_id] = OBJECT_ID(N'[Administradores]'))
        SET IDENTITY_INSERT [Administradores] OFF;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Administradores_Email] ON [Administradores] ([Email]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN

    ALTER PROCEDURE sp_Clientes_BuscarPorNome
        @Trecho NVARCHAR(100)
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT Id, Nome, Email, Cpf, Celular, DataCadastro, DataAtualizacao, Ativo
          FROM Clientes
         WHERE Nome LIKE '%' + @Trecho + '%'
         ORDER BY Nome;
    END
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261102035931_AdministradorELogin'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261102035931_AdministradorELogin', N'9.0.22');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    ALTER TABLE [Clientes] ADD [Bairro] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    ALTER TABLE [Clientes] ADD [CEP] varchar(9) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    ALTER TABLE [Clientes] ADD [Cidade] nvarchar(50) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    ALTER TABLE [Clientes] ADD [Rua] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Combos] (
        [Id] int NOT NULL IDENTITY,
        [Valor] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_Combos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Combos_Valor] CHECK ([Valor] >= 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Hoteis] (
        [Id] int NOT NULL IDENTITY,
        [Nome] nvarchar(100) NOT NULL,
        [Endereco] nvarchar(200) NOT NULL,
        [Classificacao] nvarchar(20) NULL,
        CONSTRAINT [PK_Hoteis] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Passeios] (
        [Id] int NOT NULL IDENTITY,
        [Lugar] nvarchar(100) NOT NULL,
        [Horario] datetime2 NOT NULL,
        [Capacidade] int NOT NULL,
        CONSTRAINT [PK_Passeios] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Passeios_Capacidade] CHECK ([Capacidade] > 0)
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Voos] (
        [Id] int NOT NULL IDENTITY,
        [CidadePartida] nvarchar(100) NOT NULL,
        [HoraPartida] time NOT NULL,
        [CidadeChegada] nvarchar(100) NOT NULL,
        [HoraChegada] time NOT NULL,
        CONSTRAINT [PK_Voos] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [TelefonesAdministradores] (
        [Id] int NOT NULL IDENTITY,
        [Numero] varchar(20) NOT NULL,
        [AdministradorId] int NOT NULL,
        CONSTRAINT [PK_TelefonesAdministradores] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TelefonesAdministradores_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [TelefonesClientes] (
        [Id] int NOT NULL IDENTITY,
        [Numero] varchar(20) NOT NULL,
        [ClienteId] int NOT NULL,
        CONSTRAINT [PK_TelefonesClientes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TelefonesClientes_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [TelefonesHoteis] (
        [Id] int NOT NULL IDENTITY,
        [Numero] varchar(20) NOT NULL,
        [HotelId] int NOT NULL,
        CONSTRAINT [PK_TelefonesHoteis] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_TelefonesHoteis_Hoteis_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hoteis] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Passagens] (
        [Id] int NOT NULL IDENTITY,
        [Classe] nvarchar(20) NOT NULL,
        [Assento] nvarchar(10) NOT NULL,
        [DataIda] date NOT NULL,
        [DataVolta] date NOT NULL,
        [VooId] int NOT NULL,
        CONSTRAINT [PK_Passagens] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Passagens_Periodo] CHECK ([DataVolta] >= [DataIda]),
        CONSTRAINT [FK_Passagens_Voos_VooId] FOREIGN KEY ([VooId]) REFERENCES [Voos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [ReservasQuartos] (
        [Id] int NOT NULL IDENTITY,
        [NumeroQuarto] nvarchar(10) NOT NULL,
        [Tipo] nvarchar(30) NOT NULL,
        [DataCheckin] date NOT NULL,
        [DataCheckout] date NOT NULL,
        [HotelId] int NOT NULL,
        CONSTRAINT [PK_ReservasQuartos] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_ReservasQuartos_Periodo] CHECK ([DataCheckout] > [DataCheckin]),
        CONSTRAINT [FK_ReservasQuartos_Hoteis_HotelId] FOREIGN KEY ([HotelId]) REFERENCES [Hoteis] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [Compras] (
        [Id] int NOT NULL IDENTITY,
        [ComboId] int NULL,
        [ClienteId] int NOT NULL,
        [Valor] decimal(10,2) NOT NULL,
        CONSTRAINT [PK_Compras] PRIMARY KEY ([Id]),
        CONSTRAINT [CK_Compras_Valor] CHECK ([Valor] >= 0),
        CONSTRAINT [FK_Compras_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Compras_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [ItensCombos] (
        [QuartoId] int NOT NULL,
        [PassagemId] int NOT NULL,
        [PasseioId] int NOT NULL,
        [ComboId] int NOT NULL,
        CONSTRAINT [PK_ItensCombos] PRIMARY KEY ([QuartoId], [PassagemId], [PasseioId], [ComboId]),
        CONSTRAINT [FK_ItensCombos_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ItensCombos_Passagens_PassagemId] FOREIGN KEY ([PassagemId]) REFERENCES [Passagens] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ItensCombos_Passeios_PasseioId] FOREIGN KEY ([PasseioId]) REFERENCES [Passeios] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_ItensCombos_ReservasQuartos_QuartoId] FOREIGN KEY ([QuartoId]) REFERENCES [ReservasQuartos] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE TABLE [MonitoramentosCompras] (
        [AdministradorId] int NOT NULL,
        [CompraId] int NOT NULL,
        CONSTRAINT [PK_MonitoramentosCompras] PRIMARY KEY ([AdministradorId], [CompraId]),
        CONSTRAINT [FK_MonitoramentosCompras_Administradores_AdministradorId] FOREIGN KEY ([AdministradorId]) REFERENCES [Administradores] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_MonitoramentosCompras_Compras_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compras] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_Compras_ClienteId] ON [Compras] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_Compras_ComboId] ON [Compras] ([ComboId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_ItensCombos_ComboId] ON [ItensCombos] ([ComboId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_ItensCombos_PassagemId] ON [ItensCombos] ([PassagemId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_ItensCombos_PasseioId] ON [ItensCombos] ([PasseioId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_MonitoramentosCompras_CompraId] ON [MonitoramentosCompras] ([CompraId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_Passagens_VooId] ON [Passagens] ([VooId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_ReservasQuartos_HotelId] ON [ReservasQuartos] ([HotelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_TelefonesAdministradores_AdministradorId] ON [TelefonesAdministradores] ([AdministradorId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_TelefonesClientes_ClienteId] ON [TelefonesClientes] ([ClienteId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    CREATE INDEX [IX_TelefonesHoteis_HotelId] ON [TelefonesHoteis] ([HotelId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261103000000_EsquemaViagens'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261103000000_EsquemaViagens', N'9.0.22');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Combos] ADD [Nome] nvarchar(100) NOT NULL DEFAULT N'Combo';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Combos] ADD [Destino] nvarchar(100) NOT NULL DEFAULT N'A definir';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Combos] ADD [Vagas] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Combos] ADD [Categoria] nvarchar(30) NOT NULL DEFAULT N'Geral';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    EXEC(N'ALTER TABLE [Combos] ADD CONSTRAINT [CK_Combos_Vagas] CHECK ([Vagas] >= 0)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    EXEC(N'
    CREATE OR ALTER PROCEDURE sp_Combos_BuscarPorDestino
        @Destino NVARCHAR(100) = NULL
    AS
    BEGIN
        SET NOCOUNT ON;
        SELECT Id, Nome, Destino, Vagas, Categoria, Valor
          FROM Combos
         WHERE Vagas > 0
           AND (@Destino IS NULL OR Destino LIKE N''%'' + @Destino + N''%'')
         ORDER BY Nome;
    END');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN

    IF EXISTS (SELECT 1 FROM Compras WHERE ComboId IS NULL)
        THROW 51000, 'Existem compras sem combo. Revise esses registros antes de aplicar CatalogoCombos.', 1;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Compras] DROP CONSTRAINT [FK_Compras_Combos_ComboId];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    DROP INDEX [IX_Compras_ComboId] ON [Compras];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Compras] ALTER COLUMN [ComboId] int NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    CREATE INDEX [IX_Compras_ComboId] ON [Compras] ([ComboId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    ALTER TABLE [Compras] ADD CONSTRAINT [FK_Compras_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE NO ACTION;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261107000000_CatalogoCombos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261107000000_CatalogoCombos', N'9.0.22');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108000000_ConcorrenciaCombos'
)
BEGIN
    ALTER TABLE [Combos] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261108000000_ConcorrenciaCombos'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261108000000_ConcorrenciaCombos', N'9.0.22');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    DROP INDEX [IX_Compras_ClienteId] ON [Compras];
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [CriadaEm] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Destino] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [ExpiraEm] datetime2 NOT NULL DEFAULT '0001-01-01T00:00:00.0000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Noites] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [NomePacote] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [PagamentoId] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [PagamentoUrl] nvarchar(2000) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Partida] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [PreferenciaId] nvarchar(100) NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Quartos] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [RequisicaoId] uniqueidentifier NOT NULL DEFAULT '00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [RowVersion] rowversion NOT NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Status] nvarchar(30) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Compras] ADD [Viajantes] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Descricao] nvarchar(2000) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [DiariaQuarto] decimal(10,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Evento] nvarchar(140) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Imagem] nvarchar(20) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Inclusoes] nvarchar(2000) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Noites] int NOT NULL DEFAULT 0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Origem] nvarchar(100) NOT NULL DEFAULT N'';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Partida] date NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [PrecoConfirmado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [Publicado] bit NOT NULL DEFAULT CAST(0 AS bit);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Combos] ADD [ValorExperiencias] decimal(10,2) NOT NULL DEFAULT 0.0;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Clientes] ADD [AcessoAte] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    ALTER TABLE [Administradores] ADD [AcessoAte] datetime2 NULL;
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    CREATE TABLE [Documentos] (
        [Id] int NOT NULL IDENTITY,
        [CompraId] int NOT NULL,
        [Tipo] nvarchar(40) NOT NULL,
        [Titulo] nvarchar(140) NOT NULL,
        [Url] nvarchar(1000) NOT NULL,
        [Codigo] nvarchar(100) NOT NULL,
        [EmitidoEm] datetime2 NOT NULL,
        CONSTRAINT [PK_Documentos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Documentos_Compras_CompraId] FOREIGN KEY ([CompraId]) REFERENCES [Compras] ([Id]) ON DELETE NO ACTION
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    CREATE TABLE [Favoritos] (
        [ClienteId] int NOT NULL,
        [ComboId] int NOT NULL,
        CONSTRAINT [PK_Favoritos] PRIMARY KEY ([ClienteId], [ComboId]),
        CONSTRAINT [FK_Favoritos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Favoritos_Combos_ComboId] FOREIGN KEY ([ComboId]) REFERENCES [Combos] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    EXEC(N'UPDATE [Administradores] SET [AcessoAte] = NULL
    WHERE [Id] = 1;
    SELECT @@ROWCOUNT');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN

    UPDATE Combos SET Noites=1, Imagem='beach', Origem=N'São Paulo' WHERE Noites=0;
    UPDATE c SET RequisicaoId=NEWID(), Viajantes=1, Quartos=1, Noites=1,
     Status=N'Legado - revisar', NomePacote=b.Nome, Destino=b.Destino
     FROM Compras c JOIN Combos b ON b.Id=c.ComboId
     WHERE c.RequisicaoId='00000000-0000-0000-0000-000000000000';
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    EXEC(N'
    CREATE OR ALTER PROCEDURE sp_Combos_BuscarPorDestino @Destino NVARCHAR(100)=NULL
    AS BEGIN SET NOCOUNT ON;
     SELECT * FROM Combos WHERE Publicado=1 AND Vagas>0
     AND (@Destino IS NULL OR Destino LIKE N''%'' + @Destino + N''%'') ORDER BY Nome;
    END');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    CREATE UNIQUE INDEX [IX_Compras_ClienteId_RequisicaoId] ON [Compras] ([ClienteId], [RequisicaoId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Compras_PagamentoId] ON [Compras] ([PagamentoId]) WHERE [PagamentoId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    EXEC(N'ALTER TABLE [Compras] ADD CONSTRAINT [CK_Compras_Viajantes] CHECK ([Viajantes] BETWEEN 1 AND 6)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    EXEC(N'ALTER TABLE [Combos] ADD CONSTRAINT [CK_Combos_Precificacao] CHECK ([Noites] BETWEEN 1 AND 60 AND [DiariaQuarto] >= 0 AND [ValorExperiencias] >= 0)');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    CREATE INDEX [IX_Documentos_CompraId] ON [Documentos] ([CompraId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    CREATE INDEX [IX_Favoritos_ComboId] ON [Favoritos] ([ComboId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20261109184256_IntegracaoPortal'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20261109184256_IntegracaoPortal', N'9.0.22');
END;

COMMIT;
GO

