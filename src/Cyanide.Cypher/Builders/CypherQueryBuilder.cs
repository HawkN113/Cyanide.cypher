﻿using System.Text;
using Cyanide.Cypher.Builders.Abstraction;

namespace Cyanide.Cypher.Builders;

public sealed class CypherQueryBuilder: ICypherQueryBuilder
{
    private readonly StringBuilder _createClauses = new();
    private readonly StringBuilder _deleteClauses = new();
    private readonly StringBuilder _matchClauses = new();
    private readonly StringBuilder _optMatchClauses = new();
    private readonly StringBuilder _whereClauses = new();
    private readonly StringBuilder _returnClauses = new();
    private readonly StringBuilder _orderByClauses = new();

    /// <summary>
    /// Add CREATE clause
    /// </summary>
    /// <param name="configureCreate"></param>
    /// <returns></returns>
    public CypherQueryBuilder Create(Func<CreateBuilder, CreateBuilder> configureCreate)
    {
        var createBuilder = new CreateBuilder(this, _createClauses);
        configureCreate(createBuilder).End();
        return this;
    }

    /// <summary>
    /// Add DELETE clause
    /// </summary>
    /// <param name="configureDelete"></param>
    /// <returns></returns>
    public CypherQueryBuilder Delete(Func<DeleteBuilder, DeleteBuilder> configureDelete)
    {
        var deleteBuilder = new DeleteBuilder(this, _deleteClauses, false);
        configureDelete(deleteBuilder).End();
        return this;
    }
    
    
    /// <summary>
    /// Add DETACH DELETE clause
    /// </summary>
    /// <param name="configureDelete"></param>
    /// <returns></returns>
    public CypherQueryBuilder DetachDelete(Func<DeleteBuilder, DeleteBuilder> configureDelete)
    {
        var deleteBuilder = new DeleteBuilder(this, _deleteClauses, true);
        configureDelete(deleteBuilder).End();
        return this;
    }

    /// <summary>
    /// Add MATCH clause
    /// </summary>
    /// <param name="configureMatch"></param>
    /// <param name="configureOptMatch"></param>
    /// <returns></returns>
    public CypherQueryBuilder Match(
        Func<MatchBuilder, MatchBuilder> configureMatch,
        Func<OptMatchBuilder, OptMatchBuilder>? configureOptMatch = null)
    {
        var matchBuilder = new MatchBuilder(this, _matchClauses);
        configureMatch(matchBuilder).End();
        
        if (configureOptMatch is null) return this;
        
        var optionalMatchBuilder = new OptMatchBuilder(this, _optMatchClauses);
        configureOptMatch(optionalMatchBuilder).End();
        
        return this;
    }
    
    /// <summary>
    /// Add OPTIONAL MATCH clause
    /// </summary>
    /// <param name="configureOptMatch"></param>
    /// <returns></returns>
    public CypherQueryBuilder OptionalMatch(Func<OptMatchBuilder, OptMatchBuilder> configureOptMatch)
    {
        var optMatchBuilder = new OptMatchBuilder(this, _optMatchClauses);
        configureOptMatch(optMatchBuilder).End();
        return this;
    }

    /// <summary>
    /// Add RETURN clause
    /// Use ORDER BY
    /// </summary>
    /// <param name="configureReturn"></param>
    /// <param name="configureOrderBy"></param>
    /// <returns></returns>
    public CypherQueryBuilder Select(
        Func<SelectBuilder, SelectBuilder> configureReturn,
        Func<OrderByBuilder, OrderByBuilder>? configureOrderBy = null)
    {
        var returnBuilder = new SelectBuilder(this, _returnClauses);
        configureReturn(returnBuilder).End();

        if (configureOrderBy is null) return this;
        
        var orderByBuilder = new OrderByBuilder(this, _orderByClauses);
        configureOrderBy(orderByBuilder).End();

        return this;
    }
    
    /// <summary>
    /// Add ORDER BY clause
    /// </summary>
    /// <param name="configureOrderBy"></param>
    /// <returns></returns>
    public CypherQueryBuilder OrderBy(Func<OrderByBuilder, OrderByBuilder> configureOrderBy)
    {
        var orderByBuilder = new OrderByBuilder(this, _orderByClauses);
        configureOrderBy(orderByBuilder).End();
        return this;
    }
    
    /// <summary>
    /// Add WHERE clause
    /// </summary>
    /// <param name="conditions"></param>
    /// <returns></returns>
    public CypherQueryBuilder Where(Func<WhereBuilder, WhereBuilder> conditions)
    {
        var whereBuilder = new WhereBuilder(this, _whereClauses);
        conditions(whereBuilder).End();
        return this;
    }

    /// <summary>
    /// Build the Cypher query
    /// </summary>
    /// <returns></returns>
    public string Build()
    {
        StringBuilder queryBuilder = new();
        AppendClause(_matchClauses, queryBuilder);
        AppendClause(_optMatchClauses, queryBuilder);
        AppendClause(_whereClauses, queryBuilder);
        AppendClause(_createClauses, queryBuilder);
        AppendClause(_deleteClauses, queryBuilder);
        AppendClause(_returnClauses, queryBuilder);
        AppendClause(_orderByClauses, queryBuilder);
        return queryBuilder.ToString().Trim();
    }

    private static void AppendClause(StringBuilder clause, StringBuilder queryBuilder)
    {
        if (clause.Length <= 0) return;
        if (queryBuilder.Length > 0)
        {
            queryBuilder.Append(' ');
        }
        queryBuilder.Append(clause);
    }
}