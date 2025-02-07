﻿using System.Text;
using Cyanide.Cypher.Builders.Abstraction;
using Cyanide.Cypher.Extensions;

namespace Cyanide.Cypher.Builders.Queries.Admin.Commands;

public sealed class CreateUserQuery :
    IBuilderInitializer,
    ICreateAdmQueryUser,
    ISetUserPassword,
    ISetUserStatus,
    ISetUserHomeDb
{
    private readonly List<string> _patterns = [];
    private bool _shouldReplaced;
    private StringBuilder _createUserClauses = new();

    public void Initialize(StringBuilder clauseBuilder)
    {
        _createUserClauses = clauseBuilder;
    }

    public ISetUserPassword WithUser(string userName)
    {
        _patterns.Add($"USER {userName}");
        return this;
    }

    public void Replace()
    {
        _shouldReplaced = true;
    }

    public ISetUserHomeDb SetStatus(UserStatus status = UserStatus.Active)
    {
        _patterns.Add($"SET STATUS {status.GetDescription()}");
        return this;
    }

    public ISetUserStatus WithPassword(string password, PasswordType type = PasswordType.Plaintext,
        bool changeRequired = false)
    {
        var change = !changeRequired ? "CHANGE NOT REQUIRED" : "CHANGE REQUIRED";
        _patterns.Add($"SET {type.GetDescription()} PASSWORD {password} {change}");
        return this;
    }

    public IReplaceUser SetHomeDb(string databaseName)
    {
        _patterns.Add($"SET HOME DATABASE {databaseName}");
        return this;
    }

    public void End()
    {
        if (_patterns.Count <= 0) return;
        if (_createUserClauses.Length > 0)
        {
            _createUserClauses.Append(' ');
        }

        _createUserClauses.Append(!_shouldReplaced ? "CREATE " : "CREATE OR REPLACE ");
        _createUserClauses.Append(string.Join(" ", _patterns));
    }
}